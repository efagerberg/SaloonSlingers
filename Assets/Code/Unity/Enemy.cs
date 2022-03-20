using UnityEngine;
using Unity.XR.CoreUtils;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity
{
    public class Enemy : MonoBehaviour, ISlinger
    {
        public ISlingerAttributes Attributes { get; set; }
        public Card Card
        {
            get => card;
            set
            {
                card = value;
                CardGraphicsHelper.SetFaceTexture(card, faceRenderer);
            }
        }

        [SerializeField]
        private float moveSpeed = 1;
        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;
        private Transform playerCameraTransform;

        private void Start()
        {
            playerCameraTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<XROrigin>().Camera.transform;
        }

        private void Update()
        {
            if (transform.position == playerCameraTransform.transform.position)
                gameObject.SetActive(false);
            transform.LookAt(
                new Vector3(playerCameraTransform.position.x, 1, playerCameraTransform.position.z)
            );
            transform.position += moveSpeed * Time.deltaTime * transform.forward;
        }
    }
}
