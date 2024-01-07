using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

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
        public void GetMoneyUIData_ReturnsData_ForDecrease()
        {
            var e = new ValueChangeEvent<uint>(2, 1);
            var data = PlayerAttributeUIDataGenerator.GetMoneyChangedUIData(e);

            Assert.That(data.DeltaText, Is.EqualTo("-1"));
            Assert.That(data.TotalText, Is.EqualTo("1"));
        }

        [Test]
        public void GetMoneyUIData_ReturnsData_ForIncrease()
        {
            var e = new ValueChangeEvent<uint>(20, 23);
            var data = PlayerAttributeUIDataGenerator.GetMoneyChangedUIData(e);

            Assert.That(data.DeltaText, Is.EqualTo("+3"));
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
