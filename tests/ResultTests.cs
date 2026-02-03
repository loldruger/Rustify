using Rustify.Monads;
using System;

namespace Rustify.Tests;

[TestClass]
public sealed class ResultTests
{
    private const string DefaultError = "DefaultError";
    private const string DefaultOkValue = "DefaultOk";

    [TestMethod]
    public void Result_Ok_CreatesOkWithValue()
    {
        var value = "test_ok";
        var result = Result.Ok<string, string>(value);

        Assert.IsTrue(result.IsOk());
        Assert.IsFalse(result.IsErr());
        Assert.AreEqual(value, result.Unwrap());
    }

    [TestMethod]
    public void Result_Err_CreatesErrWithError()
    {
        var error = "test_err";
        var result = Result.Err<string, string>(error);

        Assert.IsFalse(result.IsOk());
        Assert.IsTrue(result.IsErr());
        Assert.AreEqual(error, result.UnwrapErr());
    }

    // Implicit conversion tests removed - implicit operators removed in v0.4.0

    [TestMethod]
    public void Unwrap_ReturnsValue_WhenOk()
    {
        var value = 123;
        var result = Result.Ok<int, string>(value);
        Assert.AreEqual(value, result.Unwrap());
    }

    [TestMethod]
    public void Unwrap_ThrowsException_WhenErr()
    {
        var result = Result.Err<int, string>(DefaultError);
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    [TestMethod]
    public void UnwrapErr_ReturnsError_WhenErr()
    {
        var error = "custom_error";
        var result = Result.Err<int, string>(error);
        Assert.AreEqual(error, result.UnwrapErr());
    }

    [TestMethod]
    public void UnwrapErr_ThrowsException_WhenOk()
    {
        var result = Result.Ok<int, string>(123);
        Assert.Throws<InvalidOperationException>(() => result.UnwrapErr());
    }

    [TestMethod]
    public void Match_ExecutesSuccessFunc_WhenOk()
    {
        var result = Result.Ok<string, int>(DefaultOkValue);
        var matchedValue = result.Match(
            success: s => $"Ok: {s}",
            failure: e => $"Err: {e}"
        );
        Assert.AreEqual($"Ok: {DefaultOkValue}", matchedValue);
    }

    [TestMethod]
    public void Match_ExecutesFailureFunc_WhenErr()
    {
        var result = Result.Err<string, int>(777);
        var matchedValue = result.Match(
            success: s => $"Ok: {s}",
            failure: e => $"Err: {e}"
        );
        Assert.AreEqual("Err: 777", matchedValue);
    }

    [TestMethod]
    public void AndThen_ReturnsNewResult_WhenOk()
    {
        var result = Result.Ok<int, string>(10);
        var nextResult = result.AndThen(x => Result.Ok<int, string>(x * 2));
        Assert.IsTrue(nextResult.IsOk());
        Assert.AreEqual(20, nextResult.Unwrap());
    }

    [TestMethod]
    public void AndThen_ReturnsSelf_WhenErr()
    {
        var result = Result.Err<int, string>(DefaultError);
        var nextResult = result.AndThen(x => Result.Ok<int, string>(x * 2)); // This function should not be called
        Assert.IsTrue(nextResult.IsErr());
        Assert.AreEqual(DefaultError, nextResult.UnwrapErr());
    }

    [TestMethod]
    public void AndThen_ReturnsNewErr_WhenOkAndFuncReturnsErr()
    {
        var result = Result.Ok<int, string>(10);
        var newError = "new_error_from_andthen";
        var nextResult = result.AndThen(x => Result.Err<int, string>(newError));
        Assert.IsTrue(nextResult.IsErr());
        Assert.AreEqual(newError, nextResult.UnwrapErr());
    }
    
    [TestMethod]
    public void OrElse_ReturnsSelf_WhenOk()
    {
        var result = Result.Ok<string, int>(DefaultOkValue);
        var nextResult = result.OrElse(e => Result.Ok<string, string>($"fallback_from_{e}")); // This function should not be called
        Assert.IsTrue(nextResult.IsOk());
        Assert.AreEqual(DefaultOkValue, nextResult.Unwrap());
    }

    [TestMethod]
    public void OrElse_ReturnsNewResult_WhenErr()
    {
        var initialError = 404;
        var result = Result.Err<string, int>(initialError);
        var fallbackValue = "fallback_value";
        var nextResult = result.OrElse(e => Result.Ok<string, string>(fallbackValue));
        Assert.IsTrue(nextResult.IsOk());
        Assert.AreEqual(fallbackValue, nextResult.Unwrap());
    }

    [TestMethod]
    public void OrElse_ReturnsNewErr_WhenErrAndFuncReturnsErr()
    {
        var initialError = 404;
        var newErrorFromOrElse = "new_error_from_orelse";
        var result = Result.Err<string, int>(initialError);
        var nextResult = result.OrElse(e => Result.Err<string, string>(newErrorFromOrElse));
        Assert.IsTrue(nextResult.IsErr());
        Assert.AreEqual(newErrorFromOrElse, nextResult.UnwrapErr());
    }

    [TestMethod]
    public void Ok_ReturnsSome_WhenResultIsOk()
    {
        var result = Result.Ok<string, int>(DefaultOkValue);
        var option = result.Ok();
        Assert.IsTrue(option.IsSome());
        Assert.AreEqual(DefaultOkValue, option.Unwrap());
    }

    [TestMethod]
    public void Ok_ReturnsNone_WhenResultIsErr()
    {
        var result = Result.Err<string, int>(123);
        var option = result.Ok();
        Assert.IsTrue(option.IsNone());
    }

    [TestMethod]
    public void Err_ReturnsSome_WhenResultIsErr()
    {
        var result = Result.Err<string, int>(123);
        var option = result.Err();
        Assert.IsTrue(option.IsSome());
        Assert.AreEqual(123, option.Unwrap());
    }

    [TestMethod]
    public void Err_ReturnsNone_WhenResultIsOk()
    {
        var result = Result.Ok<string, int>(DefaultOkValue);
        var option = result.Err();
        Assert.IsTrue(option.IsNone());
    }

     [TestMethod]
     public void Map_TransformsValue_WhenOk()
     {
         var result = Result.Ok<int, string>(5);
         var mappedResult = result.Map(x => x * 10);
         Assert.IsTrue(mappedResult.IsOk());
         Assert.AreEqual(50, mappedResult.Unwrap());
     }

     [TestMethod]
     public void Map_ReturnsErr_WhenErr()
     {
         var result = Result.Err<int, string>(DefaultError);
         var mappedResult = result.Map(x => x * 10); // This function should not be called
         Assert.IsTrue(mappedResult.IsErr());
         Assert.AreEqual(DefaultError, mappedResult.UnwrapErr());
     }

     [TestMethod]
     public void MapErr_TransformsError_WhenErr()
     {
         var result = Result.Err<string, int>(100);
         var mappedResult = result.MapErr(e => $"Error code: {e}");
         Assert.IsTrue(mappedResult.IsErr());
         Assert.AreEqual("Error code: 100", mappedResult.UnwrapErr());
     }

     [TestMethod]
     public void MapErr_ReturnsOk_WhenOk()
     {
         var result = Result.Ok<string, int>(DefaultOkValue);
         var mappedResult = result.MapErr(e => $"Error code: {e}"); // This function should not be called
         Assert.IsTrue(mappedResult.IsOk());
         Assert.AreEqual(DefaultOkValue, mappedResult.Unwrap());
     }
    
    [TestMethod]
    public void MapOr_ReturnsMappedValue_WhenOk()
    {
        var result = Result.Ok<int, string>(7);
        var mappedValue = result.MapOr("default", x => (x * x).ToString());
        Assert.AreEqual("49", mappedValue);
    }

    [TestMethod]
    public void MapOr_ReturnsDefaultValue_WhenErr()
    {
        var result = Result.Err<int, string>(DefaultError);
        var mappedValue = result.MapOr("default_if_err", x => (x * x).ToString());
        Assert.AreEqual("default_if_err", mappedValue);
    }

    [TestMethod]
    public void MapOrElse_ReturnsOkFnResult_WhenOk()
    {
        var result = Result.Ok<int, string>(3);
        var mappedValue = result.MapOrElse(
            errFn: e => $"Error was: {e}",
            okFn: x => $"Success: {x + 1}"
        );
        Assert.AreEqual("Success: 4", mappedValue);
    }

    [TestMethod]
    public void MapOrElse_ReturnsErrFnResult_WhenErr()
    {
        var result = Result.Err<int, string>("BadInput");
        var mappedValue = result.MapOrElse(
            errFn: e => $"Error was: {e}",
            okFn: x => $"Success: {x + 1}"
        );
        Assert.AreEqual("Error was: BadInput", mappedValue);
    }

     [TestMethod]
     public void Flatten_ReturnsInnerOk_WhenOuterAndInnerAreOk()
     {
         var innerOk = Result.Ok<int, string>(100);
         var outerOk = Result.Ok<Result<int, string>, string>(innerOk);
         var flattened = Result<int, string>.Flatten(outerOk);
         Assert.IsTrue(flattened.IsOk());
         Assert.AreEqual(100, flattened.Unwrap());
     }

     [TestMethod]
     public void Flatten_ReturnsInnerErr_WhenOuterIsOkAndInnerIsErr()
     {
         var innerErr = Result.Err<int, string>("InnerError");
         var outerOk = Result.Ok<Result<int, string>, string>(innerErr);
         var flattened = Result<int, string>.Flatten(outerOk);
         Assert.IsTrue(flattened.IsErr());
         Assert.AreEqual("InnerError", flattened.UnwrapErr());
     }

     [TestMethod]
     public void Flatten_ReturnsOuterErr_WhenOuterIsErr()
     {
         var outerErr = Result.Err<Result<int, string>, string>("OuterError");
         var flattened = Result<int, string>.Flatten(outerErr);
         Assert.IsTrue(flattened.IsErr());
         Assert.AreEqual("OuterError", flattened.UnwrapErr());
     }

     [TestMethod]
     public void Transpose_ReturnsSomeOk_WhenResultIsOkWithSome()
     {
         var optionSome = Option.Some(DefaultOkValue);
         var resultOkOptionSome = Result.Ok<Option<string>, int>(optionSome);
         var transposed = Result<string, int>.Transpose(resultOkOptionSome);

         Assert.IsTrue(transposed.IsSome());
         var innerResult = transposed.Unwrap();
         Assert.IsTrue(innerResult.IsOk());
         Assert.AreEqual(DefaultOkValue, innerResult.Unwrap());
     }

    [TestMethod]
    public void Transpose_ReturnsNone_WhenResultIsOkWithNone()
    {
        var optionNone = Option.None<string>();
        var resultOkOptionNone = Result.Ok<Option<string>, int>(optionNone);
        var transposed = Result<string, int>.Transpose(resultOkOptionNone);

        Assert.IsTrue(transposed.IsNone());
    }

    [TestMethod]
    public void Transpose_ReturnsSomeErr_WhenResultIsErr()
    {
        var errorValue = 42;
        var resultErr = Result.Err<Option<string>, int>(errorValue);
        var transposed = Result<string, int>.Transpose(resultErr);

        Assert.IsTrue(transposed.IsSome());
        var innerResult = transposed.Unwrap();
        Assert.IsTrue(innerResult.IsErr());
        Assert.AreEqual(errorValue, innerResult.UnwrapErr());
    }

    [TestMethod]
    public void Expect_ReturnsValue_WhenOk()
    {
        var result = Result.Ok<int, string>(42);
        Assert.AreEqual(42, result.Expect("Should have value"));
    }

    [TestMethod]
    public void Expect_ThrowsWithMessage_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        Assert.Throws<InvalidOperationException>(() => result.Expect("Custom error message"));
    }

