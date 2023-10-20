using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField]
        private Image tempHealthBar;
        [SerializeField]
        private Image healthBar;

        private TemporaryHitPoints tempHitPoints;
        private HitPoints hitPoints;

        private void Awake()
        {
            tempHitPoints = LevelManager.Instance.Player.GetComponent<TemporaryHitPoints>();
            UpdateFill(tempHealthBar, tempHitPoints.Points);
            hitPoints = LevelManager.Instance.Player.GetComponent<HitPoints>();
            UpdateFill(healthBar, hitPoints.Points);

        }

        private void OnEnable()
        {
            tempHitPoints.Points.Increased += UpdateTempHealthBar;
            tempHitPoints.Points.Decreased += UpdateTempHealthBar;
            hitPoints.Points.Increased += UpdateHealthBar;
            hitPoints.Points.Decreased += UpdateHealthBar;
        }

        private void OnDisable()
        {
            tempHitPoints.Points.Increased -= UpdateTempHealthBar;
            tempHitPoints.Points.Decreased -= UpdateTempHealthBar;
            hitPoints.Points.Increased -= UpdateHealthBar;
            hitPoints.Points.Decreased -= UpdateHealthBar;
        }

        private void UpdateTempHealthBar(Points sender, ValueChangeEvent<uint> e)
        {
            UpdateFill(tempHealthBar, sender);
        }

        private void UpdateHealthBar(Points sender, ValueChangeEvent<uint> e)
        {
            UpdateFill(healthBar, sender);
        }

        private static void UpdateFill(Image image, Points points)
        {
            image.fillAmount = points.AsPercent();
        }
    }
}
