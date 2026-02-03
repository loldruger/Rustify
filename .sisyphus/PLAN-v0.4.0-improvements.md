# Rustify v0.4.0 Improvements Plan

## Overview

This plan implements 4 key improvements to bring Rustify closer to Rust's API patterns while maintaining C# idioms.

**Version**: 0.4.0 (no version bump)
**Breaking Changes**: Yes (GenericMutexError removal)

---

## Task 1: Remove GenericMutexError (Breaking Change)

**Priority**: High
**Files**: 
- `src/utilities/sync/GenericMutex.cs`
- `tests/GenericMutexTests.cs`

### Description
Remove the `GenericMutexError` enum and consolidate on `SynchronizerError` struct for consistency across all sync primitives.

### Implementation Steps

1. **Delete** the `GenericMutexError` enum from `GenericMutex.cs`

2. **Update** `GenericMutex<T>` methods to return `SynchronizerError`:
   - `Lock()` → `Result<GenericMutexGuard<T>, SynchronizerError>`
   - `TryLock()` → `Result<GenericMutexGuard<T>, SynchronizerError>`
   
3. **Map errors** using `SynchronizerError`:
   - `GenericMutexError.Abandoned` → `SynchronizerError.FromMutexAbandoned()`
   - `GenericMutexError.Timeout` → `SynchronizerError.FromTimeout()`

4. **Update tests** in `GenericMutexTests.cs` to expect `SynchronizerError` instead of `GenericMutexError`

### Acceptance Criteria
- [x] `GenericMutexError` enum no longer exists
- [x] All `GenericMutex<T>` methods return `SynchronizerError`
- [x] All tests pass with updated error types
- [x] No compilation warnings

---

## Task 2: Add Weak<T> for Arc<T>

**Priority**: High
**Files**:
- `src/utilities/sync/Weak.cs` (new)
- `src/utilities/sync/Arc.cs` (modify)
- `tests/WeakTests.cs` (new)

### Description
Add `Weak<T>` type to enable weak references to `Arc<T>`, allowing circular reference breaking similar to Rust's `std::sync::Weak`.

### Implementation Steps

1. **Create** `Weak.cs` with:
```csharp
public sealed class Weak<T> where T : class
{
    private readonly WeakReference<ArcInner<T>> innerRef;
    
    internal Weak(ArcInner<T> inner)
    {
        this.innerRef = new WeakReference<ArcInner<T>>(inner);
        Interlocked.Increment(ref inner.WeakCount);
    }
    
    public Option<Arc<T>> Upgrade()
    {
        var inner = this.innerRef.Target;  // Property access, NOT TryGetTarget
        
        if (inner is null)
            return Option<Arc<T>>.None();
        
        lock (inner.Lock)
        {
            if (inner.IsDisposed || inner.RefCount == 0)
                return Option<Arc<T>>.None();
            
            Interlocked.Increment(ref inner.RefCount);
            return Option<Arc<T>>.Some(Arc<T>.FromInner(inner));
        }
    }
    
    public bool IsAlive => this.innerRef.Target?.RefCount > 0;
}
```

2. **Modify** `ArcInner<T>` in `Arc.cs`:
   - Add `public int WeakCount` field
   - Add `public object Lock` for synchronization
   - Add `public bool IsDisposed` flag

3. **Add** to `Arc<T>`:
   - `public Weak<T> Downgrade()` - creates weak reference
   - `internal static Arc<T> FromInner(ArcInner<T> inner)` - for Weak.Upgrade()

4. **Create** `WeakTests.cs` with tests for:
   - Upgrade succeeds while Arc alive
   - Upgrade returns None after Arc disposed
   - Multiple Weak from same Arc
   - IsAlive property behavior

### CRITICAL CONSTRAINT
**DO NOT use `out` keyword.** Use `WeakReference<T>.Target` property directly:
```csharp
// CORRECT:
var inner = this.innerRef.Target;
if (inner is null) ...

// WRONG - DO NOT USE:
if (this.innerRef.TryGetTarget(out var inner)) ...
```

### Acceptance Criteria
- [x] `Weak<T>` class exists with `Upgrade()` and `IsAlive`
- [x] `Arc<T>.Downgrade()` creates `Weak<T>`
- [x] No `out` keyword used anywhere
- [x] All tests pass
- [x] Thread-safe implementation

---

## Task 3: Add RwLockRef<T> (Callback-Only API)

**Priority**: Medium
**Files**:
- `src/utilities/sync/RwLockRef.cs` (new)
- `tests/RwLockRefTests.cs` (new)

### Description
Create `RwLockRef<T>` that works with reference types without requiring `IClone<T>`. Uses callback-based access only (no `GetValue`).

### Implementation Steps

