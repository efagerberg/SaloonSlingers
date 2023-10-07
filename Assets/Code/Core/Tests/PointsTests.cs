using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class PointsTests
    {
        [Test]
        public void HasExpectedFieldValues_WhenConstructed()
        {
            var subject = new HitPoints(2, 4);

            Assert.AreEqual(subject.MaxValue, 4);
            Assert.AreEqual(subject.InitialValue, 2);
            Assert.AreEqual(subject.Value, 2);
        }

        [Test]
        public void Unchanged_DoesNotEmitEvent()
        {
            var subject = new HitPoints(2);
            subject.PointsIncreased += FailIfHandled;
            subject.Decreased += FailIfHandled;
            subject.Increase(0);
        }

        [Test]
        public void Decrease_EmitsEvent()
        {
            var subject = new HitPoints(1);
            bool eventHandled = false;
            void handler(HitPoints sender, ValueChangeEvent<uint> e)
            {
                eventHandled = true;
                Assert.AreEqual(e.Before, 1);
                Assert.AreEqual(e.After, 0);
            }
            subject.Decreased += handler;
            subject.PointsIncreased += FailIfHandled;
            subject.Decrease(1);

            Assert.That(eventHandled);
        }

        [Test]
        public void Decrement_EmitsEvent()
        {
            var subject = new HitPoints(1);
            bool eventHandled = false;
            void handler(HitPoints sender, ValueChangeEvent<uint> e)
            {
                eventHandled = true;
                Assert.AreEqual(e.Before, 1);
                Assert.AreEqual(e.After, 0);
            }
            subject.Decreased += handler;
            subject.PointsIncreased += FailIfHandled;
            subject.Decrement();

            Assert.That(eventHandled);
        }

        [Test]
        public void Increase_EmitsEvent()
        {
            var subject = new HitPoints(0, 1);
            bool eventHandled = false;
            void handler(HitPoints sender, ValueChangeEvent<uint> e)
            {
                eventHandled = true;
                Assert.AreEqual(e.Before, 0);
                Assert.AreEqual(e.After, 1);
            }
            subject.PointsIncreased += handler;
            subject.Decreased += FailIfHandled;
            subject.Increment();

            Assert.That(eventHandled);
        }

        [Test]
        public void PastMax_ClampsToMax()
        {
            var subject = new HitPoints(2);
            subject.PointsIncreased += FailIfHandled;
            subject.Increase(100);

            Assert.That(subject.Value, Is.EqualTo(2));
        }

        [Test]
        public void BelowMin_ClampsToMin()
        {
            var subject = new HitPoints(0, 1);
            subject.Decreased += FailIfHandled;
            subject.PointsIncreased += FailIfHandled;
            subject.Decrement();

            Assert.That(subject.Value, Is.EqualTo(0));
        }

        [Test]
        public void SetsMaxAndIncreases_ClampsToMax()
        {
            var subject = new HitPoints(2, 3);
            subject.Increase(100);

            Assert.That(subject.MaxValue, Is.EqualTo(3));
            Assert.That(subject.Value, Is.EqualTo(subject.MaxValue));
        }

        private void FailIfHandled(HitPoints sender, ValueChangeEvent<uint> e)
        {
            Assert.Fail($"Handled unexpected event {e}");
        }

        [Test]
        public void Reset_ReturnsValueToInitialValue()
        {
            var subject = new HitPoints(10);
            subject.Decrease(9);
            subject.Reset();

            Assert.That(subject.InitialValue, Is.EqualTo(subject.Value));
        }
    }

}
