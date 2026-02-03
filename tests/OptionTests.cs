using Rustify.Monads;
using System;

namespace Rustify.Tests;

[TestClass]
public sealed class OptionTests // Class name changed for clarity
{
    [TestMethod]
    public void Option_Some_CreatesSomeWithValue()
    {
        var value = "test";
        var option = Option.Some(value);

        Assert.IsTrue(option.IsSome());
        Assert.IsFalse(option.IsNone());
        Assert.AreEqual(value, option.Unwrap());
    }

    [TestMethod]
    public void Option_None_CreatesNone()
    {
        var option = Option.None<string>();

        Assert.IsFalse(option.IsSome());
        Assert.IsTrue(option.IsNone());
    }

    [TestMethod]
    public void Option_Some_ThrowsOnNullValue()
    {
        Assert.Throws<ArgumentNullException>(() => Option.Some<string>(null!));
    }

    [TestMethod]
    public void IsSomeAnd_ReturnsTrue_WhenSomeAndPredicateTrue()
    {
        var option = Option.Some(10);
        Assert.IsTrue(option.IsSomeAnd(x => x > 5));
    }

    [TestMethod]
    public void IsSomeAnd_ReturnsFalse_WhenSomeAndPredicateFalse()
    {
        var option = Option.Some(3);
        Assert.IsFalse(option.IsSomeAnd(x => x > 5));
    }

    [TestMethod]
    public void IsSomeAnd_ReturnsFalse_WhenNone()
    {
        var option = Option.None<int>();
        Assert.IsFalse(option.IsSomeAnd(x => x > 5));
    }

    [TestMethod]
    public void Unwrap_ReturnsValue_WhenSome()
    {
        var value = 42;
        var option = Option.Some(value);
        Assert.AreEqual(value, option.Unwrap());
    }

    [TestMethod]
    public void Unwrap_ThrowsException_WhenNone()
    {
        var option = Option.None<int>();
        Assert.Throws<InvalidOperationException>(() => option.Unwrap());
    }

    [TestMethod]
    public void UnwrapOr_ReturnsValue_WhenSome()
    {
        var value = "hello";
        var fallback = "world";
        var option = Option.Some(value);
        Assert.AreEqual(value, option.UnwrapOr(fallback));
    }

    [TestMethod]
    public void UnwrapOr_ReturnsFallback_WhenNone()
    {
        var fallback = "world";
        var option = Option.None<string>();
        Assert.AreEqual(fallback, option.UnwrapOr(fallback));
    }

    [TestMethod]
    public void UnwrapOrElse_ReturnsValue_WhenSome()
    {
        var value = 100;
        var option = Option.Some(value);
        Assert.AreEqual(value, option.UnwrapOrElse(() => 200));
    }

    [TestMethod]
    public void UnwrapOrElse_ReturnsFallbackResult_WhenNone()
    {
        var fallbackValue = 200;
        var option = Option.None<int>();
        Assert.AreEqual(fallbackValue, option.UnwrapOrElse(() => fallbackValue));
    }

    [TestMethod]
    public void Map_TransformsValue_WhenSome()
    {
        var option = Option.Some(5);
        var mapped = option.Map(x => x.ToString());
        Assert.IsTrue(mapped.IsSome());
        Assert.AreEqual("5", mapped.Unwrap());
    }

    [TestMethod]
    public void Map_ReturnsNone_WhenNone()
    {
        var option = Option.None<int>();
        var mapped = option.Map(x => x.ToString());
        Assert.IsTrue(mapped.IsNone());
    }

    [TestMethod]
    public void AndThen_ReturnsNewOption_WhenSome()
    {
        var option = Option.Some(10);
        var result = option.AndThen(x => Option.Some(x * 2));
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(20, result.Unwrap());
    }

