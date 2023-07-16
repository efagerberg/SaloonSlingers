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
        [SerializeField]
        private XRBaseInteractable mainInteractable;
        [SerializeField]
        private XRBaseInteractable peerInteractable;

        private HandProjectile handProjectile;
        private DeckGraphic deckGraphic;
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
                handProjectile.AssignDeck(deckGraphic.Deck);
                handProjectile.gameObject.layer = LayerMask.NameToLayer("Player");
            }
            peerInteractable.enabled = true;
            commitHandActionProperties.ForEach(prop => prop.action.started += OnToggleCommit);
            handProjectile.Pickup(deckGraphic.Spawn);
        }

        public void OnSelectExit()
        {
            handProjectile.Throw();
            commitHandActionProperties.ForEach(prop => prop.action.started -= OnToggleCommit);
            peerInteractable.enabled = false;
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
        }

        private void OnEnable()
        {
            mainInteractable.enabled = true;
            peerInteractable.enabled = false;
        }

        private void OnDisable()
        {
            mainInteractable.enabled = false;
            peerInteractable.enabled = false;
        }

        private void OnToggleCommit(InputAction.CallbackContext _)
        {
            handProjectile.ToggleCommitHand();
        }
    }
}