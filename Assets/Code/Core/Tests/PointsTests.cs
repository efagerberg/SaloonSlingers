using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class PointsTests
    {
        [Test]
        public void HasExpectedFieldValues_WhenConstructed()
        {
            var subject = new Points(2, 4);

            Assert.AreEqual(subject.MaxValue, 4);
            Assert.AreEqual(subject.InitialValue, 2);
            Assert.AreEqual(subject.Value, 2);
        }

        [Test]
        public void Unchanged_DoesNotEmitEvent()
        {
            var subject = new Points(2);
            subject.Increased += FailIfHandled;
            subject.Decreased += FailIfHandled;
            subject.Increase(0);
        }

        [Test]
        public void Decrease_EmitsEvent()
        {
            var subject = new Points(1);
            bool eventHandled = false;
            void handler(Points sender, ValueChangeEvent<uint> e)
            {
                eventHandled = true;
                Assert.AreEqual(e.Before, 1);
                Assert.AreEqual(e.After, 0);
            }
            subject.Decreased += handler;
            subject.Increased += FailIfHandled;
            subject.Decrease(1);

            Assert.That(eventHandled);
        }

        [Test]
        public void Decrement_EmitsEvent()
        {
            var subject = new Points(1);
            bool eventHandled = false;
            void handler(Points sender, ValueChangeEvent<uint> e)
            {
                eventHandled = true;
                Assert.AreEqual(e.Before, 1);
                Assert.AreEqual(e.After, 0);
            }
            subject.Decreased += handler;
            subject.Increased += FailIfHandled;
            subject.Decrement();

            Assert.That(eventHandled);
        }

        [Test]
        public void Increase_EmitsEvent()
        {
            var subject = new Points(0, 1);
            bool eventHandled = false;
            void handler(Points sender, ValueChangeEvent<uint> e)
            {
                eventHandled = true;
                Assert.AreEqual(e.Before, 0);
                Assert.AreEqual(e.After, 1);
            }
            subject.Increased += handler;
            subject.Decreased += FailIfHandled;
            subject.Increment();

            Assert.That(eventHandled);
        }

        [Test]
        public void PastMax_ClampsToMax()
        {
            var subject = new Points(2);
            subject.Increased += FailIfHandled;
            subject.Increase(100);

            Assert.That(subject.Value, Is.EqualTo(2));
        }

        [Test]
        public void BelowMin_ClampsToMin()
        {
            var subject = new Points(0, 1);
            subject.Decreased += FailIfHandled;
            subject.Increased += FailIfHandled;
            subject.Decrement();

            Assert.That(subject.Value, Is.EqualTo(0));
        }

        [Test]
        public void SetsMaxAndIncreases_ClampsToMax()
        {
            var subject = new Points(2, 3);
            subject.Increase(100);

            Assert.That(subject.MaxValue, Is.EqualTo(3));
            Assert.That(subject.Value, Is.EqualTo(subject.MaxValue));
        }

        private void FailIfHandled(Points sender, ValueChangeEvent<uint> e)
        {
            Assert.Fail($"Handled unexpected event {e}");
        }

        [Test]
        public void Reset_ReturnsValueToInitialValue()
        {
            var subject = new Points(10);
            subject.Decrease(9);
            subject.Reset();

            Assert.That(subject.InitialValue, Is.EqualTo(subject.Value));
        }
    }

}
