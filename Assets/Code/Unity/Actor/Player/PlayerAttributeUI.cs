using System.Collections;

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

        private UIData uiData;

        private void Awake()
        {
            var attributes = LevelManager.Instance.Player.GetComponent<Attributes>();
            hitPoints = attributes.Registry[AttributeType.Health];
            money = attributes.Registry[AttributeType.Money];
            deltaDelay = new(moneyDeltaPresentationTime);

            uiData = new()
            {
                HealthBar = healthBar,
                HealthPercentText = healthPercentText,
                AudioSource = audioSource,
                MoneyLostSFX = moneyLostSFX,
                MoneyGainedSFX = moneyGainedSFX,
                MoneyText = moneyText
            };
            PlayerAttributeUICoordinator.UpdateHealthBar(uiData, hitPoints);
            moneyText.text = money.Value.ToString();

            hitPoints.Increased += OnHealthChanged;
            hitPoints.Decreased += OnHealthChanged;
            money.Increased += OnMoneyChanged;
            money.Decreased += OnMoneyChanged;

            InvokeRepeating(nameof(f), 0, 2f);
        }

        private void f()
        {
            if (x) money.Increase(10);
            else money.Decrease(10);
            x = !x;
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
            var coroutine = PlayerAttributeUICoordinator.MoneyChangeEffect(uiData, e, deltaDelay);
            StartCoroutine(coroutine);
        }

        private void OnHealthChanged(IReadOnlyPoints sender, ValueChangeEvent<uint> e)
        {
            PlayerAttributeUICoordinator.UpdateHealthBar(uiData, sender);
        }
    }

    public static class PlayerAttributeUICoordinator
    {
        public static IEnumerator MoneyChangeEffect(UIData data,
                                                    ValueChangeEvent<uint> moneyChangedEvent,
                                                    WaitForSeconds deltaDelay)
        {
            bool decreased = moneyChangedEvent.Before > moneyChangedEvent.After;
            uint delta = decreased ? moneyChangedEvent.Before - moneyChangedEvent.After : moneyChangedEvent.After - moneyChangedEvent.Before;
            char deltaChar = decreased ? '-' : '+';
            if (decreased) data.AudioSource.PlayOneShot(data.MoneyLostSFX);
            else data.AudioSource.PlayOneShot(data.MoneyGainedSFX);

            data.MoneyText.text = $"{deltaChar}{delta}";
            yield return deltaDelay;

            data.MoneyText.text = moneyChangedEvent.After.ToString();
        }

        public static void UpdateHealthBar(UIData data, IReadOnlyPoints hitPoints)
        {
            data.HealthBar.fillAmount = hitPoints.AsPercent();
            data.HealthPercentText.text = hitPoints.AsPercent().ToString("P0");
        }
    }

    public struct UIData
    {
        public Image HealthBar;
        public TextMeshProUGUI HealthPercentText;
        public AudioSource AudioSource;
        public AudioClip MoneyLostSFX;
        public AudioClip MoneyGainedSFX;
        public TextMeshProUGUI MoneyText;
    }
}
