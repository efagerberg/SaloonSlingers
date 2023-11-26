using System.Collections;

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

        private void OnEnable()
        {
            hitPoints.Points.Increased += OnIncrease;
            hitPoints.Points.Decreased += OnDecrease;
            if (hitPoints.Points.Value > 0) StartCoroutine(nameof(ActivateShield));
        }

        private void OnDisable()
        {
            hitPoints.Points.Increased -= OnIncrease;
            hitPoints.Points.Decreased -= OnDecrease;
            shieldModel.SetActive(false);
            hitRippleVFX.Stop();
            hitRippleVFX.Reinit();
            hitRippleVFX.enabled = false;
            sphereCollider.enabled = false;
            shieldAudioSource.pitch = 1;
        }

        private void Awake()
        {
            shieldMaterial = shieldModel.GetComponent<MeshRenderer>().material;
            shieldMaterial.SetFloat("_BreathOffset", Random.Range(0f, 1f));
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

        private void OnCollisionEnter(Collision collision)
        {
            localCollisionPoint = transform.InverseTransformPoint(collision.GetContact(0).point);
        }

        private void OnIncrease(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.Before == 0) StartCoroutine(nameof(ActivateShield));
        }

        private void OnDecrease(Points sender, ValueChangeEvent<uint> e)
        {
            if (sender.Value == sender.InitialValue) return;
            if (e.After > 0) StartCoroutine(nameof(DoShieldHit));
            else StartCoroutine(nameof(DoShieldBreak));
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
            PlayOneShotRandomPitch(shieldChargeClip, 1f, 2f);
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
            hitRippleVFX.enabled = true;
            sphereCollider.enabled = false;
            UpdateShieldHitColor();
            hitRippleVFX.Play();
            PlayOneShotRandomPitch(shieldBrokenClip, 1f, 2f);

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
            hitRippleVFX.enabled = true;
            hitRippleVFX.Play();
            PlayOneShotRandomPitch(shieldHitClip, 1f, 2f);
            yield return new WaitForSeconds(shieldHitClip.length);
            localCollisionPoint = Vector3.zero;
        }

        private void PlayOneShotRandomPitch(AudioClip clip, float min, float max)
        {
            shieldAudioSource.pitch = Random.Range(min, max);
            shieldAudioSource.PlayOneShot(clip);
        }
    }
}
