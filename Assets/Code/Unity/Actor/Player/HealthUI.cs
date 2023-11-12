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
            var cam = LevelManager.Instance.Player.GetComponent<XROrigin>().Camera;
            shieldHitPoints = cam.GetComponentInChildren<HitPoints>();
            UpdateFill(tempHealthBar, shieldHitPoints.Points);
            tempHealthPercentText.text = shieldHitPoints.Points.AsPercent().ToString("P0");
            tempHealthPercentText.color = tempHealthBar.color;

            hitPoints = LevelManager.Instance.Player.GetComponent<HitPoints>();
            UpdateFill(healthBar, hitPoints.Points);
            healthPercentText.text = hitPoints.Points.AsPercent().ToString("P0");
            healthPercentText.color = healthBar.color;
        }

        private void OnEnable()
        {
            shieldHitPoints.Points.Increased += UpdateTempHealthBar;
            shieldHitPoints.Points.Decreased += UpdateTempHealthBar;

            hitPoints.Points.Increased += UpdateHealthBar;
            hitPoints.Points.Decreased += UpdateHealthBar;
        }

        private void OnDisable()
        {
            shieldHitPoints.Points.Increased -= UpdateTempHealthBar;
            shieldHitPoints.Points.Decreased -= UpdateTempHealthBar;

            hitPoints.Points.Increased -= UpdateHealthBar;
            hitPoints.Points.Decreased -= UpdateHealthBar;
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
