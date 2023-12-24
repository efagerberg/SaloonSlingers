using SaloonSlingers.Core;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField]
        private Image healthBar;
        [SerializeField]
        private TextMeshProUGUI healthPercentText;
        [SerializeField]
        private CanvasGroup gazeUI;
        [SerializeField]
        private float fadeDuration = 0.5f;

        private Transform gazer;
        private Points hitPoints;

        private void Awake()
        {
            hitPoints = LevelManager.Instance.Player.GetComponent<Attributes>().Registry[AttributeType.Health];
            hitPoints.Increased += UpdateHealthBar;
            hitPoints.Decreased += UpdateHealthBar;
            UpdateFill(healthBar, hitPoints);
            healthPercentText.text = hitPoints.AsPercent().ToString("P0");
            healthPercentText.color = healthBar.color;
            gazeUI.alpha = 0f;
        }

        private void OnEnable()
        {
            hitPoints.Increased += UpdateHealthBar;
            hitPoints.Decreased += UpdateHealthBar;
        }

        private void OnDisable()
        {
            hitPoints.Increased -= UpdateHealthBar;
            hitPoints.Decreased -= UpdateHealthBar;
        }

        public void OnGazeEnter(HoverEnterEventArgs args)
        {
            gazer = args.interactorObject.transform;

            if (!gameObject.activeInHierarchy) return;

            StartCoroutine(Fader.Fade((alpha) => gazeUI.alpha = alpha, fadeDuration, 1));
        }

        public void OnGazeExit()
        {
            gazer = null;

            if (!gameObject.activeInHierarchy) return;

            StartCoroutine(Fader.Fade((alpha) => gazeUI.alpha = alpha, fadeDuration, 0));
        }

        private void Update()
        {
            if (gazer == null) return;

            var newRot = gazeUI.transform.rotation.eulerAngles;
            newRot.z = gazer.rotation.z;
            gazeUI.transform.rotation = Quaternion.Euler(newRot);
        }

        private void UpdateHealthBar(IReadOnlyPoints sender, ValueChangeEvent<uint> e)
        {
            UpdateFill(healthBar, sender);
            healthPercentText.text = sender.AsPercent().ToString("P0");
        }

        private static void UpdateFill(Image image, IReadOnlyPoints points)
        {
            image.fillAmount = points.AsPercent();
        }
    }
}
