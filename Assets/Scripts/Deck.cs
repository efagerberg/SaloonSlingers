using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class Deck
{
    private List<Card> cards;
    public List<Card> Cards { get { return cards; } }

    public Deck()
    {
        cards = new List<Card>(52);

        var vals = Enum.GetValues(typeof (Card.Values)).Cast<Card.Values>();
        var suits = Enum.GetValues(typeof (Card.Suits)).Cast<Card.Suits>();
        foreach (var suit in suits)
        {
            foreach (var val in vals)
            {
                cards.Add(new Card(suit, val));
            }
        }
    }

    public Deck Shuffle()
    {
        for (var i = 0; i < cards.Count; i++)
        {
            var num = Random.Range(0, cards.Count - 1);
            var c = cards[num];
            cards[num] = cards[i];
            cards[i] = c;
        }
        return this;
    }

    public Card RemoveFromTop()
    {
        var c = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        return c;
    }

    public void ReturnCards(IList<Card> _cards)
    {
        foreach (var _card in _cards)
        {
            cards.Add(_card);
        }
        _cards.Clear();
        Shuffle();
    }
}
