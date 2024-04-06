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

        private Canvas canvas;

        public void SetProjectile(HandProjectile projectile)
        {
            if (this.projectile != null)
                this.projectile.OnDraw.RemoveListener(DrawHandler);

            this.projectile = projectile;

            if (this.projectile != null)
                this.projectile.OnDraw.AddListener(DrawHandler);
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
            var evaluation = projectile == null ? HandEvaluation.EMPTY : projectile.HandEvaluation;
            handValueText.text = evaluation.DisplayName();

            int nCards = projectile == null ? 0 : projectile.Cards.Count;
            int delta = nCards - peerOtherCardLayoutGroup.transform.childCount;

            for (; delta > 0; delta--)
                Instantiate(enemyPeerCardPrefab, peerOtherCardLayoutGroup.transform, false);

            for (; delta < 0; delta++)
                peerOtherCardLayoutGroup.transform.GetChild(nCards - delta - 1).gameObject.SetActive(false);

            for (int i = 0; i < nCards; i++)
            {
                Transform element = peerOtherCardLayoutGroup.transform.GetChild(i);

                TextMeshProUGUI text = element.GetComponentInChildren<TextMeshProUGUI>();
                text.text = projectile.Cards.ElementAt(i).ToUnicode();

                Image background = element.GetComponentInChildren<Image>();
                Color color = evaluation.KeyIndexes.Contains(i) ? Color.yellow : Color.white;
                background.color = color;

                element.gameObject.SetActive(true);
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
