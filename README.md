# Rustify

**Rustify** is a .NET library that brings some of the best features from Rust into the C# world, aiming to provide more robust and expressive ways to handle common programming patterns. This library includes popular Rust constructs like `Option<T>`, `Result<T, E>`, `Unit`, `Arc<T>` (Atomic Reference Counting), and `RwLock<T>` (Read-Write Lock).

## Features

*   **`Option<T>`**: Represents an optional value. It can be `Some(value)` or `None`, helping to avoid `null` reference exceptions and making code more explicit about the possibility of missing values.
*   **`Result<T, E>`**: Represents a value that can be either `Ok(value)` or `Err(error)`. This is useful for error handling without relying on exceptions, making control flow more predictable.
*   **`Unit`**: Represents a type with a single value, `()`. It's often used as a return type for functions that perform an action but don't return a meaningful value, similar to `void` but can be used as a generic type argument.
*   **`Arc<T>` (Atomic Reference Counter)**: A thread-safe reference-counted pointer. `Arc<T>` provides shared ownership of a value of type `T`, allocated on the heap. It ensures that the value is deallocated only when the last `Arc` pointer to it is dropped. This is particularly useful for sharing data across threads safely.
*   **`RwLock<T>` (Read-Write Lock)**: A synchronization primitive that allows multiple readers or a single writer at any point in time. `RwLock<T>` is useful when you have data that is read frequently but written infrequently, as it allows for concurrent reads, improving performance. It requires `T` to implement `IClone<T>` for safe read operations.

## Installation

You can install Rustify via NuGet Package Manager:

```powershell
Install-Package Rustify
```

Or via .NET CLI:

```powershell
dotnet add package Rustify
```

## Usage

### `Option<T>`

`Option<T>` is used to represent a value that might be absent.

```csharp
using Rustify.Monads; // For Option<T>

public class OptionExample
{
    public static Option<string> GetName(bool giveName)
    {
        if (giveName)
        {
            return Option<string>.Some("John Doe");
        }
        else
        {
            return Option<string>.None();
        }
    }

    public static void Main(string[] args)
    {
        var nameOption = GetName(true);
        nameOption.Match(
            some: name => Console.WriteLine($"Name: {name}"),
            none: () => Console.WriteLine("No name provided.")
        ); // Output: Name: John Doe

        var noNameOption = GetName(false);
        if (noNameOption.IsNone()) {
            Console.WriteLine(noNameOption.UnwrapOr("Default Name")); // Output: Default Name
        }
    }
}
```

### `Result<T, E>`

`Result<T, E>` is used for functions that can return a value or an error.

```csharp
using Rustify.Monads; // For Result<T, E>

public class ResultExample
{
    public enum FileError
    {
        NotFound,
        AccessDenied
    }

    public static Result<string, FileError> ReadFileContent(string filePath)
    {
        if (filePath == "secret.txt")
        {
            return Result<string, FileError>.Err(FileError.AccessDenied);
        }
        else if (filePath == "data.txt")
        {
            return Result<string, FileError>.Ok("File content here.");
        }
        else
        {
            return Result<string, FileError>.Err(FileError.NotFound);
        }
    }

    public static void Main(string[] args)
    {
        var contentResult = ReadFileContent("data.txt");
        contentResult.Match(
            ok: content => Console.WriteLine($"Content: {content}"),
            err: error => Console.WriteLine($"Error: {error}")
        ); // Output: Content: File content here.

        var errorResult = ReadFileContent("secret.txt");
        if (errorResult.IsErr()) {
            Console.WriteLine($"Failed to read file: {errorResult.Err().Unwrap()}"); // Output: Failed to read file: AccessDenied
        }
    }
}
```

### `Unit`

`Unit` is used when a function doesn't return a meaningful value but needs a return type for generic contexts.

```csharp
using Rustify.Monads; // For Result<T, E> which can use Unit
using Rustify.Utilities; // For Unit

public class UnitExample
{
    public static Result<Unit, string> PerformAction(bool succeed)
    {
        if (succeed)
        {
            Console.WriteLine("Action performed successfully.");
            return Result<Unit, string>.Ok(Unit.New);
        }
        else
        {
            return Result<Unit, string>.Err("Action failed.");
        }
    }

    public static void Main(string[] args)
    {
        var actionResult = PerformAction(true);
        actionResult.Match(
            ok: _ => Console.WriteLine("Confirmed success."), // We use _ as Unit carries no data
            err: error => Console.WriteLine($"Error: {error}")
        );
        // Output:
        // Action performed successfully.
        // Confirmed success.

        PerformAction(false); // Output: Error: Action failed. (if error is handled)
    }
}
```

### `Arc<T>` (Atomic Reference Counter)

`Arc<T>` allows safe sharing of data across multiple threads by using atomic operations for reference counting.

