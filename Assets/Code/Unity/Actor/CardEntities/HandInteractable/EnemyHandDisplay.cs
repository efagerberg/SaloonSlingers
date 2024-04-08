using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity.Actor
{
    public class EnemyHandDisplay : HandDisplay
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

        private Canvas canvas;
        private IReadOnlyCollection<Card> cursedCards;

        public void SetProjectiles(HandProjectile enemyProjectile, IReadOnlyCollection<Card> cursedCards)
        {
            if (projectile != null)
                projectile.OnDraw.RemoveListener(DrawHandler);

            projectile = enemyProjectile;

            if (projectile != null)
                projectile.OnDraw.AddListener(DrawHandler);
            this.cursedCards = cursedCards;
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
            var projectileToLayout = new (IReadOnlyCollection<Card>, LayoutGroup, Color, bool)[]
            {
                (projectile?.Cards, peerOtherCardLayoutGroup, Color.yellow, true),
                (cursedCards, cursedCardLayoutGroup, cursedCardColor, false)
            };

            var evaluation = projectile == null ? HandEvaluation.EMPTY : projectile.HandEvaluation;
            handValueText.text = evaluation.DisplayName();

            foreach (var (h, l, c, keyCardsOnly) in projectileToLayout)
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
                    if (keyCardsOnly)
                        color = evaluation.KeyIndexes.Contains(i) ? c : Color.white;
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
            if (projectile == null) return;

            projectile.OnDraw.AddListener(DrawHandler);
        }

        private void OnDisable()
        {
            if (projectile == null) return;

            projectile.OnDraw.RemoveListener(DrawHandler);
        }

        private void Start()
        {
            Hide();
        }

        private void DrawHandler(GameObject sender, ICardGraphic drawn)
        {
            UpdateContents();
        }
    }
}
