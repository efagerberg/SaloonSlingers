using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class DropPickup : MonoBehaviour
    {
        public Transform DropPositionReference;

        private Attributes attributes;

        public void Drop()
        {
            if (!attributes.Registry.TryGetValue(Core.AttributeType.Pot, out var pot) || pot.Value == 0)
                return;

            PickupDropper.Drop(pot,
                               LevelManager.Instance.PickupSpawner,
                               gameObject.layer,
                               DropPositionReference.position);
        }

        private void Start()
        {
            if (DropPositionReference == null) DropPositionReference = transform;
            attributes = GetComponent<Attributes>();
        }
    }
}
