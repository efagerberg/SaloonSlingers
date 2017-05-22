using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckComponent : MonoBehaviour {

    private Deck deck;

	// Use this for initialization
	void Start () {
        deck = new Deck().Shuffle();
        SpawnCards();
	}

    void SpawnCards()
    {
        for (int i = 0; i < deck.Cards.Capacity; i++)
        {
            var go = (GameObject)Instantiate(Resources.Load("Prefabs/Card"));
            go.GetComponent<CardComponent>().SetCard(deck.Cards[i]);
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = new Vector3(0, -0.0011f * i, 0);
            go.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        }
    }
}
