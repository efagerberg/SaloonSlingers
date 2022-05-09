using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public interface ICardSpawner : ISpawner<ICardGraphic>
    {
        public ICardGraphic Spawn(Card c);
    }
}
