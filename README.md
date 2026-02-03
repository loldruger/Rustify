# Rustify

**Rustify** is a .NET library that brings some of the best features from Rust into the C# world, aiming to provide more robust and expressive ways to handle common programming patterns. This library includes popular Rust constructs like `Option<T>`, `Result<T, E>`, `Unit`, `TaggedUnion`, `Arc<T>` (Atomic Reference Counting), and `RwLock<T>` (Read-Write Lock).

[![NuGet](https://img.shields.io/nuget/v/Rustify.svg)](https://www.nuget.org/packages/Rustify/)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-blue)](https://dotnet.microsoft.com/)

## Features

### Core Types

*   **`Option<T>`**: Represents an optional value. It can be `Some(value)` or `None`, helping to avoid `null` reference exceptions and making code more explicit about the possibility of missing values.
*   **`Result<T, E>`**: Represents a value that can be either `Ok(value)` or `Err(error)`. This is useful for error handling without relying on exceptions, making control flow more predictable.
*   **`Unit`**: Represents a type with a single value, `()`. It's often used as a return type for functions that perform an action but don't return a meaningful value, similar to `void` but can be used as a generic type argument.
*   **`TaggedUnion<T...>`**: A type-safe discriminated union that can hold one of several types. Available in 1-ary, 2-ary, and 3-ary variants.

### Synchronization Primitives

*   **`Arc<T>` (Atomic Reference Counter)**: A thread-safe reference-counted pointer. `Arc<T>` provides shared ownership of a value of type `T`, allocated on the heap. It ensures that the value is deallocated only when the last `Arc` pointer to it is dropped.
*   **`RwLock<T>` (Read-Write Lock)**: A synchronization primitive that allows multiple readers or a single writer at any point in time. `RwLock<T>` is useful when you have data that is read frequently but written infrequently.
*   **`GenericMutex<T>`**: A generic mutex wrapper that provides safe, exclusive access to a value.

### Extensions & LINQ Support

*   **LINQ Query Syntax**: Both `Option<T>` and `Result<T, E>` support LINQ query syntax with `Select`, `SelectMany`, and `Where` methods.
*   **Async Extensions**: `MapAsync`, `AndThenAsync`, and `UnwrapOrAsync` for seamless async/await integration.

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
using Rustify.Monads;

// Creating Options
var some = Option<string>.Some("Hello");
var none = Option<string>.None();

// Pattern matching
some.Match(
    some: value => Console.WriteLine($"Got: {value}"),
    none: () => Console.WriteLine("Nothing here")
);

// Chaining operations
var result = some
    .Map(s => s.Length)           // Option<int>
    .Filter(len => len > 3)       // Option<int>
    .UnwrapOr(0);                 // int

// Contains check
if (some.Contains("Hello"))
{
    Console.WriteLine("Found it!");
}
```

#### LINQ Query Syntax

```csharp
using Rustify.Monads;

var optionA = Option<int>.Some(10);
var optionB = Option<int>.Some(20);

// Compose Options with LINQ
var sum = from a in optionA
          from b in optionB
          where a > 0 && b > 0
          select a + b;

Console.WriteLine(sum.UnwrapOr(0)); // Output: 30
```

#### Async Operations

```csharp
using Rustify.Monads;

var option = Option<int>.Some(42);

// Async mapping
var result = await option.MapAsync(async x => 
{
    await Task.Delay(100);
    return x * 2;
});

// Async chaining
var chained = await option.AndThenAsync(async x =>
{
    var data = await FetchDataAsync(x);
    return Option<string>.Some(data);
});

// Works with Task<Option<T>> too
Task<Option<int>> optionTask = GetOptionAsync();
var mapped = await optionTask.MapAsync(x => x * 2);
```

### `Result<T, E>`

`Result<T, E>` is used for functions that can return a value or an error.

```csharp
using Rustify.Monads;

public enum ParseError { InvalidFormat, OutOfRange }

public static Result<int, ParseError> ParsePositive(string input)
{
    if (!int.TryParse(input, out var value))
        return Result<int, ParseError>.Err(ParseError.InvalidFormat);
    
    if (value <= 0)
        return Result<int, ParseError>.Err(ParseError.OutOfRange);
    
    return Result<int, ParseError>.Ok(value);
}

// Usage
var result = ParsePositive("42");

result.Match(
    ok: value => Console.WriteLine($"Parsed: {value}"),
    err: error => Console.WriteLine($"Error: {error}")
);

// Chaining
var doubled = result
    .Map(x => x * 2)
    .MapErr(e => $"Failed: {e}");

// Contains check
if (result.Contains(42))
{
    Console.WriteLine("Got 42!");
}

if (result.ContainsErr(ParseError.InvalidFormat))
{
    Console.WriteLine("Invalid format error");
}
```

#### LINQ Query Syntax

```csharp
using Rustify.Monads;

var resultA = Result<int, string>.Ok(10);
var resultB = Result<int, string>.Ok(20);

// Compose Results with LINQ
var sum = from a in resultA
          from b in resultB
          select a + b;

Console.WriteLine(sum.UnwrapOr(0)); // Output: 30
```

#### Async Operations

```csharp
using Rustify.Monads;

var result = Result<int, string>.Ok(42);

// Async mapping
var mapped = await result.MapAsync(async x =>
{
    var data = await ProcessAsync(x);
    return data;
});

// Async chaining with error propagation
var chained = await result.AndThenAsync(async x =>
{
    try
    {
        var data = await FetchAsync(x);
        return Result<string, string>.Ok(data);
    }
    catch (Exception ex)
    {
        return Result<string, string>.Err(ex.Message);
    }
});
```

### `Unit`

`Unit` is used when a function doesn't return a meaningful value but needs a return type for generic contexts.

```csharp
using Rustify.Monads;
using Rustify.Utilities;

public static Result<Unit, string> SaveData(string data)
{
    try
    {
        File.WriteAllText("data.txt", data);
        return Result<Unit, string>.Ok(Unit.New);
    }
    catch (Exception ex)
    {
        return Result<Unit, string>.Err(ex.Message);
    }
}

// Usage
SaveData("Hello").Match(
    ok: _ => Console.WriteLine("Saved successfully"),
    err: error => Console.WriteLine($"Failed: {error}")
);
```

### `TaggedUnion<T...>`

`TaggedUnion` provides type-safe discriminated unions for scenarios where a value can be one of several types.

#### 2-ary TaggedUnion

```csharp
using Rustify.Utilities;

// Can hold either a string or an int
TaggedUnion<string, int> result;

// Using implicit conversion
result = "success";  // Becomes case 0 (string)
result = 42;         // Becomes case 1 (int)

// Using static factory methods (for type inference)
var strResult = TaggedUnion._0<string, int>("hello");
var intResult = TaggedUnion._1<string, int>(100);

// Pattern matching with return value
string message = result.Match(
    case0: s => $"Got string: {s}",
    case1: i => $"Got int: {i}"
);

// Pattern matching with actions
result.Match(
    case0: s => Console.WriteLine($"String: {s}"),
    case1: i => Console.WriteLine($"Int: {i}")
);

// Type checking
if (result.Is0) Console.WriteLine("It's a string");
if (result.Is1) Console.WriteLine("It's an int");

// Safe extraction
if (result.TryGet0(out var str))
{
    Console.WriteLine($"Extracted string: {str}");
}
```

#### 3-ary TaggedUnion

```csharp
using Rustify.Utilities;

// Useful for representing states like: Value | Warning | Error
TaggedUnion<int, string, Exception> parseResult;

parseResult = 42;                           // Success value
parseResult = "Input was negative";         // Warning message  
parseResult = new FormatException("Bad");   // Error

var output = parseResult.Match(
    case0: value => $"Success: {value}",
    case1: warning => $"Warning: {warning}",
    case2: error => $"Error: {error.Message}"
);
```

#### Handling Same Types

When the union contains the same type multiple times, use instance factory methods:

```csharp
using Rustify.Utilities;

// Both cases are int, so implicit conversion won't work
var first = TaggedUnion<int, int>.__0(1);   // Explicitly case 0
var second = TaggedUnion<int, int>.__1(2);  // Explicitly case 1

Console.WriteLine(first.Is0);  // True
Console.WriteLine(second.Is1); // True
```

### `Arc<T>` (Atomic Reference Counter)

`Arc<T>` allows safe sharing of data across multiple threads by using atomic operations for reference counting.

```csharp
using Rustify.Utilities.Sync;

public class SharedData
{
    public int Value { get; set; }
}

// Create shared data
var arc = Arc<SharedData>.New(new SharedData { Value = 42 });

// Clone to share ownership
var clone1 = arc.Clone();
var clone2 = arc.Clone();

// Access the data
var data = arc.Lock();
Console.WriteLine(data.Value);

// Run parallel tasks with shared data
var tasks = Enumerable.Range(0, 5)
    .Select(_ => Task.Run(() =>
    {
        var local = arc.Clone();
        Console.WriteLine($"Value: {local.Lock().Value}");
    }));

await Task.WhenAll(tasks);
```

### `RwLock<T>` (Read-Write Lock)

`RwLock<T>` provides a mechanism for multiple readers or a single writer.

```csharp
using Rustify.Utilities.Sync;
using Rustify.Interfaces;

public class Config : IClone<Config>
{
    public string Setting { get; set; } = "";
    public Config Clone() => new Config { Setting = Setting };
}

var configLock = new RwLock<Config>(new Config { Setting = "initial" });

// Multiple concurrent readers
var readResult = configLock.Read();
if (readResult.IsOk())
{
    var config = readResult.Unwrap();
    Console.WriteLine($"Setting: {config.Setting}");
}

// Exclusive writer
var writeResult = configLock.Write();
if (writeResult.IsOk())
{
    var config = writeResult.Unwrap();
    config.Setting = "updated";
}

// Async variants with cancellation support
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var asyncRead = await configLock.ReadAsync(cts.Token);
var asyncWrite = await configLock.WriteAsync(cts.Token);
```

### `GenericMutex<T>`

`GenericMutex<T>` provides exclusive access to a value with timeout and cancellation support.

```csharp
using Rustify.Utilities.Sync;

var mutex = new GenericMutex<List<int>>(new List<int>());

// Lock and access
var result = mutex.Lock();
if (result.IsOk())
{
    var list = result.Unwrap();
    list.Add(42);
}

// With timeout
var timedResult = mutex.TryLock(TimeSpan.FromSeconds(1));

// Async with cancellation
using var cts = new CancellationTokenSource();
var asyncResult = await mutex.LockAsync(cts.Token);
```

## API Reference

### Option<T> Methods

| Method | Description |
|--------|-------------|
| `Some(T value)` | Creates an Option containing a value |
| `None()` | Creates an empty Option |
| `IsSome()` / `IsNone()` | Check if Option has a value |
| `Unwrap()` | Get value or throw if None |
| `UnwrapOr(T default)` | Get value or return default |
| `UnwrapOrElse(Func<T>)` | Get value or compute default |
| `Map(Func<T, U>)` | Transform the inner value |
| `MapOr(U default, Func<T, U>)` | Transform or return default |
| `AndThen(Func<T, Option<U>>)` | Chain Option-returning operations |
| `Filter(Func<T, bool>)` | Keep value only if predicate matches |
| `Contains(T value)` | Check if Option contains a specific value |
| `Match(some, none)` | Pattern match on the Option |
| `Ok<E>()` / `Err<T>()` | Convert to Result |

### Result<T, E> Methods

| Method | Description |
|--------|-------------|
| `Ok(T value)` | Creates a successful Result |
| `Err(E error)` | Creates an error Result |
| `IsOk()` / `IsErr()` | Check Result state |
| `Unwrap()` | Get value or throw if Err |
| `UnwrapOr(T default)` | Get value or return default |
| `UnwrapErr()` | Get error or throw if Ok |
| `Map(Func<T, U>)` | Transform the success value |
| `MapErr(Func<E, F>)` | Transform the error value |
| `AndThen(Func<T, Result<U, E>>)` | Chain Result-returning operations |
| `Contains(T value)` | Check if Result contains a specific Ok value |
| `ContainsErr(E error)` | Check if Result contains a specific Err value |
| `Match(ok, err)` | Pattern match on the Result |
| `Ok()` / `Err()` | Convert to Option |

### Async Extension Methods

| Method | Description |
|--------|-------------|
| `MapAsync(Func<T, Task<U>>)` | Async transform of inner value |
| `AndThenAsync(Func<T, Task<Option<U>>>)` | Async chaining for Option |
| `AndThenAsync(Func<T, Task<Result<U, E>>>)` | Async chaining for Result |
| `UnwrapOrAsync(T default)` | Async unwrap with default |

### TaggedUnion Properties & Methods

| Member | Description |
|--------|-------------|
| `Is0`, `Is1`, `Is2` | Check which case is active |
| `Tag` | Get the active case index (0, 1, or 2) |
| `Match(...)` | Pattern match on all cases |
| `TryGet0(out T)`, etc. | Safely extract value |
| `_0<...>(value)`, `_1<...>(value)` | Static factory methods |
| `__0(value)`, `__1(value)` | Instance factory methods (for same-type cases) |

## IEquatable & IComparable Support

Both `Option<T>` and `Result<T, E>` implement `IEquatable<T>` and `IComparable<T>`:

```csharp
var a = Option<int>.Some(5);
var b = Option<int>.Some(5);
var c = Option<int>.Some(10);

Console.WriteLine(a.Equals(b));     // True
Console.WriteLine(a.CompareTo(c));  // -1 (5 < 10)

// Works with sorting
var options = new[] { c, a, Option<int>.None() };
Array.Sort(options);  // None, Some(5), Some(10)
```

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue.

## License

This project is licensed under the MIT License.
