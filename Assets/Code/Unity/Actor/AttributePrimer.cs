using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class InvalidAttributeError : Exception
    {
        public InvalidAttributeError() { }

        public InvalidAttributeError(string message) : base(message) { }
    }


    public class AttributePrimer : MonoBehaviour
    {
        [SerializeField]
        private TextAsset slingerConfigAsset;

        private void Awake() => LoadConfig();

        private void LoadConfig()
        {
            var configs = JsonConvert.DeserializeObject<List<Config>>(slingerConfigAsset.text);

            foreach (var config in configs)
            {
                string type = config.Type;
                uint value = config.Value;
                Points points = new(value);
                string[] actionTypes = { "dash", "peer" };

                if (actionTypes.Contains(type))
                    ParseAction(type, points, config);
                else if (type == "health")
                {
                    var hp = gameObject.AddComponent<HitPoints>();
                    hp.Points = points;
                }
                else throw new InvalidAttributeError($"Unknown attribute type {type}");
            }

        }

        private void ParseAction(string type, Points points, Config config)
        {
            ActionPerformer performer;
            if (type == "dash")
            {
                var dashable = gameObject.AddComponent<Dashable>();
                dashable.Speed = config.Speed;
                performer = dashable;
            }
            else if (type == "peer")
            {
                var peerable = gameObject.AddComponent<Peerable>();
                peerable.Interval = config.Interval;
                performer = peerable;
            }

            else throw new InvalidAttributeError($"Unknown action type {type}");

            ActionMetaData metaData = new()
            {
                Duration = config.Duration,
                Cooldown = config.Cooldown,
                RecoveryPeriod = config.RecoveryPeriod
            };
            performer.Initialize(points, metaData);
        }
    }

    class Config
    {
        public string Type { get; set; }
        public uint Value { get; set; }
        public float Cooldown { get; set; }
        public float Duration { get; set; }
        public float RecoveryPeriod { get; set; }
        public float Speed { get; set; }
        public float Interval { get; set; }
    }
}
