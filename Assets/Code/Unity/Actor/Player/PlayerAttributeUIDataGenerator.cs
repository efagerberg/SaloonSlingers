using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public static class PlayerAttributeUIDataGenerator
    {
        public static MoneyChangedUIData GetMoneyUIData(ValueChangeEvent<uint> e, AudioClip lostSFX, AudioClip gainedSFX)
        {
            bool decreased = e.Before > e.After;
            uint delta = decreased ? e.Before - e.After : e.After - e.Before;
            char deltaChar = decreased ? '-' : '+';
            AudioClip clipToPlay = decreased ? lostSFX : gainedSFX;
            return new MoneyChangedUIData()
            {
                TotalText = e.After.ToString(),
                MoneyDeltaText = $"{deltaChar}{delta}",
                ClipToPlay = clipToPlay,
            };
        }

        public static string GetMoneyUIData(IReadOnlyPoints points)
        {
            return points.Value.ToString();
        }

        public static HealthUIData GetHealthUIData(IReadOnlyPoints hitPoints)
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
        public string MoneyDeltaText;
        public AudioClip ClipToPlay;
    }

    public struct HealthUIData
    {
        public float FillAmount;
        public string HealthPercentText;
    }
}
