using UnityEngine;
using UnityEngine.Pool;

namespace SaloonSlingers.Unity.CardEntities
{
    public class CardSpawner : MonoBehaviour, ICardSpawner
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private int poolSize = 32;

        private IObjectPool<ICardGraphic> pool;

        public ICardGraphic Spawn() => pool.Get();
        public void Despawn(ICardGraphic c) => pool.Release(c);

        private void Awake()
        {
            pool = new ObjectPool<ICardGraphic>(CreateInstance, GetFromPool, ReturnToPool, defaultCapacity: poolSize);
        }

        private ICardGraphic CreateInstance()
        {
            var go = Instantiate(cardPrefab, transform);
            var cardInteractable = go.GetComponent<ICardGraphic>();
            go.SetActive(false);
            return cardInteractable;
        }

        private void GetFromPool(ICardGraphic cardGraphic)
        {
            cardGraphic.gameObject.SetActive(true);
        }

        private void ReturnToPool(ICardGraphic cardGraphic)
        {
            cardGraphic.gameObject.SetActive(false);
            cardGraphic.transform.position = Vector3.zero;
            cardGraphic.transform.rotation = Quaternion.identity;
            cardGraphic.transform.SetParent(transform);
        }
    }
}
