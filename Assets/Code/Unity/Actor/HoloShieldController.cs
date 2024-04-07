using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.VFX;

namespace SaloonSlingers.Unity
{
    public class HoloShieldController : MonoBehaviour
    {
        public Attribute HitPoints { get; set; }

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
        private float transitionSeconds = 1;
        [SerializeField]
        [GradientUsage(true)]
        private Gradient shieldStrengthFrontGradient;
        [SerializeField]
        [GradientUsage(true)]
        private Gradient shieldStrengthBackGradient;

        private Vector3 localCollisionPoint;
        private Material shieldMaterial;
        private IDictionary<string, Color> materialKeyToColor;
        private Coroutine fadeOutCoroutine;

        public void DecreaseHandler(GameObject sender, IReadOnlyAttribute attribute)
        {
            if (attribute.Value == attribute.InitialValue) return;

            UpdateShieldStrengthColor();
            if (attribute.Value > 0) StartCoroutine(nameof(DoShieldHit));
            else StartCoroutine(nameof(DoShieldBreak));
        }

        public void UpdateCollisionPoint(Collision collision)
        {
            localCollisionPoint = transform.InverseTransformPoint(collision.GetContact(0).point);
        }

        public void UpdateCollisionPoint(Collider collider)
        {
            localCollisionPoint = transform.InverseTransformPoint(collider.ClosestPoint(transform.position));
        }


        private void OnEnable()
        {
            HitPoints.Increased += IncreaseHandler;
            if (HitPoints > 0) StartCoroutine(nameof(ActivateShield));
        }

        private void OnDisable()
        {
            if (HitPoints == null) return;

            HitPoints.Increased -= IncreaseHandler;
            shieldModel.SetActive(false);
            hitRippleVFX.Stop();
            hitRippleVFX.Reinit();
            shieldAudioSource.pitch = 1;
        }

        private void Awake()
        {
            HitPoints ??= new Attribute(0, uint.MaxValue);
            var damageTarget = GetComponent<HandProjectileDamageTarget>();
            damageTarget.HitPoints = HitPoints;
            shieldMaterial = shieldModel.GetComponent<MeshRenderer>().material;
            shieldMaterial.SetFloat("_BreathOffset", Random.Range(0f, 1f));
            var zipped = (new string[] { "_FresnelColor", "_FrontColor", "_BackColor" }).Select(key => (key, shieldMaterial.GetColor(key)));
            materialKeyToColor = zipped.ToDictionary(tuple => tuple.key, tuple => tuple.Item2);
        }

        private void Update()
        {
            Vector3 worldCollisionPoint = transform.TransformPoint(localCollisionPoint);
            hitRippleVFX.SetVector3("Center", worldCollisionPoint);
            shieldMaterial.SetVector("_Position", transform.position);
        }

        private void IncreaseHandler(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
        {
            UpdateShieldStrengthColor();
            if (e.Before == 0) StartCoroutine(nameof(ActivateShield));
        }

        private void UpdateShieldStrengthColor()
        {
            float ratio = HitPoints / (float)HitPoints.InitialValue;
            var frontColor = shieldStrengthFrontGradient.Evaluate(ratio);
            materialKeyToColor["_FrontColor"] = frontColor;
            var backColor = shieldStrengthBackGradient.Evaluate(ratio);
            materialKeyToColor["_BackColor"] = backColor;
            shieldMaterial.SetColor("_FrontColor", frontColor);
            shieldMaterial.SetColor("_BackColor", backColor);
        }

        private IEnumerator ActivateShield()
        {
            if (fadeOutCoroutine != null) StopCoroutine(fadeOutCoroutine);
            shatteredShieldModel.SetActive(false);

            SetShieldMaterialAlpha(1);
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
            if (fadeOutCoroutine != null) StopCoroutine(fadeOutCoroutine);
            shieldCollider.enabled = false;
            var shardRenderer = shatteredShieldModel.GetComponentsInChildren<Renderer>();
            foreach (var r in shardRenderer)
                r.material = shieldMaterial;

            shieldModel.SetActive(false);
            shatteredShieldModel.SetActive(true);
            PlayOneShotRandomPitch(shieldBrokenClip, 1f, 2f);

            fadeOutCoroutine = StartCoroutine(Fader.Fade(SetShieldMaterialAlpha, transitionSeconds));
            yield return fadeOutCoroutine;

            shatteredShieldModel.SetActive(false);
        }

        private void SetShieldMaterialAlpha(float alpha)
        {
            foreach (var (key, color) in materialKeyToColor)
            {
                // We have multiple colors, so we need to scale the color
                // and make sure we track the current colors
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
