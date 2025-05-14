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
    [ExpectedException(typeof(ArgumentNullException))]
    public void Option_Some_ThrowsOnNullValue()
    {
        _ = Option.Some<string>(null!);
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
    [ExpectedException(typeof(InvalidOperationException))]
    public void Unwrap_ThrowsException_WhenNone()
    {
        var option = Option.None<int>();
        option.Unwrap();
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
}