1. **Create** `RwLockRef.cs`:
```csharp
public sealed class RwLockRef<T> : IDisposable where T : class
{
    private readonly ReaderWriterLockSlim rwLock;
    private T value;
    private bool disposed;
    
    public RwLockRef(T value)
    {
        this.value = value;
        this.rwLock = new ReaderWriterLockSlim();
    }
    
    public Result<U, SynchronizerError> WithRead<U>(Func<T, U> reader)
    {
        if (disposed)
            return Result<U, SynchronizerError>.Err(SynchronizerError.FromObjectDisposed());
        
        try
        {
            rwLock.EnterReadLock();
            try
            {
                return Result<U, SynchronizerError>.Ok(reader(value));
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }
        catch (LockRecursionException)
        {
            return Result<U, SynchronizerError>.Err(SynchronizerError.FromLockRecursion());
        }
    }
    
    public Result<Unit, SynchronizerError> WithWrite(Func<T, T> writer)
    {
        if (disposed)
            return Result<Unit, SynchronizerError>.Err(SynchronizerError.FromObjectDisposed());
        
        try
        {
            rwLock.EnterWriteLock();
            try
            {
                value = writer(value);
                return Result<Unit, SynchronizerError>.Ok(Unit.Value);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }
        catch (LockRecursionException)
        {
            return Result<Unit, SynchronizerError>.Err(SynchronizerError.FromLockRecursion());
        }
    }
    
    // NO GetValue() method - callback-only for safety
    
    public void Dispose() { ... }
}
```

2. **Create** `RwLockRefTests.cs` with tests for:
   - `WithRead` returns value through callback
   - `WithWrite` modifies value
   - Multiple concurrent readers allowed
   - Writer blocks readers
   - Disposed lock returns error

### Design Decision
**NO `GetValue()` method** - callback-only access ensures the lock is always properly acquired/released.

### Acceptance Criteria
- [x] `RwLockRef<T>` class with `WithRead<U>` and `WithWrite`
- [x] No `IClone<T>` constraint
- [x] No `GetValue()` method
- [x] Returns `SynchronizerError` on failure
- [x] All tests pass

---

## Task 4: Extend TaggedUnion to 6 Arity

**Priority**: Medium
**Files**:
- `src/utilities/TaggedUnion.cs`
- `tests/TaggedUnionTests.cs`

### Description
Extend `TaggedUnion` from current 1-3 arity to support 4, 5, and 6 type parameters.

### Implementation Steps

1. **Add** `TaggedUnion<T1, T2, T3, T4>`:
```csharp
public readonly struct TaggedUnion<T1, T2, T3, T4> : IEquatable<TaggedUnion<T1, T2, T3, T4>>
{
    private readonly byte tag;
    private readonly object? value;
    
    private TaggedUnion(byte tag, object? value) { ... }
    
    public static TaggedUnion<T1, T2, T3, T4> From1(T1 value) => new(1, value);
    public static TaggedUnion<T1, T2, T3, T4> From2(T2 value) => new(2, value);
    public static TaggedUnion<T1, T2, T3, T4> From3(T3 value) => new(3, value);
    public static TaggedUnion<T1, T2, T3, T4> From4(T4 value) => new(4, value);
    
    public bool Is1 => tag == 1;
    public bool Is2 => tag == 2;
    public bool Is3 => tag == 3;
    public bool Is4 => tag == 4;
    
    public Option<T1> As1() => Is1 ? Option<T1>.Some((T1)value!) : Option<T1>.None();
    public Option<T2> As2() => Is2 ? Option<T2>.Some((T2)value!) : Option<T2>.None();
    public Option<T3> As3() => Is3 ? Option<T3>.Some((T3)value!) : Option<T3>.None();
    public Option<T4> As4() => Is4 ? Option<T4>.Some((T4)value!) : Option<T4>.None();
    
    public TResult Match<TResult>(
        Func<T1, TResult> case1,
        Func<T2, TResult> case2,
        Func<T3, TResult> case3,
        Func<T4, TResult> case4) => tag switch { ... };
    
    // IEquatable, ToString, GetHashCode...
}
```

2. **Add** `TaggedUnion<T1, T2, T3, T4, T5>` (5 arity) - same pattern

3. **Add** `TaggedUnion<T1, T2, T3, T4, T5, T6>` (6 arity) - same pattern

4. **Add tests** for each new arity:
   - Construction via `FromN`
   - `IsN` property checks
   - `AsN()` returns correct Option
   - `Match` dispatches correctly
   - Equality comparisons

### Acceptance Criteria
- [x] `TaggedUnion<T1, T2, T3, T4>` implemented
- [x] `TaggedUnion<T1, T2, T3, T4, T5>` implemented
- [x] `TaggedUnion<T1, T2, T3, T4, T5, T6>` implemented
- [x] All implement `IEquatable<>` and proper `GetHashCode`
- [x] All tests pass

---

## Task 5: Update Documentation

**Priority**: Low
**Files**:
- `README.md`

### Description
Document the new features and breaking changes.

### Implementation Steps

1. **Add** Breaking Changes section noting `GenericMutexError` removal

2. **Document** `Weak<T>`:
   - Purpose (breaking circular references)
   - `Arc<T>.Downgrade()` usage
   - `Weak<T>.Upgrade()` returns `Option<Arc<T>>`

3. **Document** `RwLockRef<T>`:
   - Purpose (reference types without `IClone`)
   - `WithRead<U>` and `WithWrite` patterns
   - Why no `GetValue()`

4. **Document** extended `TaggedUnion`:
   - Now supports 1-6 type parameters

### Acceptance Criteria
- [x] Breaking changes documented
- [x] All new types documented with examples
- [x] README builds correctly

---

## Execution Order

1. **Task 1**: GenericMutexError removal (enables consistent error handling)
2. **Task 2**: Weak<T> (builds on Arc internals)
3. **Task 3**: RwLockRef<T> (independent, uses SynchronizerError)
4. **Task 4**: TaggedUnion extension (independent)
5. **Task 5**: Documentation (after all features complete)

## Verification

After all tasks:
```bash
dotnet build
dotnet test
```

All tests must pass with no warnings.
