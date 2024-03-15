using System;
using System.Linq;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace SaloonSlingers.Unity.Actor
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Homable))]
    [RequireComponent(typeof(HandProjectile))]
    public class PlayerHandInteractableController : MonoBehaviour
    {
        [SerializeField]
        private float maxDeckDistance = 0.08f;
        [SerializeField]
        private XRBaseInteractable mainInteractable;
        [SerializeField]
        private XRBaseInteractable peerInteractable;

        private HandProjectile handProjectile;
        private DeckGraphic deckGraphic;
        private Homable homable;

        private Rigidbody rb;
        private HomingStrength homingStrength;
        private CharacterControllerThrowOffsetCalculator throwOffsetCalculator;
        private CharacterController characterController;
        private VisibilityDetector visibilityDetector;
        private bool initialized = false;
        private bool eventsRegistered = false;

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            if (args.interactorObject.GetType().IsAssignableFrom(typeof(XRSocketInteractor)))
                return;

            var player = LevelManager.Instance.Player;
            int newSlingerId = player.transform.GetInstanceID();
            if (!initialized) Initialize(player);

            handProjectile.Pickup(deckGraphic.Spawn);
            handProjectile.gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
            peerInteractable.enabled = true;
            homable.enabled = true;
            if (!eventsRegistered)
            {
                handProjectile.Killed += OnHandProjectileDied;
                eventsRegistered = true;
            }
        }

        /// <summary>
        /// Sets up the expensive to initialize state once for an object even if it is disabled and renabled (pooling).
        /// </summary>
        /// <param name="player"></param>
        private void Initialize(GameObject player)
        {
            initialized = true;
            ActorHandedness handedness = player.GetComponent<ActorHandedness>();
            homingStrength = player.GetComponent<HomingStrength>();
            throwOffsetCalculator = new(); ;
            characterController = player.GetComponent<CharacterController>();
            visibilityDetector = player.GetComponent<VisibilityDetector>();
            deckGraphic = handedness.DeckGraphic;
            var attributes = player.GetComponent<Attributes>();
            handProjectile = GetComponent<HandProjectile>();
            handProjectile.Assign(deckGraphic.Deck, attributes.Registry);
            rb = GetComponent<Rigidbody>();
            homable = GetComponent<Homable>();
        }

        public void OnSelectExit(SelectExitEventArgs args)
        {
            if (args.interactorObject.GetType().IsAssignableFrom(typeof(XRSocketInteractor))) return;

            var asGrabbable = (XRGrabInteractable)args.interactableObject;
            Vector3 offset = throwOffsetCalculator.Calculate(asGrabbable.throwVelocityScale);
            handProjectile.Throw(offset);
            peerInteractable.enabled = false;

            var target = visibilityDetector.GetVisible(LayerMask.GetMask("Enemy"))
                                           .FirstOrDefault();
            if (target != null) homable.Target = target;

            homingStrength.Calculator.StartNewThrow();
        }

        private void OnHandProjectileDied(object sender, EventArgs e)
        {
            homable.Target = null;
            homable.Strength = 0;
            homable.enabled = false;
            eventsRegistered = false;
            handProjectile.Killed -= OnHandProjectileDied;
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

        private void OnEnable()
        {
            mainInteractable.enabled = true;
            peerInteractable.enabled = false;

        }

        private void OnDisable()
        {
            mainInteractable.enabled = false;
            peerInteractable.enabled = false;
            initialized = false;
        }

        private void FixedUpdate()
        {
            if (homable != null && homable.Target && homingStrength != null)
                homable.Strength = homingStrength.Calculator.Calculate(rb.angularVelocity.y);

            if (throwOffsetCalculator != null) throwOffsetCalculator.RecordVelocity(characterController.velocity);
        }
    }
}