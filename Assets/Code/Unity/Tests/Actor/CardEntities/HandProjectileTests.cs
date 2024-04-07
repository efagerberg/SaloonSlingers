using System;
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
    public class ProjectileModeTests
    {
        [Test]
        public void Getter_DefaultsToDamageMode()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();

            Assert.That(subject.Mode, Is.EqualTo(HandProjectileMode.Damage));
        }

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(2)]
        public void Setter_DoesNotSetCursed_WhenHandDoesNotHave1Card(int nCards)
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            for (int i = 0; i < nCards; i++)
            {
                subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);
            }
            subject.Mode = HandProjectileMode.Curse;

            Assert.That(subject.Mode, Is.EqualTo(HandProjectileMode.Damage));
        }

        [Test]
        public void Setter_SetsCursed_WhenHandOnlyHas1Card()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);
            subject.Mode = HandProjectileMode.Curse;

            Assert.That(subject.Mode, Is.EqualTo(HandProjectileMode.Curse));
        }

        [Test]
        public void Setter_AlwaysChangingToDamageMode()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);
            subject.Mode = HandProjectileMode.Curse;
            subject.Mode = HandProjectileMode.Damage;

            Assert.That(subject.Mode, Is.EqualTo(HandProjectileMode.Damage));
        }

        [Test]
        public void Setter_Throws_WhenGivenInvalidMode()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);

            Assert.Throws<NotImplementedException>(() =>
            {
                subject.Mode = (HandProjectileMode)100;
            });
        }
    }

    public class ProjectileTryDrawTests
    {
        [Test]
        public void AddsCard_ToProjectile()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);

            Assert.That(subject.Cards.Count, Is.EqualTo(1));
        }

        [Test]
        public void ChangesModeBackToDamage_WhenDrawResultsInMoreThanOneCard()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);
            subject.Mode = HandProjectileMode.Curse;
            subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);

            Assert.That(subject.Mode, Is.EqualTo(HandProjectileMode.Damage));
        }

        [Test]
        public void EmitsDrawEvent()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            ICardGraphic cardDrawn = null;
            void drawHandler(GameObject sender, ICardGraphic c) => cardDrawn = c;
            subject.OnDraw.AddListener(drawHandler);
            subject.Pickup(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);

            Assert.IsNotNull(cardDrawn);
            Assert.That(cardDrawn.Card, Is.EqualTo(subject.Cards.ElementAt(0)));
        }

    }

    public class ProjectilePickupTests
    {
        [Test]
        public void WhenEmptyHand_EmitsPickupAndDrawEvents()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            var eventsConsumed = new List<string>();
            void pickupHandler(GameObject sender) => eventsConsumed.Add("pickup");
            void drawHandler(GameObject sender, ICardGraphic c) => eventsConsumed.Add("draw");
            subject.OnPickup.AddListener(pickupHandler);
            subject.OnDraw.AddListener(drawHandler);
            subject.Pickup(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);

            Assert.That(eventsConsumed, Is.EqualTo(new List<string> { "pickup", "draw" }));
        }

        [Test]
        public void WhenNotEmptyHand_EmitsOnlyPickupEvent()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);
            var eventsConsumed = new List<string>();
            void pickupHandler(GameObject sender) => eventsConsumed.Add("pickup");
            void drawHandler(GameObject sender, ICardGraphic c) => eventsConsumed.Add("draw");
            subject.OnPickup.AddListener(pickupHandler);
            subject.OnDraw.AddListener(drawHandler);
            subject.Pickup(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);

            Assert.That(eventsConsumed, Is.EqualTo(new List<string> { "pickup" }));
        }

        [Test]
        public void WhenNoCardsToDraw_EmitsOnlyPickupEvent()
        {
            var subject = ProjectileTestHelpers.BuildProjectile(0);
            subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);
            var eventsConsumed = new List<string>();
            void pickupHandler(GameObject sender) => eventsConsumed.Add("pickup");
            void drawHandler(GameObject sender, ICardGraphic c) => eventsConsumed.Add("draw");
            subject.OnPickup.AddListener(pickupHandler);
            subject.OnDraw.AddListener(drawHandler);
            subject.Pickup(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);

            Assert.That(eventsConsumed, Is.EqualTo(new List<string> { "pickup" }));
        }
    }

    public class ProjectileThrowTests
    {
        [Test]
        public void EmitsEvent()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            var thrown = false;
            void throwHandler(GameObject sender) => thrown = true;
            subject.OnThrow.AddListener(throwHandler);
            subject.Throw();

            Assert.That(thrown);
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator AddsOffsetForce_WhenOffsetSupplied()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            var rb = subject.GetComponent<Rigidbody>();
            yield return null;
            var offset = new Vector3(1, 2, 3);
            subject.Throw(offset);
            yield return new WaitForFixedUpdate();

            Assert.That(rb.velocity, Is.EqualTo(offset));
        }
    }

    public class ProjectileKillTests
    {
        [Test]
        public void EmitsEvent()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            var killed = false;
            void killedHandler(GameObject sender) => killed = true;
            subject.OnKilled.AddListener(killedHandler);
            subject.Kill();

            Assert.That(killed);
        }
    }

    public class ProjectilePauseTests
    {
        [Test]
        public void EmitsEvent()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            var paused = false;
            void pausedHandler(GameObject sender) => paused = true;
            subject.OnPause.AddListener(pausedHandler);
            subject.Pause();

            Assert.That(paused);
        }
    }

    public class ProjectileHandEvaluationTests
    {
        [Test]
        public void GetsCurrentEvaluation()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);

            Assert.That(subject.HandEvaluation.Name, Is.EqualTo(HandNames.HIGH_CARD));
        }
    }

    public class ProjectileResetTests
    {
        [UnityTest]
        public IEnumerator ResetsState()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            subject.runInEditMode = true;
            yield return null;
            subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);
            var rb = subject.GetComponent<Rigidbody>();
            subject.ResetActor();

            Assert.That(subject.HandEvaluation.Name, Is.EqualTo(HandNames.NONE));
            Assert.That(rb.gameObject.layer, Is.EqualTo(LayerMask.NameToLayer("UnassignedProjectile")));
            Assert.That(subject.Mode, Is.EqualTo(HandProjectileMode.Damage));
        }

        [UnityTest]
        public IEnumerator Emits()
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            subject.runInEditMode = true;
            yield return null;
            subject.TryDrawCard(ProjectileTestHelpers.TestCardSpawner, ProjectileTestHelpers.TestPokerGame);
            var reset = false;
            void resetListener(GameObject arg0)
            {
                reset = true;
            }
            subject.OnReset.AddListener(resetListener);
            subject.ResetActor();

            Assert.That(reset);
        }
    }

    public class ProjectileHandleCollisionTests
    {
        [UnityTest]
        public IEnumerator OnlyDies_WhenCollisionShouldBeLethal(
            [ValueSource(nameof(TestCases))]
            CollisionTestCase testCase
        )
        {
            var subject = ProjectileTestHelpers.BuildProjectile();
            subject.runInEditMode = true;
            yield return null;
            var rb = subject.GetComponent<Rigidbody>();
            rb.isKinematic = testCase.isKinematic;
            var collidingObject = new GameObject("CollidingObject")
            {
                layer = LayerMask.NameToLayer(testCase.layer)
            };
            var killed = false;
            void killedHandler(GameObject sender) => killed = true;
            subject.OnKilled.AddListener(killedHandler);
            subject.HandleCollision(collidingObject);

            Assert.That(killed, Is.EqualTo(testCase.expected));
        }

        private static IEnumerable TestCases()
        {
            yield return new CollisionTestCase { isKinematic = false, layer = "Environment", expected = false };
            yield return new CollisionTestCase { isKinematic = false, layer = "Hand", expected = false };
            yield return new CollisionTestCase { isKinematic = true, layer = "Default", expected = false };
            yield return new CollisionTestCase { isKinematic = false, layer = "Default", expected = true };
        }

        public struct CollisionTestCase
        {
            public bool isKinematic;
            public string layer;
            public bool expected;
        }
    }


    public static class ProjectileTestHelpers
    {
        public static GameObject TestCardSpawner() => TestUtils.CreateComponent<TestCardGraphic>().gameObject;
        public static CardGame TestPokerGame = CardGame.Load(new CardGameConfig() { HandEvaluator = "poker" });
        public static HandProjectile BuildProjectile(int nCards = 10)
        {
            var rb = TestUtils.CreateComponent<Rigidbody>();
            rb.useGravity = false;
            var subject = rb.gameObject.AddComponent<HandProjectile>();
            subject.Assign(new Deck(nCards), new Dictionary<AttributeType, Core.Attribute> { });
            return subject;
        }
    }
}
