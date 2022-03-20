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

        private IObjectPool<ITangibleCard> pool;

        public ITangibleCard Spawn() => pool.Get();
        public ITangibleCard Spawn(Card card)
        {
            var c = Spawn();
            c.Card = card;
            return c;
        }
        public void Despawn(ITangibleCard c) => pool.Release(c);

        private void Awake()
        {
            pool = new ObjectPool<ITangibleCard>(CreateInstance, GetFromPool, ReturnToPool, defaultCapacity: poolSize);
        }

        private TangibleCard CreateInstance()
        {
            var go = Instantiate(cardPrefab, transform);
            var cardInteractable = go.GetComponent<TangibleCard>();
            go.SetActive(false);
            return cardInteractable;
        }

        private void GetFromPool(ITangibleCard tangibleCard)
        {
            tangibleCard.gameObject.SetActive(true);
        }

        private void ReturnToPool(ITangibleCard tangibleCard)
        {
            tangibleCard.gameObject.SetActive(true);
            tangibleCard.transform.position = Vector3.zero;
        }
    }
}
