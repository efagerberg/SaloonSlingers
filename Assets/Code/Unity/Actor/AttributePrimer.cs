using System;
using System.Collections.Generic;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    using RawTypeToTypeMeta = IReadOnlyDictionary<string, (bool isActive, int enumVal)>;

    public static class AttributePrimer
    {
        public static void Prime(IEnumerable<AttributeConfig> configs, GameObject root)
        {
            foreach (var config in configs)
            {
                string rawType = config.Type;
                var found = RawTypeToTypeMeta.TryGetValue(rawType, out var typeMetaData);
                if (!found) throw new InvalidAttributeError($"Unknown attribute type {rawType}");

                uint value = config.Value;
                Points points = new(value);
                if (typeMetaData.isActive)
                {
                    ActiveAttributeType type = (ActiveAttributeType)typeMetaData.enumVal;
                    var performer = ParseActionPerformer(type, config, root);
                    ActionMetaData metaData = new()
                    {
                        Duration = config.Duration,
                        Cooldown = config.Cooldown,
                        RecoveryPeriod = config.RecoveryPeriod
                    };
                    performer.Initialize(points, metaData);
                }
                else
                {
                    PassiveAttributeType type = (PassiveAttributeType)typeMetaData.enumVal;
                    if (type == PassiveAttributeType.Health)
                    {
                        var hp = root.AddComponent<HitPoints>();
                        hp.Points = points;
                    }
                }
            }
        }

        private static ActionPerformer ParseActionPerformer(ActiveAttributeType type, AttributeConfig config, GameObject root)
        {
            ActionPerformer performer = null;
            if (type == ActiveAttributeType.Dash)
            {
                var dashable = root.AddComponent<Dashable>();
                dashable.Speed = config.Speed;
                performer = dashable;
            }
            else if (type == ActiveAttributeType.Peer)
            {
                var peerable = root.AddComponent<Peerable>();
                peerable.Interval = config.Interval;
                performer = peerable;
            }
            return performer;
        }

        private static readonly RawTypeToTypeMeta RawTypeToTypeMeta = new Dictionary<string, (bool isActive, int enumValue)>() {
            { "health", (false, (int)PassiveAttributeType.Health) },
            { "dash", (true, (int)ActiveAttributeType.Dash) },
            { "peer", (true, (int)ActiveAttributeType.Peer) },
        };
    }

    public enum PassiveAttributeType
    {
        Health,
    };

    public enum ActiveAttributeType
    {
        Peer,
        Dash
    }

    public class AttributeConfig
    {
        public string Type { get; set; }
        public uint Value { get; set; }
        public float Cooldown { get; set; }
        public float Duration { get; set; }
        public float RecoveryPeriod { get; set; }
        public float Speed { get; set; }
        public float Interval { get; set; }
    }

    public class InvalidAttributeError : Exception
    {
        public InvalidAttributeError() { }

        public InvalidAttributeError(string message) : base(message) { }
    }
}