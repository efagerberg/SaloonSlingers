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
        private float activationTransitionSeconds = 2;
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
            if (hitPoints.Points.Value > 0) nextStates.Enqueue(ShieldState.Active);

        }

        private void Update()
        {
            Vector3 worldCollisionPoint = transform.TransformPoint(localCollisionPoint);
            hitRippleVFX.SetVector3("Center", worldCollisionPoint);
            shieldMaterial.SetVector("_Position", transform.position);
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
            if (e.Before == 0) nextStates.Enqueue(ShieldState.Active);
        }

        private void OnDecrease(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After > 0) StartCoroutine(nameof(DoShieldHit));
            else nextStates.Enqueue(ShieldState.Broken);
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

            while (elapsedTime < activationTransitionSeconds)
            {
                elapsedTime += Time.deltaTime;
                float scaleValue = scaleCurve.Evaluate(elapsedTime / activationTransitionSeconds);
                transform.localScale = Vector3.one * scaleValue;

                yield return null;
            }

            transform.localScale = Vector3.one * scaleCurve.Evaluate(1f);
        }

        private IEnumerator DoShieldBreak()
        {
            sphereCollider.enabled = false;
            UpdateShieldHitColor();
            hitRippleVFX.SetFloat("Lifetime", shieldBrokenClip.length);
            hitRippleVFX.Play();
            shieldAudioSource.pitch = 1;
            shieldAudioSource.PlayOneShot(shieldBrokenClip);

            float elapsedTime = 0f;
            while (elapsedTime < shieldBrokenClip.length)
            {
                elapsedTime += Time.deltaTime;
                var decayColor = fresnelDecayGradient.Evaluate(elapsedTime / shieldBrokenClip.length);
                shieldMaterial.SetColor("_FresnelColor", decayColor);

                yield return null;
            }

            shieldModel.SetActive(false);
        }
        private IEnumerator DoShieldHit()
        {
            UpdateShieldHitColor();
            hitRippleVFX.SetFloat("Lifetime", shieldHitClip.length);
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
            Active,
            Broken
        }

        private IEnumerator StateMachine()
        {
            while (true)
            {
                var currentState = nextStates.Count == 0 ? ShieldState.Idle : nextStates.Dequeue();
                switch (currentState)
                {
                    case ShieldState.Active:
                        yield return StartCoroutine(nameof(ActivateShield));
                        break;
                    case ShieldState.Broken:
                        yield return StartCoroutine(nameof(DoShieldBreak));
                        break;
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

    }
}
