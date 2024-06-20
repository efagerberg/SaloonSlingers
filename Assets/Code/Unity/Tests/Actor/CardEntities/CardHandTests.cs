using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{

    public class HandTryDrawTests
    {
        [Test]
        public void AddsCard_ToProjectile()
        {
            var subject = HandTestHelper.BuildSubject();
            subject.TryDrawCard(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);

            Assert.That(subject.Cards.Count, Is.EqualTo(1));
        }

        [Test]
        public void EmitsDrawEvent()
        {
            var subject = HandTestHelper.BuildSubject();
            ICardGraphic cardDrawn = null;
            void drawHandler(CardHand sender, ICardGraphic c) => cardDrawn = c;
            subject.Drawn.AddListener(drawHandler);
            subject.Pickup(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);

            Assert.IsNotNull(cardDrawn);
            Assert.That(cardDrawn.Card, Is.EqualTo(subject.Cards.ElementAt(0)));
        }

    }

    public class HandPickupTests
    {
        [Test]
        public void WhenEmptyHand_EmitsPickupAndDrawEvents()
        {
            var subject = HandTestHelper.BuildSubject();
            var eventsConsumed = new List<string>();
            void pickupHandler(CardHand sender) => eventsConsumed.Add("pickup");
            void drawHandler(CardHand sender, ICardGraphic c) => eventsConsumed.Add("draw");
            subject.PickedUp.AddListener(pickupHandler);
            subject.Drawn.AddListener(drawHandler);
            subject.Pickup(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);

            Assert.That(eventsConsumed, Is.EqualTo(new List<string> { "pickup", "draw" }));
        }

        [Test]
        public void WhenNotEmptyHand_EmitsOnlyPickupEvent()
        {
            var subject = HandTestHelper.BuildSubject();
            subject.TryDrawCard(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);
            var eventsConsumed = new List<string>();
            void pickupHandler(CardHand sender) => eventsConsumed.Add("pickup");
            void drawHandler(CardHand sender, ICardGraphic c) => eventsConsumed.Add("draw");
            subject.PickedUp.AddListener(pickupHandler);
            subject.Drawn.AddListener(drawHandler);
            subject.Pickup(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);

            Assert.That(eventsConsumed, Is.EqualTo(new List<string> { "pickup" }));
        }

        [Test]
        public void WhenNoCardsToDraw_EmitsOnlyPickupEvent()
        {
            var subject = HandTestHelper.BuildSubject(0);
            subject.TryDrawCard(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);
            var eventsConsumed = new List<string>();
            void pickupHandler(CardHand sender) => eventsConsumed.Add("pickup");
            void drawHandler(CardHand sender, ICardGraphic c) => eventsConsumed.Add("draw");
            subject.PickedUp.AddListener(pickupHandler);
            subject.Drawn.AddListener(drawHandler);
            subject.Pickup(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);

            Assert.That(eventsConsumed, Is.EqualTo(new List<string> { "pickup" }));
        }
    }

    public class HandPauseTests
    {
        [Test]
        public void EmitsEvent()
        {
            var subject = HandTestHelper.BuildSubject();
            var paused = false;
            void pausedHandler(CardHand sender) => paused = true;
            subject.Paused.AddListener(pausedHandler);
            subject.Pause();

            Assert.That(paused);
        }
    }

    public class HandEvaluationTests
    {
        [Test]
        public void GetsCurrentEvaluation()
        {
            var subject = HandTestHelper.BuildSubject();
            subject.TryDrawCard(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);

            Assert.That(subject.Evaluation.Name, Is.EqualTo(HandNames.HIGH_CARD));
        }
    }

    public class ResetHandTests
    {
        [UnityTest]
        public IEnumerator ResetsState()
        {
            var subject = HandTestHelper.BuildSubject();
            yield return null;
            subject.TryDrawCard(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);
            var rb = subject.GetComponent<Rigidbody>();
            subject.ResetHand();

            Assert.That(subject.Evaluation.Name, Is.EqualTo(HandNames.NONE));
        }
    }

    public class HandInitialEvaluateTests
    {
        [UnityTest]
        public IEnumerator Evaluates_WhenEmpty()
        {
            var subject = HandTestHelper.BuildSubject();
            yield return null;
            subject.InitialEvaluate(HandTestHelper.TestPokerGame);

            Assert.That(subject.Cards.Count, Is.EqualTo(0));
            Assert.That(subject.Evaluation, Is.EqualTo(new HandEvaluation(HandNames.NONE, 0)));
        }
    }

    public static class HandTestHelper
    {
        public static GameObject TestCardSpawner() => TestUtils.CreateComponent<TestUtils.TestCardGraphic>().gameObject;
        public static CardGame TestPokerGame = CardGame.Load(new CardGameConfig() { HandEvaluator = "poker" });
        public static CardHand BuildSubject(int nCards = 10)
        {
            var subject = TestUtils.CreateComponent<CardHand>();
            subject.runInEditMode = true;
            subject.Assign(new Deck(nCards), new Dictionary<AttributeType, Core.Attribute> { });
            return subject;
        }
    }
}
