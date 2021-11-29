using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.Pool;


namespace SaloonSlingers.Unity
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private int poolSize = 10;

        private IObjectPool<CardInteractable> pool;
        private Player player;

        public CardInteractable Spawn() => pool.Get();
        public CardInteractable Spawn(Vector3 position, Quaternion rotation)
        {
            var c = Spawn();
            c.transform.SetPositionAndRotation(position, rotation);
            return c;
        }
        public void Despawn(CardInteractable c) => pool.Release(c);

        private void Start()
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            player = playerGO.GetComponent<Player>();
            pool = new ObjectPool<CardInteractable>(CreateInstance, GetFromPool, defaultCapacity: poolSize);
            CardInteractable.OnCardDeactivated += (sender, _) => Despawn(sender);
        }

        private CardInteractable CreateInstance()
        {
            var go = Instantiate(cardPrefab, transform);
            var cardInteractable = go.GetComponent<CardInteractable>();
            go.SetActive(false);
            return cardInteractable;
        }

        private void GetFromPool(CardInteractable cardInteractable)
        {
            Card card = player.Attributes.Deck.RemoveFromTop();
            cardInteractable.gameObject.SetActive(true);
            cardInteractable.SetCard(card);
        }
    }
}