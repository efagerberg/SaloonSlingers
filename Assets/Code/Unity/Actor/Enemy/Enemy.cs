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
        }

        private void Start()
        {
            AttributeRegistry ??= GetComponent<Attributes>().Registry;
            holoShieldController ??= GetComponent<HoloShieldController>();
            ShieldHitPoints ??= holoShieldController?.HitPoints;
        }

        public void ResetEnemy()
        {
            ShieldHitPoints?.Reset(0);
            Deck = new Deck().Shuffle();
        }
    }
}
