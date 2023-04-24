using SaloonSlingers.Unity.CardEntities;

using TMPro;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity
{
    public class HandPeerer : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private TextMeshProUGUI handValueText;

        private GameRulesManager gameRulesManager;
        private HandProjectile projectile;
        private bool isPeering = false;
        private Transform interactorTransform;

        public void OnHoverEnter(HoverEnterEventArgs args)
        {
            isPeering = true;
            canvas.enabled = true;
            interactorTransform = args.interactorObject.transform;
        }

        public void OnHoverExit(HoverExitEventArgs _)
        {
            canvas.enabled = false;
            isPeering = false;
        }

        private void Start()
        {
            gameRulesManager = GameObject.FindGameObjectWithTag("GameRulesManager").GetComponent<GameRulesManager>();
            projectile = transform.parent.GetComponent<HandProjectile>();
        }

        private void Update()
        {
            if (isPeering && interactorTransform)
            {
                var value = gameRulesManager.GameRules.HandEvaluator.Evaluate(projectile.Cards);
                handValueText.text = value.DisplayName();
                canvas.transform.LookAt(interactorTransform);
            }
        }
    }
}
