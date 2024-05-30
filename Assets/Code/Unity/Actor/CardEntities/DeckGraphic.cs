using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class DeckGraphic : MonoBehaviour, ISpawner<GameObject>
    {
        public Transform TopCardTransform
        {
            get => coordinator.PeekTop(gameObject).transform;
        }
        public bool CanDraw { get => coordinator.PeekTop() != null; }
        public Deck Deck { get; private set; }

        [SerializeField]
        private int numberOfCards = Deck.NUMBER_OF_CARDS_IN_STANDARD_DECK;

        private DeckGraphicCoordinator coordinator;

        public GameObject Spawn() => coordinator.Pop();

        private void Awake()
        {
            Deck = new Deck(numberOfCards).Shuffle();
        }

        private void Start()
        {
            coordinator = new();
            coordinator.SpawnDeck(Deck, transform, LevelManager.Instance.CardSpawner);
        }
    }
}
