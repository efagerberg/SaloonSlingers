using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class PickupElevatorPlacer : MonoBehaviour
    {
        [SerializeField]
        private GameObject interactable;

        private PickupElevator placed;

        public void Place()
        {
            if (placed != null) return;

            var spawned = LevelManager.Instance.PickupElevatorSpawner.Spawn();
            placed = spawned.GetComponent<PickupElevator>();
            placed.Associate(interactable);
        }

        public void Return()
        {
            if (placed == null) return;

            placed.Kill();
            placed = null;
        }
    }
}
