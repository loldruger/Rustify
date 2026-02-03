using Rustify.Utilities;

namespace Rustify.Tests;

[TestClass]
public sealed class UnitTests
{
    [TestMethod]
    public void New_ReturnsDefaultUnit()
    {
        var unit = Unit.New;
        Assert.AreEqual(default(Unit), unit);
    }

    [TestMethod]
    public void Equals_TwoUnits_ReturnsTrue()
    {
        var unit1 = Unit.New;
        var unit2 = Unit.New;
        Assert.AreEqual(unit1, unit2);
        Assert.IsTrue(unit1.Equals(unit2));
    }

    [TestMethod]
    public void Equals_UnitAndObject_ReturnsTrueForUnit()
    {
        var unit = Unit.New;
        object obj = Unit.New;
        Assert.IsTrue(unit.Equals(obj));
    }

    [TestMethod]
    public void Equals_UnitAndNonUnit_ReturnsFalse()
    {
        var unit = Unit.New;
        Assert.IsFalse(unit.Equals("not a unit"));
        Assert.IsFalse(unit.Equals(42));
        Assert.IsFalse(unit.Equals(null));
    }

    [TestMethod]
    public void GetHashCode_AlwaysReturnsZero()
    {
        var unit1 = Unit.New;
        var unit2 = Unit.New;
        Assert.AreEqual(0, unit1.GetHashCode());
        Assert.AreEqual(0, unit2.GetHashCode());
        Assert.AreEqual(unit1.GetHashCode(), unit2.GetHashCode());
    }

    [TestMethod]
    public void ToString_ReturnsEmptyTuple()
    {
        var unit = Unit.New;
        Assert.AreEqual("()", unit.ToString());
    }

    [TestMethod]
    public void EqualityOperator_TwoUnits_ReturnsTrue()
    {
        var unit1 = Unit.New;
        var unit2 = Unit.New;
        Assert.IsTrue(unit1 == unit2);
    }

    [TestMethod]
    public void InequalityOperator_TwoUnits_ReturnsFalse()
    {
        var unit1 = Unit.New;
        var unit2 = Unit.New;
        Assert.IsFalse(unit1 != unit2);
    }

    [TestMethod]
    public void CanBeUsedInGenericContext()
     {
         var result = Rustify.Monads.Result<Unit, string>.Ok(Unit.New);
         Assert.IsTrue(result.IsOk());
         Assert.AreEqual(Unit.New, result.Unwrap());
     }
}
