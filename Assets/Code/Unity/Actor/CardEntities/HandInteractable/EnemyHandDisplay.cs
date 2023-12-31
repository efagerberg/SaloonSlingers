using SaloonSlingers.Core;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity.Actor
{
    public class EnemyHandDisplay : HandDisplay
    {
        [SerializeField]
        private GameObject enemyPeerPanel;
        [SerializeField]
        private GameObject enemyPeerCardPrefab;
        [SerializeField]
        private LayoutGroup peerOtherCardLayoutGroup;
        [SerializeField]
        private TextMeshProUGUI handValueText;

        public void SetProjectile(HandProjectile projectile)
        {
            this.projectile = projectile;
        }

        public override void Hide()
        {
            base.Hide();
            enemyPeerPanel.SetActive(false);
            handValueText.text = "";
            projectile = null;
        }

        public override void Show()
        {
            enemyPeerPanel.SetActive(true);
            base.Show();
        }

        protected override void UpdateContents(HandEvaluation evaluation)
        {
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

        private void Start()
        {
            Hide();
        }
    }
}
