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
        public void Start_ResetsToOriginalTime()
        {
            float original = 0.1f;
            var subject = new Timer(original);
            subject.Tick(0.05f);
            subject.Start();

            Assert.That(subject.Remaining, Is.EqualTo(original));
        }

        [Test]
        public void Start_OverridesDurationWhenGiven()
        {
            var subject = new Timer(0.1f);
            var expectedTime = 2f;
            subject.Start(expectedTime);

            Assert.That(subject.Remaining, Is.EqualTo(expectedTime));
        }
    }
}
