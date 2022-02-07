using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Core
{
    public static class HealthSystem
    {
        public static void DoDamage(ISlingerAttributes source, IEnumerable<ISlingerAttributes> targets)
        {
            uint sourceScore = source.Hand.HandType.Score;
            bool targetFilter(ISlingerAttributes target)
            {
                return target.Health > 0 && sourceScore > target.Hand.HandType.Score;
            }

            targets.Where(targetFilter).ToList().ForEach(target => target.Health -= 1);
        }
    }
}
