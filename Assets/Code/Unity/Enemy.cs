using UnityEngine;
using Unity.XR.CoreUtils;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public class Enemy : MonoBehaviour
    {
        public EnemyAttributes attributes = new EnemyAttributes();
        [SerializeField]
        private float moveSpeed = 1;
        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;

        private Transform playerCameraTransform;
        public Card GetCard() => card;
        public void SetCard(Card inCard)
        {
            card = inCard;
            CardGraphicsHelper.SetFaceTexture(card, faceRenderer);
        }

        private void Start()
        {
            playerCameraTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<XROrigin>().Camera.transform;
        }

        private void Update()
        {
            if (transform.position == playerCameraTransform.transform.position) gameObject.SetActive(false);
            transform.LookAt(new Vector3(playerCameraTransform.position.x, 1, playerCameraTransform.position.z));
            transform.position += moveSpeed * Time.deltaTime * transform.forward;
        }
    }
}
