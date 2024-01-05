using System.Collections;

using SaloonSlingers.Core;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity.Actor
{
    public class PlayerAttributeUI : MonoBehaviour
    {
        [SerializeField]
        private Image healthBar;
        [SerializeField]
        private TextMeshProUGUI healthPercentText;
        [SerializeField]
        private TextMeshProUGUI moneyText;
        [SerializeField]
        private AudioClip moneyLostSFX;
        [SerializeField]
        private AudioClip moneyGainedSFX;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private float moneyDeltaPresentationTime;

        private Points hitPoints;
        private Points money;
        private WaitForSeconds deltaDelay;
        private bool x = false;
        private Coroutine moneyUICoroutine;

        private void Awake()
        {
            var attributes = LevelManager.Instance.Player.GetComponent<Attributes>();
            hitPoints = attributes.Registry[AttributeType.Health];
            money = attributes.Registry[AttributeType.Money];
            deltaDelay = new(moneyDeltaPresentationTime);

            UpdateHealthBar(hitPoints);
            moneyText.text = PlayerAttributeUIDataGenerator.GetMoneyUIData(money);

            hitPoints.Increased += OnHealthChanged;
            hitPoints.Decreased += OnHealthChanged;
            money.Increased += OnMoneyChanged;
            money.Decreased += OnMoneyChanged;
        }

        private void OnEnable()
        {
            hitPoints.Increased += OnHealthChanged;
            hitPoints.Decreased += OnHealthChanged;
            money.Increased += OnMoneyChanged;
            money.Decreased += OnMoneyChanged;
        }

        private void OnDisable()
        {
            hitPoints.Increased -= OnHealthChanged;
            hitPoints.Decreased -= OnHealthChanged;
            money.Increased -= OnMoneyChanged;
            money.Decreased -= OnMoneyChanged;
        }

        private void OnMoneyChanged(IReadOnlyPoints sender, ValueChangeEvent<uint> e)
        {
            if (moneyUICoroutine != null) StopCoroutine(moneyUICoroutine);

            moneyUICoroutine = StartCoroutine(MoneyChangeEffect(e));
        }

        private IEnumerator MoneyChangeEffect(ValueChangeEvent<uint> e)
        {
            var data = PlayerAttributeUIDataGenerator.GetMoneyUIData(e, moneyLostSFX, moneyGainedSFX);
            audioSource.PlayOneShot((AudioClip)data.ClipToPlay);
            moneyText.text = data.MoneyDeltaText;
            yield return deltaDelay;
            moneyText.text = data.TotalText;
        }

        private void OnHealthChanged(IReadOnlyPoints sender, ValueChangeEvent<uint> e)
        {
            UpdateHealthBar(sender);
        }

        private void UpdateHealthBar(IReadOnlyPoints hitPoints)
        {
            var healthUIData = PlayerAttributeUIDataGenerator.GetHealthUIData(hitPoints);
            healthBar.fillAmount = healthUIData.FillAmount;
            healthPercentText.text = healthUIData.HealthPercentText;
        }
    }
}
