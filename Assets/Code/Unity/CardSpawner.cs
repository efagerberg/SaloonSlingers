using UnityEngine;
using UnityEngine.Pool;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private int poolSize = 32;

        private IObjectPool<TangibleCard> pool;

        public TangibleCard Spawn() => pool.Get();
        public TangibleCard Spawn(Card card)
        {
            var c = Spawn();
            c.Card = card;
            return c;
        }
        public void Despawn(TangibleCard c) => pool.Release(c);

        private void Awake()
        {
            pool = new ObjectPool<TangibleCard>(CreateInstance, GetFromPool, ReturnToPool, defaultCapacity: poolSize);
        }

        private TangibleCard CreateInstance()
        {
            var go = Instantiate(cardPrefab, transform);
            var cardInteractable = go.GetComponent<TangibleCard>();
            go.SetActive(false);
            return cardInteractable;
        }

        private void GetFromPool(TangibleCard tangibleCard)
        {
            tangibleCard.gameObject.SetActive(true);
        }

        private void ReturnToPool(TangibleCard tangibleCard)
        {
            tangibleCard.gameObject.SetActive(true);
            tangibleCard.transform.position = Vector3.zero;
        }
    }
}
