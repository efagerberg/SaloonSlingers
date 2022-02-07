using System;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity
{
    public interface ISlinger
    {
        public ISlingerAttributes Attributes { get; }
        public void GameRulesChangedHandler(GameRules rules, EventArgs eventArgs);
    }
}
