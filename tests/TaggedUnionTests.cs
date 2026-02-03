using Rustify.Utilities;
using System;

namespace Rustify.Tests;

[TestClass]
public sealed class TaggedUnionTests
{
    #region TaggedUnion<A> Tests

    [TestMethod]
    public void TaggedUnion1_Create_ViaStaticMethod()
    {
        var union = TaggedUnion._0("test");
        var result = union.Match(value => value);
        Assert.AreEqual("test", result);
    }

    [TestMethod]
    public void TaggedUnion1_Create_ViaImplicitConversion()
    {
        TaggedUnion<int> union = 42;
        var result = union.Match(value => value);
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void TaggedUnion1_Match_ExecutesCorrectFunction()
    {
        var union = TaggedUnion._0(100);
        var result = union.Match(value => value * 2);
        Assert.AreEqual(200, result);
    }

    [TestMethod]
    public void TaggedUnion1_Match_WithStringTransformation()
    {
        var union = TaggedUnion._0(42);
        var result = union.Match(value => $"Value is {value}");
        Assert.AreEqual("Value is 42", result);
    }

    [TestMethod]
    public void TaggedUnion1_WithReferenceType()
    {
        var union = TaggedUnion._0(new TestData { Value = 10 });
        var result = union.Match(obj => obj.Value);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void TaggedUnion1_ImplicitConversion_FromValue()
    {
        TaggedUnion<string> union = "hello";
        var result = union.Match(s => s.ToUpper());
        Assert.AreEqual("HELLO", result);
    }

    [TestMethod]
    public void TaggedUnion1_Match_WithSideEffect()
    {
        var union = TaggedUnion._0(5);
        int sideEffectValue = 0;
        var result = union.Match(value =>
        {
            sideEffectValue = value * 10;
            return sideEffectValue;
        });
        Assert.AreEqual(50, sideEffectValue);
        Assert.AreEqual(50, result);
    }

    [TestMethod]
    public void TaggedUnion1_WithDifferentReturnType()
    {
        var union = TaggedUnion._0(10);
        bool result = union.Match(value => value > 5);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TaggedUnion1_NestedMatch()
    {
        var outer = TaggedUnion._0(TaggedUnion._0(42));
        var result = outer.Match(inner => inner.Match(value => value));
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void TaggedUnion1_WithComplexType()
    {
        var data = new ComplexData { Name = "Test", Count = 5 };
        var union = TaggedUnion._0(data);
        var result = union.Match(d => $"{d.Name}:{d.Count}");
        Assert.AreEqual("Test:5", result);
    }

    [TestMethod]
    public void TaggedUnion1_WithEmptyString()
    {
        TaggedUnion<string> union = "";
        var result = union.Match(s => s.Length);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void TaggedUnion1_WithZero()
    {
        TaggedUnion<int> union = 0;
        var result = union.Match(v => v == 0);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TaggedUnion1_WithNegativeNumber()
    {
        TaggedUnion<int> union = -42;
        var result = union.Match(v => v);
        Assert.AreEqual(-42, result);
    }

    #endregion

    #region TaggedUnion<A, B> Tests

    [TestMethod]
    public void TaggedUnion2_Create_Case0_ViaStaticMethod()
    {
        var union = TaggedUnion._0<string, int>("hello");
        var result = union.Match(s => s, i => i.ToString());
        Assert.AreEqual("hello", result);
    }

    [TestMethod]
    public void TaggedUnion2_Create_Case1_ViaStaticMethod()
    {
        var union = TaggedUnion._1<string, int>(42);
        var result = union.Match(s => s, i => i.ToString());
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void TaggedUnion2_Create_Case0_ViaInstanceMethod()
    {
        var union = TaggedUnion<string, int>.__0("hello");
        Assert.IsTrue(union.Is0);
        Assert.IsFalse(union.Is1);
    }

    [TestMethod]
    public void TaggedUnion2_Create_Case1_ViaInstanceMethod()
    {
        var union = TaggedUnion<string, int>.__1(42);
        Assert.IsFalse(union.Is0);
        Assert.IsTrue(union.Is1);
    }

    [TestMethod]
    public void TaggedUnion2_ImplicitConversion_FromTypeA()
    {
        TaggedUnion<string, int> union = "implicit";
        Assert.IsTrue(union.Is0);
        Assert.IsFalse(union.Is1);
        var result = union.Match(s => s, i => "not this");
        Assert.AreEqual("implicit", result);
    }

    [TestMethod]
    public void TaggedUnion2_ImplicitConversion_FromTypeB()
    {
        TaggedUnion<string, int> union = 100;
        Assert.IsFalse(union.Is0);
        Assert.IsTrue(union.Is1);
        var result = union.Match(s => 0, i => i);
        Assert.AreEqual(100, result);
    }

    [TestMethod]
    public void TaggedUnion2_Match_ExecutesCorrectCase()
    {
        TaggedUnion<string, int> unionA = "test";
        TaggedUnion<string, int> unionB = 99;

        var resultA = unionA.Match(s => $"string:{s}", i => $"int:{i}");
        var resultB = unionB.Match(s => $"string:{s}", i => $"int:{i}");

        Assert.AreEqual("string:test", resultA);
        Assert.AreEqual("int:99", resultB);
    }

    [TestMethod]
    public void TaggedUnion2_ActionMatch_ExecutesCase0()
    {
        TaggedUnion<string, int> union = "action";
        string executed = "";

        union.Match(
            s => executed = $"case0:{s}",
            i => executed = $"case1:{i}"
        );

        Assert.AreEqual("case0:action", executed);
    }

    [TestMethod]
    public void TaggedUnion2_ActionMatch_ExecutesCase1()
    {
        TaggedUnion<string, int> union = 123;
        string executed = "";

        union.Match(
            s => executed = $"case0:{s}",
            i => executed = $"case1:{i}"
        );

        Assert.AreEqual("case1:123", executed);
    }

    [TestMethod]
    public void TaggedUnion2_WithReferenceTypes()
    {
        var union = TaggedUnion<TestData, ComplexData>.__0(new TestData { Value = 42 });
        var result = union.Match(t => t.Value, c => c.Count);
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void TaggedUnion2_Either_Pattern_Success()
    {
        TaggedUnion<string, Exception> success = "Success!";
        var successResult = success.Match(s => s, e => e.Message);
        Assert.AreEqual("Success!", successResult);
    }

    [TestMethod]
    public void TaggedUnion2_Either_Pattern_Failure()
    {
        TaggedUnion<string, Exception> failure = new InvalidOperationException("Failed");
        var failureResult = failure.Match(s => s, e => e.Message);
        Assert.AreEqual("Failed", failureResult);
    }

    [TestMethod]
    public void TaggedUnion2_WithEmptyStringCase0()
    {
        TaggedUnion<string, int> union = "";
        Assert.IsTrue(union.Is0);
        var result = union.Match(s => s.Length, i => i);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void TaggedUnion2_WithZeroCase1()
    {
        TaggedUnion<string, int> union = 0;
        Assert.IsTrue(union.Is1);
        var result = union.Match(s => -1, i => i);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void TaggedUnion2_WithNegativeNumberCase1()
    {
        TaggedUnion<string, int> union = -100;
        Assert.IsTrue(union.Is1);
        var result = union.Match(s => 0, i => i);
        Assert.AreEqual(-100, result);
    }

    [TestMethod]
    public void TaggedUnion2_Match_ThrowsInSelector()
    {
        TaggedUnion<string, int> union = "test";
        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            union.Match<string>(
                s => throw new InvalidOperationException("Test exception"),
                i => i.ToString()
            );
        });
    }

    [TestMethod]
    public void TaggedUnion2_NestedUnion()
    {
        TaggedUnion<TaggedUnion<string, int>, bool> outer = TaggedUnion._0<string, int>("nested");
        var result = outer.Match(
            inner => inner.Match(s => s, i => i.ToString()),
            b => b.ToString()
        );
        Assert.AreEqual("nested", result);
    }

    [TestMethod]
    public void TaggedUnion2_ChainedMatching()
    {
        TaggedUnion<int, string>[] unions = 
        {
            1,
            "two",
            3,
            "four"
        };

        var results = new string[4];
        for (int i = 0; i < unions.Length; i++)
        {
            results[i] = unions[i].Match(n => $"num:{n}", s => $"str:{s}");
        }

        Assert.AreEqual("num:1", results[0]);
        Assert.AreEqual("str:two", results[1]);
        Assert.AreEqual("num:3", results[2]);
        Assert.AreEqual("str:four", results[3]);
    }

    #endregion

    #region TaggedUnion<A, B, C> Tests

    [TestMethod]
    public void TaggedUnion3_Create_Case0_ViaStaticMethod()
    {
        var union = TaggedUnion._0<string, int, bool>("hello");
        var result = union.Match(s => s, i => i.ToString(), b => b.ToString());
        Assert.AreEqual("hello", result);
    }

    [TestMethod]
    public void TaggedUnion3_Create_Case1_ViaStaticMethod()
    {
        var union = TaggedUnion._1<string, int, bool>(42);
        var result = union.Match(s => s, i => i.ToString(), b => b.ToString());
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void TaggedUnion3_Create_Case2_ViaStaticMethod()
    {
        var union = TaggedUnion._2<string, int, bool>(true);
        var result = union.Match(s => s, i => i.ToString(), b => b.ToString());
        Assert.AreEqual("True", result);
    }

    [TestMethod]
    public void TaggedUnion3_Create_Case0_ViaInstanceMethod()
    {
        var union = TaggedUnion<string, int, bool>.__0("hello");
        Assert.IsTrue(union.Is0);
        Assert.IsFalse(union.Is1);
        Assert.IsFalse(union.Is2);
    }

    [TestMethod]
    public void TaggedUnion3_Create_Case1_ViaInstanceMethod()
    {
        var union = TaggedUnion<string, int, bool>.__1(42);
        Assert.IsFalse(union.Is0);
        Assert.IsTrue(union.Is1);
        Assert.IsFalse(union.Is2);
    }

    [TestMethod]
    public void TaggedUnion3_Create_Case2_ViaInstanceMethod()
    {
        var union = TaggedUnion<string, int, bool>.__2(true);
        Assert.IsFalse(union.Is0);
        Assert.IsFalse(union.Is1);
        Assert.IsTrue(union.Is2);
    }

    [TestMethod]
    public void TaggedUnion3_ImplicitConversion_FromTypeA()
    {
        TaggedUnion<string, int, bool> union = "implicit";
        Assert.IsTrue(union.Is0);
        Assert.IsFalse(union.Is1);
        Assert.IsFalse(union.Is2);
    }

    [TestMethod]
    public void TaggedUnion3_ImplicitConversion_FromTypeB()
    {
        TaggedUnion<string, int, bool> union = 100;
        Assert.IsFalse(union.Is0);
        Assert.IsTrue(union.Is1);
        Assert.IsFalse(union.Is2);
    }

    [TestMethod]
    public void TaggedUnion3_ImplicitConversion_FromTypeC()
    {
        TaggedUnion<string, int, bool> union = false;
        Assert.IsFalse(union.Is0);
        Assert.IsFalse(union.Is1);
        Assert.IsTrue(union.Is2);
    }

    [TestMethod]
    public void TaggedUnion3_Match_ExecutesCase0()
    {
        TaggedUnion<string, int, bool> union = "test";
        var result = union.Match(s => 0, i => 1, b => 2);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void TaggedUnion3_Match_ExecutesCase1()
    {
        TaggedUnion<string, int, bool> union = 99;
        var result = union.Match(s => 0, i => 1, b => 2);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void TaggedUnion3_Match_ExecutesCase2()
    {
        TaggedUnion<string, int, bool> union = true;
        var result = union.Match(s => 0, i => 1, b => 2);
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void TaggedUnion3_ActionMatch_ExecutesCase0()
    {
        TaggedUnion<string, int, bool> union = "action";
        int executed = -1;

        union.Match(
            s => executed = 0,
            i => executed = 1,
            b => executed = 2
        );

        Assert.AreEqual(0, executed);
    }

    [TestMethod]
    public void TaggedUnion3_ActionMatch_ExecutesCase1()
    {
        TaggedUnion<string, int, bool> union = 42;
        int executed = -1;

        union.Match(
            s => executed = 0,
            i => executed = 1,
            b => executed = 2
        );

        Assert.AreEqual(1, executed);
    }

    [TestMethod]
    public void TaggedUnion3_ActionMatch_ExecutesCase2()
    {
        TaggedUnion<string, int, bool> union = true;
        int executed = -1;

        union.Match(
            s => executed = 0,
            i => executed = 1,
            b => executed = 2
        );

        Assert.AreEqual(2, executed);
    }

    [TestMethod]
    public void TaggedUnion3_RealWorldScenario_ParseResult()
    {
        TaggedUnion<int, string, Exception> parseSuccess = 42;
        TaggedUnion<int, string, Exception> parseWarning = "Parsed with warnings";
        TaggedUnion<int, string, Exception> parseError = new FormatException("Invalid format");

        string HandleParse(TaggedUnion<int, string, Exception> result) =>
            result.Match(
                value => $"Success: {value}",
                warning => $"Warning: {warning}",
                error => $"Error: {error.Message}"
            );

        Assert.AreEqual("Success: 42", HandleParse(parseSuccess));
        Assert.AreEqual("Warning: Parsed with warnings", HandleParse(parseWarning));
        Assert.AreEqual("Error: Invalid format", HandleParse(parseError));
    }

    [TestMethod]
    public void TaggedUnion3_HttpResponseScenario()
    {
        TaggedUnion<string, int, Exception> success = "OK";
        TaggedUnion<string, int, Exception> redirect = 301;
        TaggedUnion<string, int, Exception> error = new Exception("Server Error");

        string HandleResponse(TaggedUnion<string, int, Exception> response) =>
            response.Match(
                body => $"Body: {body}",
                statusCode => $"Redirect: {statusCode}",
                ex => $"Error: {ex.Message}"
            );

        Assert.AreEqual("Body: OK", HandleResponse(success));
        Assert.AreEqual("Redirect: 301", HandleResponse(redirect));
        Assert.AreEqual("Error: Server Error", HandleResponse(error));
    }

    [TestMethod]
    public void TaggedUnion3_WithAllEdgeValues()
    {
        TaggedUnion<string, int, bool> empty = "";
        TaggedUnion<string, int, bool> zero = 0;
        TaggedUnion<string, int, bool> falseVal = false;

        Assert.IsTrue(empty.Is0);
        Assert.IsTrue(zero.Is1);
        Assert.IsTrue(falseVal.Is2);

        var r1 = empty.Match(s => s.Length, i => i, b => b ? 1 : 0);
        var r2 = zero.Match(s => s.Length, i => i, b => b ? 1 : 0);
        var r3 = falseVal.Match(s => s.Length, i => i, b => b ? 1 : 0);

        Assert.AreEqual(0, r1);
        Assert.AreEqual(0, r2);
        Assert.AreEqual(0, r3);
    }

    [TestMethod]
    public void TaggedUnion3_ChainedMatching()
    {
        TaggedUnion<string, int, bool>[] unions = 
        {
            "one",
            2,
            true,
            "four",
            5,
            false
        };

        var results = new string[6];
        for (int i = 0; i < unions.Length; i++)
        {
            results[i] = unions[i].Match(
                s => $"str:{s}",
                n => $"num:{n}",
                b => $"bool:{b}"
            );
        }

        Assert.AreEqual("str:one", results[0]);
        Assert.AreEqual("num:2", results[1]);
        Assert.AreEqual("bool:True", results[2]);
        Assert.AreEqual("str:four", results[3]);
        Assert.AreEqual("num:5", results[4]);
        Assert.AreEqual("bool:False", results[5]);
    }

    [TestMethod]
    public void TaggedUnion3_NestedUnion()
    {
        TaggedUnion<TaggedUnion<string, int>, bool, Exception> outer = 
            TaggedUnion._0<string, int>("nested");

        var result = outer.Match(
            inner => inner.Match(s => s, i => i.ToString()),
            b => b.ToString(),
            e => e.Message
        );
        Assert.AreEqual("nested", result);
    }

    [TestMethod]
    public void TaggedUnion3_WithNullableReferenceTypes()
    {
        TaggedUnion<string, int, Exception> union = new ArgumentNullException("param");
        var result = union.Match(
            s => s,
            i => i.ToString(),
            e => e is ArgumentNullException ? "null arg" : "other error"
        );
        Assert.AreEqual("null arg", result);
    }

    #endregion

    #region Edge Cases and Error Scenarios

    [TestMethod]
    public void TaggedUnion2_DefaultStruct_IsCase0()
    {
        TaggedUnion<string, int> union = default;
        Assert.IsTrue(union.Is0);
    }

    [TestMethod]
    public void TaggedUnion3_DefaultStruct_IsCase0()
    {
        TaggedUnion<string, int, bool> union = default;
        Assert.IsTrue(union.Is0);
    }

    [TestMethod]
    public void TaggedUnion2_SameTypes_WorksWithExplicitFactory()
    {
        var case0 = TaggedUnion<int, int>.__0(1);
        var case1 = TaggedUnion<int, int>.__1(2);

        Assert.IsTrue(case0.Is0);
        Assert.IsTrue(case1.Is1);

        var r0 = case0.Match(x => $"case0:{x}", x => $"case1:{x}");
        var r1 = case1.Match(x => $"case0:{x}", x => $"case1:{x}");

        Assert.AreEqual("case0:1", r0);
        Assert.AreEqual("case1:2", r1);
    }

    [TestMethod]
    public void TaggedUnion3_SameTypes_WorksWithExplicitFactory()
    {
        var case0 = TaggedUnion<int, int, int>.__0(1);
        var case1 = TaggedUnion<int, int, int>.__1(2);
        var case2 = TaggedUnion<int, int, int>.__2(3);

        Assert.IsTrue(case0.Is0);
        Assert.IsTrue(case1.Is1);
        Assert.IsTrue(case2.Is2);

        var r0 = case0.Match(x => $"c0:{x}", x => $"c1:{x}", x => $"c2:{x}");
        var r1 = case1.Match(x => $"c0:{x}", x => $"c1:{x}", x => $"c2:{x}");
        var r2 = case2.Match(x => $"c0:{x}", x => $"c1:{x}", x => $"c2:{x}");

        Assert.AreEqual("c0:1", r0);
        Assert.AreEqual("c1:2", r1);
        Assert.AreEqual("c2:3", r2);
    }

    [TestMethod]
    public void TaggedUnion2_LargeValue()
    {
        var largeString = new string('x', 10000);
        TaggedUnion<string, int> union = largeString;
        var result = union.Match(s => s.Length, i => i);
        Assert.AreEqual(10000, result);
    }

    [TestMethod]
    public void TaggedUnion2_WithGuid()
    {
        var guid = Guid.NewGuid();
        TaggedUnion<Guid, string> union = guid;
        var result = union.Match(g => g, s => Guid.Empty);
        Assert.AreEqual(guid, result);
    }

    [TestMethod]
    public void TaggedUnion2_WithDateTime()
    {
        var now = DateTime.UtcNow;
        TaggedUnion<DateTime, string> union = now;
        var result = union.Match(d => d, s => DateTime.MinValue);
        Assert.AreEqual(now, result);
    }

    #endregion

    #region Helper Types

    private class TestData
    {
        public int Value { get; set; }
    }

    private class ComplexData
    {
        public required string Name { get; set; }
        public int Count { get; set; }
    }

    #endregion
}
