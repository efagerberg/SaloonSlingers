using System.Collections.Generic;

public interface IDeck
{
    Deck Shuffle();
    Card RemoveFromTop();
    void RemoveFromTop(int _amount, IList<Card> _cards);
    void ReturnCard(Card _card);
    void ReturnCards(IEnumerable<Card> _cards);
}