using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class PickupOnCollision : MonoBehaviour
    {
        private Pickup pickup;

        private void Start()
        {
            pickup = GetComponent<Pickup>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<Attributes>(out var attributes) &&
                attributes.Registry.TryGetValue(AttributeType.Money, out var money))
            {
                money.Increase(pickup.Value);
                pickup.Kill();
            }
        }
    }
}
