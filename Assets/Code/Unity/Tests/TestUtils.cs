using UnityEngine;

namespace SaloonSlingers.Unity.Tests
{
    public static class TestUtils
    {
        public static T CreateComponent<T>(string name = null) where T : Component
        {
            GameObject go = new();
            T comp = go.AddComponent<T>();
            if (name != null) comp.name = name;
            return comp;
        }
    }
}
