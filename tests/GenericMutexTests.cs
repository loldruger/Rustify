using Rustify.Utilities.Sync;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rustify.Tests;

[TestClass]
public sealed class GenericMutexTests
{
    [TestMethod]
    public void GetValue_ReturnsCorrectValue()
    {
        using var mutex = new GenericMutex<int>(42);
        var result = mutex.GetValue();
        Assert.IsTrue(result.IsOk());
        Assert.AreEqual(42, result.Unwrap());
    }

    [TestMethod]
    public void TryGetValue_ReturnsValue_WhenNotLocked()
    {
        using var mutex = new GenericMutex<string>("test");
        var result = mutex.TryGetValue();
        Assert.IsTrue(result.IsOk());
        Assert.AreEqual("test", result.Unwrap());
    }

    [TestMethod]
    public async Task GetValueAsync_ReturnsCorrectValue()
    {
        using var mutex = new GenericMutex<int>(100);
        var result = await mutex.GetValueAsync();
        Assert.IsTrue(result.IsOk());
        Assert.AreEqual(100, result.Unwrap());
    }

    [TestMethod]
    public async Task TryGetValueAsync_ReturnsValue_WhenNotLocked()
    {
        using var mutex = new GenericMutex<string>("async_test");
        var result = await mutex.TryGetValueAsync();
        Assert.IsTrue(result.IsOk());
        Assert.AreEqual("async_test", result.Unwrap());
    }

    [TestMethod]
    public void UpdateValue_ModifiesValue()
    {
        using var mutex = new GenericMutex<int>(10);
        var updateResult = mutex.UpdateValue(x => x + 5);
        Assert.IsTrue(updateResult.IsOk());

        var getResult = mutex.GetValue();
        Assert.IsTrue(getResult.IsOk());
        Assert.AreEqual(15, getResult.Unwrap());
    }

    [TestMethod]
    public async Task UpdateValueAsync_ModifiesValue()
    {
        using var mutex = new GenericMutex<int>(20);
        var updateResult = await mutex.UpdateValueAsync(x => x * 2);
        Assert.IsTrue(updateResult.IsOk());

        var getResult = await mutex.GetValueAsync();
        Assert.IsTrue(getResult.IsOk());
        Assert.AreEqual(40, getResult.Unwrap());
    }

    [TestMethod]
    public void WithLock_ExecutesAction()
    {
        using var mutex = new GenericMutex<int>(5);
        var result = mutex.WithLock(value => Rustify.Monads.Result<int, string>.Ok(value * 3));
        
        Assert.IsTrue(result.IsOk());
        var innerResult = result.Unwrap();
        Assert.IsTrue(innerResult.IsOk());
        Assert.AreEqual(15, innerResult.Unwrap());
    }

    [TestMethod]
    public async Task WithLockAsync_ExecutesAsyncAction()
    {
        using var mutex = new GenericMutex<int>(7);
        var result = await mutex.WithLockAsync(async value =>
        {
            await Task.Delay(1);
            return Rustify.Monads.Result<int, string>.Ok(value + 3);
        });

        Assert.IsTrue(result.IsOk());
        var innerResult = result.Unwrap();
        Assert.IsTrue(innerResult.IsOk());
        Assert.AreEqual(10, innerResult.Unwrap());
    }

    [TestMethod]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var mutex = new GenericMutex<int>(1);
        mutex.Dispose();
        mutex.Dispose();
    }

    [TestMethod]
    public void GetValue_ReturnsErr_AfterDispose()
    {
        var mutex = new GenericMutex<int>(1);
        mutex.Dispose();
        var result = mutex.GetValue();
        Assert.IsTrue(result.IsErr());
        Assert.AreEqual(GenericMutexError.MutexFailed, result.Err().Unwrap());
    }

    [TestMethod]
    public async Task GetValueAsync_ReturnsErr_AfterDispose()
    {
        var mutex = new GenericMutex<int>(1);
        mutex.Dispose();
        var result = await mutex.GetValueAsync();
        Assert.IsTrue(result.IsErr());
        Assert.AreEqual(GenericMutexError.MutexFailed, result.Err().Unwrap());
    }

    [TestMethod]
    public void TryGetValue_ReturnsErr_AfterDispose()
    {
        var mutex = new GenericMutex<int>(1);
        mutex.Dispose();
        var result = mutex.TryGetValue();
        Assert.IsTrue(result.IsErr());
        Assert.AreEqual(GenericMutexError.MutexFailed, result.Err().Unwrap());
    }

    [TestMethod]
    public void UpdateValue_ReturnsErr_AfterDispose()
    {
        var mutex = new GenericMutex<int>(1);
        mutex.Dispose();
        var result = mutex.UpdateValue(x => x + 1);
        Assert.IsTrue(result.IsErr());
        Assert.AreEqual(GenericMutexError.MutexFailed, result.Err().Unwrap());
    }

    [TestMethod]
    public async Task ConcurrentAccess_WorksCorrectly()
    {
        using var mutex = new GenericMutex<int>(0);
        int numTasks = 10;
        int incrementsPerTask = 100;

        var tasks = new Task[numTasks];
        for (int i = 0; i < numTasks; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                for (int j = 0; j < incrementsPerTask; j++)
                {
                    await mutex.UpdateValueAsync(x => x + 1);
                }
            });
        }

        await Task.WhenAll(tasks);

        var result = mutex.GetValue();
        Assert.IsTrue(result.IsOk());
        Assert.AreEqual(numTasks * incrementsPerTask, result.Unwrap());
    }

    [TestMethod]
    public async Task CancellationToken_Cancels_GetValueAsync()
    {
        using var mutex = new GenericMutex<int>(1);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var result = await mutex.GetValueAsync(cts.Token);
        Assert.IsTrue(result.IsErr());
        Assert.AreEqual(GenericMutexError.MutexFailed, result.Err().Unwrap());
    }

    [TestMethod]
    public async Task CancellationToken_Cancels_UpdateValueAsync()
    {
        using var mutex = new GenericMutex<int>(1);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var result = await mutex.UpdateValueAsync(x => x + 1, cts.Token);
        Assert.IsTrue(result.IsErr());
        Assert.AreEqual(GenericMutexError.MutexFailed, result.Err().Unwrap());
    }
}
