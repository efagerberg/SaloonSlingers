using System;
using System.Collections.Generic;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    using RawTypeToTypeMeta = IReadOnlyDictionary<string, (bool forAction, AttributeType type)>;

    public static class AttributePrimer
    {
        public static void Prime(IEnumerable<AttributeConfig> configs, GameObject root)
        {
            var attributes = root.AddComponent<Attributes>();
            foreach (var config in configs)
            {
                string rawType = config.Type;
                var found = PointMetaDataLookup.TryGetValue(rawType, out var typeMetaData);
                if (!found) throw new InvalidAttributeError($"Unknown point type {rawType}");

                uint value = config.Value;
                Points points = new(value, uint.MaxValue);
                attributes.Registry.Add(typeMetaData.type, points);

                if (typeMetaData.forAction)
                {
                    AttributeType type = typeMetaData.type;
                    var performer = ParseActionPerformer(type, config, root);
                    ActionMetaData metaData = new()
                    {
                        Duration = config.Duration,
                        Cooldown = config.Cooldown,
                        RecoveryPeriod = config.RecoveryPeriod
                    };
                    performer.Initialize(points, metaData);
                }
            }
        }

        private static ActionPerformer ParseActionPerformer(AttributeType type, AttributeConfig config, GameObject root)
        {
            ActionPerformer performer = null;
            if (type == AttributeType.Dash)
            {
                var dashable = root.AddComponent<Dashable>();
                dashable.Speed = config.Speed;
                performer = dashable;
            }
            else if (type == AttributeType.Peer)
            {
                var peerable = root.AddComponent<Peerable>();
                peerable.Interval = config.Interval;
                performer = peerable;
            }
            return performer;
        }

        private static readonly RawTypeToTypeMeta PointMetaDataLookup = new Dictionary<string, (bool forAction, AttributeType type)>() {
            { "health", (false, AttributeType.Health) },
            { "money", (false, AttributeType.Money) },
            { "dash", (true, AttributeType.Dash) },
            { "peer", (true, AttributeType.Peer) },
        };
    }

    public enum AttributeType
    {
        Health,
        Money,
        Peer,
        Dash
    }

    public struct AttributeConfig
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