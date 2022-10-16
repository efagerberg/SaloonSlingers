using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using SaloonSlingers.Core.HandEvaluators;

namespace SaloonSlingers.Core
{
    public struct GameRules
    {
        public string Name { get; set; }
        public int MaxHandSize { get; set; }
        public IHandEvaluator HandEvaluator { get; set; }

        public static GameRules Load(string raw)
        {
            IReadOnlyDictionary<string, object> rawConfig;
            rawConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);

            return new GameRules
            {
                Name = (string)rawConfig["Name"],
                MaxHandSize = Convert.ToInt32(rawConfig["MaxHandSize"]),
                HandEvaluator = GetHandEvaluatorFromString((string)rawConfig["HandEvaluator"])
            };
        }

        private static IHandEvaluator GetHandEvaluatorFromString(string v)
        {
            return Regex.Replace(v.ToLower(), @"\s", "") switch
            {
                "blackjack" => new BlackJackHandEvaluator(),
                "poker" => new PokerHandEvaluator(),
                _ => throw new InvalidGameRulesConfig($"Unknown value for HandEvaluator {v}"),
            };
        }
    }

    public class InvalidGameRulesConfig : Exception
    {
        public InvalidGameRulesConfig(string message) : base(message) { }
    }
}
