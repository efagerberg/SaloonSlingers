using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.CardEntities;
using SaloonSlingers.Unity.Slingers;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.InputSystem;


namespace SaloonSlingers.Unity
{
    public class Peerable : ActionPerformer
    {
        public ActionPoints Points { get; private set; }

        [SerializeField]
        private List<InputActionProperty> peerActionProperties;
        [SerializeField]
        private float peerRadius = 10;
        [SerializeField]
        private float peerDistance = 5;
        [SerializeField]
        private uint startingPeers = 3;
        [SerializeField]
        private float startingCooldown = 3;
        [SerializeField]
        private float startingDuration = 5f;
        [SerializeField]
        private float startingRecoveryPeriod = 1f;
        [SerializeField]
        private Transform cameraTransform;
        [SerializeField]
        private SlingerHandedness handedness;
        [SerializeField]
        private float peerInterval = 0.2f;
        [SerializeField]
        private const int maxPeerHits = 10;
        [SerializeField]
        private Collider[] peerHits = new Collider[maxPeerHits];

        public void CastPeer()
        {
            IEnumerator coroutine = GetActionCoroutine(Points, DoPeer);
            if (coroutine == null) return;

            StartCoroutine(coroutine);
        }

        private IEnumerator DoPeer()
        {
            Enemy lastEnemy = null;
            Outline lastOutline = null;
            Enemy currentEnemy = null;
            Outline currentOutline = null;
            float currentDuration = Points.Duration;
            var intervalWait = new WaitForSeconds(peerInterval);

            while (currentDuration > 0)
            {
                int nHits = Physics.OverlapSphereNonAlloc(PeerSpherePosition, peerRadius, peerHits, LayerMask.GetMask("Enemy"));

                var closest = peerHits.Take(nHits)
                                      .OrderByDescending(hit => CalculateGazeAlignment(hit.transform, cameraTransform))
                                      .FirstOrDefault();
                if (closest == null)
                {
                    handedness.EnemyPeerDisplay.SetProjectile(null);
                    if (lastOutline != null) lastOutline.enabled = false;
                }
                else
                {
                    currentEnemy = closest.GetComponentInParent<Enemy>();
                    var projectile = currentEnemy.GetComponentInChildren<HandProjectile>();
                    handedness.EnemyPeerDisplay.SetProjectile(projectile);

                    if (currentEnemy != null && currentEnemy != lastEnemy)
                    {
                        currentOutline = currentEnemy.GetComponent<Outline>();
                        currentOutline.enabled = true;
                    }
                }
                handedness.EnemyPeerDisplay.Show();

                if (lastOutline != null && currentOutline != lastOutline) lastOutline.enabled = false;
                lastEnemy = currentEnemy;
                lastOutline = currentOutline;
                yield return intervalWait;
                currentDuration -= peerInterval;
            };
            handedness.EnemyPeerDisplay.SetProjectile(null);
            handedness.EnemyPeerDisplay.Hide();
            if (lastEnemy != null) lastOutline.enabled = false;
        }

        private static float CalculateGazeAlignment(Transform targetTransform, Transform gazeTransform)
        {
            var toPlayerVector = (targetTransform.position - gazeTransform.position).normalized;
            return Mathf.Abs(Vector3.Dot(gazeTransform.forward, toPlayerVector));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsPerforming ? Color.green : Color.red;
            Gizmos.DrawWireSphere(PeerSpherePosition, peerRadius);
        }

        private void OnEnable()
        {
            foreach (var property in peerActionProperties)
                property.action.performed += HandlePeer;
        }

        private void OnDisable()
        {
            foreach (var property in peerActionProperties)
                property.action.performed -= HandlePeer;
        }

        private void HandlePeer(InputAction.CallbackContext _) => CastPeer();

        private void Awake()
        {
            if (cameraTransform != null) return;
            cameraTransform = GetComponent<XROrigin>().Camera.transform;
        }

        private Vector3 PeerSpherePosition
        {
            get => (cameraTransform.forward * peerDistance) + cameraTransform.position;
        }

        private void Start()
        {
            Points = new ActionPoints(
                startingPeers,
                startingDuration,
                startingCooldown,
                startingRecoveryPeriod
            );
        }
    }
}
