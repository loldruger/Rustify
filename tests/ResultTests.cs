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

        Assert.IsTrue(result.IsOk);
        Assert.IsFalse(result.IsErr);
        Assert.AreEqual(value, result.Unwrap());
    }

    [TestMethod]
    public void Result_Err_CreatesErrWithError()
    {
        var error = "test_err";
        var result = Result.Err<string, string>(error);

        Assert.IsFalse(result.IsOk);
        Assert.IsTrue(result.IsErr);
        Assert.AreEqual(error, result.UnwrapErr());
    }

    [TestMethod]
    public void ImplicitConversion_FromValue_CreatesOk()
    {
        Result<string, int> result = DefaultOkValue;
        Assert.IsTrue(result.IsOk);
        Assert.AreEqual(DefaultOkValue, result.Unwrap());
    }

    [TestMethod]
    public void ImplicitConversion_FromError_CreatesErr()
    {
        Result<int, string> result = DefaultError;
        Assert.IsTrue(result.IsErr);
        Assert.AreEqual(DefaultError, result.UnwrapErr());
    }

    [TestMethod]
    public void Unwrap_ReturnsValue_WhenOk()
    {
        var value = 123;
        var result = Result.Ok<int, string>(value);
        Assert.AreEqual(value, result.Unwrap());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Unwrap_ThrowsException_WhenErr()
    {
        var result = Result.Err<int, string>(DefaultError);
        result.Unwrap();
    }

    [TestMethod]
    public void UnwrapErr_ReturnsError_WhenErr()
    {
        var error = "custom_error";
        var result = Result.Err<int, string>(error);
        Assert.AreEqual(error, result.UnwrapErr());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void UnwrapErr_ThrowsException_WhenOk()
    {
        var result = Result.Ok<int, string>(123);
        result.UnwrapErr();
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
        Assert.IsTrue(nextResult.IsOk);
        Assert.AreEqual(20, nextResult.Unwrap());
    }

    [TestMethod]
    public void AndThen_ReturnsSelf_WhenErr()
    {
        var result = Result.Err<int, string>(DefaultError);
        var nextResult = result.AndThen(x => Result.Ok<int, string>(x * 2)); // This function should not be called
        Assert.IsTrue(nextResult.IsErr);
        Assert.AreEqual(DefaultError, nextResult.UnwrapErr());
    }

    [TestMethod]
    public void AndThen_ReturnsNewErr_WhenOkAndFuncReturnsErr()
    {
        var result = Result.Ok<int, string>(10);
        var newError = "new_error_from_andthen";
        var nextResult = result.AndThen(x => Result.Err<int, string>(newError));
        Assert.IsTrue(nextResult.IsErr);
        Assert.AreEqual(newError, nextResult.UnwrapErr());
    }
    
    [TestMethod]
    public void OrElse_ReturnsSelf_WhenOk()
    {
        var result = Result.Ok<string, int>(DefaultOkValue);
        var nextResult = result.OrElse(e => Result.Ok<string, string>($"fallback_from_{e}")); // This function should not be called
        Assert.IsTrue(nextResult.IsOk);
        Assert.AreEqual(DefaultOkValue, nextResult.Unwrap());
    }

    [TestMethod]
    public void OrElse_ReturnsNewResult_WhenErr()
    {
        var initialError = 404;
        var result = Result.Err<string, int>(initialError);
        var fallbackValue = "fallback_value";
        var nextResult = result.OrElse(e => Result.Ok<string, string>(fallbackValue));
        Assert.IsTrue(nextResult.IsOk);
        Assert.AreEqual(fallbackValue, nextResult.Unwrap());
    }

    [TestMethod]
    public void OrElse_ReturnsNewErr_WhenErrAndFuncReturnsErr()
    {
        var initialError = 404;
        var newErrorFromOrElse = "new_error_from_orelse";
        var result = Result.Err<string, int>(initialError);
        var nextResult = result.OrElse(e => Result.Err<string, string>(newErrorFromOrElse));
        Assert.IsTrue(nextResult.IsErr);
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
        Assert.IsTrue(mappedResult.IsOk);
        Assert.AreEqual(50, mappedResult.Unwrap());
    }

    [TestMethod]
    public void Map_ReturnsErr_WhenErr()
    {
        var result = Result.Err<int, string>(DefaultError);
        var mappedResult = result.Map(x => x * 10); // This function should not be called
        Assert.IsTrue(mappedResult.IsErr);
        Assert.AreEqual(DefaultError, mappedResult.UnwrapErr());
    }

    [TestMethod]
    public void MapErr_TransformsError_WhenErr()
    {
        var result = Result.Err<string, int>(100);
        var mappedResult = result.MapErr(e => $"Error code: {e}");
        Assert.IsTrue(mappedResult.IsErr);
        Assert.AreEqual("Error code: 100", mappedResult.UnwrapErr());
    }

    [TestMethod]
    public void MapErr_ReturnsOk_WhenOk()
    {
        var result = Result.Ok<string, int>(DefaultOkValue);
        var mappedResult = result.MapErr(e => $"Error code: {e}"); // This function should not be called
        Assert.IsTrue(mappedResult.IsOk);
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
        Assert.IsTrue(flattened.IsOk);
        Assert.AreEqual(100, flattened.Unwrap());
    }

    [TestMethod]
    public void Flatten_ReturnsInnerErr_WhenOuterIsOkAndInnerIsErr()
    {
        var innerErr = Result.Err<int, string>("InnerError");
        var outerOk = Result.Ok<Result<int, string>, string>(innerErr);
        var flattened = Result<int, string>.Flatten(outerOk);
        Assert.IsTrue(flattened.IsErr);
        Assert.AreEqual("InnerError", flattened.UnwrapErr());
    }

    [TestMethod]
    public void Flatten_ReturnsOuterErr_WhenOuterIsErr()
    {
        var outerErr = Result.Err<Result<int, string>, string>("OuterError");
        var flattened = Result<int, string>.Flatten(outerErr);
        Assert.IsTrue(flattened.IsErr);
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
        Assert.IsTrue(innerResult.IsOk);
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
        Assert.IsTrue(innerResult.IsErr);
        Assert.AreEqual(errorValue, innerResult.UnwrapErr());
    }
}
