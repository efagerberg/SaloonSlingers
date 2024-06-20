using TMPro;

using UnityEngine;


namespace SaloonSlingers.Unity.Actor
{
    public class PlayerHandInfoDisplay : HiddableDisplay
    {
        [SerializeField]
        private GameObject cardsPanel;
        [SerializeField]
        private CanvasGroup handValueCanvasGroup;
        [SerializeField]
        private TextMeshProUGUI handValueText;
        [SerializeField]
        private float fadeDuration = 0.5f;
        [SerializeField]
        private Color keyCardColor;
        [SerializeField]
        private Color markCardColor;
        [SerializeField]
        private Color damageCardColor;

        private CardHand projectile;

        private Color NonKeyColor
        {
            get
            {
                return projectile.Mode switch
                {
                    HandProjectileMode.Damage => damageCardColor,
                    HandProjectileMode.Curse => markCardColor,
                    _ => throw new System.NotImplementedException(),
                };
            }
        }

        public override void Hide()
        {
            base.Hide();
            if (handValueText.text == "") return;

            for (int i = 0; projectile != null && i < projectile.Cards.Count; i++)
            {
                Transform element = cardsPanel.transform.GetChild(i);
                ICardGraphic graphic = element.GetComponent<ICardGraphic>();
                graphic.Color = NonKeyColor;
            }
            var coroutine = Fader.Fade((alpha) => handValueCanvasGroup.alpha = alpha, fadeDuration, endAlpha: 0);
            StartCoroutine(coroutine);
        }

        public override void Show()
        {
            base.Show();
            if (handValueText.text == "") return;

            var coroutine = Fader.Fade(
                (alpha) => handValueCanvasGroup.alpha = alpha,
                fadeDuration,
                startAlpha: handValueCanvasGroup.alpha, endAlpha: 1
            );
            StartCoroutine(coroutine);
        }

        public override void UpdateContents()
        {
            if (projectile == null) return;

            handValueText.text = projectile.HandEvaluation.DisplayName();
            for (int i = 0; i < projectile.Cards.Count; i++)
            {
                Transform element = cardsPanel.transform.GetChild(i);
                Color color;
                if (IsDisplaying && projectile.HandEvaluation.KeyIndexes.Contains(i))
                    color = keyCardColor;
                else
                    color = NonKeyColor;
                ICardGraphic graphic = element.GetComponent<ICardGraphic>();
                graphic.Color = color;
            }
        }

        private void Start()
        {
            projectile = transform.parent.GetComponent<CardHand>();
        }

        private void OnDisable()
        {
            // Sometimes the hand interactable can be released before the hide coroutine finishes
            handValueCanvasGroup.alpha = 0;
        }
    }
}
