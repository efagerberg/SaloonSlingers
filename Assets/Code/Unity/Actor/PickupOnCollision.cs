using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class PickupOnCollision : MonoBehaviour
    {
        [SerializeField]
        private LayerMask ignoreMask;

        private Pickup pickup;

        private void Start()
        {
            pickup = GetComponent<Pickup>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((ignoreMask & (1 << collision.gameObject.layer)) != 0)
                return;

            if (collision.gameObject.TryGetComponent<Attributes>(out var attributes) &&
                attributes.Registry.TryGetValue(AttributeType.Money, out var money))
            {
                money.Increase(pickup.Value);
                pickup.Kill();
            }
        }
    }
}
