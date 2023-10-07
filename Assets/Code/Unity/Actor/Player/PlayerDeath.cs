using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class PlayerDeath : MonoBehaviour
    {
        [SerializeField]
        private Actor.HitPoints hitPoints;
        [SerializeField]
        private string gameOverSceneName;

        private void Awake()
        {
            if (hitPoints == null) hitPoints = GetComponent<Actor.HitPoints>();
        }

        private void OnEnable()
        {
            hitPoints.Points.Decreased += OnHitPointsDecreased;
        }

        private void OnDisable()
        {
            hitPoints.Points.Decreased -= OnHitPointsDecreased;
        }

        private void OnHitPointsDecreased(Core.HitPoints sender, ValueChangeEvent<uint> e)
        {
            if (e.After != 0) return;

            GameManager.Instance.SceneLoader.LoadScene(gameOverSceneName);
        }
    }
}
