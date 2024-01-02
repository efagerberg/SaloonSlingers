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
            this.projectile = projectile;
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

        protected override void UpdateContents(HandEvaluation evaluation)
        {
            var hasValueToDisplay = evaluation.Name != HandNames.NONE;
            canvas.enabled = hasValueToDisplay;
            if (!hasValueToDisplay) return;

            handValueText.text = evaluation.DisplayName();

            int nCards = projectile.Cards.Count;
            int delta = nCards - peerOtherCardLayoutGroup.transform.childCount;

            for (; delta > 0; delta--)
                Instantiate(enemyPeerCardPrefab, peerOtherCardLayoutGroup.transform, false);

            for (; delta < 0; delta++)
                peerOtherCardLayoutGroup.transform.GetChild(nCards - delta - 1).gameObject.SetActive(false);

            for (int i = 0; i < nCards; i++)
            {
                Transform element = peerOtherCardLayoutGroup.transform.GetChild(i);

                TextMeshProUGUI text = element.GetComponentInChildren<TextMeshProUGUI>();
                text.text = projectile.Cards[i].ToUnicode();

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

        private void Start()
        {
            Hide();
        }
    }
}