    [TestMethod]
    public void AndThen_ReturnsNone_WhenSomeAndFunctionReturnsNone()
    {
        var option = Option.Some(10);
        var result = option.AndThen(x => Option.None<int>());
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void AndThen_ReturnsNone_WhenNone()
    {
        var option = Option.None<int>();
        var result = option.AndThen(x => Option.Some(x * 2));
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void OrElse_ReturnsSelf_WhenSome()
    {
        var option = Option.Some(1);
        var fallbackOption = Option.Some(2);
        var result = option.OrElse(() => fallbackOption);
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(1, result.Unwrap());
    }

    [TestMethod]
    public void OrElse_ReturnsFallback_WhenNone()
    {
        var option = Option.None<int>();
        var fallbackOption = Option.Some(2);
        var result = option.OrElse(() => fallbackOption);
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(2, result.Unwrap());
    }

    [TestMethod]
    public void OrElse_ReturnsNone_WhenNoneAndFallbackIsNone()
    {
        var option = Option.None<int>();
        var result = option.OrElse(() => Option.None<int>());
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void OkOr_ReturnsOk_WhenSome()
    {
        var option = Option.Some("value");
        var result = option.OkOr("error");
        Assert.IsTrue(result.IsOk); // Corrected: IsOk is a property
        Assert.AreEqual("value", result.Unwrap());
    }

    [TestMethod]
    public void OkOr_ReturnsErr_WhenNone()
    {
        var option = Option.None<string>();
        var error = "error_message";
        var result = option.OkOr(error);
        Assert.IsTrue(result.IsErr); // Corrected: IsErr is a property
        Assert.AreEqual(error, result.UnwrapErr());
    }

    [TestMethod]
    public void OkOrElse_ReturnsOk_WhenSome()
    {
        var option = Option.Some(123);
        var result = option.OkOrElse(() => "fallback_error");
        Assert.IsTrue(result.IsOk); // Corrected: IsOk is a property
        Assert.AreEqual(123, result.Unwrap());
    }

    [TestMethod]
    public void OkOrElse_ReturnsErrFromFunc_WhenNone()
    {
        var option = Option.None<int>();
        var errorMessage = "generated_error";
        var result = option.OkOrElse(() => errorMessage);
        Assert.IsTrue(result.IsErr); // Corrected: IsErr is a property
        Assert.AreEqual(errorMessage, result.UnwrapErr());
    }

    [TestMethod]
    public void Match_ExecutesSomeFunc_WhenSome()
    {
        var option = Option.Some("data");
        var result = option.Match(
            some: s => $"Some: {s}",
            none: () => "None"
        );
        Assert.AreEqual("Some: data", result);
    }

    [TestMethod]
    public void Match_ExecutesNoneFunc_WhenNone()
    {
        var option = Option.None<string>();
        var result = option.Match(
            some: s => $"Some: {s}",
            none: () => "None"
        );
        Assert.AreEqual("None", result);
    }

    [TestMethod]
    public void Filter_ReturnsSome_WhenPredicateTrue()
    {
        var option = Option.Some(10);
        var filtered = option.Filter(x => x > 5);
        Assert.IsTrue(filtered.IsSome());
        Assert.AreEqual(10, filtered.Unwrap());
    }

    [TestMethod]
    public void Filter_ReturnsNone_WhenPredicateFalse()
    {
        var option = Option.Some(3);
        var filtered = option.Filter(x => x > 5);
        Assert.IsTrue(filtered.IsNone());
    }

    [TestMethod]
    public void Filter_ReturnsNone_WhenNone()
    {
        var option = Option.None<int>();
        var filtered = option.Filter(x => x > 5);
        Assert.IsTrue(filtered.IsNone());
    }

    [TestMethod]
    public void Equals_ReturnsTrue_ForTwoSomesWithSameValue()
    {
        var option1 = Option.Some(10);
        var option2 = Option.Some(10);
        Assert.IsTrue(option1.Equals(option2));
        Assert.IsTrue(option1 == option2);
        Assert.IsFalse(option1 != option2);
    }

    [TestMethod]
    public void Equals_ReturnsFalse_ForTwoSomesWithDifferentValues()
    {
        var option1 = Option.Some(10);
        var option2 = Option.Some(20);
        Assert.IsFalse(option1.Equals(option2));
        Assert.IsFalse(option1 == option2);
        Assert.IsTrue(option1 != option2);
    }

    [TestMethod]
    public void Equals_ReturnsTrue_ForTwoNones()
    {
        var option1 = Option.None<int>();
        var option2 = Option.None<int>();
        Assert.IsTrue(option1.Equals(option2));
        Assert.IsTrue(option1 == option2);
        Assert.IsFalse(option1 != option2);
    }

    [TestMethod]
    public void Equals_ReturnsFalse_ForSomeAndNone()
    {
        var option1 = Option.Some(10);
        var option2 = Option.None<int>();
        Assert.IsFalse(option1.Equals(option2));
        Assert.IsFalse(option1 == option2);
        Assert.IsTrue(option1 != option2);
    }

    [TestMethod]
    public void Equals_ReturnsFalse_ForNoneAndSome()
    {
        var option1 = Option.None<int>();
        var option2 = Option.Some(10);
        Assert.IsFalse(option1.Equals(option2));
        Assert.IsFalse(option1 == option2);
        Assert.IsTrue(option1 != option2);
    }

    [TestMethod]
    public void Equals_ReturnsFalse_WhenComparedWithDifferentType()
    {
        var option = Option.Some(10);
        Assert.IsFalse(option.Equals("not an option"));
    }

    [TestMethod]
    public void GetHashCode_ReturnsSameForEqualSomes()
    {
        var option1 = Option.Some(100);
        var option2 = Option.Some(100);
        Assert.AreEqual(option1.GetHashCode(), option2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_ReturnsDifferentForDifferentSomes()
    {
        // It's not strictly required for different objects to have different hash codes,
        // but for simple types like int, it's usually the case.
        var option1 = Option.Some(100);
        var option2 = Option.Some(200);
        Assert.AreNotEqual(option1.GetHashCode(), option2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_ReturnsSameForNones()
    {
        var option1 = Option.None<int>();
        var option2 = Option.None<int>();
        Assert.AreEqual(option1.GetHashCode(), option2.GetHashCode());
    }

    [TestMethod]
    public void Expect_ReturnsValue_WhenSome()
    {
        var option = Option.Some(42);
        Assert.AreEqual(42, option.Expect("Should have a value"));
    }

    [TestMethod]
    public void Expect_ThrowsWithMessage_WhenNone()
    {
        var option = Option.None<int>();
        Assert.Throws<InvalidOperationException>(() => option.Expect("Custom error message"));
    }

    [TestMethod]
    public void Expect_ThrowsCorrectMessage_WhenNone()
    {
        var option = Option.None<int>();
        try
        {
            option.Expect("Custom error message");
            Assert.Fail("Should have thrown");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Custom error message", ex.Message);
        }
    }

    [TestMethod]
    public void UnwrapOrDefault_ReturnsValue_WhenSome()
    {
        var option = Option.Some(42);
        Assert.AreEqual(42, option.UnwrapOrDefault());
    }

    [TestMethod]
    public void UnwrapOrDefault_ReturnsDefault_WhenNone()
    {
        var option = Option.None<int>();
        Assert.AreEqual(0, option.UnwrapOrDefault());
    }

    [TestMethod]
    public void UnwrapOrDefault_ReturnsNull_WhenNoneForReferenceType()
    {
        var option = Option.None<string>();
        Assert.IsNull(option.UnwrapOrDefault());
    }

    [TestMethod]
    public void IsNoneOr_ReturnsTrue_WhenNone()
    {
        var option = Option.None<int>();
        Assert.IsTrue(option.IsNoneOr(x => x > 100));
    }

    [TestMethod]
    public void IsNoneOr_ReturnsTrue_WhenSomeAndPredicateTrue()
    {
        var option = Option.Some(10);
        Assert.IsTrue(option.IsNoneOr(x => x > 5));
    }

    [TestMethod]
    public void IsNoneOr_ReturnsFalse_WhenSomeAndPredicateFalse()
    {
        var option = Option.Some(3);
        Assert.IsFalse(option.IsNoneOr(x => x > 5));
    }

    [TestMethod]
    public void MapOr_ReturnsMappedValue_WhenSome()
    {
        var option = Option.Some(5);
        var result = option.MapOr("default", x => $"Value: {x}");
        Assert.AreEqual("Value: 5", result);
    }

    [TestMethod]
    public void MapOr_ReturnsDefault_WhenNone()
    {
        var option = Option.None<int>();
        var result = option.MapOr("default", x => $"Value: {x}");
        Assert.AreEqual("default", result);
    }

    [TestMethod]
    public void MapOrElse_ReturnsMappedValue_WhenSome()
    {
        var option = Option.Some(5);
        var result = option.MapOrElse(() => "computed default", x => $"Value: {x}");
        Assert.AreEqual("Value: 5", result);
    }

    [TestMethod]
    public void MapOrElse_ReturnsComputedDefault_WhenNone()
    {
        var option = Option.None<int>();
        var result = option.MapOrElse(() => "computed default", x => $"Value: {x}");
        Assert.AreEqual("computed default", result);
    }

    [TestMethod]
    public void Or_ReturnsSelf_WhenSome()
    {
        var option = Option.Some(1);
        var other = Option.Some(2);
        var result = option.Or(other);
        Assert.AreEqual(1, result.Unwrap());
    }

    [TestMethod]
    public void Or_ReturnsOther_WhenNone()
    {
        var option = Option.None<int>();
        var other = Option.Some(2);
        var result = option.Or(other);
        Assert.AreEqual(2, result.Unwrap());
    }

    [TestMethod]
    public void Or_ReturnsNone_WhenBothNone()
    {
        var option = Option.None<int>();
        var other = Option.None<int>();
        var result = option.Or(other);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void Xor_ReturnsSelf_WhenSomeAndOtherNone()
    {
        var option = Option.Some(1);
        var other = Option.None<int>();
        var result = option.Xor(other);
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(1, result.Unwrap());
    }

    [TestMethod]
    public void Xor_ReturnsOther_WhenNoneAndOtherSome()
    {
        var option = Option.None<int>();
        var other = Option.Some(2);
        var result = option.Xor(other);
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(2, result.Unwrap());
    }

    [TestMethod]
    public void Xor_ReturnsNone_WhenBothSome()
    {
        var option = Option.Some(1);
        var other = Option.Some(2);
        var result = option.Xor(other);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void Xor_ReturnsNone_WhenBothNone()
    {
        var option = Option.None<int>();
        var other = Option.None<int>();
        var result = option.Xor(other);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void Zip_ReturnsTuple_WhenBothSome()
    {
        var option1 = Option.Some(1);
        var option2 = Option.Some("one");
        var result = option1.Zip(option2);
        Assert.IsTrue(result.IsSome());
        var tuple = result.Unwrap();
        Assert.AreEqual(1, tuple.Item1);
        Assert.AreEqual("one", tuple.Item2);
    }

    [TestMethod]
    public void Zip_ReturnsNone_WhenFirstNone()
    {
        var option1 = Option.None<int>();
        var option2 = Option.Some("one");
        var result = option1.Zip(option2);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void Zip_ReturnsNone_WhenSecondNone()
    {
        var option1 = Option.Some(1);
        var option2 = Option.None<string>();
        var result = option1.Zip(option2);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void ZipWith_ReturnsCombinedValue_WhenBothSome()
    {
        var option1 = Option.Some(2);
        var option2 = Option.Some(3);
        var result = option1.ZipWith(option2, (a, b) => a * b);
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(6, result.Unwrap());
    }

    [TestMethod]
    public void ZipWith_ReturnsNone_WhenFirstNone()
    {
        var option1 = Option.None<int>();
        var option2 = Option.Some(3);
        var result = option1.ZipWith(option2, (a, b) => a * b);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void ZipWith_ReturnsNone_WhenSecondNone()
    {
        var option1 = Option.Some(2);
        var option2 = Option.None<int>();
        var result = option1.ZipWith(option2, (a, b) => a * b);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void Inspect_ExecutesAction_WhenSome()
    {
        var option = Option.Some(42);
        var inspected = 0;
        var result = option.Inspect(x => inspected = x);
        Assert.AreEqual(42, inspected);
        Assert.IsTrue(result.IsSome());
    }

    [TestMethod]
    public void Inspect_DoesNotExecuteAction_WhenNone()
    {
        var option = Option.None<int>();
        var executed = false;
        var result = option.Inspect(x => executed = true);
        Assert.IsFalse(executed);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void Flatten_ReturnsInnerOption_WhenOuterIsSome()
    {
        var inner = Option.Some(42);
        var outer = Option.Some(inner);
        var flattened = Option<int>.Flatten(outer);
        Assert.IsTrue(flattened.IsSome());
        Assert.AreEqual(42, flattened.Unwrap());
    }

    [TestMethod]
    public void Flatten_ReturnsNone_WhenOuterIsNone()
    {
        var outer = Option.None<Option<int>>();
        var flattened = Option<int>.Flatten(outer);
        Assert.IsTrue(flattened.IsNone());
    }

    [TestMethod]
    public void Flatten_ReturnsNone_WhenInnerIsNone()
    {
        var inner = Option.None<int>();
        var outer = Option.Some(inner);
        var flattened = Option<int>.Flatten(outer);
        Assert.IsTrue(flattened.IsNone());
    }

    [TestMethod]
    public void ToString_ReturnsSomeWithValue_WhenSome()
    {
        var option = Option.Some(42);
        Assert.AreEqual("Some(42)", option.ToString());
    }

    [TestMethod]
    public void ToString_ReturnsNone_WhenNone()
    {
        var option = Option.None<int>();
        Assert.AreEqual("None", option.ToString());
    }

    [TestMethod]
    public void IEnumerable_YieldsValue_WhenSome()
    {
        var option = Option.Some(42);
        var count = 0;
        var sum = 0;
        foreach (var value in option)
        {
            count++;
            sum += value;
        }
        Assert.AreEqual(1, count);
        Assert.AreEqual(42, sum);
    }

    [TestMethod]
    public void IEnumerable_YieldsNothing_WhenNone()
    {
        var option = Option.None<int>();
        var count = 0;
        foreach (var value in option)
        {
            count++;
        }
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void IEnumerable_WorksWithLinq_WhenSome()
    {
        var option = Option.Some(10);
        var doubled = option.Select(x => x * 2).FirstOrDefault();
        Assert.AreEqual(20, doubled);
    }

    [TestMethod]
    public void IEnumerable_WorksWithLinq_WhenNone()
    {
        var option = Option.None<int>();
        var result = option.Select(x => x * 2).FirstOrDefault();
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void CompareTo_ReturnsZero_ForTwoNones()
    {
        var option1 = Option.None<int>();
        var option2 = Option.None<int>();
        Assert.AreEqual(0, option1.CompareTo(option2));
    }

    [TestMethod]
    public void CompareTo_ReturnsNegative_WhenNoneComparedToSome()
    {
        var none = Option.None<int>();
        var some = Option.Some(10);
        Assert.IsTrue(none.CompareTo(some) < 0);
    }

    [TestMethod]
    public void CompareTo_ReturnsPositive_WhenSomeComparedToNone()
    {
        var some = Option.Some(10);
        var none = Option.None<int>();
        Assert.IsTrue(some.CompareTo(none) > 0);
    }

    [TestMethod]
    public void CompareTo_ReturnsZero_ForTwoSomesWithSameValue()
    {
        var option1 = Option.Some(42);
        var option2 = Option.Some(42);
        Assert.AreEqual(0, option1.CompareTo(option2));
    }

    [TestMethod]
    public void CompareTo_ReturnsNegative_WhenSmallerValue()
    {
        var option1 = Option.Some(10);
        var option2 = Option.Some(20);
        Assert.IsTrue(option1.CompareTo(option2) < 0);
    }

    [TestMethod]
    public void CompareTo_ReturnsPositive_WhenLargerValue()
    {
        var option1 = Option.Some(20);
        var option2 = Option.Some(10);
        Assert.IsTrue(option1.CompareTo(option2) > 0);
    }

    [TestMethod]
    public void CompareTo_WorksWithStrings()
    {
        var option1 = Option.Some("apple");
        var option2 = Option.Some("banana");
        Assert.IsTrue(option1.CompareTo(option2) < 0);
    }

    [TestMethod]
    public void IEquatable_Equals_WorksCorrectly()
    {
        IEquatable<Option<int>> option1 = Option.Some(42);
        var option2 = Option.Some(42);
        Assert.IsTrue(option1.Equals(option2));
    }

    [TestMethod]
    public void IComparable_CompareTo_WorksCorrectly()
    {
        IComparable<Option<int>> option1 = Option.Some(10);
        var option2 = Option.Some(20);
        Assert.IsTrue(option1.CompareTo(option2) < 0);
    }

    [TestMethod]
    public void LinqSelect_TransformsValue_WhenSome()
    {
        var option = Option.Some(5);
        var result = option.Select(x => x * 2);
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(10, result.Unwrap());
    }

    [TestMethod]
    public void LinqSelect_ReturnsNone_WhenNone()
    {
        var option = Option.None<int>();
        var result = option.Select(x => x * 2);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void LinqSelectMany_ChainsOptions_WhenBothSome()
    {
        var option = Option.Some(5);
        var result = option.SelectMany(x => Option.Some(x * 2));
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(10, result.Unwrap());
    }

    [TestMethod]
    public void LinqSelectMany_ReturnsNone_WhenFirstNone()
    {
        var option = Option.None<int>();
        var result = option.SelectMany(x => Option.Some(x * 2));
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void LinqSelectMany_ReturnsNone_WhenSelectorReturnsNone()
    {
        var option = Option.Some(5);
        var result = option.SelectMany(x => Option.None<int>());
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void LinqQuerySyntax_WorksWithOptions()
    {
        var option1 = Option.Some(10);
        var option2 = Option.Some(20);

        var result = from a in option1
                     from b in option2
                     select a + b;

        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(30, result.Unwrap());
    }

    [TestMethod]
    public void LinqQuerySyntax_ReturnsNone_WhenAnyIsNone()
    {
        var option1 = Option.Some(10);
        var option2 = Option.None<int>();

        var result = from a in option1
                     from b in option2
                     select a + b;

        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void LinqWhere_FiltersSome()
    {
        var option = Option.Some(10);
        var result = option.Where(x => x > 5);
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(10, result.Unwrap());
    }

    [TestMethod]
    public void LinqWhere_ReturnsNone_WhenPredicateFalse()
    {
        var option = Option.Some(3);
        var result = option.Where(x => x > 5);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public void Contains_ReturnsTrue_WhenSomeWithMatchingValue()
    {
        var option = Option.Some(42);
        Assert.IsTrue(option.Contains(42));
    }

    [TestMethod]
    public void Contains_ReturnsFalse_WhenSomeWithDifferentValue()
    {
        var option = Option.Some(42);
        Assert.IsFalse(option.Contains(100));
    }

    [TestMethod]
    public void Contains_ReturnsFalse_WhenNone()
    {
        var option = Option.None<int>();
        Assert.IsFalse(option.Contains(42));
    }

    [TestMethod]
    public void Contains_WorksWithStrings()
    {
        var option = Option.Some("hello");
        Assert.IsTrue(option.Contains("hello"));
        Assert.IsFalse(option.Contains("world"));
    }

    [TestMethod]
    public async Task MapAsync_TransformsValue_WhenSome()
    {
        var option = Option.Some(5);
        var result = await option.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(10, result.Unwrap());
    }

    [TestMethod]
    public async Task MapAsync_ReturnsNone_WhenNone()
    {
        var option = Option.None<int>();
        var result = await option.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public async Task AndThenAsync_ChainsOptions_WhenBothSome()
    {
        var option = Option.Some(5);
        var result = await option.AndThenAsync(async x =>
        {
            await Task.Delay(1);
            return Option.Some(x * 2);
        });
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(10, result.Unwrap());
    }

    [TestMethod]
    public async Task AndThenAsync_ReturnsNone_WhenSelectorReturnsNone()
    {
        var option = Option.Some(5);
        var result = await option.AndThenAsync(async x =>
        {
            await Task.Delay(1);
            return Option.None<int>();
        });
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public async Task TaskMapAsync_TransformsValue_FromTask()
    {
        var optionTask = Task.FromResult(Option.Some(5));
        var result = await optionTask.MapAsync(x => x * 2);
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(10, result.Unwrap());
    }

    [TestMethod]
    public async Task UnwrapOrAsync_ReturnsValue_WhenSome()
    {
        var optionTask = Task.FromResult(Option.Some(42));
        var result = await optionTask.UnwrapOrAsync(0);
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public async Task UnwrapOrAsync_ReturnsDefault_WhenNone()
    {
        var optionTask = Task.FromResult(Option.None<int>());
        var result = await optionTask.UnwrapOrAsync(99);
        Assert.AreEqual(99, result);
    }

    #region Additional Async Tests

    [TestMethod]
    public async Task MapAsync_DoesNotCallSelector_WhenNone()
    {
        var option = Option.None<int>();
        bool selectorCalled = false;

        var result = await option.MapAsync(async x =>
        {
            selectorCalled = true;
            await Task.Delay(1);
            return x * 2;
        });

        Assert.IsFalse(selectorCalled);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public async Task AndThenAsync_DoesNotCallSelector_WhenNone()
    {
        var option = Option.None<int>();
        bool selectorCalled = false;

        var result = await option.AndThenAsync(async x =>
        {
            selectorCalled = true;
            await Task.Delay(1);
            return Option.Some(x * 2);
        });

        Assert.IsFalse(selectorCalled);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public async Task MapAsync_ChainedOperations()
    {
        var option = Option.Some(5);

        var result = await option
            .MapAsync(async x =>
            {
                await Task.Delay(1);
                return x * 2;
            });

        var finalResult = await result
            .MapAsync(async x =>
            {
                await Task.Delay(1);
                return x + 10;
            });

        Assert.IsTrue(finalResult.IsSome());
        Assert.AreEqual(20, finalResult.Unwrap());
    }

    [TestMethod]
    public async Task TaskMapAsync_ReturnsNone_WhenTaskContainsNone()
    {
        var optionTask = Task.FromResult(Option.None<int>());
        var result = await optionTask.MapAsync(x => x * 2);
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public async Task TaskAndThenAsync_ChainsCorrectly()
    {
        var optionTask = Task.FromResult(Option.Some(5));
        var result = await optionTask.AndThenAsync(x => Option.Some(x * 2));
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(10, result.Unwrap());
    }

    [TestMethod]
    public async Task TaskAndThenAsync_ReturnsNone_WhenTaskContainsNone()
    {
        var optionTask = Task.FromResult(Option.None<int>());
        var result = await optionTask.AndThenAsync(x => Option.Some(x * 2));
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public async Task TaskAndThenAsync_ReturnsNone_WhenSelectorReturnsNone()
    {
        var optionTask = Task.FromResult(Option.Some(5));
        var result = await optionTask.AndThenAsync(x => Option.None<int>());
        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public async Task MapAsync_WithStringTransformation()
    {
        var option = Option.Some("hello");
        var result = await option.MapAsync(async s =>
        {
            await Task.Delay(1);
            return s.ToUpper();
        });
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual("HELLO", result.Unwrap());
    }

    [TestMethod]
    public async Task MapAsync_WithTypeConversion()
    {
        var option = Option.Some(42);
        var result = await option.MapAsync(async n =>
        {
            await Task.Delay(1);
            return n.ToString();
        });
        Assert.IsTrue(result.IsSome());
        Assert.AreEqual("42", result.Unwrap());
    }

    [TestMethod]
    public async Task AndThenAsync_WithValidation()
    {
        var option = Option.Some(10);

        var result = await option.AndThenAsync(async x =>
        {
            await Task.Delay(1);
            return x > 5 ? Option.Some(x) : Option.None<int>();
        });

        Assert.IsTrue(result.IsSome());
        Assert.AreEqual(10, result.Unwrap());
    }

    [TestMethod]
    public async Task AndThenAsync_WithValidationFailure()
    {
        var option = Option.Some(3);

        var result = await option.AndThenAsync(async x =>
        {
            await Task.Delay(1);
            return x > 5 ? Option.Some(x) : Option.None<int>();
        });

        Assert.IsTrue(result.IsNone());
    }

    [TestMethod]
    public async Task UnwrapOrAsync_WithReferenceType()
    {
        var optionTask = Task.FromResult(Option.Some("value"));
        var result = await optionTask.UnwrapOrAsync("default");
        Assert.AreEqual("value", result);
    }

    [TestMethod]
    public async Task UnwrapOrAsync_WithReferenceType_WhenNone()
    {
        var optionTask = Task.FromResult(Option.None<string>());
        var result = await optionTask.UnwrapOrAsync("default");
        Assert.AreEqual("default", result);
    }

    [TestMethod]
    public async Task MapAsync_ParallelExecution()
    {
        var options = new[]
        {
            Option.Some(1),
            Option.Some(2),
            Option.Some(3),
            Option.None<int>(),
            Option.Some(5)
        };

        var tasks = options.Select(opt => opt.MapAsync(async x =>
        {
            await Task.Delay(10);
            return x * 10;
        }));

        var results = await Task.WhenAll(tasks);

        Assert.AreEqual(10, results[0].UnwrapOr(0));
        Assert.AreEqual(20, results[1].UnwrapOr(0));
        Assert.AreEqual(30, results[2].UnwrapOr(0));
        Assert.AreEqual(0, results[3].UnwrapOr(0));
        Assert.AreEqual(50, results[4].UnwrapOr(0));
    }

    #endregion
}
