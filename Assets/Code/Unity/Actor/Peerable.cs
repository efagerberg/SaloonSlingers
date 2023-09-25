using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.InputSystem;


namespace SaloonSlingers.Unity.Actor
{
    public class Peerable : ActionPerformer
    {
        public ActionPoints Points { get; private set; }

        [SerializeField]
        private List<InputActionProperty> peerActionProperties;
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
        private ActorHandedness handedness;
        [SerializeField]
        private float peerInterval = 0.2f;
        [SerializeField]
        private VisibilityDetector visibilityDetector;

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
                var closest = visibilityDetector.GetVisible(LayerMask.GetMask("Enemy"), xRay: true)
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
            visibilityDetector = GetComponent<VisibilityDetector>();
            if (cameraTransform != null) return;
            cameraTransform = GetComponent<XROrigin>().Camera.transform;
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
