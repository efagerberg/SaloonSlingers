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
        private VisibilityDetector visibilityDetector;
        private CharacterControllerAverageVelocity ccAverageVelocity;
        private bool initialized = false;
        private bool eventsRegistered = false;

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            if (args.interactorObject.GetType().IsAssignableFrom(typeof(XRSocketInteractor)))
                return;

            var player = LevelManager.Instance.Player;
            int newSlingerId = player.transform.GetInstanceID();
            if (!initialized) Initialize(player);

            handProjectile.Pickup(deckGraphic.Spawn, GameManager.Instance.Saloon.HouseGame);
            handProjectile.gameObject.layer = LayerMask.NameToLayer("PlayerInteractable");
            peerInteractable.enabled = true;
            homable.enabled = true;
            if (!eventsRegistered)
            {
                var actor = handProjectile.GetComponent<Actor>();
                actor.Killed.AddListener(OnHandProjectileDied);
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
            ccAverageVelocity = player.GetComponent<CharacterControllerAverageVelocity>();
            handProjectile = GetComponent<HandProjectile>();
            handProjectile.Assign(deckGraphic.Deck, attributes.Registry);
            rb = GetComponent<Rigidbody>();
            homable = GetComponent<Homable>();
        }

        public void OnSelectExit(SelectExitEventArgs args)
        {
            if (args.interactorObject.GetType().IsAssignableFrom(typeof(XRSocketInteractor))) return;

            var asGrabbable = (XRGrabInteractable)args.interactableObject;
            Vector3 offset = -ccAverageVelocity.AverageVelocity * asGrabbable.throwVelocityScale;
            handProjectile.Throw(offset);
            peerInteractable.enabled = false;

            var target = visibilityDetector.GetVisible(LayerMask.GetMask("Enemy"))
                                           .FirstOrDefault();

            homable.Target = target;
            homingStrength.Calculator.StartNewThrow();
        }

        private void OnHandProjectileDied(Actor sender)
        {
            homable.Target = null;
            homable.Strength = 0;
            homable.enabled = false;
            eventsRegistered = false;
            sender.Killed.RemoveListener(OnHandProjectileDied);
        }

        public void OnActivate()
        {
            if (deckGraphic.CanDraw && IsTouchingDeck())
            {
                handProjectile.TryDrawCard(deckGraphic.Spawn, GameManager.Instance.Saloon.HouseGame);
                return;
            }

            int enumCount = Enum.GetValues(typeof(HandProjectileMode)).Length;
            int nextEnumValue = ((int)handProjectile.Mode + 1) % enumCount;
            handProjectile.Mode = (HandProjectileMode)nextEnumValue;
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