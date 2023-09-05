using System;

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

    public static class TransformExtensions
    {
        // Calculates how aligned two transforms are.
        // Returns a value between 0 and 1 where values close to 0 are closely aligned, and values closed to one are not.
        public static float GetAlignment(this Transform self, Transform other)
        {
            var direction = (other.position - self.position).normalized;
            return Math.Abs(Vector3.Dot(self.forward, direction));
        }

        public static float GetAlignment(this Transform self, Vector3 other)
        {
            var direction = (other - self.position).normalized;
            return Math.Abs(Vector3.Dot(self.forward, direction));
        }
    }
}
