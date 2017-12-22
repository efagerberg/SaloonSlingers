using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckComponent : MonoBehaviour {

    private Deck deck;

	void Start () {
        deck = new Deck(numOfDecks: 3).Shuffle();
	}

    public GameObject SpawnCard()
    {
	    Card card = deck.RemoveFromTop();
        var go = (GameObject)Instantiate(Resources.Load("Prefabs/Card"));
        go.GetComponent<CardComponent>().SetCard(card);
        return go;
    }
}
