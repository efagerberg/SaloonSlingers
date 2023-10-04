using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity.Actor
{
    public class PlayerHit : MonoBehaviour
    {
        [SerializeField]
        private HitPoints hitPoints;
        [SerializeField]
        private Image hitFlashImage;
        [SerializeField]
        private float duration = 1f;
        [SerializeField]
        private string gameOverSceneName;

        private IEnumerator flashCoroutine;
        private float originalAlpha;

        private void Awake()
        {
            if (hitPoints == null) hitPoints = GetComponent<HitPoints>();
            originalAlpha = hitFlashImage.color.a;
        }

        private void OnEnable()
        {
            hitPoints.Points.OnPointsChanged += HitPointsChangedHandler;
        }

        private void OnDisable()
        {
            hitPoints.Points.OnPointsChanged -= HitPointsChangedHandler;
        }

        private void HitPointsChangedHandler(Points sender, ValueChangeEvent<uint> e)
        {
            flashCoroutine = Flash(hitFlashImage, originalAlpha, 0, duration, flashCoroutine);
            StartCoroutine(flashCoroutine);
            if (e.After != 0) return;

            GameManager.Instance.SceneLoader.LoadScene(gameOverSceneName);
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