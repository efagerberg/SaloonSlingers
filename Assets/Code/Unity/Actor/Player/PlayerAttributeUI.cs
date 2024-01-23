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
        private TextMeshProUGUI potText;
        [SerializeField]
        private AudioClip betPlacedSFX;
        [SerializeField]
        private AudioClip moneyGainedSFX;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private float deltaPresentationTime;

        private Attribute hitPoints;
        private Attribute money;
        private Attribute pot;
        private WaitForSeconds deltaDelay;
        private Coroutine moneyChangedCoroutine;
        private Coroutine potIncreasedCoroutine;

        private void Awake()
        {
            var attributes = LevelManager.Instance.Player.GetComponent<Attributes>();
            hitPoints = attributes.Registry[AttributeType.Health];
            money = attributes.Registry[AttributeType.Money];
            pot = attributes.Registry[AttributeType.Pot];
            deltaDelay = new(deltaPresentationTime);

            UpdateHealthBar(hitPoints);
            moneyText.text = PlayerAttributeUIDataGenerator.GetMoneyUIData(money);

            hitPoints.Increased += OnHealthChanged;
            hitPoints.Decreased += OnHealthChanged;
            money.Increased += OnMoneyIncreased;
            money.Decreased += OnMoneyDecreased;
            pot.Increased += OnPotIncreased;
        }

        private void OnEnable()
        {
            hitPoints.Increased += OnHealthChanged;
            hitPoints.Decreased += OnHealthChanged;
            money.Increased += OnMoneyIncreased;
            money.Decreased += OnMoneyDecreased;
            pot.Increased += OnPotIncreased;
        }

        private void OnDisable()
        {
            hitPoints.Increased -= OnHealthChanged;
            hitPoints.Decreased -= OnHealthChanged;
            money.Increased -= OnMoneyIncreased;
            money.Decreased -= OnMoneyDecreased;
            pot.Increased -= OnPotIncreased;
        }

        private void OnMoneyIncreased(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
        {
            ChangeEffect(e, moneyText, ref moneyChangedCoroutine, moneyGainedSFX);
        }

        private void OnMoneyDecreased(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
        {
            moneyText.text = e.After.ToString();
        }

        private void OnPotIncreased(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
        {
            ChangeEffect(e, potText, ref potIncreasedCoroutine, betPlacedSFX);
        }

        private void ChangeEffect(ValueChangeEvent<uint> e, TextMeshProUGUI textElement, ref Coroutine coroutine, AudioClip clipToPlay)
        {
            if (coroutine != null) StopCoroutine(coroutine);

            coroutine = StartCoroutine(DoChangeEffect(e, textElement, clipToPlay));
        }

        private IEnumerator DoChangeEffect(ValueChangeEvent<uint> e, TextMeshProUGUI textElement, AudioClip clipToPlay)
        {
            var data = PlayerAttributeUIDataGenerator.GetMoneyChangedUIData(e);
            audioSource.PlayOneShot(clipToPlay);
            textElement.text = data.DeltaText;
            yield return deltaDelay;
            textElement.text = data.TotalText;
        }

        private void OnHealthChanged(IReadOnlyAttribute sender, ValueChangeEvent<uint> e)
        {
            UpdateHealthBar(sender);
        }

        private void UpdateHealthBar(IReadOnlyAttribute hitPoints)
        {
            var healthUIData = PlayerAttributeUIDataGenerator.GetHealthUIData(hitPoints);
            healthBar.fillAmount = healthUIData.FillAmount;
            healthPercentText.text = healthUIData.HealthPercentText;
        }
    }
}
