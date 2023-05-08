using System.Collections.Generic;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.CardEntities
{
    public class PlayerHandInteractableController : MonoBehaviour
    {
        [SerializeField]
        private float maxDeckDistance = 0.08f;
        [SerializeField]
        private List<InputActionProperty> commitHandActionProperties;

        private HandProjectile handProjectile;
        private XRBaseInteractable interactable;
        private DeckGraphic deckGraphic;
        private ControllerSwapper swapper;
        private int? slingerId;

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            XROrigin origin = args.interactorObject.transform.GetComponentInParent<XROrigin>();
            int newSlingerId = origin.transform.GetInstanceID();
            if (slingerId == null || slingerId != newSlingerId)
            {
                slingerId = newSlingerId;
                SlingerHandedness handedness = origin.GetComponent<SlingerHandedness>();
                deckGraphic = handedness.DeckGraphic;
                swapper.SetController(ControllerTypes.PLAYER);
                handProjectile.AssignDeck(deckGraphic.Deck);
            }
            commitHandActionProperties.ForEach(prop => prop.action.started += OnToggleCommit);
            handProjectile.Pickup(deckGraphic.Spawn);
        }

        public void OnSelectExit()
        {
            handProjectile.Throw();
            commitHandActionProperties.ForEach(prop => prop.action.started -= OnToggleCommit);
        }

        public void OnActivate()
        {
            if (!deckGraphic.CanDraw || !IsTouchingDeck()) return;
            handProjectile.TryDrawCard(deckGraphic.Spawn);
        }

        private bool IsTouchingDeck()
        {
            float dist = Mathf.Abs(Vector3.Distance(transform.position, deckGraphic.TopCardTransform.position));
            return dist <= maxDeckDistance;
        }

        private void Awake()
        {
            handProjectile = GetComponent<HandProjectile>();
            swapper = GetComponent<ControllerSwapper>();
            interactable = GetComponent<XRBaseInteractable>();
        }

        private void OnEnable()
        {
            interactable.enabled = true;
        }

        private void OnDisable()
        {
            interactable.enabled = false;
        }

        private void OnToggleCommit(InputAction.CallbackContext _)
        {
            handProjectile.ToggleCommitHand();
        }
    }
}