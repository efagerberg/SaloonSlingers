using System.Linq;

using UnityEngine;

public class SpherecastVisualizer : MonoBehaviour
{
    [SerializeField]
    private float maxDistance = 10f;
    [SerializeField]
    private float radius = 0.25f;
    [SerializeField]
    private LayerMask mask;

    private void OnDrawGizmosSelected()
    {
        DrawSpherecast(transform, radius, maxDistance, mask);
    }

    public static void DrawSpherecast(Transform transform, float radius, float maxDistance, LayerMask mask)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
        bool isHit = Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, maxDistance, mask);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        if (isHit)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hit.point, radius);
        }
    }

    public static void DrawSphereCastAll(Transform transform, float radius, float maxDistance, LayerMask mask)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
        var hits = new RaycastHit[5];
        int nHits = Physics.SphereCastNonAlloc(transform.position, radius, transform.forward, hits, maxDistance, mask);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        foreach (var hit in hits.Take(nHits))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.point, radius);
        }
    }
}
