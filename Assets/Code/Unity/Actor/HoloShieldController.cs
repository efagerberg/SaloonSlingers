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
        private GameObject shatteredShieldModel;
        [SerializeField]
        private Collider shieldCollider;
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
        private float transitionSeconds = 2;
        [SerializeField]
        [GradientUsage(true)]
        private Gradient shieldStrengthFrontGradient;
        [SerializeField]
        [GradientUsage(true)]
        private Gradient shieldStrengthBackGradient;

        private Vector3 localCollisionPoint;
        private Material shieldMaterial;

        private void OnEnable()
        {
            hitPoints.Points.Increased += OnIncrease;
            hitPoints.Points.Decreased += OnDecrease;
            if (hitPoints > 0) StartCoroutine(nameof(ActivateShield));
        }

        private void OnDisable()
        {
            hitPoints.Points.Increased -= OnIncrease;
            hitPoints.Points.Decreased -= OnDecrease;
            shieldModel.SetActive(false);
            hitRippleVFX.Stop();
            hitRippleVFX.Reinit();
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

        private void OnCollisionEnter(Collision collision)
        {
            localCollisionPoint = transform.InverseTransformPoint(collision.GetContact(0).point);
        }

        private void OnTriggerEnter(Collider collider)
        {
            localCollisionPoint = transform.InverseTransformPoint(collider.ClosestPoint(transform.position));
        }

        private void OnIncrease(Points sender, ValueChangeEvent<uint> e)
        {
            UpdateShieldHitColor();
            if (e.Before == 0) StartCoroutine(nameof(ActivateShield));
        }

        private void OnDecrease(Points sender, ValueChangeEvent<uint> e)
        {
            if (sender.Value == sender.InitialValue) return;

            UpdateShieldHitColor();
            if (e.After > 0) StartCoroutine(nameof(DoShieldHit));
            else StartCoroutine(nameof(DoShieldBreak));
        }

        private void UpdateShieldHitColor()
        {
            float ratio = hitPoints / (float)hitPoints.Points.InitialValue;
            var frontColor = shieldStrengthFrontGradient.Evaluate(ratio);
            var backColor = shieldStrengthBackGradient.Evaluate(ratio);
            shieldMaterial.SetColor("_FrontColor", frontColor);
            shieldMaterial.SetColor("_BackColor", backColor);
        }

        private IEnumerator ActivateShield()
        {
            PlayOneShotRandomPitch(shieldChargeClip, 1f, 2f);
            shieldModel.SetActive(true);
            shieldCollider.enabled = true;
            float elapsedTime = 0f;

            while (elapsedTime < transitionSeconds)
            {
                elapsedTime += Time.deltaTime;
                float scaleValue = scaleCurve.Evaluate(elapsedTime / transitionSeconds);
                transform.localScale = Vector3.one * scaleValue;

                yield return null;
            }

            transform.localScale = Vector3.one * scaleCurve.Evaluate(1f);
        }

        private IEnumerator DoShieldBreak()
        {
            shieldCollider.enabled = false;
            var shardRenderer = shatteredShieldModel.GetComponentsInChildren<Renderer>();
            foreach (var r in shardRenderer)
                r.material = shieldMaterial;

            shieldModel.SetActive(false);
            shatteredShieldModel.SetActive(true);
            PlayOneShotRandomPitch(shieldBrokenClip, 1f, 2f);

            yield return Fader.Fade(SetShieldMaterialAlpha, transitionSeconds);

            shatteredShieldModel.SetActive(false);
            SetShieldMaterialAlpha(1);
        }

        private void SetShieldMaterialAlpha(float alpha)
        {
            foreach (var key in new string[] { "_FresnelColor", "_FrontColor", "_BackColor" })
            {
                var color = shieldMaterial.GetColor(key);
                shieldMaterial.SetColor(key, color * alpha);
            }
        }

        private IEnumerator DoShieldHit()
        {
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
