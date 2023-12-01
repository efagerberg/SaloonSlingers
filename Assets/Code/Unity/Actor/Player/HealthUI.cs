using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using TMPro;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField]
        private Image tempHealthBar;
        [SerializeField]
        private TextMeshProUGUI tempHealthPercentText;
        [SerializeField]
        private Image healthBar;
        [SerializeField]
        private TextMeshProUGUI healthPercentText;

        private HitPoints shieldHitPoints;
        private HitPoints hitPoints;

        private void Awake()
        {
            hitPoints = LevelManager.Instance.Player.GetComponent<HitPoints>();
            hitPoints.Points.Increased += UpdateHealthBar;
            hitPoints.Points.Decreased += UpdateHealthBar;
            UpdateFill(healthBar, hitPoints.Points);
            healthPercentText.text = hitPoints.Points.AsPercent().ToString("P0");
            healthPercentText.color = healthBar.color;
        }

        private void OnEnable()
        {
            hitPoints.Points.Increased += UpdateHealthBar;
            hitPoints.Points.Decreased += UpdateHealthBar;

            if (shieldHitPoints)
            {
                shieldHitPoints.Points.Increased += UpdateTempHealthBar;
                shieldHitPoints.Points.Decreased += UpdateTempHealthBar;
            }
        }

        private void OnDisable()
        {
            hitPoints.Points.Increased -= UpdateHealthBar;
            hitPoints.Points.Decreased -= UpdateHealthBar;

            shieldHitPoints.Points.Increased -= UpdateTempHealthBar;
            shieldHitPoints.Points.Decreased -= UpdateTempHealthBar;
        }

        private void Start()
        {
            var origin = LevelManager.Instance.Player.GetComponent<XROrigin>();
            shieldHitPoints = origin.Camera.transform.parent.GetComponentInChildren<HitPoints>();
            shieldHitPoints.Points.Increased += UpdateTempHealthBar;
            shieldHitPoints.Points.Decreased += UpdateTempHealthBar;
            UpdateFill(tempHealthBar, shieldHitPoints.Points);
            tempHealthPercentText.text = shieldHitPoints.Points.AsPercent().ToString("P0");
            tempHealthPercentText.color = tempHealthBar.color;
        }

        private void UpdateTempHealthBar(Points sender, ValueChangeEvent<uint> e)
        {
            UpdateFill(tempHealthBar, sender);
            tempHealthPercentText.text = sender.AsPercent().ToString("P0");
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
