using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class DieOnHealthDepleted : MonoBehaviour
    {
        [SerializeField]
        private Actor actor;
        private IReadOnlyDictionary<AttributeType, Core.Attribute> attributes;

        private void Awake()
        {
            if (actor == null) actor = GetComponent<Actor>();
        }

        private void OnEnable()
        {
            if (attributes != null)
                attributes[AttributeType.Health].Depleted += OnHealthDepleted;
            actor.Killed.AddListener(OnKilled);
        }

        private void OnDisable()
        {
            if (attributes != null)
                attributes[AttributeType.Health].Depleted -= OnHealthDepleted;
            actor.Killed.RemoveListener(OnKilled);
        }

        private void Start()
        {
            // Attributes are set up dynamically after object initialization
            attributes ??= GetComponent<Attributes>().Registry;
            attributes[AttributeType.Health].Depleted += OnHealthDepleted;
        }

        private void OnKilled(Actor sender)
        {
            foreach (var attribute in attributes.Values)
                attribute.Reset();
        }

        private void OnHealthDepleted(IReadOnlyAttribute sender, EventArgs e)
        {
            actor.Kill(delay: true);
        }
    }
}
