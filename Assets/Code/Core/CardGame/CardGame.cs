using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace SaloonSlingers.Core
{
    public struct CardGame : IDrawRule, IHandEvaluator
    {
        public string Name { get; private set; }

        private IList<IDrawRule> drawRules;
        private IHandEvaluator handEvaluator;

        public readonly bool CanDraw(DrawContext ctx)
        {
            return ctx.Deck.HasCards && drawRules.All(x => x.CanDraw(ctx));
        }

        public readonly HandEvaluation Evaluate(IEnumerable<Card> hand) => handEvaluator.Evaluate(hand);

        public static CardGame Load(CardGameConfig config)
        {
            var rules = new CardGame
            {
                Name = config.Name,
                handEvaluator = GetHandEvaluatorFromString(config.HandEvaluator),
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

        private static IList<IDrawRule> GetDrawRulesFromConfig(CardGameConfig config)
        {
            var rules = new List<IDrawRule>();
            if (config.MaxHandSize.HasValue)
                rules.Add(new MaxHandSizeDrawRule(config.MaxHandSize.Value));
            if (config.MaxScore.HasValue)
                rules.Add(new MaxScoreDrawRule(config.MaxScore.Value));
            if (config.MinScore.HasValue)
                rules.Add(new MinScoreDrawRule(config.MinScore.Value));
            return rules;
        }
    }

    public class InvalidGameRulesConfig : Exception
    {
        public InvalidGameRulesConfig(string message) : base(message) { }
    }

    public struct CardGameConfig
    {
        public string Name { get; set; }
        public string HandEvaluator { get; set; }
        public int? MaxHandSize { get; set; }
        public uint? MaxScore { get; set; }
        public uint? MinScore { get; set; }
    }
}
