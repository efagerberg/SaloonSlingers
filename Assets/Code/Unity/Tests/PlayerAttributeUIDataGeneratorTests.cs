using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity.Tests
{
    public class PlayerAttributeUIDataGeneratorTests
    {
        [Test]
        public void GetMoneyUIData_ReturnsTotal_WhenGivenPoints()
        {
            var money = new Points(100);
            var data = PlayerAttributeUIDataGenerator.GetMoneyUIData(money);

            Assert.That(data, Is.EqualTo("100"));
        }

        [Test]
        public void GetMoneyUIData_ReturnsData_ForMoneyLost()
        {
            var lostSFX = AudioClip.Create("TestClip1", 100, 1, 1000, false);
            var gainedSFX = AudioClip.Create("TestClip2", 1, 1, 1200, false);
            var e = new ValueChangeEvent<uint>(2, 1);
            var data = PlayerAttributeUIDataGenerator.GetMoneyUIData(e, lostSFX, gainedSFX);

            Assert.That(data.ClipToPlay, Is.EqualTo(lostSFX));
            Assert.That(data.MoneyDeltaText, Is.EqualTo("-1"));
            Assert.That(data.TotalText, Is.EqualTo("1"));
        }

        [Test]
        public void GetMoneyUIData_ReturnsData_ForMoneyGained()
        {
            var lostSFX = AudioClip.Create("TestClip1", 100, 1, 1000, false);
            var gainedSFX = AudioClip.Create("TestClip2", 1, 1, 1200, false);
            var e = new ValueChangeEvent<uint>(20, 23);
            var data = PlayerAttributeUIDataGenerator.GetMoneyUIData(e, lostSFX, gainedSFX);

            Assert.That(data.ClipToPlay, Is.EqualTo(gainedSFX));
            Assert.That(data.MoneyDeltaText, Is.EqualTo("+3"));
            Assert.That(data.TotalText, Is.EqualTo("23"));
        }

        [Test]
        public void GetHealthUIData_ReturnsExpected_FillAndPercentText()
        {
            var points = new Points(10);
            points.Decrease(2);
            var data = PlayerAttributeUIDataGenerator.GetHealthUIData(points);

            Assert.That(data.FillAmount, Is.EqualTo(0.8f));
            Assert.That(data.HealthPercentText, Is.EqualTo("80 %"));
        }
    }
}
