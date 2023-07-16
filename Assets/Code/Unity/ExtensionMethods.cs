using UnityEngine;

namespace SaloonSlingers.Unity
{
    public static class GameObjectExtensions
    {
        // Useful when you have multiple levels of children that could have the component, but you
        // only want the first level candidates.
        public static T GetComponentInImmediateChildren<T>(this GameObject go) where T : Component
        {
            foreach (Transform child in go.transform)
            {
                if (child.TryGetComponent(out T x)) return x;
            }
            return null;
        }
    }
}
