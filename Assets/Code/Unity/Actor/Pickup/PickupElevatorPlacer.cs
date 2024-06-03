using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class PickupElevatorPlacer : MonoBehaviour
    {
        [SerializeField]
        private GameObject interactable;

        private PickupElevator placed;
        private Actor placedActor;

        public void Place()
        {
            if (placed != null) return;

            var spawned = LevelManager.Instance.PickupElevatorSpawner.Spawn();
            placed = spawned.GetComponent<PickupElevator>();
            placedActor = spawned.GetComponent<Actor>();
            placed.Associate(interactable);
        }

        public void Return()
        {
            if (placed == null) return;

            placedActor.Kill();
            placed = null;
            placedActor = null;
        }
    }
}
