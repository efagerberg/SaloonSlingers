using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;
using SaloonSlingers.Unity.Tests;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity
{
    public class HandAbsorberTests
    {
        [UnityTest]
        public IEnumerator Absorb_AbsorbsHandEvaluationWithOneLessStack_WhenStackaAvailable()
        {
            var subject = TestUtils.CreateComponent<HandAbsorber>();
            subject.runInEditMode = true;

            SetupCardHand(out CardHand hand, out CardGame game);
            hand.TryDrawCard(SpawnCardGraphic, game);
            hand.runInEditMode = true;
            var absorbingAttribute = new Attribute(0, uint.MaxValue);
            subject.Absorb(absorbingAttribute, hand);
            yield return null;

            Assert.That(absorbingAttribute.Value, Is.EqualTo(hand.Evaluation.Score));
            Assert.That(subject.Stacks.Value, Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator Absorb_CannotAbsorb_WhenNoStacksLeft()
        {
            var subject = TestUtils.CreateComponent<HandAbsorber>();
            subject.runInEditMode = true;

            SetupCardHand(out CardHand hand, out CardGame game);
            hand.TryDrawCard(SpawnCardGraphic, game);
            var absorbingAttribute = new Attribute(0, uint.MaxValue);
            while (subject.Stacks.Value > 0)
                subject.Absorb(absorbingAttribute, hand);
            subject.Absorb(absorbingAttribute, hand);
            yield return null;

            Assert.False(subject.CanAbsorb);
        }

        private static void SetupCardHand(out CardHand projectile,
                                            out CardGame game)
        {
            var rb = TestUtils.CreateComponent<Rigidbody>();
            var actor = rb.gameObject.AddComponent<Actor.Actor>();
            actor.runInEditMode = true;
            projectile = rb.gameObject.AddComponent<CardHand>();
            projectile.runInEditMode = true;
            var gameConfig = new CardGameConfig()
            {
                HandEvaluator = "War"
            };
            game = CardGame.Load(gameConfig);

            projectile.Assign(new Deck(), new Dictionary<AttributeType, Attribute>());
        }

        private static GameObject SpawnCardGraphic()
        {
            var toSpawn = TestUtils.CreateComponent<TestUtils.TestCardGraphic>();
            return toSpawn.gameObject;
        }
    }
}
