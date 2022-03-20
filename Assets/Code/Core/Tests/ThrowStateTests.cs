using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class ThrowStateTests
    {
        public class TestUpdate
        {
            [TestCase(10f, 1f, 0.1f)]
            [TestCase(0f, 0f, 0f)]
            [TestCase(10f, 0f, 10f)]
            public void Test_WhenNotThrow_IsAlive(float maxLifetime, float velocityMagniutude, float deltaTime)
            {
                ThrowState x = new(maxLifetime);
                Assert.True(x.Update(velocityMagniutude, deltaTime).IsAlive);
            }

            [TestCase(0f, 1f, 1f, false)]
            [TestCase(1f, 1f, 0.5f, true)]
            [TestCase(1f, 1f, 1f, false)]
            [TestCase(1f, 0f, 0.5f, false)]
            public void Test_WhenThrown_IsAlive_ReturnsExpectedValue(float maxLifetime, float velocityMagnitude, float deltaTime, bool expected)
            {
                ThrowState x = new(maxLifetime);
                Assert.That(
                    x.Throw().Update(velocityMagnitude, deltaTime).IsAlive,
                    Is.EqualTo(expected)
               );
            }
        }

        public class TestReset
        {
            [Test]
            public void Test_IsAlive()
            {
                ThrowState x = new(10f);
                Assert.IsTrue(x.Reset().IsAlive);
            }

            [Test]
            public void Test_WhenWasNotAlive_IsAlive()
            {
                ThrowState x = new(0.4f);
                Assert.IsTrue(x.Update(0f, 100f).Reset().IsAlive);
            }
        }
    }
}
