using System.Collections;
using System.Collections.Generic;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.VFX;

namespace SaloonSlingers.Unity
{
    public class HoloShieldController : MonoBehaviour
    {
        [SerializeField]
        private HitPoints hitPoints;
        [SerializeField]
        private GameObject shieldModel;
        [SerializeField]
        private SphereCollider sphereCollider;
        [SerializeField]
        private VisualEffect hitRippleVFX;
        [SerializeField]
        [Tooltip("Shield strength from weakest to strongest")]
        [GradientUsage(true)]
        private Gradient shieldStrengthGradient;
        [SerializeField]
        private AudioSource shieldAudioSource;
        [SerializeField]
        private AudioClip shieldHitClip;
        [SerializeField]
        private AudioClip shieldBrokenClip;
        [SerializeField]
        private AudioClip shieldChargeClip;
        [SerializeField]
        private AnimationCurve scaleCurve;
        [SerializeField]
        private float activityTransitionTime = 2;
        [SerializeField]
        [GradientUsage(true)]
        private Gradient fresnelDecayGradient;

        private Vector3 localCollisionPoint;
        private Material shieldMaterial;
        private readonly Queue<ShieldState> nextStates = new();

        private void OnEnable()
        {
            hitPoints.Points.Increased += OnIncrease;
            hitPoints.Points.Decreased += OnDecrease;
            StartCoroutine(nameof(StateMachine));
        }

        private void OnDisable()
        {
            hitPoints.Points.Increased -= OnIncrease;
            hitPoints.Points.Decreased -= OnDecrease;
            CancelInvoke(nameof(StateMachine));
            nextStates.Clear();
            shieldModel.SetActive(false);
            hitRippleVFX.Reinit();
            sphereCollider.enabled = false;
        }

        private void Awake()
        {
            shieldMaterial = shieldModel.GetComponent<MeshRenderer>().material;
            hitRippleVFX.SetGradient("Gradient", shieldStrengthGradient);
            if (hitPoints.Points.Value > 0) nextStates.Enqueue(ShieldState.Activating);

        }

        private void Update()
        {
            Vector3 worldCollisionPoint = transform.TransformPoint(localCollisionPoint);
            hitRippleVFX.SetVector3("Center", worldCollisionPoint);
        }

        private void LateUpdate()
        {
            // Makes the hit effects more consistent where the collision happened on the shield.
            sphereCollider.transform.rotation = Quaternion.identity;
        }

        private void OnTriggerEnter(Collider collider)
        {
            localCollisionPoint = transform.InverseTransformPoint(collider.ClosestPoint(transform.position));
        }

        private void OnIncrease(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.Before == 0) nextStates.Enqueue(ShieldState.Activating);
        }

        private void OnDecrease(Points sender, ValueChangeEvent<uint> e)
        {
            nextStates.Enqueue(e.After == 0 ? ShieldState.Broken : ShieldState.Hit);
        }

        private void UpdateShieldHitColor()
        {
            if (hitPoints.Points.Value != 0)
            {
                float ratio = hitPoints.Points.Value / (float)hitPoints.Points.InitialValue;
                hitRippleVFX.SetFloat("ShieldStrength", ratio);
            }
        }

        private IEnumerator ActivateShield()
        {
            shieldAudioSource.PlayOneShot(shieldChargeClip);
            shieldMaterial.SetColor("_FresnelColor", fresnelDecayGradient.Evaluate(0f));
            shieldModel.SetActive(true);
            sphereCollider.enabled = true;
            float elapsedTime = 0f;

            while (elapsedTime < activityTransitionTime)
            {
                elapsedTime += Time.deltaTime;
                float scaleValue = scaleCurve.Evaluate(elapsedTime / activityTransitionTime);
                transform.localScale = Vector3.one * scaleValue;

                yield return null;
            }

            transform.localScale = Vector3.one * scaleCurve.Evaluate(1f);
        }

        private IEnumerator DoShieldBreak()
        {
            sphereCollider.enabled = false;
            UpdateShieldHitColor();
            hitRippleVFX.Play();
            shieldAudioSource.pitch = 1;
            shieldAudioSource.PlayOneShot(shieldBrokenClip);

            float elapsedTime = 0f;
            while (elapsedTime < activityTransitionTime)
            {
                elapsedTime += Time.deltaTime;
                shieldMaterial.SetColor("_FresnelColor", fresnelDecayGradient.Evaluate(elapsedTime / activityTransitionTime));

                yield return null;
            }

            shieldModel.SetActive(false);
        }
        private IEnumerator DoShieldHit()
        {
            UpdateShieldHitColor();
            hitRippleVFX.Play();
            shieldAudioSource.pitch = hitPoints.Points.InitialValue / ((float)hitPoints.Points.Value + 1);
            shieldAudioSource.PlayOneShot(shieldHitClip);
            yield return new WaitForSeconds(shieldHitClip.length);
            localCollisionPoint = Vector3.zero;
            shieldAudioSource.pitch = 1;
        }

        public enum ShieldState
        {
            Idle,
            Activating,
            Hit,
            Broken
        }

        private IEnumerator StateMachine()
        {
            while (true)
            {
                var currentState = nextStates.Count == 0 ? ShieldState.Idle : nextStates.Dequeue();
                switch (currentState)
                {
                    case ShieldState.Activating:
                        yield return StartCoroutine(nameof(ActivateShield));
                        break;
                    case ShieldState.Broken:
                        yield return StartCoroutine(nameof(DoShieldBreak));
                        break;
                    case ShieldState.Hit:
                        yield return StartCoroutine(nameof(DoShieldHit));
                        break;
                    default:
                        yield return new WaitForSeconds(0.2f);
                        break;
                }
            }
        }

    }
}
