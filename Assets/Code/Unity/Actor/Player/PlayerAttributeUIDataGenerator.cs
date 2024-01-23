using SaloonSlingers.Core;

namespace SaloonSlingers.Unity.Actor
{
    public static class PlayerAttributeUIDataGenerator
    {
        public static MoneyChangedUIData GetMoneyChangedUIData(ValueChangeEvent<uint> e)
        {
            bool decreased = e.Before > e.After;
            uint delta = decreased ? e.Before - e.After : e.After - e.Before;
            char deltaChar = decreased ? '-' : '+';
            return new MoneyChangedUIData()
            {
                TotalText = e.After.ToString(),
                DeltaText = $"{deltaChar}{delta}"
            };
        }

        public static string GetMoneyUIData(IReadOnlyAttribute points)
        {
            return points.Value.ToString();
        }

        public static HealthUIData GetHealthUIData(IReadOnlyAttribute hitPoints)
        {
            return new HealthUIData()
            {
                FillAmount = hitPoints.AsPercent(),
                HealthPercentText = hitPoints.AsPercent().ToString("P0")
            };
        }
    }

    public struct MoneyChangedUIData
    {
        public string TotalText;
        public string DeltaText;
    }

    public struct HealthUIData
    {
        public float FillAmount;
        public string HealthPercentText;
    }
}
