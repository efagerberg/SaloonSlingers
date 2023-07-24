using SaloonSlingers.Core;
using SaloonSlingers.Unity.CardEntities;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity
{
    public class EnemyHandDisplay : HandDisplay
    {
        [SerializeField]
        private GameObject enemyPeerPanel;
        [SerializeField]
        private GameObject enemyPeerCardPrefab;

        private TextMeshProUGUI handValueText;
        private LayoutGroup peerOtherCardLayoutGroup;

        public void SetProjectile(HandProjectile projectile)
        {
            this.projectile = projectile;
        }

        public override void Hide()
        {
            base.Hide();
            enemyPeerPanel.SetActive(false);
            projectile = null;
        }

        public override void Show()
        {
            base.Show();
            enemyPeerPanel.SetActive(true);
            if (projectile == null)
            {
                for (int i = 0; i < peerOtherCardLayoutGroup.transform.childCount; i++)
                    peerOtherCardLayoutGroup.transform.GetChild(i).gameObject.SetActive(false);
                handValueText.text = "";
            }
        }

        protected override void UpdateContents(HandEvaluation evaluation)
        {
            handValueText.text = evaluation.DisplayName();

            LayoutGroup peerOtherCardLayoutGroup = enemyPeerPanel.GetComponentInChildren<LayoutGroup>();

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
            handValueText = enemyPeerPanel.GetComponentInImmediateChildren<TextMeshProUGUI>();
            peerOtherCardLayoutGroup = enemyPeerPanel.GetComponentInChildren<LayoutGroup>();
        }
    }
}
