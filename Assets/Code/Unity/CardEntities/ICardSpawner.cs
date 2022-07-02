using SaloonSlingers.Core;

namespace SaloonSlingers.Unity.CardEntities
{
    public interface ICardSpawner : ISpawner<ICardGraphic>
    {
        public ICardGraphic Spawn(Card c);
    }
}
