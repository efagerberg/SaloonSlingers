using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity.Actor
{
    public class EnemyInfoDisplay : HiddableDisplay
    {
        [SerializeField]
        private GameObject enemyPeerCardPrefab;
        [SerializeField]
        private LayoutGroup peerOtherCardLayoutGroup;
        [SerializeField]
        private TextMeshProUGUI handValueText;
        [SerializeField]
        private Color cursedCardColor;
        [SerializeField]
        private LayoutGroup cursedCardLayoutGroup;
        [SerializeField]
        private Image healthBar;

        private Canvas canvas;
        private IReadOnlyCollection<Card> cursedCards;
        private CardHand hand;
        private Attribute health;

        public void SetTarget(EnemyData enemyData, CardHand enemyHand)
        {
            if (hand != null)
                hand.Drawn.RemoveListener(DrawHandler);

            hand = enemyHand;

            if (hand != null)
                hand.Drawn.AddListener(DrawHandler);
            cursedCards = enemyData.CurseTarget.Cards;
            health = enemyData.Attributes.Registry[AttributeType.Health];
            UpdateContents();
        }

        public override void Hide()
        {
            base.Hide();
            canvas.enabled = false;
            handValueText.text = "";
        }

        public override void Show()
        {
            canvas.enabled = true;
            base.Show();
        }

        public override void UpdateContents()
        {
            var handToLayout = new (IReadOnlyCollection<Card>, LayoutGroup, Color, bool)[]
            {
                (hand != null ? hand.Cards : null, peerOtherCardLayoutGroup, Color.yellow, true),
                (cursedCards, cursedCardLayoutGroup, cursedCardColor, false)
            };

            if (health != null)
                healthBar.fillAmount = health.AsPercent();

            var evaluation = hand?.Evaluation;
            handValueText.text = evaluation?.DisplayName();

            foreach (var (h, l, c, keyCardsOnly) in handToLayout)
            {
                int nCards = h == null ? 0 : h.Count;
                int delta = nCards - l.transform.childCount;

                for (; delta > 0; delta--)
                    Instantiate(enemyPeerCardPrefab, l.transform, false);

                for (; delta < 0; delta++)
                    l.transform.GetChild(nCards - delta - 1).gameObject.SetActive(false);

                for (int i = 0; i < nCards; i++)
                {
                    Transform element = l.transform.GetChild(i);

                    TextMeshProUGUI text = element.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = h.ElementAt(i).ToUnicode();

                    Image background = element.GetComponentInChildren<Image>();
                    Color color;
                    if (keyCardsOnly && evaluation.HasValue)
                        color = evaluation.Value.KeyIndexes.Contains(i) ? c : Color.white;
                    else
                        color = c;
                    background.color = color;

                    element.gameObject.SetActive(true);
                }
            }
        }

        private void Awake()
        {
            canvas = GetComponentInChildren<Canvas>();
        }

        private void OnEnable()
        {
            if (hand == null) return;

            hand.Drawn.AddListener(DrawHandler);
        }

        private void OnDisable()
        {
            if (hand == null) return;

            hand.Drawn.RemoveListener(DrawHandler);
        }

        private void Start()
        {
            Hide();
        }

        private void DrawHandler(CardHand sender, ICardGraphic drawn)
        {
            UpdateContents();
        }
    }
}
