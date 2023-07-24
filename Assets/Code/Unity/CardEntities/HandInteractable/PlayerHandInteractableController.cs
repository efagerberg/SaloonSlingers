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
            bool sameSlinger = newSlingerId == slingerId;
            if (slingerId == null || !sameSlinger)
            {
                slingerId = newSlingerId;
                SlingerHandedness handedness = origin.GetComponent<SlingerHandedness>();
                deckGraphic = handedness.DeckGraphic;
                handProjectile.AssignDeck(deckGraphic.Deck);
                handProjectile.gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
            }
            peerInteractable.enabled = true;
            commitHandActionProperties.ForEach(prop => prop.action.started += OnToggleCommit);
            handProjectile.Pickup(deckGraphic.Spawn);

            // If we try to set the parent when we are passing between our hands, the interactable
            // will oscillate between the two controller parents making a weird effect. So only
            // set parent when this is a new interactable.
            if (!sameSlinger) handProjectile.transform.SetParent(args.interactorObject.transform, false);
        }

        public void OnSelectExit()
        {
            handProjectile.Throw();
            handProjectile.transform.parent = null;
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