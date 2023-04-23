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

        public void OnHoverEnter(HoverEnterEventArgs args)
        {
            Debug.Log("Seen");
            var value = gameRulesManager.GameRules.HandEvaluator.Evaluate(projectile.Cards);
            handValueText.text = value.DisplayName();
            canvas.transform.LookAt(args.interactorObject.transform);
            canvas.enabled = true;
        }

        public void OnHoverExit(HoverExitEventArgs args)
        {
            Debug.Log("Unseen");
            canvas.enabled = false;
        }

        private void Start()
        {
            gameRulesManager = GameObject.FindGameObjectWithTag("GameRulesManager").GetComponent<GameRulesManager>();
            projectile = transform.parent.GetComponent<HandProjectile>();
        }
    }
}
