using Rustify.Monads;
using Rustify.Utilities.Sync;
using Rustify.Interfaces;
using Rustify.Utilities.Synchronizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rustify.Tests;

// Helper class for testing RwLock with a cloneable type
public class CloneableData : IClone<CloneableData>
{
    public int Value { get; set; }
    public string Name { get; set; }

    public CloneableData(int value, string name)
    {
        this.Value = value;
        this.Name = name;
    }

    public CloneableData Clone()
    {
        return new CloneableData(this.Value, this.Name) { /* Potentially copy other members if they are complex */ };
    }

    public override bool Equals(object? obj)
    {
        return obj is CloneableData data &&
               this.Value == data.Value &&
               this.Name == data.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value, this.Name);
    }
}

[TestClass]
public sealed class RwLockTests
{
    [TestMethod]
    public void RwLock_Creation_InitializesWithValue()
    {
        var initialData = new CloneableData(10, "Initial");
        var rwLock = new RwLock<CloneableData>(initialData);

        var result = rwLock.GetValue();
        Assert.IsTrue(result.IsOk);
        Assert.AreEqual(initialData, result.Unwrap());
    }

    [TestMethod]
    public void RwLock_Creation_ThrowsOnNullInitialValue()
    {
        Assert.Throws<ArgumentNullException>(() => new RwLock<CloneableData>(null!));
    }

    [TestMethod]
    public async Task GetValueAsync_ReturnsCorrectValue()
    {
        var initialData = new CloneableData(20, "AsyncGet");
        var rwLock = new RwLock<CloneableData>(initialData);

        var result = await rwLock.GetValueAsync();
        Assert.IsTrue(result.IsOk);
        Assert.AreEqual(initialData, result.Unwrap());
    }

    [TestMethod]
    public void UpdateValue_ModifiesValueCorrectly()
    {
        var initialData = new CloneableData(30, "BeforeUpdate");
        var rwLock = new RwLock<CloneableData>(initialData);
        var updatedName = "AfterUpdate";

        var updateResult = rwLock.UpdateValue(data => 
        {
            data.Name = updatedName; 
            return data; 
        });
        Assert.IsTrue(updateResult.IsOk);

        var getResult = rwLock.GetValue();
        Assert.IsTrue(getResult.IsOk);
        Assert.AreEqual(updatedName, getResult.Unwrap().Name);
        Assert.AreEqual(initialData.Value, getResult.Unwrap().Value); // Value should be the same
    }

    [TestMethod]
    public async Task UpdateValueAsync_ModifiesValueCorrectly()
    {
        var initialData = new CloneableData(40, "AsyncBeforeUpdate");
        var rwLock = new RwLock<CloneableData>(initialData);
        var updatedValue = 50;

        var updateResult = await rwLock.UpdateValueAsync(data => 
        {
            data.Value = updatedValue;
            return data;
        });
        Assert.IsTrue(updateResult.IsOk);

        var getResult = await rwLock.GetValueAsync();
        Assert.IsTrue(getResult.IsOk);
        Assert.AreEqual(updatedValue, getResult.Unwrap().Value);
        Assert.AreEqual(initialData.Name, getResult.Unwrap().Name); // Name should be the same
    }

