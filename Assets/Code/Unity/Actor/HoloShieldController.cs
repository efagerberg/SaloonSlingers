using System.Collections;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

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
        private ParticleSystem hitRippleParticleSystem;
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

        private Material hitRippleMaterial;
        private Vector3 localCollisionPoint;

        private void OnEnable()
        {
            hitPoints.Points.Increased += OnIncrease;
            hitPoints.Points.Decreased += OnDecreased;
        }

        private void OnDisable()
        {
            hitPoints.Points.Increased -= OnIncrease;
            hitPoints.Points.Decreased -= OnDecreased;
        }

        private void Awake()
        {
            SetShieldActive(hitPoints.Points.Value > 0);
            hitRippleMaterial = hitRippleParticleSystem.GetComponent<Renderer>().material;
        }

        private void LateUpdate()
        {
            // Looks better if the rotation is frozen
            // Eventually, there will be collision ripples to tell
            // the player where they hit an enemy or where they were hit.
            sphereCollider.transform.rotation = Quaternion.identity;
        }

        private void Update()
        {
            if (localCollisionPoint != Vector3.zero)
            {
                Vector3 worldCollisionPoint = transform.TransformPoint(localCollisionPoint);
                hitRippleMaterial.SetVector("_Center", worldCollisionPoint);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (hitRippleMaterial == null)
                hitRippleMaterial = hitRippleParticleSystem.GetComponent<Renderer>().material;
            localCollisionPoint = transform.InverseTransformPoint(collider.ClosestPoint(transform.position));
        }

        private void OnIncrease(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.Before == 0)
            {
                SetShieldActive(true);
                shieldAudioSource.PlayOneShot(shieldChargeClip);
            }
        }

        private void OnDecreased(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After == 0) StartCoroutine(DoShieldBreak(e.After));
            else StartCoroutine(DoShieldHit(e.After));
        }

        private void SetShieldStrengthColor(uint current)
        {
            if (hitRippleMaterial != null)
            {
                float ratio = current / (float)hitPoints.Points.InitialValue;
                hitRippleMaterial.SetColor("_Color", shieldStrengthGradient.Evaluate(ratio));
            }
        }

        private void SetShieldActive(bool active)
        {
            shieldModel.SetActive(active);
            sphereCollider.enabled = active;
        }

        private IEnumerator DoShieldBreak(uint after)
        {
            sphereCollider.enabled = false;
            SetShieldStrengthColor(after);
            shieldAudioSource.pitch = 1;
            shieldAudioSource.PlayOneShot(shieldBrokenClip);
            hitRippleParticleSystem.Emit(1);
            yield return new WaitForSeconds(hitRippleParticleSystem.main.startLifetime.constant);
            localCollisionPoint = Vector3.zero;
            SetShieldActive(false);
        }
        private IEnumerator DoShieldHit(uint after)
        {
            SetShieldStrengthColor(after);
            hitRippleParticleSystem.Emit(1);
            shieldAudioSource.pitch = hitPoints.Points.InitialValue / ((float)after + 1);
            shieldAudioSource.PlayOneShot(shieldHitClip);
            yield return new WaitForSeconds(shieldHitClip.length);
            localCollisionPoint = Vector3.zero;
            shieldAudioSource.pitch = 1;
        }
    }
}
