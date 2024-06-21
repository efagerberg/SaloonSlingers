using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class TimerTests
    {
        [TestCase(0.1f, 0.1f, true)]
        [TestCase(0.1f, 0f, false)]
        [TestCase(0.2f, 0.1f, false)]
        [TestCase(0.1f, 0.3f, true)]
        public void Tick_ReturnsExpectedResult(float seconds, float timePassed, bool expected)
        {
            var subject = new Timer(seconds);

            Assert.AreEqual(subject.Tick(timePassed), expected);
        }

        [Test]
        public void Tick_IncreasesElapsed()
        {
            var subject = new Timer(10);
            subject.Tick(2);

            Assert.That(subject.Elapsed, Is.EqualTo(2));

            subject.Tick(3);
            Assert.That(subject.Elapsed, Is.EqualTo(5));
        }

        [Test]
        public void Tick_DecreasesRemaining()
        {
            var subject = new Timer(10);
            subject.Tick(2);

            Assert.That(subject.Remaining, Is.EqualTo(8));

            subject.Tick(3);
            Assert.That(subject.Elapsed, Is.EqualTo(5));
        }

        [Test]
        public void Reset_ResetsToOriginalTime()
        {
            float original = 0.1f;
            var subject = new Timer(original);
            subject.Tick(0.05f);
            subject.Reset();

            Assert.That(subject.Remaining, Is.EqualTo(original));
            Assert.That(subject.Elapsed, Is.EqualTo(0));
        }

        [Test]
        public void Reset_OverridesDurationWhenGiven()
        {
            var subject = new Timer(0.1f);
            var expectedTime = 2f;
            subject.Reset(expectedTime);

            Assert.That(subject.Remaining, Is.EqualTo(expectedTime));
            Assert.That(subject.Elapsed, Is.EqualTo(0));
        }
    }
}
