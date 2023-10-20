using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity.Actor
{
    public class PlayerHitPointDamageEffect : MonoBehaviour
    {
        [SerializeField]
        private HitPoints hitPoints;
        [SerializeField]
        private Image hitFlashImage;
        [SerializeField]
        private float duration = 1f;

        private IEnumerator flashCoroutine;
        private float originalAlpha;

        private void Awake()
        {
            if (hitPoints == null) hitPoints = GetComponent<HitPoints>();
            originalAlpha = hitFlashImage.color.a;
        }

        private void OnEnable()
        {
            hitPoints.Points.Decreased += OnHitPointsDecreased;
        }

        private void OnDisable()
        {
            hitPoints.Points.Decreased -= OnHitPointsDecreased;
        }

        private void OnHitPointsDecreased(Core.Points sender, ValueChangeEvent<uint> e)
        {
            flashCoroutine = Flash(hitFlashImage, originalAlpha, 0, duration, flashCoroutine);
            StartCoroutine(flashCoroutine);
        }

        public IEnumerator Flash(Image image, float startAlpha, float endAlpha, float duration, IEnumerator previousCoroutine)
        {
            if (previousCoroutine != null)
            {
                StopCoroutine(previousCoroutine);
                Color newColor = image.color;
                newColor.a = startAlpha;
                image.color = newColor;
            }

            image.gameObject.SetActive(true);
            yield return Fader.FadeTo(image, endAlpha, duration);
            image.gameObject.SetActive(false);
            var originalColor = image.color;
            originalColor.a = startAlpha;
            image.color = originalColor;
        }
    }
}