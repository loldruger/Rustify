using Rustify.Utilities.Sync;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rustify.Tests;

[TestClass]
public sealed class ArcTests
{
    private class DisposableData : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            this.IsDisposed = true;
        }
    }

    [TestMethod]
    public void Arc_Creation_InitializesWithValueAndCount()
    {
        var data = "test_data";
        using var arc = new Arc<string>(data);
        Assert.AreEqual(data, arc.GetValue());
        // Internal count is not directly testable without modification or reflection,
        // but clone and release will test its behavior.
    }

    [TestMethod]
    public void GetValue_ReturnsCorrectValue()
    {
        var data = 12345;
        using var arc = new Arc<int>(data);
        Assert.AreEqual(data, arc.GetValue());
    }

    [TestMethod]
    public void Clone_IncrementsCountAndSharesValue()
    {
        var data = "shared_string";
        using var arc1 = new Arc<string>(data);
        using var arc2 = arc1.Clone();

        Assert.AreEqual(data, arc1.GetValue());
        Assert.AreEqual(data, arc2.GetValue());
        Assert.AreSame(arc1.GetValue(), arc2.GetValue()); // For reference types, they should be the same instance initially.
    }

    [TestMethod]
    public void Release_DecrementsCount()
    {
        var data = "release_test";
        var arc1 = new Arc<string>(data);
        var arc2 = arc1.Clone(); // Count is now 2 (arc1, arc2)

        var countAfterRelease1 = arc2.Release(); // arc2 releases, count should be 1
        // We can't directly assert countAfterRelease1 is 1 without exposing count or more complex setup.
        // Instead, we'll check that arc1 is still valid.
        Assert.AreEqual(data, arc1.GetValue());

        var countAfterRelease2 = arc1.Release(); // arc1 releases, count should be 0
        Assert.AreEqual(0, countAfterRelease2);

        // After all releases, GetValue should throw
        Assert.ThrowsException<InvalidOperationException>(() => arc1.GetValue());
        Assert.ThrowsException<InvalidOperationException>(() => arc2.GetValue()); // arc2 is also effectively disposed
    }

    [TestMethod]
    public void Dispose_ReleasesArc()
    {
        var data = "dispose_test";
        var arc = new Arc<string>(data);
        var arcClone = arc.Clone();

        arc.Dispose(); // Releases arc's reference
        Assert.AreEqual(data, arcClone.GetValue()); // Clone should still be valid

        arcClone.Dispose(); // Releases clone's reference, object should be disposed
        Assert.ThrowsException<InvalidOperationException>(() => arc.GetValue());
        Assert.ThrowsException<InvalidOperationException>(() => arcClone.GetValue());
    }

    [TestMethod]
    public void GetValue_Throws_WhenObjectDisposed()
    {
        var data = "disposed_arc";
        var arc = new Arc<string>(data);
        arc.Release(); // Release the initial reference

        Assert.ThrowsException<InvalidOperationException>(() => arc.GetValue());
    }

    [TestMethod]
    public void Clone_Throws_WhenObjectDisposed()
    {
        var data = "disposed_clone";
        var arc = new Arc<string>(data);
        arc.Release(); // Release the initial reference

        Assert.ThrowsException<InvalidOperationException>(() => arc.Clone());
    }

    [TestMethod]
    public void Release_HandlesMultipleReleasesCorrectly()
    {
        var data = "multi_release";
        var arc = new Arc<string>(data);
        arc.Release(); // Count becomes 0
        var finalCount = arc.Release(); // Should not go negative, still 0 and disposed
        Assert.AreEqual(0, finalCount);
        Assert.ThrowsException<InvalidOperationException>(() => arc.GetValue());
    }

    [TestMethod]
    public void DisposableValue_IsDisposed_WhenLastArcReleased()
    {
        var disposableData = new DisposableData();
        var arc = new Arc<DisposableData>(disposableData);
        var arcClone = arc.Clone();

        Assert.IsFalse(disposableData.IsDisposed);
        arc.Release();
        Assert.IsFalse(disposableData.IsDisposed); // Still one reference (arcClone)
        arcClone.Release(); // Last reference released
        Assert.IsTrue(disposableData.IsDisposed);
    }

    [TestMethod]
    public async Task Concurrent_ClonesAndReleases_WorkCorrectly()
    {
        var data = "concurrent_data";
        var arc = new Arc<string>(data);
        int numTasks = 10;
        int clonesPerTask = 5;

        var tasks = new Task[numTasks];

        for (int i = 0; i < numTasks; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                Arc<string>[] localClones = new Arc<string>[clonesPerTask];
                for (int j = 0; j < clonesPerTask; j++)
                {
                    localClones[j] = arc.Clone();
                    Assert.AreEqual(data, localClones[j].GetValue());
                }
                // Simulate some work
                Thread.Sleep(10); 
                for (int j = 0; j < clonesPerTask; j++)
                {
                    localClones[j].Release();
                }
            });
        }

        await Task.WhenAll(tasks);

        // All clones from tasks are released, only original 'arc' reference might remain if not disposed.
        // Here, we test the main 'arc' reference.
        Assert.AreEqual(data, arc.GetValue()); 
        arc.Release(); // Release the original reference
        Assert.ThrowsException<InvalidOperationException>(() => arc.GetValue());
    }

     [TestMethod]
    public void Arc_WithValueType_WorksCorrectly()
    {
        var data = 123;
        using var arc = new Arc<int>(data);
        using var arcClone = arc.Clone();

        Assert.AreEqual(data, arc.GetValue());
        Assert.AreEqual(data, arcClone.GetValue());

        arc.Release();
        Assert.AreEqual(data, arcClone.GetValue()); // Clone still valid

        arcClone.Release();
        Assert.ThrowsException<InvalidOperationException>(() => arc.GetValue());
        Assert.ThrowsException<InvalidOperationException>(() => arcClone.GetValue());
    }
}
