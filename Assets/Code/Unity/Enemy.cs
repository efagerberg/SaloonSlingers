using UnityEngine;

using SaloonSlingers.Core;
using System;

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

        private GameObject player;
        private CharacterController controller;

        public Card GetCard() => card;
        public void SetCard(Card inCard)
        {
            card = inCard;
            CardGraphicsHelper.SetFaceTexture(card, faceRenderer);
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (transform.position == player.transform.position) gameObject.SetActive(false);
            transform.LookAt(new Vector3(player.transform.position.x, 1, player.transform.position.z));
            controller.Move(moveSpeed * Time.deltaTime * transform.forward);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag("Player"))
            {
                Player p = hit.gameObject.GetComponent<Player>();
                p.TakeDamage(1);
                Destroy(gameObject);
            }
            if (hit.gameObject.CompareTag("CardInteractable"))
            {
                CardInteractable c = hit.gameObject.GetComponent<CardInteractable>();
                Card inCard = c.GetCard();
                int remainingValue = Mathf.Max(card.Value - inCard.Value, 0);
                if (remainingValue == 0)
                    Destroy(gameObject);
                else
                {
                    Card newCard = new Card(card.Suit, (Values)remainingValue);
                    SetCard(newCard);
                }
                c.DeactivateCard();
            }
        }
    }
}
