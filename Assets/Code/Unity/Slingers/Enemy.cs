using UnityEngine;
using Unity.XR.CoreUtils;

using SaloonSlingers.Core.SlingerAttributes;

using SaloonSlingers.Unity.CardEntities;
using UnityEngine.AI;

namespace SaloonSlingers.Unity.Slingers
{
    public class Enemy : CardGraphic, ISlinger
    {
        public ISlingerAttributes Attributes { get; set; }

        [SerializeField]
        private float lookRadius = 10f;
        [SerializeField]
        private float stoppingDistance = 0.1f;

        private Transform target;
        private NavMeshAgent agent;

        private void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<XROrigin>().Camera.transform;
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = stoppingDistance;
        }

        private void Update()
        {
            FaceTarget();
            float distance = Vector3.Distance(target.position, transform.position);
            if (distance <= lookRadius) agent.SetDestination(target.position);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, lookRadius);
        }

        private void FaceTarget()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotations = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotations, Time.deltaTime * 5);
        }
    }
}
