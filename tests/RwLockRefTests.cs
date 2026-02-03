using Rustify.Utilities.Sync;
using Rustify.Utilities.Synchronizer;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rustify.Tests;

[TestClass]
public sealed class RwLockRefTests
{
    [TestMethod]
    public void WithRead_ReturnsValue()
    {
        using var rwLock = new RwLockRef<List<int>>(new List<int> { 1, 2, 3 });
        
        var result = rwLock.WithRead(list => list.Count);
        
        Assert.IsTrue(result.IsOk());
        Assert.AreEqual(3, result.Unwrap());
    }

    [TestMethod]
    public void WithWrite_TransformsValue()
    {
        using var rwLock = new RwLockRef<List<int>>(new List<int> { 1 });
        
        var writeResult = rwLock.WithWrite(list => new List<int> { 1, 2, 3 });
        Assert.IsTrue(writeResult.IsOk());
        
        var readResult = rwLock.WithRead(list => list.Count);
        Assert.AreEqual(3, readResult.Unwrap());
    }

    [TestMethod]
    public void WithWriteMutate_ModifiesInPlace()
    {
        using var rwLock = new RwLockRef<List<int>>(new List<int> { 1 });
        
        var result = rwLock.WithWriteMutate(list => list.Add(2));
        Assert.IsTrue(result.IsOk());
        
        var count = rwLock.WithRead(list => list.Count);
        Assert.AreEqual(2, count.Unwrap());
    }

    [TestMethod]
    public void WithRead_ReturnsErr_AfterDispose()
    {
        var rwLock = new RwLockRef<List<int>>(new List<int>());
        rwLock.Dispose();
        
        var result = rwLock.WithRead(list => list.Count);
        
        Assert.IsTrue(result.IsErr());
        Assert.AreEqual(SynchronizerErrorKind.Disposed, result.Err().Unwrap().Kind);
    }

    [TestMethod]
    public void WithWrite_ReturnsErr_AfterDispose()
    {
        var rwLock = new RwLockRef<List<int>>(new List<int>());
        rwLock.Dispose();
        
        var result = rwLock.WithWrite(list => list);
        
        Assert.IsTrue(result.IsErr());
        Assert.AreEqual(SynchronizerErrorKind.Disposed, result.Err().Unwrap().Kind);
    }

    [TestMethod]
    public async Task ConcurrentReaders_AllowedSimultaneously()
    {
        using var rwLock = new RwLockRef<List<int>>(new List<int> { 1, 2, 3 });
        int readersCompleted = 0;

        var tasks = new Task[5];
        for (int i = 0; i < 5; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                var result = rwLock.WithRead(list =>
                {
                    Thread.Sleep(10);
                    return list.Count;
                });
                Assert.IsTrue(result.IsOk());
                Interlocked.Increment(ref readersCompleted);
            });
        }

        await Task.WhenAll(tasks);
        Assert.AreEqual(5, readersCompleted);
    }

    [TestMethod]
    public async Task WithReadAsync_Works()
    {
        using var rwLock = new RwLockRef<List<int>>(new List<int> { 1, 2, 3 });
        
        var result = await rwLock.WithReadAsync(async list =>
        {
            await Task.Delay(1);
            return list.Count;
        });
        
        Assert.IsTrue(result.IsOk());
        Assert.AreEqual(3, result.Unwrap());
    }

    [TestMethod]
    public async Task WithWriteAsync_Works()
    {
        using var rwLock = new RwLockRef<List<int>>(new List<int> { 1 });
        
        var result = await rwLock.WithWriteAsync(async list =>
        {
            await Task.Delay(1);
            return new List<int> { 1, 2, 3, 4 };
        });
        
        Assert.IsTrue(result.IsOk());
        
        var count = rwLock.WithRead(list => list.Count);
        Assert.AreEqual(4, count.Unwrap());
    }

    [TestMethod]
    public async Task CancellationToken_CancelsAsync()
    {
        using var rwLock = new RwLockRef<List<int>>(new List<int>());
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        var result = await rwLock.WithReadAsync(async list =>
        {
            await Task.Delay(100);
            return list.Count;
        }, cts.Token);
        
        Assert.IsTrue(result.IsErr());
        Assert.AreEqual(SynchronizerErrorKind.Cancelled, result.Err().Unwrap().Kind);
    }

    [TestMethod]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var rwLock = new RwLockRef<List<int>>(new List<int>());
        rwLock.Dispose();
        rwLock.Dispose();
    }
}
