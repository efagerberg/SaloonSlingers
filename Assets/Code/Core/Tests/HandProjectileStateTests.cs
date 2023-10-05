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

        public class TestToggleCommit
        {
            [Test]
            public void WhenNotCommitted_IsCommitted()
            {
                HandProjectileState x = new HandProjectileState(1f);
                Assert.IsTrue(x.ToggleCommit().IsCommitted);
            }

            [Test]
            public void WhenCommitted_IsUncommitted()
            {
                HandProjectileState x = new(1f);
                Assert.IsFalse(x.ToggleCommit().ToggleCommit().IsCommitted);
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
