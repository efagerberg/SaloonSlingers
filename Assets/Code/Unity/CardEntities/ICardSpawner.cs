using SaloonSlingers.Core;

namespace SaloonSlingers.Unity.CardEntities
{
    public interface ICardSpawner : ISpawner<ICardGraphic>
    {
        public ICardGraphic Spawn(Card c)
        {
            ICardGraphic cardGraphic = Spawn();
            cardGraphic.SetGraphics(c);
            return cardGraphic;
        }
    }
}
