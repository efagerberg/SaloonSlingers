using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class PlayerDeath : MonoBehaviour
    {
        public string GameOverSceneName;
        public Behaviour[] ComponentsToDisable;

        [SerializeField]
        private HitPoints hitPoints;

        private void OnEnable()
        {
            if (hitPoints == null) return;

            hitPoints.Points.Decreased += OnHitPointsDecreased;
        }

        private void OnDisable()
        {
            hitPoints.Points.Decreased -= OnHitPointsDecreased;
        }

        private void Start()
        {
            if (hitPoints == null) hitPoints = GetComponent<HitPoints>();

            hitPoints.Points.Decreased += OnHitPointsDecreased;
        }

        private void OnHitPointsDecreased(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After != 0) return;

            foreach (var component in ComponentsToDisable)
                component.enabled = false;
            GameManager.Instance.SceneLoader.LoadScene(GameOverSceneName);
        }
    }
}
