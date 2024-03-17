using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class HandProjectileStateTests
    {
        public class TestIsAlive
        {
            [Test]
            public void WhenNotThrown_IsAlive()
            {
                HandProjectileState x = new(1f);
                Assert.IsTrue(x.IsAlive);
            }

            [TestCase(1f, 0.5f, true)]
            [TestCase(1f, 1f, false)]
            public void WhenThrownAndUpdated_HasCorrectAliveValue(float lifespanInSeconds, float deltaTime, bool expected)
            {
                HandProjectileState x = new HandProjectileState(lifespanInSeconds).Throw();
                x.Update(deltaTime);

                Assert.That(
                    x.Throw().IsAlive,
                    Is.EqualTo(expected)
               );
            }
        }

        public class TestPause
        {
            [Test]
            public void WhenThrown_PauseSetsIsThrownToFalse()
            {
                HandProjectileState x = new(1f);
                Assert.IsFalse(x.Throw().Pause().IsThrown);
            }

            [Test]
            public void WhenNotThrown_PauseSetsIsThrownToFalse()
            {
                HandProjectileState x = new(1f);
                Assert.IsFalse(x.Pause().IsThrown);
            }
        }

        public class TestReset
        {
            [Test]
            public void WhenUnthrownStateWithCheckAliveTrueAndReset_IsAlive()
            {
                HandProjectileState x = new(1f);
                Assert.IsTrue(x.Reset().IsAlive);
            }

            [Test]
            public void WhenThrownStateWithCheckAliveFalseAndReset_IsAlive()
            {
                HandProjectileState x = new HandProjectileState(1f).Throw();
                x.Update(1f);

                Assert.IsFalse(x.IsAlive);
                Assert.IsTrue(x.Reset().IsAlive);
            }
        }
    }
}
