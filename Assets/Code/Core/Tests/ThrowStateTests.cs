using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class ThrowStateTests
    {
        public class TestUpdate
        {
            [TestCase(false)]
            [TestCase(true)]
            public void Test_WhenNotThrow_IsAlive(bool shouldDespawn)
            {
                ThrowState x = new();
                Assert.True(x.Update(shouldDespawn).IsAlive);
            }

            [TestCase(false, false)]
            [TestCase(true, true)]
            public void Test_WhenThrown_IsAlive_ReturnsExpectedValue(bool shouldDespawn, bool expected)
            {
                ThrowState x = new();
                Assert.That(
                    x.Throw().Update(shouldDespawn).IsAlive,
                    Is.EqualTo(expected)
               );
            }
        }

        public class TestReset
        {
            [Test]
            public void Test_IsAlive()
            {
                ThrowState x = new();
                Assert.IsTrue(x.Reset().IsAlive);
            }

            [Test]
            public void Test_WhenWasNotAlive_IsAlive()
            {
                ThrowState x = new();
                Assert.IsTrue(x.Update(true).Reset().IsAlive);
            }
        }
    }
}
