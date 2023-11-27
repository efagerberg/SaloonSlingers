using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class PlayerHitPointDamageEffect : MonoBehaviour
    {
        [SerializeField]
        private HitPoints hitPoints;
        [SerializeField]
        private CanvasGroup hitFlashCanvasGroup;
        [SerializeField]
        private float duration = 1f;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip hitSoundFX;

        private IEnumerator flashCoroutine;
        private float originalAlpha;

        private void Awake()
        {
            if (hitPoints == null) hitPoints = GetComponent<HitPoints>();
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            originalAlpha = hitFlashCanvasGroup.alpha;
        }

        private void OnEnable()
        {
            hitPoints.Points.Decreased += OnHitPointsDecreased;
        }

        private void OnDisable()
        {
            hitPoints.Points.Decreased -= OnHitPointsDecreased;
        }

        private void OnHitPointsDecreased(Points sender, ValueChangeEvent<uint> e)
        {
            flashCoroutine = Flash(hitFlashCanvasGroup, originalAlpha, 0, duration, flashCoroutine);
            audioSource.PlayOneShot(hitSoundFX);
            StartCoroutine(flashCoroutine);
        }

        public IEnumerator Flash(CanvasGroup flashCanvasGroup, float startAlpha, float endAlpha, float duration, IEnumerator previousCoroutine)
        {
            if (previousCoroutine != null)
            {
                StopCoroutine(previousCoroutine);
                hitFlashCanvasGroup.alpha = startAlpha;
            }

            flashCanvasGroup.gameObject.SetActive(true);
            yield return Fader.FadeTo(flashCanvasGroup, endAlpha, duration);
            flashCanvasGroup.gameObject.SetActive(false);
            flashCanvasGroup.alpha = startAlpha;
        }
    }
}