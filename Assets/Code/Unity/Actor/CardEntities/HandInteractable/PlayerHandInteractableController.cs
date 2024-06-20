using System;
using System.Linq;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace SaloonSlingers.Unity.Actor
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Homable))]
    [RequireComponent(typeof(CardHand))]
    public class PlayerHandInteractableController : MonoBehaviour
    {
        [SerializeField]
        private float maxDeckDistance = 0.08f;
        [SerializeField]
        private XRBaseInteractable mainInteractable;
        [SerializeField]
        private XRBaseInteractable peerInteractable;

        private CardHand hand;
        private CardHandStatusType collisionEffect;
        private Projectile projectile;
        private DeckGraphic deckGraphic;
        private Homable homable;

        private Rigidbody rb;
        private HomingStrength homingStrength;
        private VisibilityDetector visibilityDetector;
        private VelocityTracker ccAverageVelocity;
        private bool initialized = false;
        private bool eventsRegistered = false;

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            if (args.interactorObject.GetType().IsAssignableFrom(typeof(XRSocketInteractor)))
                return;

            var player = LevelManager.Instance.Player;
            int newSlingerId = player.transform.GetInstanceID();
            if (!initialized) Initialize(player);

            hand.Pickup(deckGraphic.Spawn, GameManager.Instance.Saloon.HouseGame);
            hand.gameObject.layer = LayerMask.NameToLayer("PlayerInteractable");
            peerInteractable.enabled = true;
            homable.enabled = true;
            if (!eventsRegistered)
            {
                var actor = hand.GetComponent<Actor>();
                actor.Killed.AddListener(OnActorDeath);
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
            visibilityDetector = player.GetComponent<VisibilityDetector>();
            deckGraphic = handedness.DeckGraphic;
            var attributes = player.GetComponent<Attributes>();
            ccAverageVelocity = player.GetComponent<VelocityTracker>();
            hand = GetComponent<CardHand>();
            hand.Assign(deckGraphic.Deck, attributes.Registry);
            projectile = hand.GetComponent<Projectile>();
            collisionEffect = hand.GetComponent<CardHandStatusType>();
            rb = GetComponent<Rigidbody>();
            homable = GetComponent<Homable>();
        }

        public void OnSelectExit(SelectExitEventArgs args)
        {
            if (args.interactorObject.GetType().IsAssignableFrom(typeof(XRSocketInteractor))) return;

            var asGrabbable = (XRGrabInteractable)args.interactableObject;
            Vector3 offset = -ccAverageVelocity.AverageVelocity * asGrabbable.throwVelocityScale;
            projectile.Throw(offset);
            peerInteractable.enabled = false;

            var target = visibilityDetector.GetVisible(LayerMask.GetMask("Enemy"))
                                           .FirstOrDefault();

            homable.Target = target;
            homingStrength.Calculator.StartNewThrow();
        }

        private void OnActorDeath(Actor sender)
        {
            homable.Target = null;
            homable.Strength = 0;
            homable.enabled = false;
            eventsRegistered = false;
            sender.Killed.RemoveListener(OnActorDeath);
        }

        public void OnActivate()
        {
            if (deckGraphic.CanDraw && IsTouchingDeck())
            {
                hand.TryDrawCard(deckGraphic.Spawn, GameManager.Instance.Saloon.HouseGame);
                return;
            }

            int enumCount = Enum.GetValues(typeof(StatusType)).Length;
            int nextEnumValue = ((int)collisionEffect.Current + 1) % enumCount;
            collisionEffect.Current = (StatusType)nextEnumValue;
        }

        private bool IsTouchingDeck()
        {
            float dist = Mathf.Abs(Vector3.Distance(transform.position, deckGraphic.Peek().position));
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
        }
    }
}
