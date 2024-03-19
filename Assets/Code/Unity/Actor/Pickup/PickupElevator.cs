using System.Collections;

using UnityEngine;
using UnityEngine.Animations;

namespace SaloonSlingers.Unity.Actor
{

    public class PickupElevator : Actor
    {
        [SerializeField]
        private GameObject interactable;
        [SerializeField]
        private LayerMask elevateLayerMask;
        [SerializeField]
        private LayerMask environmentLayerMask;
        [SerializeField]
        private float detectionMaxDistance = 1f;
        [SerializeField]
        private float elevationSpeed = 1f;
        [SerializeField]
        private float peakPauseSeconds = 0.5f;
        [SerializeField]
        private PositionConstraint constraint;
        [SerializeField]
        private LineRenderer lineRenderer;

        private bool isPerforming = false;
        private RaycastHit hit;
        private Rigidbody interactableRigidBody;
        private Collider interactableCollider;
        private Transform interactableTransform;

        public void Associate(GameObject interactable)
        {
            this.interactable = interactable;
            interactableRigidBody = interactable.GetComponent<Rigidbody>();
            interactableCollider = interactable.GetComponent<Collider>();
            interactableTransform = interactable.transform;
            var source = new ConstraintSource { sourceTransform = interactable.transform, weight = 1 };
            constraint.AddSource(source);
            constraint.constraintActive = true;
            lineRenderer.enabled = true;
        }

        public override void ResetActor()
        {
            lineRenderer.enabled = false;
            constraint.constraintActive = false;
            constraint.RemoveSource(0);
            interactableTransform = null;
            interactableCollider = null;
            interactableRigidBody = null;
            interactable = null;
            isPerforming = false;
        }

        public void Kill()
        {
            OnKilled?.Invoke(gameObject);
        }

        private void Awake()
        {
            if (interactable == null) return;

            Associate(interactable);
        }

        private void OnEnable()
        {
            isPerforming = false;
        }

        private void Update()
        {
            if (isPerforming) return;

            if (Physics.Raycast(transform.position,
                                Vector3.up,
                                out hit,
                                detectionMaxDistance,
                                elevateLayerMask))
                StartCoroutine(nameof(MoveTo), hit.point);
        }

        private IEnumerator MoveTo(Vector3 targetPosition)
        {
            isPerforming = true;
            interactableRigidBody.isKinematic = true;
            Vector3 initialPosition = interactableTransform.position;
            float distanceToTarget = Vector3.Distance(initialPosition, targetPosition);
            constraint.constraintActive = false;

            float duration = distanceToTarget / elevationSpeed;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                interactableTransform.position = Vector3.Lerp(initialPosition, targetPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(peakPauseSeconds);
            isPerforming = false;
            interactableRigidBody.isKinematic = false;
        }

    }
}
