using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity
{
    public class PlayerHit : MonoBehaviour
    {
        [SerializeField]
        private Health health;
        [SerializeField]
        private Image hitFlashImage;
        [SerializeField]
        private float hitFlashDuration = 1f;
        [SerializeField]
        private SceneLoader sceneLoader;

        private Vector3 startingPosition;
        private Coroutine flashCoroutine;
        private float startingFlashAlpha;

        private void Awake()
        {
            if (health == null) health = GetComponent<Health>();
            startingPosition = transform.position;
            startingFlashAlpha = hitFlashImage.color.a;
        }

        private void OnEnable()
        {
            health.Points.OnPointsChanged += HealthPointsChangedHandler;
        }

        private void OnDisable()
        {
            health.Points.OnPointsChanged -= HealthPointsChangedHandler;
        }

        private void HealthPointsChangedHandler(Points sender, ValueChangeEvent<uint> e)
        {
            if (flashCoroutine != null)
                StopCoroutine(nameof(DisableFlashSmoothly));

            if (e.After == 0)
            {
                sceneLoader.LoadScene("GameOver");
                return;
            }

            hitFlashImage.gameObject.SetActive(true);
            flashCoroutine = StartCoroutine(nameof(DisableFlashSmoothly));
        }

        private IEnumerator DisableFlashSmoothly()
        {
            float targetAlpha = 0f;
            float elapsedTime = 0f;

            while (elapsedTime < hitFlashDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / hitFlashDuration);

                // Lerp the alpha value from the original alpha to the target alpha
                float lerpedAlpha = Mathf.Lerp(startingFlashAlpha, targetAlpha, t);

                // Set the red flash image color with the updated alpha
                Color redFlashColor = hitFlashImage.color;
                redFlashColor.a = lerpedAlpha;
                hitFlashImage.color = redFlashColor;

                yield return null;
            }

            hitFlashImage.gameObject.SetActive(false);
        }
    }
}