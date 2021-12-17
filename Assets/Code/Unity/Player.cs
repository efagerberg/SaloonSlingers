using UnityEngine;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public class Player : MonoBehaviour
    {
        public PlayerAttributes Attributes;
        [SerializeField]
        private int numberOfCards = Deck.NUMBER_OF_CARDS_IN_STANDARD_DECK;
        [SerializeField]
        private int startingHealth = 5;

        public void TakeDamage(int amount)
        {
            Attributes.Health -= Mathf.Max(amount, 0);
            if (Attributes.Health == 0)
                Debug.Log("Player died");
        }

        private void Awake()
        {
            Attributes = new PlayerAttributes(numberOfCards, startingHealth);
        }

        private void OnTriggerEnter(Collider hit)
        {
            if (!hit.CompareTag("Enemy")) return;
            TakeDamage(1);
            Destroy(hit.gameObject);
        }
    }
}