    [TestMethod]
    public async Task Concurrent_Reads_AreAllowed()
    {
        var initialData = new CloneableData(100, "ConcurrentRead");
        var rwLock = new RwLock<CloneableData>(initialData);
        int numReaders = 5;
        var tasks = new List<Task<Result<CloneableData, ISynchronizerError>>>();

        for (int i = 0; i < numReaders; i++)
        {
            tasks.Add(rwLock.GetValueAsync());
        }

        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(initialData, result.Unwrap());
        }
    }

    [TestMethod]
    public async Task Concurrent_ReadAndWrite_AreSynchronized()
    {
        var initialData = new CloneableData(200, "ReadWriteTest");
        var rwLock = new RwLock<CloneableData>(initialData);
        var finalValue = 300;
        var finalName = "UpdatedName";

        // Start a reader task that will try to read multiple times
        var readerTask = Task.Run(async () =>
        {
            List<CloneableData> readValues = new List<CloneableData>();
            for (int i = 0; i < 5; i++)
            {
                var res = await rwLock.GetValueAsync();
                if (res.IsOk) readValues.Add(res.Unwrap());
                await Task.Delay(10); // Small delay to allow writer to interleave
            }
            return readValues;
        });

        // Start a writer task after a short delay
        await Task.Delay(20);
        var writerTask = rwLock.UpdateValueAsync(data =>
        {
            data.Value = finalValue;
            data.Name = finalName;
            return data;
        });

        await Task.WhenAll(readerTask, writerTask);

        Assert.IsTrue(writerTask.Result.IsOk);

        var finalReadResult = await rwLock.GetValueAsync();
        Assert.IsTrue(finalReadResult.IsOk);
        Assert.AreEqual(finalValue, finalReadResult.Unwrap().Value);
        Assert.AreEqual(finalName, finalReadResult.Unwrap().Name);

        // Check values read by the reader task - some might be initial, some updated
        var readValues = await readerTask;
        Assert.IsTrue(readValues.Any(v => v.Value == initialData.Value || v.Value == finalValue));
        Assert.AreEqual(finalValue, readValues.Last().Value); // Last read should see the final value
    }

    [TestMethod]
    public async Task WithLockedAsync_PerformsActionWithWriteLock()
    {
        var initialData = new CloneableData(500, "WithLock");
        var rwLock = new RwLock<CloneableData>(initialData);
        var newValue = 600;

        var outerResult = await rwLock.WithLockedAsync<int, string>(async clone => 
        {
            clone.Value = newValue;
            await Task.Delay(10);
            return Result.Ok<int, string>(clone.Value); 
        });

        Assert.IsTrue(outerResult.IsOk);
        var actionResult = outerResult.Unwrap();
        Assert.IsTrue(actionResult.IsOk);
        Assert.AreEqual(newValue, actionResult.Unwrap());
    }

    [TestMethod]
    public async Task WithReadLockedAsync_PerformsActionWithReadLock()
    {
        var initialData = new CloneableData(700, "WithReadLock");
        var rwLock = new RwLock<CloneableData>(initialData);

        var outerResult = await rwLock.WithReadLockedAsync<string, int>(async clone => 
        {
            // Action operates on a clone of the data with a read lock held during cloning.
            await Task.Delay(10); // Simulate some async work with the clone
            return Result.Ok<string, int>($"Read: {clone.Name} - {clone.Value}");
        });

        Assert.IsTrue(outerResult.IsOk);
        var actionResult = outerResult.Unwrap();
        Assert.IsTrue(actionResult.IsOk);
        Assert.AreEqual($"Read: {initialData.Name} - {initialData.Value}", actionResult.Unwrap());

        // Ensure original value is unchanged
        var originalValue = await rwLock.GetValueAsync();
        Assert.IsTrue(originalValue.IsOk);
        Assert.AreEqual(initialData, originalValue.Unwrap());
    }

    [TestMethod]
    public void UpdateValue_HandlesExceptionInUpdateFunc()
    {
        var initialData = new CloneableData(1, "ErrorTest");
        var rwLock = new RwLock<CloneableData>(initialData);

        var result = rwLock.UpdateValue(data => 
        {
            throw new InvalidOperationException("Update failed");
            // return data; // Unreachable
        });

        Assert.IsTrue(result.IsErr);
        Assert.AreEqual(ISynchronizerError.Failed, result.UnwrapErr());

        // Check that the original value is unchanged
        var originalValue = rwLock.GetValue();
        Assert.IsTrue(originalValue.IsOk);
        Assert.AreEqual(initialData, originalValue.Unwrap());
    }

    [TestMethod]
    public async Task UpdateValueAsync_HandlesExceptionInUpdateFunc()
    {
        var initialData = new CloneableData(2, "AsyncErrorTest");
        var rwLock = new RwLock<CloneableData>(initialData);

        var result = await rwLock.UpdateValueAsync(data => 
        {
            throw new InvalidOperationException("Async update failed");
            // return data; // Unreachable
        });

        Assert.IsTrue(result.IsErr);
        Assert.AreEqual(ISynchronizerError.Failed, result.UnwrapErr());

        var originalValue = await rwLock.GetValueAsync();
        Assert.IsTrue(originalValue.IsOk);
        Assert.AreEqual(initialData, originalValue.Unwrap());
    }
}
