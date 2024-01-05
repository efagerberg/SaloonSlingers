using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class PlayerDeath : MonoBehaviour
    {
        public IReadOnlyPoints HitPoints { get; set; }

        public string GameOverSceneName;
        public Behaviour[] ComponentsToDisable;

        private void OnEnable()
        {
            if (HitPoints == null) return;

            HitPoints.Decreased += OnHitPointsDecreased;
        }

        private void OnDisable()
        {
            HitPoints.Decreased -= OnHitPointsDecreased;
        }

        private void Start()
        {
            HitPoints ??= GetComponent<Attributes>().Registry[AttributeType.Health];
            HitPoints.Decreased += OnHitPointsDecreased;
        }

        private void OnHitPointsDecreased(IReadOnlyPoints sender, ValueChangeEvent<uint> e)
        {
            if (e.After != 0) return;

            foreach (var component in ComponentsToDisable)
                component.enabled = false;
            GameManager.Instance.SceneLoader.LoadScene(GameOverSceneName);
        }
    }
}
