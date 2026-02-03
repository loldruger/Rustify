using Rustify.Utilities.Sync;

namespace Rustify.Tests;

[TestClass]
public sealed class WeakTests
{
    [TestMethod]
    public void Upgrade_ReturnsArc_WhenAlive()
    {
        using var arc = Arc<string>.New("test");
        var weak = arc.Downgrade();

        Assert.IsTrue(weak.IsAlive);
        var upgraded = weak.Upgrade();
        Assert.IsTrue(upgraded.IsSome());

        using var arcFromWeak = upgraded.Unwrap();
        Assert.AreEqual("test", arcFromWeak.GetValue());
    }

    [TestMethod]
    public void Upgrade_ReturnsNone_WhenDisposed()
    {
        var weak = CreateWeakFromDisposedArc();

        Assert.IsFalse(weak.IsAlive);
        var upgraded = weak.Upgrade();
        Assert.IsTrue(upgraded.IsNone());
    }

    private static Weak<string> CreateWeakFromDisposedArc()
    {
        var arc = Arc<string>.New("test");
        var weak = arc.Downgrade();
        arc.Dispose();
        return weak;
    }

    [TestMethod]
    public void MultipleWeakReferences_FromSameArc()
    {
        using var arc = Arc<string>.New("shared");
        var weak1 = arc.Downgrade();
        var weak2 = arc.Downgrade();

        Assert.IsTrue(weak1.IsAlive);
        Assert.IsTrue(weak2.IsAlive);
        Assert.AreEqual(2, weak1.WeakCount());
    }

    [TestMethod]
    public void IsAlive_ReturnsFalse_AfterAllArcsDisposed()
    {
        var arc1 = Arc<string>.New("test");
        var arc2 = arc1.Clone();
        var weak = arc1.Downgrade();

        Assert.IsTrue(weak.IsAlive);

        arc1.Dispose();
        Assert.IsTrue(weak.IsAlive);

        arc2.Dispose();
        Assert.IsFalse(weak.IsAlive);
    }

    [TestMethod]
    public void Downgrade_ThrowsWhenDisposed()
    {
        var arc = Arc<string>.New("test");
        arc.Dispose();

        Assert.Throws<InvalidOperationException>(() => arc.Downgrade());
    }
}
