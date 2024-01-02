using SaloonSlingers.Core;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity
{
    public class PlayerAttributeUI : MonoBehaviour
    {
        [SerializeField]
        private Image healthBar;
        [SerializeField]
        private TextMeshProUGUI healthPercentText;
        [SerializeField]
        private TextMeshProUGUI moneyText;

        private Points hitPoints;
        private Points money;

        private void Awake()
        {
            var attributes = LevelManager.Instance.Player.GetComponent<Attributes>();
            hitPoints = attributes.Registry[AttributeType.Health];
            hitPoints.Increased += UpdateHealthBar;
            hitPoints.Decreased += UpdateHealthBar;
            UpdateFill(healthBar, hitPoints);
            healthPercentText.text = hitPoints.AsPercent().ToString("P0");

            money = attributes.Registry[AttributeType.Money];
            money.Increased += UpdateMoneyText;
            money.Decreased += UpdateMoneyText;
            moneyText.text = money.Value.ToString();
        }

        private void OnEnable()
        {
            hitPoints.Increased += UpdateHealthBar;
            hitPoints.Decreased += UpdateHealthBar;
            money.Increased += UpdateMoneyText;
            money.Decreased += UpdateMoneyText;
        }

        private void OnDisable()
        {
            hitPoints.Increased -= UpdateHealthBar;
            hitPoints.Decreased -= UpdateHealthBar;
            money.Increased -= UpdateMoneyText;
            money.Decreased -= UpdateMoneyText;
        }

        private void UpdateMoneyText(IReadOnlyPoints sender, ValueChangeEvent<uint> e)
        {
            moneyText.text = e.After.ToString();
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
