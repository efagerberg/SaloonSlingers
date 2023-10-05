using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class PlayerDeath : MonoBehaviour
    {
        [SerializeField]
        private HitPoints hitPoints;
        [SerializeField]
        private string gameOverSceneName;

        private void Awake()
        {
            if (hitPoints == null) hitPoints = GetComponent<HitPoints>();
        }

        private void OnEnable()
        {
            hitPoints.Points.PointsDecreased += OnHitPointsDecreased;
        }

        private void OnDisable()
        {
            hitPoints.Points.PointsDecreased -= OnHitPointsDecreased;
        }

        private void OnHitPointsDecreased(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After != 0) return;

            GameManager.Instance.SceneLoader.LoadScene(gameOverSceneName);
        }
    }
}
