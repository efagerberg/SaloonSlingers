using SaloonSlingers.Core;

using TMPro;

using UnityEngine;


namespace SaloonSlingers.Unity.Actor
{
    public class PlayerHandDisplay : HandDisplay
    {
        [SerializeField]
        private GameObject cardsPanel;
        [SerializeField]
        private GameObject handValuePanel;

        public override void Hide()
        {
            base.Hide();
            for (int i = 0; i < projectile.Cards.Count; i++)
            {
                Transform element = cardsPanel.transform.GetChild(i);
                CardGraphic graphic = element.GetComponent<CardGraphic>();
                graphic.FaceMaterial.color = Color.white;
            }
            handValuePanel.SetActive(false);
        }

        protected override void UpdateContents(HandEvaluation evaluation)
        {
            TextMeshProUGUI handValueText = handValuePanel.GetComponentInChildren<TextMeshProUGUI>();
            handValueText.text = evaluation.DisplayName();
            for (int i = 0; i < projectile.Cards.Count; i++)
            {
                Transform element = cardsPanel.transform.GetChild(i);
                Color color = evaluation.KeyIndexes.Contains(i) ? Color.yellow : Color.white;
                CardGraphic graphic = element.GetComponent<CardGraphic>();
                graphic.FaceMaterial.color = color;
            }
            handValuePanel.SetActive(true);
        }

        private void Start() => projectile = transform.parent.GetComponent<HandProjectile>();
    }
}
