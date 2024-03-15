using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Enemy : MonoBehaviour
    {
        [field: SerializeField]
        public Deck Deck { get; private set; }
        public Attribute ShieldHitPoints { get; set; }
        public IDictionary<AttributeType, Attribute> AttributeRegistry { get; private set; }

        [SerializeField]
        private HoloShieldController holoShieldController;

        private void Awake()
        {
            Deck = new Deck().Shuffle();
            holoShieldController ??= GetComponent<HoloShieldController>();
            ShieldHitPoints ??= holoShieldController?.HitPoints;
        }

        private void Start()
        {
            AttributeRegistry ??= GetComponent<Attributes>().Registry;
        }

        public void ResetAttributes()
        {
            AttributeRegistry[AttributeType.Health].Reset();
            ShieldHitPoints?.Reset(0);
            Deck = new Deck().Shuffle();
        }
    }
}
