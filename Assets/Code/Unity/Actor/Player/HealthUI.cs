using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

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

        private HitPoints hitPoints;
        private Transform gazer;

        private void Awake()
        {
            hitPoints = LevelManager.Instance.Player.GetComponent<HitPoints>();
            hitPoints.Points.Increased += UpdateHealthBar;
            hitPoints.Points.Decreased += UpdateHealthBar;
            UpdateFill(healthBar, hitPoints.Points);
            healthPercentText.text = hitPoints.Points.AsPercent().ToString("P0");
            healthPercentText.color = healthBar.color;
            gazeUI.alpha = 0f;
        }

        private void OnEnable()
        {
            hitPoints.Points.Increased += UpdateHealthBar;
            hitPoints.Points.Decreased += UpdateHealthBar;
        }

        private void OnDisable()
        {
            hitPoints.Points.Increased -= UpdateHealthBar;
            hitPoints.Points.Decreased -= UpdateHealthBar;
        }

        public void OnGazeEnter(HoverEnterEventArgs args)
        {
            gazer = args.interactorObject.transform;

            if (!gameObject.activeInHierarchy) return;

            StartCoroutine(Fader.FadeTo(gazeUI, 1, fadeDuration));
        }

        public void OnGazeExit()
        {
            gazer = null;

            if (!gameObject.activeInHierarchy) return;

            StartCoroutine(Fader.FadeTo(gazeUI, 0, fadeDuration));
        }

        private void Update()
        {
            if (gazer == null) return;

            //gazeUI.transform.forward = gazer.forward;
            //gazeUI.transform.right = gazer.right;
            //gazeUI.transform.up = gazer.up;
            var newRot = gazeUI.transform.rotation.eulerAngles;
            newRot.z = gazer.rotation.z;
            gazeUI.transform.rotation = Quaternion.Euler(newRot);
        }

        private void UpdateHealthBar(Points sender, ValueChangeEvent<uint> e)
        {
            UpdateFill(healthBar, sender);
            healthPercentText.text = sender.AsPercent().ToString("P0");
        }

        private static void UpdateFill(Image image, Points points)
        {
            image.fillAmount = points.AsPercent();
        }
    }
}
