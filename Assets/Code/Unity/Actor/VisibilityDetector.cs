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
        private float perceptionRadius = 2f;
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

        private RaycastHit[] spherecastHits;
        private Collider[] perceptionHits;

        public IEnumerable<Transform> GetVisible(LayerMask mask = default, bool xRay = false)
        {
            if (mask == default) mask = layerMask;

            int nSpherecastHits = Physics.SphereCastNonAlloc(sightTransform.position,
                                                             sightRadius,
                                                             sightTransform.forward,
                                                             spherecastHits,
                                                             sightDistance,
                                                             mask);

            var seen = spherecastHits.Take(nSpherecastHits)
                                     .OrderByDescending(hit => sightTransform.GetAlignment(hit.point))
                                     .Where(hit => xRay || !IsObstructed(hit.point, sightTransform))
                                     .Select(hit => hit.transform);
            if (perceptionRadius > 0)
            {
                // Spherecasts don't really work when the distance is too close compared to the radius so also use overlap
                // sphere close to the entity to model their perception.
                int nPerceptionHits = Physics.OverlapSphereNonAlloc(sightTransform.position,
                                                                    perceptionRadius,
                                                                    perceptionHits,
                                                                    mask);
                if (perceptionHits == default) perceptionHits = new Collider[maxSeeable];
                var percepted = perceptionHits.Take(nPerceptionHits).Where(hit => xRay || !IsObstructed(hit.bounds.max, sightTransform)).Select(x => x.transform);
                seen = percepted.Concat(seen);
            }
            return seen.Take(maxSeeable).Distinct();

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
            if (perceptionRadius > 0)
                Gizmos.DrawWireSphere(sightTransform.position, perceptionRadius);
            SpherecastVisualizer.DrawSphereCastAll(sightTransform,
                                                   sightRadius,
                                                   sightDistance,
                                                   layerMask,
                                                   maxSeeable);
        }
    }
}