```csharp
using Rustify.Utilities.Sync; // For Arc<T>
using System.Threading.Tasks;

public class ArcExample
{
    public class SharedData
    {
        public int Value { get; set; }
        public SharedData(int value) { Value = value; }
    }

    public static async Task UseSharedDataAsync(Arc<SharedData> dataArc)
    {
        // Clone the Arc to get another pointer to the same data.
        // This increases the reference count.
        var localArc = dataArc.Clone();

        await Task.Run(() =>
        {
            // Access the data through the Arc.
            // The `Lock()` method provides safe access to the inner value.
            // In this basic Arc implementation, Lock() might simply return the value
            // if T is a primitive or if direct access is considered safe enough
            // for the specific use case, or it might involve a simple lock.
            // For truly concurrent modification, a more complex structure like RwLock
            // or Mutex around the data itself might be needed if Arc<T> only manages lifetime.
            // However, typical Arc<T> focuses on shared ownership and lifetime,
            // assuming T itself is either immutable or internally synchronized if mutable.

            // Let's assume Arc<T>.Lock() gives us direct access or a simple lock for this example.
            // And that modifications are controlled if T is mutable.
            // For this example, we'll just read.
            Console.WriteLine($"Thread {Task.CurrentId}: Shared data value: {localArc.Lock().Value}");

            // If SharedData was mutable and we wanted to change it:
            // lock(localArc.Lock()) // External lock if Arc<T>.Lock() returns T directly
            // {
            //    localArc.Lock().Value += 1;
            // }
        });

        // When localArc goes out of scope, its reference count is decremented.
        // The actual data is deallocated when the count reaches zero.
    }

    public static async Task Main(string[] args)
    {
        var initialData = new SharedData(42);
        var dataArc = Arc<SharedData>.New(initialData);

        var tasks = new List<Task>();
        for (int i = 0; i < 5; i = i + 1)
        {
            // Pass a clone of the Arc to each task.
            tasks.Add(UseSharedDataAsync(dataArc.Clone()));
        }

        await Task.WhenAll(tasks);

        Console.WriteLine($"Main thread: Shared data value after tasks: {dataArc.Lock().Value}");
        // The Arc in the main thread still holds a reference.
        // The data is cleaned up when dataArc also goes out of scope.
    }
}
```

### `RwLock<T>` (Read-Write Lock)

`RwLock<T>` provides a mechanism for multiple readers or a single writer, which is efficient for data structures that are read more often than written. `T` must implement `IClone<T>` to allow readers to work with a clone of the data, ensuring thread safety.

```csharp
using Rustify.Utilities.Sync; // For RwLock<T>
using Rustify.Interfaces;    // For IClone<T>
using System.Threading.Tasks;

public class Config : IClone<Config>
{
    public string SettingA { get; set; }
    public int SettingB { get; set; }

    public Config(string a, int b)
    {
        SettingA = a;
        SettingB = b;
    }

    // Deep clone implementation
    public Config Clone()
    {
        return new Config(SettingA, SettingB);
    }

    public override string ToString()
    {
        return $"SettingA: {SettingA}, SettingB: {SettingB}";
    }
}

public class RwLockExample
{
    public static async Task ReaderTask(RwLock<Config> configLock, int readerId)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(50, 150))); // Simulate some work
        var readGuard = configLock.Read(); // Acquire read lock
        if (readGuard.IsOk())
        {
            Config config = readGuard.Unwrap(); // Get the cloned data
            Console.WriteLine($"Reader {readerId}: {config}");
            // readGuard is automatically disposed when it goes out of scope, releasing the read lock.
        }
        else
        {
            Console.WriteLine($"Reader {readerId}: Could not acquire read lock: {readGuard.Err().Unwrap()}");
        }
    }

    public static async Task WriterTask(RwLock<Config> configLock)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(100, 200))); // Simulate some work
        var writeGuard = configLock.Write(); // Acquire write lock
        if (writeGuard.IsOk())
        {
            Config config = writeGuard.Unwrap(); // Get a mutable reference to the data
            config.SettingA = $"Updated by writer at {DateTime.Now.Ticks}";
            config.SettingB += 10;
            Console.WriteLine($"Writer: Updated config to {config}");
            // writeGuard is automatically disposed when it goes out of scope, releasing the write lock.
        }
        else
        {
            Console.WriteLine($"Writer: Could not acquire write lock: {writeGuard.Err().Unwrap()}");
        }
    }

    public static async Task Main(string[] args)
    {
        var initialConfig = new Config("Initial Value", 100);
        var configLock = new RwLock<Config>(initialConfig);

        var tasks = new List<Task>();

        // Create multiple reader tasks
        for (int i = 0; i < 5; i = i + 1)
        {
            int readerId = i; // Capture loop variable
            tasks.Add(Task.Run(() => ReaderTask(configLock, readerId)));
        }

        // Create a writer task
        tasks.Add(Task.Run(() => WriterTask(configLock)));

        // Create more reader tasks to see if they wait for the writer
        for (int i = 5; i < 10; i = i + 1)
        {
            int readerId = i; // Capture loop variable
            tasks.Add(Task.Run(() => ReaderTask(configLock, readerId)));
        }
        
        // Create another writer task
        tasks.Add(Task.Run(() => WriterTask(configLock)));


        await Task.WhenAll(tasks);

        // Final read from main thread
        var finalReadGuard = configLock.Read();
        if (finalReadGuard.IsOk())
        {
            Console.WriteLine($"Main thread (final read): {finalReadGuard.Unwrap()}");
        }
    }
}
```

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue.

## License

This project is licensed under the MIT License.
