using System.Collections.Generic;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;
using SaloonSlingers.Unity.Tests;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class HandAbsorberTests
    {
        [Test]
        public void Absorb_AbsorbsHandEvaluationWithOneLessStack_WhenStackaAvailable()
        {
            var subject = TestUtils.CreateComponent<HandAbsorber>();
            subject.runInEditMode = true;

            SetupProjectile(out HandProjectile projectile, out CardGame game);
            projectile.TryDrawCard(SpawnCardGraphic, game);
            var absorbingAttribute = new Attribute(0, uint.MaxValue);
            subject.Absorb(absorbingAttribute, projectile);

            Assert.That(absorbingAttribute.Value, Is.EqualTo(projectile.HandEvaluation.Score));
            Assert.That(subject.Stacks.Value, Is.EqualTo(2));
        }

        [Test]
        public void Absorb_CannotAbsorb_WhenNoStacksLeft()
        {
            var subject = TestUtils.CreateComponent<HandAbsorber>();
            subject.runInEditMode = true;

            SetupProjectile(out HandProjectile projectile, out CardGame game);
            projectile.TryDrawCard(SpawnCardGraphic, game);
            var absorbingAttribute = new Attribute(0, uint.MaxValue);
            while (subject.Stacks.Value > 0)
                subject.Absorb(absorbingAttribute, projectile);
            subject.Absorb(absorbingAttribute, projectile);

            Assert.False(subject.CanAbsorb);
        }

        private static void SetupProjectile(out HandProjectile projectile, out CardGame game)
        {
            projectile = TestUtils.CreateComponent<HandProjectile>();
            var gameConfig = new CardGameConfig()
            {
                HandEvaluator = "War"
            };
            game = CardGame.Load(gameConfig);

            projectile.Assign(new Deck(), new Dictionary<AttributeType, Attribute>());
        }

        private static GameObject SpawnCardGraphic()
        {
            var toSpawn = TestUtils.CreateComponent<TestCardGraphic>();
            return toSpawn.gameObject;
        }
    }

    class TestCardGraphic : MonoBehaviour, ICardGraphic
    {
        public Card Card { get; set; }
        public void Kill() { }
        public Color Color { get; set; }
    }
}
