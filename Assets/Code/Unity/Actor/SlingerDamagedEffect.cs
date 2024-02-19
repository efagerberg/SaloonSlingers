using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class SlingerDamagedEffect : MonoBehaviour
    {
        [SerializeField]
        private float duration = 1f;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip hitSoundFX;

        private IEnumerator flashCoroutine;
        private Attribute hitPoints;
        private IHitFlasher hitFlasher;

        private void Awake()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            hitFlasher = GetComponent<IHitFlasher>();
        }

        private void OnEnable()
        {
            if (hitPoints == null) return;

            hitPoints.Decreased += OnHitPointsDecreased;
        }

        private void OnDisable()
        {
            if (hitPoints == null) return;

            hitPoints.Decreased -= OnHitPointsDecreased;
        }

        private void Start()
        {
            hitPoints ??= GetComponent<Attributes>().Registry[AttributeType.Health];
            hitPoints.Decreased += OnHitPointsDecreased;
        }

        private void OnHitPointsDecreased(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);

            flashCoroutine = hitFlasher.Flash(duration);
            audioSource.PlayOneShot(hitSoundFX);
            StartCoroutine(flashCoroutine);
        }
    }
}