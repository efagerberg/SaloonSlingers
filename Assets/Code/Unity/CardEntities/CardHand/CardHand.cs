using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;
using SaloonSlingers.Unity.Slingers;
using System.Linq;

namespace SaloonSlingers.Unity.CardEntities
{
    public delegate void HandInteractableHeld(CardHand sender, EventArgs e);
    public delegate void HandInteractableReadyToRespawn(CardHand sender, EventArgs e);

    public class CardHand : MonoBehaviour
    {
        public event HandInteractableHeld OnHandInteractableHeld;
        public event HandInteractableReadyToRespawn OnHandInterableReadyToRespawn;

        [SerializeField]
        private RectTransform handPanelRectTransform;
        [SerializeField]
        private RectTransform handCanvasRectTransform;
        [SerializeField]
        private float totalCardDegrees = 30f;
        [SerializeField]
        private List<InputActionProperty> commitHandActionProperties;
        [SerializeField]
        private float lifespanInSeconds = 1f;
        [SerializeField]
        private int maxAngularVelocity = 100;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip drawSFX;
        [SerializeField]
        private AudioClip throwSFX;
        [SerializeField]
        private float maxDeckDistance = 0.08f;

        private TrailRenderer trailRenderer;
        private Rigidbody rigidBody;
        private CardHandState state;
        private CardHandLayoutMediator handLayoutMediator;
        private Func<int, IEnumerable<float>> cardRotationCalculator;
        private ISlingerAttributes slingerAttributes;
        private ICardSpawner cardSpawner;
        private GameRulesManager gameRulesManager;
        private float originlLifespanInSeconds;
        private Transform deckGraphicTransform;
        private IList<Card> cards;

        public void AssociateWithSlinger(ISlingerAttributes attributes)
        {
            slingerAttributes = attributes;
        }

        public void Pickup()
        {
            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            commitHandActionProperties.ForEach(prop => prop.action.started += ToggleCommitHand);
            state = state.Reset();
            lifespanInSeconds = originlLifespanInSeconds;
            if (slingerAttributes.Hand.Count == 0) TryDrawCard();
            OnHandInteractableHeld?.Invoke(this, EventArgs.Empty);
        }

        public void TryDrawCard()
        {
            if (!state.CanDraw) return;

            Card card = slingerAttributes.Deck.Dequeue();
            slingerAttributes.Hand.Add(card);
            audioSource.clip = drawSFX;
            audioSource.Play();
            ICardGraphic cardGraphic = cardSpawner.Spawn();
            cardGraphic.Card = card;
            handLayoutMediator.AddCardToLayout(cardGraphic, cardRotationCalculator);
        }

        public void Throw(Rigidbody characterControllerRb)
        {
            trailRenderer.enabled = true;
            rigidBody.isKinematic = false;
            commitHandActionProperties.ForEach(prop => prop.action.started -= ToggleCommitHand);
            state = state.Throw();
            audioSource.clip = throwSFX;
            audioSource.Play();
            lifespanInSeconds = originlLifespanInSeconds;
            NegateCharacterControllerVelocity(characterControllerRb);
            cards = slingerAttributes.Hand;
            slingerAttributes.Hand.Clear();
        }

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            SwapHandIfDifferentSlinger(args.interactorObject.transform);
            Pickup();
        }

        private void SwapHandIfDifferentSlinger(Transform slingerTransform)
        {
            ISlingerAttributes newAttributes = slingerTransform.GetComponentInParent<ISlinger>().Attributes;
            if (slingerAttributes == null)
            {
                AssociateWithSlinger(newAttributes);
                return;
            }
    
            if (newAttributes != slingerAttributes)
            {
                IList<Card> tmpHand = slingerAttributes.Hand.ToList();
                slingerAttributes.Hand.Clear();
                AssociateWithSlinger(newAttributes);
                slingerAttributes.Hand = tmpHand;
            }
        }

        public void OnSelectExit(SelectExitEventArgs args)
        {
            Rigidbody characterControllerRb = args.interactorObject.transform.GetComponentInParent<Rigidbody>();
            Throw(characterControllerRb);
        }

        private void OnEnable()
        {
            cardRotationCalculator = (n) => HandRotationCalculator.CalculateRotations(n, totalCardDegrees);
            state = new(
                () => lifespanInSeconds > 0 && rigidBody.velocity.magnitude != 0,
                () => slingerAttributes.Hand.Count < gameRulesManager.GameRules.MaxHandSize &&
                      slingerAttributes.Deck.Count > 0 &&
                      IsTouchingDeck()
            );
        }

        private bool IsTouchingDeck()
        {
            float dist = Mathf.Abs(Vector3.Distance(transform.position, deckGraphicTransform.transform.position));
            return dist <= maxDeckDistance;
        }

        private void Start()
        {
            gameRulesManager = GameObject.FindGameObjectWithTag("GameRulesManager").GetComponent<GameRulesManager>();
            deckGraphicTransform = GameObject.FindGameObjectWithTag("DeckGraphic").transform;
            cardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<ICardSpawner>();
            trailRenderer = GetComponent<TrailRenderer>();
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.maxAngularVelocity = maxAngularVelocity;
            if (handCanvasRectTransform == null) handCanvasRectTransform = handPanelRectTransform.parent.GetComponent<RectTransform>();

            handLayoutMediator = new(handPanelRectTransform, handCanvasRectTransform);
            originlLifespanInSeconds = lifespanInSeconds;
        }

        private void ToggleCommitHand(InputAction.CallbackContext _)
        {
            state = state.ToggleCommit();
            handLayoutMediator.ApplyLayout(state.IsCommitted, cardRotationCalculator);
        }

        private void NegateCharacterControllerVelocity(Rigidbody characterControllerRb)
        {
            if (characterControllerRb == null) return;
            rigidBody.AddForce(-characterControllerRb.velocity);
        }

        private void FixedUpdate()
        {
            if (state.IsThrown) lifespanInSeconds -= Time.fixedDeltaTime;
            if (state.IsAlive) return;

            trailRenderer.enabled = false;
            rigidBody.isKinematic = true;
            state = state.Reset();
            slingerAttributes.Hand.Clear();
            cards.Clear();
            handLayoutMediator.ApplyLayout(state.IsCommitted, cardRotationCalculator);
            handLayoutMediator.Dispose(cardSpawner.Despawn);
            lifespanInSeconds = originlLifespanInSeconds;
            OnHandInterableReadyToRespawn?.Invoke(this, EventArgs.Empty);
        }
    }
}