using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity
{
    public class PlayerDeath : MonoBehaviour
    {
        public string GameOverSceneName;

        [SerializeField]
        private HitPoints hitPoints;
        [SerializeField]
        private LocomotionSystem locomationSystem;

        private void Awake()
        {
            if (hitPoints == null) hitPoints = GetComponent<HitPoints>();
        }

        private void OnEnable()
        {
            hitPoints.Points.Decreased += OnHitPointsDecreased;
        }

        private void OnDisable()
        {
            hitPoints.Points.Decreased -= OnHitPointsDecreased;
        }

        private void OnHitPointsDecreased(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After != 0) return;

            locomationSystem.enabled = false;
            GameManager.Instance.SceneLoader.LoadScene(GameOverSceneName);
        }
    }
}
