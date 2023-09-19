using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace SaloonSlingers.Core
{
    public struct GameRules : IDrawRule, IHandEvaluator
    {
        public string Name { get; private set; }

        private IList<IDrawRule> drawRules;
        private IHandEvaluator handEvaluator;

        public readonly bool CanDraw(DrawContext ctx)
        {
            return ctx.Deck.HasCards && drawRules.All(x => x.CanDraw(ctx));
        }

        public readonly HandEvaluation Evaluate(IEnumerable<Card> hand) => handEvaluator.Evaluate(hand);

        public static GameRules Load(string raw)
        {
            IReadOnlyDictionary<string, object> config;
            config = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);

            var rules = new GameRules
            {
                Name = (string)config["Name"],
                handEvaluator = GetHandEvaluatorFromString((string)config["HandEvaluator"]),
                drawRules = GetDrawRulesFromConfig(config)
            };
            return rules;
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

        private static IList<IDrawRule> GetDrawRulesFromConfig(IReadOnlyDictionary<string, object> rawConfig)
        {
            var rules = new List<IDrawRule>();
            if (rawConfig.ContainsKey("MaxHandSize"))
            {
                int maxHandSize = Convert.ToInt32(rawConfig["MaxHandSize"]);
                rules.Add(new MaxHandSizeDrawRule(maxHandSize));
            }
            if (rawConfig.ContainsKey("MaxScore"))
            {
                uint maxScore = Convert.ToUInt32(rawConfig["MaxScore"]);
                rules.Add(new MaxScoreDrawRule(maxScore));
            }
            if (rawConfig.ContainsKey("MinScore"))
            {
                uint minScore = Convert.ToUInt32(rawConfig["MinScore"]);
                rules.Add(new MinScoreDrawRule(minScore));
            }
            return rules;
        }
    }

    public class InvalidGameRulesConfig : Exception
    {
        public InvalidGameRulesConfig(string message) : base(message) { }
    }
}
