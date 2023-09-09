using System;
using System.Collections.Generic;
using System.Linq;

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
    private Homable homable;
    private int? slingerId;

    private Rigidbody rb;
    private HomingStrengthCalculator homingStrengthCalculator;
    private CharacterControllerThrowOffsetCalculator throwOffsetCalculator;
    private VisibilityDetector visibilityDetector;

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        homable.enabled = true;
        XROrigin origin = args.interactorObject.transform.GetComponentInParent<XROrigin>();
        int newSlingerId = origin.transform.GetInstanceID();
        bool sameSlinger = newSlingerId == slingerId;
        if (slingerId == null || !sameSlinger)
        {
            slingerId = newSlingerId;
            ActorHandedness handedness = origin.GetComponent<ActorHandedness>();
            homingStrengthCalculator = origin.GetComponent<HomingStrengthCalculator>();
            throwOffsetCalculator = origin.GetComponent<CharacterControllerThrowOffsetCalculator>();
            visibilityDetector = origin.GetComponent<VisibilityDetector>();
            deckGraphic = handedness.DeckGraphic;
            handProjectile.AssignDeck(deckGraphic.Deck);
        }
        handProjectile.transform.SetParent(args.interactorObject.transform);
        peerInteractable.enabled = true;
        commitHandActionProperties.ForEach(prop => prop.action.started += OnToggleCommit);
        handProjectile.Pickup(deckGraphic.Spawn);
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        handProjectile.transform.parent = null;

        Vector3 offset = throwOffsetCalculator.Calculate((XRGrabInteractable)args.interactableObject);
        handProjectile.Throw(offset);
        commitHandActionProperties.ForEach(prop => prop.action.started -= OnToggleCommit);
        peerInteractable.enabled = false;
        handProjectile.Death += OnHandProjectileDied;

        var target = visibilityDetector.GetVisible(LayerMask.GetMask("Enemy"))
                                        .FirstOrDefault();
        if (target != null) homable.Target = target;

        handProjectile.gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
        homingStrengthCalculator.StartNewThrow();
    }


    private void OnHandProjectileDied(object sender, EventArgs e)
    {
        var instance = sender as GameObject;
        var actor = instance.GetComponent<IActor>();
        actor.Death -= OnHandProjectileDied;
        homable.Target = null;
        homable.Strength = 0;
        homable.enabled = false;
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
        rb = GetComponent<Rigidbody>();
        homable = GetComponent<Homable>();
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

    private void FixedUpdate()
    {
        if (homable.Target && homingStrengthCalculator != null)
            homable.Strength = homingStrengthCalculator.Calculate(rb.angularVelocity.y);

        if (throwOffsetCalculator) throwOffsetCalculator.RecordVelocity();
    }
}
}