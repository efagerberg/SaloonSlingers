using NUnit.Framework;

using UnityEngine;

namespace SaloonSlingers.Unity.Tests
{
    public class PickupTests
    {
        [Test]
        public void DoesNothing_WhenGrabberMissingAttributes()
        {
            var subject = TestUtils.CreateComponent<Pickup>();
            subject.Value = 5;
            var pickupKilled = false;
            void KilledHandler(GameObject sender) => pickupKilled = true;
            subject.OnKilled.AddListener(KilledHandler);
            var grabber = new GameObject();
            subject.TryPickup(grabber);

            Assert.That(pickupKilled, Is.False);
        }

        [Test]
        public void DoesNothing_WhenGrabberMissingMoney()
        {
            var subject = TestUtils.CreateComponent<Pickup>();
            subject.Value = 5;
            var pickupKilled = false;
            void KilledHandler(GameObject sender) => pickupKilled = true;
            subject.OnKilled.AddListener(KilledHandler);
            var attributes = TestUtils.CreateComponent<Attributes>();
            subject.TryPickup(attributes.gameObject);

            Assert.That(pickupKilled, Is.False);
        }

        [Test]
        public void PicksUp_WhenGrabberHasMoney()
        {
            var subject = TestUtils.CreateComponent<Pickup>();
            subject.Value = 5;
            var pickupKilled = false;
            void KilledHandler(GameObject sender) => pickupKilled = true;
            subject.OnKilled.AddListener(KilledHandler);
            var attributes = TestUtils.CreateComponent<Attributes>();
            attributes.Registry[Core.AttributeType.Money] = new Core.Attribute(10, uint.MaxValue);
            subject.TryPickup(attributes.gameObject);

            Assert.That(pickupKilled);
            Assert.That(attributes.Registry[Core.AttributeType.Money].Value, Is.EqualTo(15));
        }

        [Test]
        public void ScalesPickup_BasedOnValue()
        {
            var subject = TestUtils.CreateComponent<Pickup>();
            subject.Value = 1000;

            Assert.That(subject.transform.localScale.magnitude, Is.GreaterThan(2));
        }

        [Test]
        public void ResetActor_RevertsStateToDefaultState()
        {
            var subject = TestUtils.CreateComponent<Pickup>();
            var rb = subject.gameObject.AddComponent<Rigidbody>();
            subject.Value = 1000;

            subject.ResetActor();

            Assert.That(subject.Value, Is.EqualTo(0));
            Assert.That(subject.transform.localScale, Is.EqualTo(Vector3.one));
            Assert.That(subject.transform.position, Is.EqualTo(Vector3.zero));
            Assert.That(subject.transform.rotation, Is.EqualTo(Quaternion.identity));
            Assert.That(rb.isKinematic, Is.False);
            Assert.That(rb.transform.position, Is.EqualTo(Vector3.zero));
            Assert.That(rb.transform.rotation, Is.EqualTo(Quaternion.identity));
        }
    }
}
