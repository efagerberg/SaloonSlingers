using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SaloonSlingers.Unity
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
        private uint nHits = 5;
        [SerializeField]
        private Transform sightTransform;
        [SerializeField]
        private LayerMask layerMask;
        [SerializeField]
        private bool drawGizmo = false;

        private RaycastHit[] hits;

        public IEnumerable<RaycastHit> GetVisible(LayerMask mask = default, bool xRay = false)
        {
            if (mask == default) mask = layerMask;
            int nHitsFound = Physics.SphereCastNonAlloc(sightTransform.position,
                                                        sightRadius,
                                                        sightTransform.forward,
                                                        hits,
                                                        sightDistance,
                                                        mask);

            return hits.Take(nHitsFound)
                       .OrderByDescending(hit => sightTransform.GetAlignment(hit.point))
                       .Where(hit => xRay || !IsObstructed(hit, sightTransform));
        }

        private bool IsObstructed(RaycastHit hit, Transform gazeTransform)
        {
            return Physics.Linecast(gazeTransform.position, hit.point, LayerMask.GetMask("Environment"));
        }

        private void Awake()
        {
            hits = new RaycastHit[nHits];
        }

        private void OnDrawGizmos()
        {
            if (sightTransform == null || !drawGizmo) return;
            SpherecastVisualizer.DrawSphereCastAll(sightTransform,
                                                   sightRadius,
                                                   sightDistance,
                                                   layerMask,
                                                   nHits);
        }
    }
}
