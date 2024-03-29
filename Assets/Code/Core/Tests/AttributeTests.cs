using System;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class AttributeTests
    {
        [Test]
        public void Constructor_HasExpectedFieldValues()
        {
            var subject = new Attribute(2, 4);

            Assert.AreEqual(subject.MaxValue, 4);
            Assert.AreEqual(subject.InitialValue, 2);
            Assert.That<uint>(subject, Is.EqualTo(2));
        }

        [Test]
        public void Unchanged_DoesNotEmitEvent()
        {
            var subject = new Attribute(2);
            subject.Increased += FailIfHandled;
            subject.Decreased += FailIfHandled;
            subject.Increase(0);
        }

        [Test]
        public void Decrease_EmitsDecreaseEvent()
        {
            var subject = new Attribute(1);
            bool eventHandled = false;
            void handler(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
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
        public void Decrement_EmitsDecreaseEvent()
        {
            var subject = new Attribute(1);
            bool eventHandled = false;
            void handler(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
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
        public void Increase_EmitsIncreaseEvent()
        {
            var subject = new Attribute(0, 1);
            bool eventHandled = false;
            void handler(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
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
        public void DecreaseTo0_EmitsDepleteEvent()
        {
            var subject = new Attribute(1, 1);
            bool eventHandled = false;
            void handler(IReadOnlyAttribute sender, EventArgs e)
            {
                eventHandled = true;
                Assert.AreEqual(sender.Value, 0);
            }
            subject.Depleted += handler;
            subject.Decrease(10);

            Assert.That(eventHandled);
        }

        [Test]
        public void PastMax_ClampsToMax()
        {
            var subject = new Attribute(2);
            subject.Increased += FailIfHandled;
            subject.Increase(100);

            Assert.That<uint>(subject, Is.EqualTo(2));
        }

        [Test]
        public void BelowMin_ClampsToMin()
        {
            var subject = new Attribute(0, 1);
            subject.Decreased += FailIfHandled;
            subject.Increased += FailIfHandled;
            subject.Decrement();

            Assert.That<uint>(subject, Is.EqualTo(0));
        }

        [Test]
        public void SetsMaxAndIncreases_ClampsToMax()
        {
            var subject = new Attribute(2, 3);
            subject.Increase(100);

            Assert.That(subject.MaxValue, Is.EqualTo(3));
            Assert.That<uint>(subject, Is.EqualTo(subject.MaxValue));
        }

        private void FailIfHandled(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
        {
            Assert.Fail($"Handled unexpected event {e}");
        }

        [Test]
        public void Reset_ReturnsValueToInitialValue()
        {
            var subject = new Attribute(10);
            subject.Decrease(9);
            subject.Reset();

            Assert.That(subject.InitialValue, Is.EqualTo(subject.Value));
        }


        [Test]
        public void Reset_WithNewValue_SetsValueAndInitialToNewValue()
        {
            var subject = new Attribute(10);
            subject.Decrease(9);
            subject.Reset(2);

            Assert.That<uint>(subject, Is.EqualTo(2));
            Assert.That(subject.InitialValue, Is.EqualTo(subject.Value));
        }

        [Test]
        public void AsPercent_Returns0_WhenNoStartingValue()
        {
            var subject = new Attribute(0);

            Assert.That(subject.AsPercent(), Is.EqualTo(0));
        }

        [Test]
        public void AsPercent_Returns1_WhenStartingValuePositive()
        {
            var subject = new Attribute(1);

            Assert.That(subject.AsPercent(), Is.EqualTo(1));
        }

        [Test]
        public void AsPercent_ReturnsFraction_WhenDecreasedWithRemainder()
        {
            var subject = new Attribute(2);
            subject.Decrement();

            Assert.That(subject.AsPercent(), Is.EqualTo(0.5));
        }
    }

}
