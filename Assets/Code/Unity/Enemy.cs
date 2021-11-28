using UnityEngine;

using GambitSimulator.Core;
using System;

namespace GambitSimulator.Unity
{
    public class Enemy : MonoBehaviour
    {
        public EnemyAttributes attributes = new EnemyAttributes();
        [SerializeField]
        private float moveSpeed = 2;
        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;

        private GameObject player;

        public Card GetCard() => card;
        public void SetCard(Card inCard)
        {
            card = inCard;
            CardGraphicsHelper.SetFaceTexture(card, faceRenderer);
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            if (transform.position == player.transform.position) gameObject.SetActive(false);
            transform.LookAt(new Vector3(player.transform.position.x, 1, player.transform.position.z));
            transform.position += moveSpeed * Time.deltaTime * transform.forward;
        }
    }
}
