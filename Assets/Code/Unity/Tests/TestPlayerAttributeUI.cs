using System.Collections;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity.Tests
{
    public class PlayerAttributeUICoordinatorTests
    {
        [Test]
        public void MoneyChangeEffect_DisplaysDecrease_ThenNewTotal()
        {
            UIData data = GenerateUIData();
            var e = new ValueChangeEvent<uint>(2, 1);
            var delay = new WaitForSeconds(0.1f);
            var coroutine = PlayerAttributeUICoordinator.MoneyChangeEffect(data, e, delay);

            AssertDoesMoneyEventEffect(data, "-1", "1", delay, coroutine);
        }

        [Test]
        public void MoneyChangeEffect_DisplaysIncrease_ThenNewTotal()
        {
            UIData data = GenerateUIData();
            var e = new ValueChangeEvent<uint>(10, 20);
            var delay = new WaitForSeconds(0.1f);
            var coroutine = PlayerAttributeUICoordinator.MoneyChangeEffect(data, e, delay);

            AssertDoesMoneyEventEffect(data, "+10", "20", delay, coroutine);
        }

        [Test]
        public void UpdateHealthBar_UpdatesHealthBarFill_And_PercentText()
        {
            UIData data = GenerateUIData();
            var points = new Points(10);
            points.Decrease(2);
            PlayerAttributeUICoordinator.UpdateHealthBar(data, points);

            Assert.That(data.HealthBar.fillAmount, Is.EqualTo(0.8f));
            Assert.That(data.HealthPercentText.text, Is.EqualTo("80 %"));
        }

        private static void AssertDoesMoneyEventEffect(UIData data, string changeStr, string totalString, WaitForSeconds delay, IEnumerator coroutine)
        {
            coroutine.MoveNext();
            var current = coroutine.Current;
            Assert.That(current, Is.EqualTo(delay));
            Assert.That(data.MoneyText.text, Is.EqualTo(changeStr));
            Assert.That(data.AudioSource.isPlaying);
            coroutine.MoveNext();
            Assert.That(data.MoneyText.text, Is.EqualTo(totalString));
            Assert.That(coroutine.MoveNext(), Is.False);
        }

        private static UIData GenerateUIData()
        {
            var image = TestUtils.CreateComponent<Image>();
            var healthText = TestUtils.CreateComponent<TextMeshProUGUI>();
            var audioSource = image.gameObject.AddComponent<AudioSource>();
            var moneyLostSFX = AudioClip.Create("TestClip1", 100, 1, 1000, false);
            var moneyGainedSFX = AudioClip.Create("TestClip2", 1, 1, 1200, false);
            var moneyText = TestUtils.CreateComponent<TextMeshProUGUI>();
            var data = new UIData()
            {
                HealthBar = image,
                HealthPercentText = healthText,
                AudioSource = audioSource,
                MoneyLostSFX = moneyLostSFX,
                MoneyGainedSFX = moneyGainedSFX,
                MoneyText = moneyText
            };
            return data;
        }
    }
}