    [TestMethod]
    public void Expect_ThrowsCorrectMessage_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        try
        {
            result.Expect("Custom error message");
            Assert.Fail("Should have thrown");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Custom error message", ex.Message);
        }
    }

    [TestMethod]
    public void ExpectErr_ReturnsError_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        Assert.AreEqual("error", result.ExpectErr("Should have error"));
    }

    [TestMethod]
    public void ExpectErr_ThrowsWithMessage_WhenOk()
    {
        var result = Result.Ok<int, string>(42);
        Assert.Throws<InvalidOperationException>(() => result.ExpectErr("Custom error message"));
    }

    [TestMethod]
    public void UnwrapOr_ReturnsValue_WhenOk()
    {
        var result = Result.Ok<int, string>(42);
        Assert.AreEqual(42, result.UnwrapOr(100));
    }

    [TestMethod]
    public void UnwrapOr_ReturnsFallback_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        Assert.AreEqual(100, result.UnwrapOr(100));
    }

    [TestMethod]
    public void UnwrapOrElse_ReturnsValue_WhenOk()
    {
        var result = Result.Ok<int, string>(42);
        Assert.AreEqual(42, result.UnwrapOrElse(e => e.Length));
    }

    [TestMethod]
    public void UnwrapOrElse_ReturnsFallbackFromError_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        Assert.AreEqual(5, result.UnwrapOrElse(e => e.Length));
    }

    [TestMethod]
    public void UnwrapOrDefault_ReturnsValue_WhenOk()
    {
        var result = Result.Ok<int, string>(42);
        Assert.AreEqual(42, result.UnwrapOrDefault());
    }

    [TestMethod]
    public void UnwrapOrDefault_ReturnsDefault_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        Assert.AreEqual(0, result.UnwrapOrDefault());
    }

    [TestMethod]
    public void UnwrapOrDefault_ReturnsNull_WhenErrForReferenceType()
    {
        var result = Result.Err<string, int>(42);
        Assert.IsNull(result.UnwrapOrDefault());
    }

    [TestMethod]
    public void IsOkAnd_ReturnsTrue_WhenOkAndPredicateTrue()
    {
        var result = Result.Ok<int, string>(10);
        Assert.IsTrue(result.IsOkAnd(x => x > 5));
    }

    [TestMethod]
    public void IsOkAnd_ReturnsFalse_WhenOkAndPredicateFalse()
    {
        var result = Result.Ok<int, string>(3);
        Assert.IsFalse(result.IsOkAnd(x => x > 5));
    }

    [TestMethod]
    public void IsOkAnd_ReturnsFalse_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        Assert.IsFalse(result.IsOkAnd(x => x > 5));
    }

    [TestMethod]
    public void IsErrAnd_ReturnsTrue_WhenErrAndPredicateTrue()
    {
        var result = Result.Err<int, string>("error");
        Assert.IsTrue(result.IsErrAnd(e => e.Length > 3));
    }

    [TestMethod]
    public void IsErrAnd_ReturnsFalse_WhenErrAndPredicateFalse()
    {
        var result = Result.Err<int, string>("err");
        Assert.IsFalse(result.IsErrAnd(e => e.Length > 5));
    }

    [TestMethod]
    public void IsErrAnd_ReturnsFalse_WhenOk()
    {
        var result = Result.Ok<int, string>(42);
        Assert.IsFalse(result.IsErrAnd(e => e.Length > 0));
    }

    [TestMethod]
    public void And_ReturnsOther_WhenOk()
    {
        var result1 = Result.Ok<int, string>(1);
        var result2 = Result.Ok<string, string>("hello");
        var combined = result1.And(result2);
        Assert.IsTrue(combined.IsOk());
        Assert.AreEqual("hello", combined.Unwrap());
    }

    [TestMethod]
    public void And_ReturnsErr_WhenFirstIsErr()
    {
        var result1 = Result.Err<int, string>("error");
        var result2 = Result.Ok<string, string>("hello");
        var combined = result1.And(result2);
        Assert.IsTrue(combined.IsErr());
        Assert.AreEqual("error", combined.UnwrapErr());
    }

    [TestMethod]
    public void And_ReturnsOtherErr_WhenOkAndOtherIsErr()
    {
        var result1 = Result.Ok<int, string>(1);
        var result2 = Result.Err<string, string>("other_error");
        var combined = result1.And(result2);
        Assert.IsTrue(combined.IsErr());
        Assert.AreEqual("other_error", combined.UnwrapErr());
    }

    [TestMethod]
    public void Or_ReturnsSelf_WhenOk()
    {
        var result1 = Result.Ok<int, string>(1);
        var result2 = Result.Ok<int, string>(2);
        var combined = result1.Or(result2);
        Assert.IsTrue(combined.IsOk());
        Assert.AreEqual(1, combined.Unwrap());
    }

    [TestMethod]
    public void Or_ReturnsOther_WhenErr()
    {
        var result1 = Result.Err<int, string>("error");
        var result2 = Result.Ok<int, string>(2);
        var combined = result1.Or(result2);
        Assert.IsTrue(combined.IsOk());
        Assert.AreEqual(2, combined.Unwrap());
    }

    [TestMethod]
    public void Or_ReturnsOtherErr_WhenBothErr()
    {
        var result1 = Result.Err<int, string>("error1");
        var result2 = Result.Err<int, string>("error2");
        var combined = result1.Or(result2);
        Assert.IsTrue(combined.IsErr());
        Assert.AreEqual("error2", combined.UnwrapErr());
    }

    [TestMethod]
    public void Inspect_ExecutesAction_WhenOk()
    {
        var result = Result.Ok<int, string>(42);
        var inspected = 0;
        var returned = result.Inspect(x => inspected = x);
        Assert.AreEqual(42, inspected);
        Assert.IsTrue(returned.IsOk());
    }

    [TestMethod]
    public void Inspect_DoesNotExecuteAction_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        var executed = false;
        var returned = result.Inspect(x => executed = true);
        Assert.IsFalse(executed);
        Assert.IsTrue(returned.IsErr());
    }

    [TestMethod]
    public void InspectErr_ExecutesAction_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        var inspected = "";
        var returned = result.InspectErr(e => inspected = e);
        Assert.AreEqual("error", inspected);
        Assert.IsTrue(returned.IsErr());
    }

    [TestMethod]
    public void InspectErr_DoesNotExecuteAction_WhenOk()
    {
        var result = Result.Ok<int, string>(42);
        var executed = false;
        var returned = result.InspectErr(e => executed = true);
        Assert.IsFalse(executed);
        Assert.IsTrue(returned.IsOk());
    }

    [TestMethod]
    public void ToString_ReturnsOkWithValue_WhenOk()
    {
        var result = Result.Ok<int, string>(42);
        Assert.AreEqual("Ok(42)", result.ToString());
    }

    [TestMethod]
    public void ToString_ReturnsErrWithError_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        Assert.AreEqual("Err(error)", result.ToString());
    }

    [TestMethod]
    public void Equals_ReturnsTrue_ForTwoOksWithSameValue()
    {
        var result1 = Result.Ok<int, string>(42);
        var result2 = Result.Ok<int, string>(42);
        Assert.IsTrue(result1.Equals(result2));
        Assert.IsTrue(result1 == result2);
        Assert.IsFalse(result1 != result2);
    }

    [TestMethod]
    public void Equals_ReturnsFalse_ForTwoOksWithDifferentValues()
    {
        var result1 = Result.Ok<int, string>(42);
        var result2 = Result.Ok<int, string>(100);
        Assert.IsFalse(result1.Equals(result2));
        Assert.IsFalse(result1 == result2);
        Assert.IsTrue(result1 != result2);
    }

    [TestMethod]
    public void Equals_ReturnsTrue_ForTwoErrsWithSameError()
    {
        var result1 = Result.Err<int, string>("error");
        var result2 = Result.Err<int, string>("error");
        Assert.IsTrue(result1.Equals(result2));
        Assert.IsTrue(result1 == result2);
        Assert.IsFalse(result1 != result2);
    }

    [TestMethod]
    public void Equals_ReturnsFalse_ForTwoErrsWithDifferentErrors()
    {
        var result1 = Result.Err<int, string>("error1");
        var result2 = Result.Err<int, string>("error2");
        Assert.IsFalse(result1.Equals(result2));
        Assert.IsFalse(result1 == result2);
        Assert.IsTrue(result1 != result2);
    }

    [TestMethod]
    public void Equals_ReturnsFalse_ForOkAndErr()
    {
        var result1 = Result.Ok<int, string>(42);
        var result2 = Result.Err<int, string>("error");
        Assert.IsFalse(result1.Equals(result2));
        Assert.IsFalse(result1 == result2);
        Assert.IsTrue(result1 != result2);
    }

    [TestMethod]
    public void Equals_ReturnsFalse_WhenComparedWithDifferentType()
    {
        var result = Result.Ok<int, string>(42);
        Assert.IsFalse(result.Equals("not a result"));
    }

    [TestMethod]
    public void GetHashCode_ReturnsSameForEqualOks()
    {
        var result1 = Result.Ok<int, string>(42);
        var result2 = Result.Ok<int, string>(42);
        Assert.AreEqual(result1.GetHashCode(), result2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_ReturnsSameForEqualErrs()
    {
        var result1 = Result.Err<int, string>("error");
        var result2 = Result.Err<int, string>("error");
        Assert.AreEqual(result1.GetHashCode(), result2.GetHashCode());
    }

    [TestMethod]
    public void IEnumerable_YieldsValue_WhenOk()
    {
        var result = Result.Ok<int, string>(42);
        var count = 0;
        var sum = 0;
        foreach (var value in result)
        {
            count++;
            sum += value;
        }
        Assert.AreEqual(1, count);
        Assert.AreEqual(42, sum);
    }

    [TestMethod]
    public void IEnumerable_YieldsNothing_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        var count = 0;
        foreach (var value in result)
        {
            count++;
        }
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void IEnumerable_WorksWithLinq_WhenOk()
    {
        var result = Result.Ok<int, string>(10);
        var doubled = result.Select(x => x * 2).FirstOrDefault();
        Assert.AreEqual(20, doubled);
    }

    [TestMethod]
    public void IEnumerable_WorksWithLinq_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        var value = result.Select(x => x * 2).FirstOrDefault();
        Assert.AreEqual(0, value);
    }

    [TestMethod]
    public void CompareTo_ReturnsZero_ForTwoErrsWithSameError()
    {
        var result1 = Result.Err<int, string>("error");
        var result2 = Result.Err<int, string>("error");
        Assert.AreEqual(0, result1.CompareTo(result2));
    }

    [TestMethod]
    public void CompareTo_ReturnsNegative_WhenErrComparedToOk()
    {
        var err = Result.Err<int, string>("error");
        var ok = Result.Ok<int, string>(10);
        Assert.IsTrue(err.CompareTo(ok) < 0);
    }

    [TestMethod]
    public void CompareTo_ReturnsPositive_WhenOkComparedToErr()
    {
        var ok = Result.Ok<int, string>(10);
        var err = Result.Err<int, string>("error");
        Assert.IsTrue(ok.CompareTo(err) > 0);
    }

    [TestMethod]
    public void CompareTo_ReturnsZero_ForTwoOksWithSameValue()
    {
        var result1 = Result.Ok<int, string>(42);
        var result2 = Result.Ok<int, string>(42);
        Assert.AreEqual(0, result1.CompareTo(result2));
    }

    [TestMethod]
    public void CompareTo_ReturnsNegative_WhenSmallerOkValue()
    {
        var result1 = Result.Ok<int, string>(10);
        var result2 = Result.Ok<int, string>(20);
        Assert.IsTrue(result1.CompareTo(result2) < 0);
    }

    [TestMethod]
    public void CompareTo_ReturnsPositive_WhenLargerOkValue()
    {
        var result1 = Result.Ok<int, string>(20);
        var result2 = Result.Ok<int, string>(10);
        Assert.IsTrue(result1.CompareTo(result2) > 0);
    }

    [TestMethod]
    public void CompareTo_ComparesErrors_WhenBothErr()
    {
        var result1 = Result.Err<int, string>("aaa");
        var result2 = Result.Err<int, string>("bbb");
        Assert.IsTrue(result1.CompareTo(result2) < 0);
    }

    [TestMethod]
    public void IEquatable_Equals_WorksCorrectly()
    {
        IEquatable<Result<int, string>> result1 = Result.Ok<int, string>(42);
        var result2 = Result.Ok<int, string>(42);
        Assert.IsTrue(result1.Equals(result2));
    }

    [TestMethod]
    public void IComparable_CompareTo_WorksCorrectly()
    {
        IComparable<Result<int, string>> result1 = Result.Ok<int, string>(10);
        var result2 = Result.Ok<int, string>(20);
        Assert.IsTrue(result1.CompareTo(result2) < 0);
    }

    [TestMethod]
    public void LinqSelect_TransformsValue_WhenOk()
    {
        var result = Result.Ok<int, string>(5);
        var mapped = result.Select(x => x * 2);
        Assert.IsTrue(mapped.IsOk());
        Assert.AreEqual(10, mapped.Unwrap());
    }

    [TestMethod]
    public void LinqSelect_ReturnsErr_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        var mapped = result.Select(x => x * 2);
        Assert.IsTrue(mapped.IsErr());
        Assert.AreEqual("error", mapped.UnwrapErr());
    }

    [TestMethod]
    public void LinqSelectMany_ChainsResults_WhenBothOk()
    {
        var result = Result.Ok<int, string>(5);
        var chained = result.SelectMany(x => Result.Ok<int, string>(x * 2));
        Assert.IsTrue(chained.IsOk());
        Assert.AreEqual(10, chained.Unwrap());
    }

    [TestMethod]
    public void LinqSelectMany_ReturnsErr_WhenFirstErr()
    {
        var result = Result.Err<int, string>("error");
        var chained = result.SelectMany(x => Result.Ok<int, string>(x * 2));
        Assert.IsTrue(chained.IsErr());
        Assert.AreEqual("error", chained.UnwrapErr());
    }

    [TestMethod]
    public void LinqSelectMany_ReturnsErr_WhenSelectorReturnsErr()
    {
        var result = Result.Ok<int, string>(5);
        var chained = result.SelectMany(x => Result.Err<int, string>("inner error"));
        Assert.IsTrue(chained.IsErr());
        Assert.AreEqual("inner error", chained.UnwrapErr());
    }

    [TestMethod]
    public void LinqQuerySyntax_WorksWithResults()
    {
        var result1 = Result.Ok<int, string>(10);
        var result2 = Result.Ok<int, string>(20);

        var combined = from a in result1
                       from b in result2
                       select a + b;

        Assert.IsTrue(combined.IsOk());
        Assert.AreEqual(30, combined.Unwrap());
    }

    [TestMethod]
    public void LinqQuerySyntax_ReturnsErr_WhenAnyIsErr()
    {
        var result1 = Result.Ok<int, string>(10);
        var result2 = Result.Err<int, string>("error");

        var combined = from a in result1
                       from b in result2
                       select a + b;

        Assert.IsTrue(combined.IsErr());
        Assert.AreEqual("error", combined.UnwrapErr());
    }

    [TestMethod]
    public void Contains_ReturnsTrue_WhenOkWithMatchingValue()
    {
        var result = Result.Ok<int, string>(42);
        Assert.IsTrue(result.Contains(42));
    }

    [TestMethod]
    public void Contains_ReturnsFalse_WhenOkWithDifferentValue()
    {
        var result = Result.Ok<int, string>(42);
        Assert.IsFalse(result.Contains(100));
    }

    [TestMethod]
    public void Contains_ReturnsFalse_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        Assert.IsFalse(result.Contains(42));
    }

    [TestMethod]
    public void ContainsErr_ReturnsTrue_WhenErrWithMatchingError()
    {
        var result = Result.Err<int, string>("error");
        Assert.IsTrue(result.ContainsErr("error"));
    }

    [TestMethod]
    public void ContainsErr_ReturnsFalse_WhenErrWithDifferentError()
    {
        var result = Result.Err<int, string>("error");
        Assert.IsFalse(result.ContainsErr("other"));
    }

    [TestMethod]
    public void ContainsErr_ReturnsFalse_WhenOk()
    {
        var result = Result.Ok<int, string>(42);
        Assert.IsFalse(result.ContainsErr("error"));
    }

    [TestMethod]
    public async Task MapAsync_TransformsValue_WhenOk()
    {
        var result = Result.Ok<int, string>(5);
        var mapped = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });
        Assert.IsTrue(mapped.IsOk());
        Assert.AreEqual(10, mapped.Unwrap());
    }

    [TestMethod]
    public async Task MapAsync_PreservesError_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        var mapped = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });
        Assert.IsTrue(mapped.IsErr());
        Assert.AreEqual("error", mapped.UnwrapErr());
    }

    [TestMethod]
    public async Task AndThenAsync_ChainsResults_WhenBothOk()
    {
        var result = Result.Ok<int, string>(5);
        var chained = await result.AndThenAsync(async x =>
        {
            await Task.Delay(1);
            return Result.Ok<int, string>(x * 2);
        });
        Assert.IsTrue(chained.IsOk());
        Assert.AreEqual(10, chained.Unwrap());
    }

    [TestMethod]
    public async Task AndThenAsync_ReturnsErr_WhenSelectorReturnsErr()
    {
        var result = Result.Ok<int, string>(5);
        var chained = await result.AndThenAsync(async x =>
        {
            await Task.Delay(1);
            return Result.Err<int, string>("failed");
        });
        Assert.IsTrue(chained.IsErr());
        Assert.AreEqual("failed", chained.UnwrapErr());
    }

    [TestMethod]
    public async Task TaskMapAsync_TransformsValue_FromTask()
    {
        var resultTask = Task.FromResult(Result.Ok<int, string>(5));
        var mapped = await resultTask.MapAsync(x => x * 2);
        Assert.IsTrue(mapped.IsOk());
        Assert.AreEqual(10, mapped.Unwrap());
    }

    [TestMethod]
    public async Task UnwrapOrAsync_ReturnsValue_WhenOk()
    {
        var resultTask = Task.FromResult(Result.Ok<int, string>(42));
        var value = await resultTask.UnwrapOrAsync(0);
        Assert.AreEqual(42, value);
    }

    [TestMethod]
    public async Task UnwrapOrAsync_ReturnsDefault_WhenErr()
    {
        var resultTask = Task.FromResult(Result.Err<int, string>("error"));
        var value = await resultTask.UnwrapOrAsync(99);
        Assert.AreEqual(99, value);
    }

    #region Additional Async Tests

    [TestMethod]
    public async Task MapAsync_DoesNotCallSelector_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        bool selectorCalled = false;

        var mapped = await result.MapAsync(async x =>
        {
            selectorCalled = true;
            await Task.Delay(1);
            return x * 2;
        });

        Assert.IsFalse(selectorCalled);
        Assert.IsTrue(mapped.IsErr());
        Assert.AreEqual("error", mapped.UnwrapErr());
    }

    [TestMethod]
    public async Task AndThenAsync_DoesNotCallSelector_WhenErr()
    {
        var result = Result.Err<int, string>("error");
        bool selectorCalled = false;

        var chained = await result.AndThenAsync(async x =>
        {
            selectorCalled = true;
            await Task.Delay(1);
            return Result.Ok<int, string>(x * 2);
        });

        Assert.IsFalse(selectorCalled);
        Assert.IsTrue(chained.IsErr());
        Assert.AreEqual("error", chained.UnwrapErr());
    }

    [TestMethod]
    public async Task MapAsync_ChainedOperations()
    {
        var result = Result.Ok<int, string>(5);

        var step1 = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });

        var step2 = await step1.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x + 10;
        });

        Assert.IsTrue(step2.IsOk());
        Assert.AreEqual(20, step2.Unwrap());
    }

    [TestMethod]
    public async Task MapAsync_ChainedOperations_PropagatesError()
    {
        var result = Result.Err<int, string>("initial error");

        var step1 = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });

        var step2 = await step1.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x + 10;
        });

        Assert.IsTrue(step2.IsErr());
        Assert.AreEqual("initial error", step2.UnwrapErr());
    }

    [TestMethod]
    public async Task TaskMapAsync_ReturnsErr_WhenTaskContainsErr()
    {
        var resultTask = Task.FromResult(Result.Err<int, string>("error"));
        var mapped = await resultTask.MapAsync(x => x * 2);
        Assert.IsTrue(mapped.IsErr());
        Assert.AreEqual("error", mapped.UnwrapErr());
    }

    [TestMethod]
    public async Task TaskAndThenAsync_ChainsCorrectly()
    {
        var resultTask = Task.FromResult(Result.Ok<int, string>(5));
        var chained = await resultTask.AndThenAsync(x => Result.Ok<int, string>(x * 2));
        Assert.IsTrue(chained.IsOk());
        Assert.AreEqual(10, chained.Unwrap());
    }

    [TestMethod]
    public async Task TaskAndThenAsync_ReturnsErr_WhenTaskContainsErr()
    {
        var resultTask = Task.FromResult(Result.Err<int, string>("error"));
        var chained = await resultTask.AndThenAsync(x => Result.Ok<int, string>(x * 2));
        Assert.IsTrue(chained.IsErr());
        Assert.AreEqual("error", chained.UnwrapErr());
    }

    [TestMethod]
    public async Task TaskAndThenAsync_ReturnsErr_WhenSelectorReturnsErr()
    {
        var resultTask = Task.FromResult(Result.Ok<int, string>(5));
        var chained = await resultTask.AndThenAsync(x => Result.Err<int, string>("selector error"));
        Assert.IsTrue(chained.IsErr());
        Assert.AreEqual("selector error", chained.UnwrapErr());
    }

    [TestMethod]
    public async Task MapAsync_WithStringTransformation()
    {
        var result = Result.Ok<string, int>("hello");
        var mapped = await result.MapAsync(async s =>
        {
            await Task.Delay(1);
            return s.ToUpper();
        });
        Assert.IsTrue(mapped.IsOk());
        Assert.AreEqual("HELLO", mapped.Unwrap());
    }

    [TestMethod]
    public async Task MapAsync_WithTypeConversion()
    {
        var result = Result.Ok<int, string>(42);
        var mapped = await result.MapAsync(async n =>
        {
            await Task.Delay(1);
            return n.ToString();
        });
        Assert.IsTrue(mapped.IsOk());
        Assert.AreEqual("42", mapped.Unwrap());
    }

    [TestMethod]
    public async Task AndThenAsync_WithValidation()
    {
        var result = Result.Ok<int, string>(10);

        var validated = await result.AndThenAsync(async x =>
        {
            await Task.Delay(1);
            return x > 5
                ? Result.Ok<int, string>(x)
                : Result.Err<int, string>("Value too small");
        });

        Assert.IsTrue(validated.IsOk());
        Assert.AreEqual(10, validated.Unwrap());
    }

    [TestMethod]
    public async Task AndThenAsync_WithValidationFailure()
    {
        var result = Result.Ok<int, string>(3);

        var validated = await result.AndThenAsync(async x =>
        {
            await Task.Delay(1);
            return x > 5
                ? Result.Ok<int, string>(x)
                : Result.Err<int, string>("Value too small");
        });

        Assert.IsTrue(validated.IsErr());
        Assert.AreEqual("Value too small", validated.UnwrapErr());
    }

    [TestMethod]
    public async Task UnwrapOrAsync_WithReferenceType()
    {
        var resultTask = Task.FromResult(Result.Ok<string, int>("value"));
        var value = await resultTask.UnwrapOrAsync("default");
        Assert.AreEqual("value", value);
    }

    [TestMethod]
    public async Task UnwrapOrAsync_WithReferenceType_WhenErr()
    {
        var resultTask = Task.FromResult(Result.Err<string, int>(404));
        var value = await resultTask.UnwrapOrAsync("default");
        Assert.AreEqual("default", value);
    }

    [TestMethod]
    public async Task MapAsync_ParallelExecution()
    {
        var results = new[]
        {
            Result.Ok<int, string>(1),
            Result.Ok<int, string>(2),
            Result.Err<int, string>("error"),
            Result.Ok<int, string>(4),
            Result.Ok<int, string>(5)
        };

        var tasks = results.Select(r => r.MapAsync(async x =>
        {
            await Task.Delay(10);
            return x * 10;
        }));

        var mappedResults = await Task.WhenAll(tasks);

        Assert.AreEqual(10, mappedResults[0].UnwrapOr(0));
        Assert.AreEqual(20, mappedResults[1].UnwrapOr(0));
        Assert.AreEqual(0, mappedResults[2].UnwrapOr(0));
        Assert.IsTrue(mappedResults[2].IsErr());
        Assert.AreEqual(40, mappedResults[3].UnwrapOr(0));
        Assert.AreEqual(50, mappedResults[4].UnwrapOr(0));
    }

    [TestMethod]
    public async Task AndThenAsync_RealWorldScenario_MultiStepValidation()
    {
        async Task<Result<int, string>> ParseAsync(string input)
        {
            await Task.Delay(1);
            if (int.TryParse(input, out var num))
                return Result.Ok<int, string>(num);
            return Result.Err<int, string>($"Failed to parse '{input}'");
        }

        async Task<Result<int, string>> ValidatePositiveAsync(int num)
        {
            await Task.Delay(1);
            return num > 0
                ? Result.Ok<int, string>(num)
                : Result.Err<int, string>("Number must be positive");
        }

        async Task<Result<int, string>> ValidateRangeAsync(int num)
        {
            await Task.Delay(1);
            return num <= 100
                ? Result.Ok<int, string>(num)
                : Result.Err<int, string>("Number must be <= 100");
        }

        var input = "42";
        var result = await ParseAsync(input);
        var step1 = await result.AndThenAsync(ValidatePositiveAsync);
        var step2 = await step1.AndThenAsync(ValidateRangeAsync);

        Assert.IsTrue(step2.IsOk());
        Assert.AreEqual(42, step2.Unwrap());

        input = "invalid";
        result = await ParseAsync(input);
        step1 = await result.AndThenAsync(ValidatePositiveAsync);
        step2 = await step1.AndThenAsync(ValidateRangeAsync);

        Assert.IsTrue(step2.IsErr());
        Assert.AreEqual("Failed to parse 'invalid'", step2.UnwrapErr());

        input = "-5";
        result = await ParseAsync(input);
        step1 = await result.AndThenAsync(ValidatePositiveAsync);
        step2 = await step1.AndThenAsync(ValidateRangeAsync);

        Assert.IsTrue(step2.IsErr());
        Assert.AreEqual("Number must be positive", step2.UnwrapErr());

        input = "150";
        result = await ParseAsync(input);
        step1 = await result.AndThenAsync(ValidatePositiveAsync);
        step2 = await step1.AndThenAsync(ValidateRangeAsync);

        Assert.IsTrue(step2.IsErr());
        Assert.AreEqual("Number must be <= 100", step2.UnwrapErr());
    }

    #endregion
}
