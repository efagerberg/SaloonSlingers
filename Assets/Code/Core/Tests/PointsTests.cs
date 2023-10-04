using System.Runtime.InteropServices;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class PointsTests
    {
        [Test]
        public void TestPointsSetButUnchangedDoesNotEmitEvent()
        {
            var subject = new Points(2);
            subject.PointsIncreased += failIfHandled;
            subject.PointsDecreased += failIfHandled;
            subject.Value = subject.Value;
        }

        [Test]
        public void TestPointsDecreaseEmitsEvent()
        {
            var subject = new Points(2);
            subject.Value--;
            bool eventHandled = false;
            void handler(Points sender, ValueChangeEvent<uint> e)
            {
                eventHandled = true;
                Assert.AreEqual(e.Before, 2);
                Assert.AreEqual(e.After, 1);
            }
            subject.PointsDecreased += handler;
            subject.PointsIncreased += failIfHandled;
            subject.Value--;

            Assert.That(eventHandled);
        }

        [Test]
        public void TestPointsIncreaseEmitsEvent()
        {
            var subject = new Points(2);
            subject.Value--;
            bool eventHandled = false;
            void handler(Points sender, ValueChangeEvent<uint> e)
            {
                eventHandled = true;
                Assert.AreEqual(e.Before, 2);
                Assert.AreEqual(e.After, 1);
            }
            subject.PointsIncreased += handler;
            subject.PointsDecreased += failIfHandled;
            subject.Value++;

            Assert.That(eventHandled);
        }

        [Test]
        public void TestPointsPastMaxIsClampedToMax()
        {
            var subject = new Points(2);
            subject.Value *= 2;
            subject.PointsIncreased += failIfHandled;

            Assert.That(subject.Value, Is.EqualTo(2));
        }

        [Test]
        public void TestPointsBelowMinIsClampedTo0()
        {
            var subject = new Points(2);
            subject.Value -= 10;
            subject.PointsDecreased += failIfHandled;

            Assert.That(subject.Value, Is.EqualTo(0));
        }

        private void failIfHandled(Points sender, ValueChangeEvent<uint> e)
        {
            Assert.Fail($"Handled unexpected event {e}");
        }
    }

}
