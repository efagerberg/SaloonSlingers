using UnityEngine;
using Unity.XR.CoreUtils;

using SaloonSlingers.Core.SlingerAttributes;

using SaloonSlingers.Unity.CardEntities;

namespace SaloonSlingers.Unity.Slingers
{
    public class Enemy : CardGraphic, ISlinger
    {
        public ISlingerAttributes Attributes { get; set; }

        [SerializeField]
        private float moveSpeed = 1;
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
