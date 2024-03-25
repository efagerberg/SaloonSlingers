using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class TimerTests
    {
        [TestCase(0.1f, 0.1f, true)]
        [TestCase(0.1f, 0f, false)]
        [TestCase(0.2f, 0.1f, false)]
        [TestCase(0.1f, 0.3f, true)]
        public void CheckPassed_ReturnsExpectedResult(float seconds, float timePassed, bool expected)
        {
            var subject = new Timer(seconds);

            Assert.AreEqual(subject.CheckPassed(timePassed), expected);
        }

        [Test]
        public void Reset_ResetsToOriginalTime()
        {
            float original = 0.1f;
            var subject = new Timer(original);
            subject.CheckPassed(0.05f);
            subject.Reset();

            Assert.That(subject.DurationInSeconds, Is.EqualTo(original));
        }

        [Test]
        public void Reset_ResetsToPassedTime()
        {
            var subject = new Timer(0.1f);
            var expectedTime = 2f;
            subject.Reset(expectedTime);

            Assert.That(subject.DurationInSeconds, Is.EqualTo(expectedTime));
        }
    }
}
