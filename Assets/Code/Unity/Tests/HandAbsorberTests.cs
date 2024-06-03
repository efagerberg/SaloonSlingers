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

            SetupProjectile(out HandProjectile projectile, out CardGame game);
            projectile.TryDrawCard(SpawnCardGraphic, game);
            projectile.runInEditMode = true;
            var absorbingAttribute = new Attribute(0, uint.MaxValue);
            subject.Absorb(absorbingAttribute, projectile);
            yield return null;

            Assert.That(absorbingAttribute.Value, Is.EqualTo(projectile.HandEvaluation.Score));
            Assert.That(subject.Stacks.Value, Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator Absorb_CannotAbsorb_WhenNoStacksLeft()
        {
            var subject = TestUtils.CreateComponent<HandAbsorber>();
            subject.runInEditMode = true;

            SetupProjectile(out HandProjectile projectile, out CardGame game);
            projectile.TryDrawCard(SpawnCardGraphic, game);
            var absorbingAttribute = new Attribute(0, uint.MaxValue);
            while (subject.Stacks.Value > 0)
                subject.Absorb(absorbingAttribute, projectile);
            subject.Absorb(absorbingAttribute, projectile);
            yield return null;

            Assert.False(subject.CanAbsorb);
        }

        private static void SetupProjectile(out HandProjectile projectile,
                                            out CardGame game)
        {
            var rb = TestUtils.CreateComponent<Rigidbody>();
            var actor = rb.gameObject.AddComponent<Actor.Actor>();
            actor.runInEditMode = true;
            projectile = rb.gameObject.AddComponent<HandProjectile>();
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
