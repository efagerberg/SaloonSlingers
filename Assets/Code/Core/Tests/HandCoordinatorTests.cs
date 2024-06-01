using System.Collections.Generic;

using NUnit.Framework;

namespace SaloonSlingers.Core.Tests
{
    public class HandCoordinatorTests
    {
        [Test]
        public void Pickup_WithNoCards_DrawsCard()
        {
            HandCoordinator subject = new();
            var dependencies = CreateDependencies();
            subject.Assign(dependencies.deck, dependencies.registry);
            subject.Pickup(dependencies.game);

            Assert.That(subject.Cards.Count, Is.EqualTo(1));
        }

        [Test]
        public void Pickup_WithCards_DoesNotDrawAnotherCard()
        {
            HandCoordinator subject = new();
            var dependencies = CreateDependencies();
            subject.Assign(dependencies.deck, dependencies.registry);
            subject.TryDrawCard(dependencies.game);
            subject.Pickup(dependencies.game);

            Assert.That(subject.Cards.Count, Is.EqualTo(1));
        }

        private static (CardGame game,
                        Deck deck,
                        IReadOnlyDictionary<AttributeType, Attribute> registry) CreateDependencies()
        {
            CardGameConfig config = new()
            {
                HandEvaluator = "war"
            };
            CardGame game = CardGame.Load(config);
            Deck deck = new();
            IReadOnlyDictionary<AttributeType, Attribute> registry = new Dictionary<AttributeType, Attribute>();
            return (game, deck, registry);
        }

        [Test]
        public void Reset_ClearsCardsAndEvaluation()
        {
            HandCoordinator subject = new();
            var dependencies = CreateDependencies();
            subject.Assign(dependencies.deck, dependencies.registry);
            subject.Pickup(dependencies.game);
            subject.Reset();

            Assert.That(subject.Cards.Count, Is.EqualTo(0));
            Assert.That(subject.HandEvaluation,
                        Is.EqualTo(new HandEvaluation(HandNames.NONE, 0)));
        }
    }
}

