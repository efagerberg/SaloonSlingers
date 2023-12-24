using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class Attributes : MonoBehaviour
    {
        public Dictionary<AttributeType, Points> Registry { get; private set; } = new();
    }
}
