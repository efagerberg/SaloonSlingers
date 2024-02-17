using System;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class PlayerDeath : MonoBehaviour, IActor
    {
        public IReadOnlyAttribute HitPoints { get; set; }

        public string GameOverSceneName;
        public Behaviour[] ComponentsToDisable;

        public event EventHandler Death;

        private void OnEnable()
        {
            if (HitPoints == null) return;

            HitPoints.Depleted += OnHealthDepleted;
        }

        private void OnDisable()
        {
            HitPoints.Depleted -= OnHealthDepleted;
        }

        private void Start()
        {
            HitPoints ??= GetComponent<Attributes>().Registry[AttributeType.Health];
            HitPoints.Depleted += OnHealthDepleted;
        }

        private void OnHealthDepleted(IReadOnlyAttribute sender, EventArgs e)
        {
            foreach (var component in ComponentsToDisable)
                component.enabled = false;
            Death?.Invoke(this, EventArgs.Empty);
            GameManager.Instance.SceneLoader.LoadScene(GameOverSceneName);
        }
    }
}
