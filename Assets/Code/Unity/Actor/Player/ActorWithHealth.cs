using System;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity.Actor
{
    public class ActorWithHealth : Actor
    {
        private Attributes attributes;

        public override void ResetActor()
        {
            foreach (var attribute in attributes.Registry.Values)
                attribute.Reset();
            base.ResetActor();
        }

        private void OnEnable()
        {
            if (attributes == null) return;

            attributes.Registry[AttributeType.Health].Depleted += OnHealthDepleted;
        }

        private void OnDisable()
        {
            if (attributes == null) return;

            attributes.Registry[AttributeType.Health].Depleted -= OnHealthDepleted;
        }

        private void Start()
        {
            attributes ??= GetComponent<Attributes>();
            attributes.Registry[AttributeType.Health].Depleted += OnHealthDepleted;
        }

        private void OnHealthDepleted(IReadOnlyAttribute sender, EventArgs e)
        {
            StartCoroutine(nameof(DelayDeath));
        }
    }
}
