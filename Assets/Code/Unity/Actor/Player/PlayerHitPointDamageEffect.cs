using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class PlayerHitPointDamageEffect : MonoBehaviour
    {
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
        private Points hitPoints;

        private void Awake()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            originalAlpha = hitFlashCanvasGroup.alpha;
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

        private void OnHitPointsDecreased(IReadOnlyPoints sender, ValueChangeEvent<uint> e)
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
            yield return Fader.Fade((alpha) => flashCanvasGroup.alpha = alpha, duration);
            flashCanvasGroup.gameObject.SetActive(false);
            flashCanvasGroup.alpha = startAlpha;
        }
    }
}