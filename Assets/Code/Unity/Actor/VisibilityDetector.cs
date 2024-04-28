using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class VisibilityDetector : MonoBehaviour
    {
        public float SightRadius => sightRadius;
        public float SightDistance => sightDistance;

        [SerializeField]
        private float sightRadius = 0.25f;
        [SerializeField]
        private float sightDistance = 15f;
        [SerializeField]
        private int maxSeeable = 5;
        [SerializeField]
        private Transform sightTransform;
        [SerializeField]
        private LayerMask layerMask;
        [SerializeField]
        private bool drawGizmo = false;
        [SerializeField]
        private float distanceWeight = 0.2f;


        private RaycastHit[] spherecastHits;

        public IEnumerable<Transform> GetVisible(LayerMask mask = default, bool xRay = false)
        {
            if (mask == default) mask = layerMask;

            int nSpherecastHits = Physics.SphereCastNonAlloc(sightTransform.position,
                                                             sightRadius,
                                                             sightTransform.forward,
                                                             spherecastHits,
                                                             sightDistance,
                                                             mask);
            float maxDistance = spherecastHits.Max(hit => hit.distance);

            var seen = spherecastHits.Take(nSpherecastHits)
                                     .Where(hit => xRay || !IsObstructed(hit.point, sightTransform))
                                     .OrderByDescending(hit => GetVisibilityScore(hit, maxDistance))
                                     .Select(hit => hit.transform);
            return seen.Take(maxSeeable).Distinct();
        }

        private float GetVisibilityScore(RaycastHit hit, float maxDistance)
        {
            return sightTransform.GetAlignment(hit.point) - (distanceWeight * (hit.distance / maxDistance));
        }

        private static bool IsObstructed(Vector3 point, Transform sightTransform)
        {
            return Physics.Linecast(sightTransform.position,
                                    point,
                                    LayerMask.GetMask("Environment"));
        }

        private void Awake()
        {
            spherecastHits = new RaycastHit[maxSeeable];
        }

        private void OnDrawGizmos()
        {
            if (sightTransform == null || !drawGizmo) return;
            SpherecastVisualizer.DrawSphereCastAll(sightTransform,
                                                   sightRadius,
                                                   sightDistance,
                                                   layerMask,
                                                   maxSeeable);
        }
    }
}
