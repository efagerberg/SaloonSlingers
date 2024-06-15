using UnityEngine;

namespace SaloonSlingers.Unity
{
    public static class ProjectileMotion
    {
        // Source: https://en.wikipedia.org/wiki/Projectile_motion#Angle_%CE%B8_required_to_hit_coordinate_(x,_y)
        public static float CalculateLaunchAngle(Vector3 source, Vector3 target, float speed, bool low = true)
        {
            Vector3 targetDirection = (target - source);
            float y = targetDirection.y;
            float x = new Vector2(targetDirection.x, targetDirection.z).magnitude;
            float gravity = Physics.gravity.magnitude;

            float speedSquared = Mathf.Pow(speed, 2);
            float underSqrt = Mathf.Pow(speed, 4) - gravity * (gravity * Mathf.Pow(x, 2) + (2 * y * speedSquared));

            float root = Mathf.Sqrt(underSqrt);
            if (low) return Mathf.Atan2(speedSquared - root, gravity * x) * Mathf.Rad2Deg;
            else return Mathf.Atan2(speedSquared + root, gravity * x) * Mathf.Rad2Deg;
        }
    }
}
