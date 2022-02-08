using System.Collections.Generic;


namespace SaloonSlingers.Core
{
    public class HandRotationCalculator
    {
        public static IEnumerable<float> CalculateRotations(int n, float totalDegrees)
        {
            float anglePerCard = totalDegrees / n;
            float start = -totalDegrees / 2f;
            int mid = n / 2;
            bool isCenterElement(int i) => n % 2 != 0 && i == mid;
            bool isPastMid(int i) => n % 2 == 0 ? i >= mid : i > mid;

            for (int i = 0; i < n; i++)
            {
                float offset;
                if (isCenterElement(i)) offset = -start;
                else if (isPastMid(i)) offset = (i + 1) * anglePerCard;
                else offset = i * anglePerCard;
                yield return start + offset;
            }
        }
    }
}
