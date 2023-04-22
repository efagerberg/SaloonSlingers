using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class HandProjectileStateTests
    {
        public class TestIsAlive
        {
            [Test]
            public void Test_WhenNotThrown_IsAlive()
            {
                HandProjectileState x = new(1f, () => false);
                Assert.IsTrue(x.IsAlive);
            }

            [TestCase(1f, 0.5f, true)]
            [TestCase(1f, 1f, false)]
            public void Test_WhenThrown_AndUpdated_IsAlive_ReturnsExpectedValue(float lifespanInSeconds, float deltaTime, bool expected)
            {
                HandProjectileState x = new HandProjectileState(lifespanInSeconds, () => false).Throw();
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
            public void Test_WhenNotCommitted_IsCommitted()
            {
                HandProjectileState x = new HandProjectileState(1f, () => true);
                Assert.IsTrue(x.ToggleCommit().IsCommitted);
            }

            [Test]
            public void Test_WhenCommitted_IsUncommitted()
            {
                HandProjectileState x = new(1f, () => true);
                Assert.IsFalse(x.ToggleCommit().ToggleCommit().IsCommitted);
            }
        }

        public class TestCanDraw
        {
            [TestCase(false)]
            [TestCase(true)]
            public void Test_WhenNotCommitte_ReturnsCheckCanDraw(bool shouldBeAbleToDraw)
            {
                HandProjectileState x = new(1f, () => shouldBeAbleToDraw);
                Assert.AreEqual(x.CanDraw, shouldBeAbleToDraw);
            }

            [TestCase(false)]
            [TestCase(true)]
            public void Test_WhenCommitted_ReturnsFalse(bool shouldBeAbleToDraw)
            {
                HandProjectileState x = new(1f, () => shouldBeAbleToDraw);
                Assert.IsFalse(x.ToggleCommit().CanDraw);
            }
        }

        public class TestReset
        {
            [Test]
            public void Test_WhenUnthrownStateWithCheckAliveTrue_IsAliveAfterResetting()
            {
                HandProjectileState x = new(1f, () => false);
                Assert.IsTrue(x.Reset().IsAlive);
            }

            [Test]
            public void Test_WhenThrownStateWithCheckAliveFalse_IsAliveAfterResetting()
            {
                HandProjectileState x = new HandProjectileState(1f, () => false).Throw();
                x.Update(1f);

                Assert.IsFalse(x.IsAlive);
                Assert.IsTrue(x.Reset().IsAlive);
            }
        }
    }
}
