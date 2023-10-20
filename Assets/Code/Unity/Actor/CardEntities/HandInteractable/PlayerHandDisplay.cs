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
        private CanvasGroup handValueCanvasGroup;
        [SerializeField]
        private TextMeshProUGUI handValueText;
        [SerializeField]
        private float fadeDuration = 0.5f;

        public override void Hide()
        {
            base.Hide();
            for (int i = 0; i < projectile.Cards.Count; i++)
            {
                Transform element = cardsPanel.transform.GetChild(i);
                CardGraphic graphic = element.GetComponent<CardGraphic>();
                graphic.FaceMaterial.color = Color.white;
            }
            StartCoroutine(Fader.FadeTo(handValueCanvasGroup, 0, fadeDuration));
        }

        public override void Show()
        {
            base.Show();
            StartCoroutine(Fader.FadeTo(handValueCanvasGroup, 1, fadeDuration));
        }

        protected override void UpdateContents(HandEvaluation evaluation)
        {
            handValueText.text = evaluation.DisplayName();
            for (int i = 0; i < projectile.Cards.Count; i++)
            {
                Transform element = cardsPanel.transform.GetChild(i);
                Color color = evaluation.KeyIndexes.Contains(i) ? Color.yellow : Color.white;
                CardGraphic graphic = element.GetComponent<CardGraphic>();
                graphic.FaceMaterial.color = color;
            }
        }

        private void Start() => projectile = transform.parent.GetComponent<HandProjectile>();
    }
}
