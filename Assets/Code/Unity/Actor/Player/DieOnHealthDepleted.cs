using System;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class DieOnHealthDepleted : MonoBehaviour
    {
        [SerializeField]
        private Actor actor;
        private Attributes attributes;

        private void OnReset(Actor sender)
        {
            foreach (var attribute in attributes.Registry.Values)
                attribute.Reset();
        }

        private void Awake()
        {
            actor = GetComponent<Actor>();
        }

        private void OnEnable()
        {
            if (attributes != null)
                attributes.Registry[AttributeType.Health].Depleted += OnHealthDepleted;
            if (actor != null)
                actor.Reset.AddListener(OnReset);
        }

        private void OnDisable()
        {
            if (attributes != null)
                attributes.Registry[AttributeType.Health].Depleted -= OnHealthDepleted;
            if (actor != null)
                actor.Reset.RemoveListener(OnReset);
        }

        private void Start()
        {
            // Attributes are set up dynamically after object initialization
            if (attributes == null)
                attributes = GetComponent<Attributes>();
            attributes.Registry[AttributeType.Health].Depleted += OnHealthDepleted;
        }

        private void OnHealthDepleted(IReadOnlyAttribute sender, EventArgs e)
        {
            actor.Kill(delay: true);
        }
    }
}
