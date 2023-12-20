using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;


namespace SaloonSlingers.Unity.Actor
{
    public class PeerController : MonoBehaviour
    {
        [SerializeField]
        private List<InputActionProperty> peerActionProperties;
        [SerializeField]
        private Peerable peerable;
        [SerializeField]
        private VisibilityDetector visibilityDetector;
        [SerializeField]
        private ActorHandedness handedness;

        private void OnEnable()
        {
            foreach (var property in peerActionProperties)
                property.action.performed += HandlePeer;
        }

        private void OnDisable()
        {
            foreach (var property in peerActionProperties)
                property.action.performed -= HandlePeer;
        }

        private void HandlePeer(InputAction.CallbackContext _)
        {
            if (peerable == null) peerable = GetComponent<Peerable>();
            peerable.CastPeer(visibilityDetector, handedness.EnemyPeerDisplay);
        }

        private void Awake()
        {
            visibilityDetector = GetComponent<VisibilityDetector>();
        }
    }
}
