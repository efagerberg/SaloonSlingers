using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class HandInteractableStateTests
    {
        public class TestIsAlive
        {
            [TestCase(false)]
            [TestCase(true)]
            public void Test_WhenNotThrown_IsAlive(bool shouldDespawn)
            {
                CardHandState x = new(() => true, () => false);
                Assert.IsTrue(x.IsAlive);
            }

            [TestCase(false, true)]
            [TestCase(true, false)]
            public void Test_WhenThrown_IsAlive_ReturnsExpectedValue(bool shouldDespawn, bool expected)
            {
                CardHandState x = new(() => !shouldDespawn, () => false);
                Assert.That(
                    x.Throw().IsAlive,
                    Is.EqualTo(expected)
               );
            }
        }

        public class TestToggleCommit {
            [Test]
            public void Test_WhenNotCommitted_IsCommitted()
            {
                CardHandState x = new(() => true, () => true);
                Assert.IsTrue(x.ToggleCommit().IsCommitted);
            }

            [Test]
            public void Test_WhenCommitted_IsUncommitted()
            {
                CardHandState x = new(() => true, () => true);
                Assert.IsFalse(x.ToggleCommit().ToggleCommit().IsCommitted);
            }
        }

        public class TestCanDraw
        {
            [TestCase(false)]
            [TestCase(true)]
            public void Test_WhenNotCommitte_ReturnsCheckCanDraw(bool shouldBeAbleToDraw)
            {
                CardHandState x = new(() => true, () => shouldBeAbleToDraw);
                Assert.AreEqual(x.CanDraw, shouldBeAbleToDraw);
            }

            [TestCase(false)]
            [TestCase(true)]
            public void Test_WhenCommitted_ReturnsFalse(bool shouldBeAbleToDraw)
            {
                CardHandState x = new(() => true, () => shouldBeAbleToDraw);
                Assert.IsFalse(x.ToggleCommit().CanDraw);
            }
        }

        public class TestReset
        {
            [Test]
            public void Test_WhenUnthrownStateWithCheckAliveTrue_IsAliveAfterResetting()
            {
                CardHandState x = new(() => true, () => false);
                Assert.IsTrue(x.Reset().IsAlive);
            }

            [Test]
            public void Test_WhenThrownStateWithCheckAliveFalse_IsAliveAfterResetting()
            {
                CardHandState x = new CardHandState(() => false, () => false).Throw();
                Assert.IsFalse(x.IsAlive);
                Assert.IsTrue(x.Reset().IsAlive);
            }
        }
    }
}