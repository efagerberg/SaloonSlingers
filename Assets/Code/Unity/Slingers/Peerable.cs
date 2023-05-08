using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core;

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
        private uint startingPeers = 3;
        [SerializeField]
        private float startingCooldown = 3;
        [SerializeField]
        private float startingDuration = 5f;
        [SerializeField]
        private float startingRecoveryPeriod = 1f;

        private Transform cameraTransform;

        public void Peer()
        {
            IEnumerator coroutine = GetActionCoroutine(Points, DoPeer);
            if (coroutine == null) return;

            StartCoroutine(coroutine);
        }

        private IEnumerator DoPeer()
        {
            HashSet<HandPeerer> seenPeerers = new();
            float currentDuration = Points.Duration;
            while (currentDuration > 0)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, peerRadius, LayerMask.GetMask("HandPeerer"));
                HashSet<HandPeerer> newPeerers = hits.Where(x => x.transform.parent.gameObject.layer != LayerMask.NameToLayer("PlayerBody"))
                                                     .Select(x => x.GetComponent<HandPeerer>())
                                                     .Except(seenPeerers).ToHashSet();
                foreach (HandPeerer peerer in newPeerers)
                {
                    peerer.Peer(cameraTransform);
                }
                seenPeerers.UnionWith(newPeerers);
                currentDuration -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            };
            foreach (HandPeerer peerer in seenPeerers)
            {
                peerer.Hide();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsPerforming ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, peerRadius);
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

        private void HandlePeer(InputAction.CallbackContext _) => Peer();

        private void Awake()
        {
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
