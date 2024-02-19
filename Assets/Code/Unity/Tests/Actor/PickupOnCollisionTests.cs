using System;
using System.Collections;

using NUnit.Framework;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class PickupOnCollisionTests
    {
        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator DoesNothing_WhenCollidingObjectMissingAttributes()
        {
            var subject = TestUtils.CreateComponent<PickupOnCollision>();
            subject.gameObject.AddComponent<Rigidbody>();
            subject.gameObject.AddComponent<SphereCollider>();
            var pickup = subject.gameObject.AddComponent<Pickup>();
            pickup.Value = 5;
            var pickupKilled = false;
            void OnKilled(object sender, EventArgs e)
            {
                GameObject.Destroy(pickup.gameObject);
                pickupKilled = true;
            }
            pickup.Killed += OnKilled;
            var collidingObject = TestUtils.CreateComponent<SphereCollider>();
            yield return new WaitForFixedUpdate();

            Assert.That(pickupKilled, Is.False);
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator DoesNothing_WhenCollidingObjectMissingMoney()
        {
            var subject = TestUtils.CreateComponent<PickupOnCollision>();
            subject.gameObject.AddComponent<Rigidbody>();
            subject.gameObject.AddComponent<SphereCollider>();
            var pickup = subject.gameObject.AddComponent<Pickup>();
            pickup.Value = 5;
            var pickupKilled = false;
            void OnKilled(object sender, EventArgs e)
            {
                GameObject.Destroy(pickup.gameObject);
                pickupKilled = true;
            }
            pickup.Killed += OnKilled;
            var collidingObject = TestUtils.CreateComponent<SphereCollider>();
            var attributes = collidingObject.gameObject.AddComponent<Attributes>();
            yield return new WaitForFixedUpdate();

            Assert.That(pickupKilled, Is.False);
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator PicksUp_WhenCollidingObjectHasMoney()
        {
            var subject = TestUtils.CreateComponent<PickupOnCollision>();
            subject.gameObject.AddComponent<Rigidbody>();
            subject.gameObject.AddComponent<SphereCollider>();
            var pickup = subject.gameObject.AddComponent<Pickup>();
            pickup.Value = 5;
            var pickupKilled = false;
            void OnKilled(object sender, EventArgs e)
            {
                GameObject.Destroy(pickup.gameObject);
                pickupKilled = true;
            }
            pickup.Killed += OnKilled;
            var collidingObject = TestUtils.CreateComponent<SphereCollider>();
            var attributes = collidingObject.gameObject.AddComponent<Attributes>();
            attributes.Registry[AttributeType.Money] = new Core.Attribute(10, uint.MaxValue);
            yield return new WaitForFixedUpdate();

            Assert.That(pickupKilled);
            Assert.That(attributes.Registry[AttributeType.Money].Value, Is.GreaterThan(15));
        }
    }
}
