using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {

    public List<GameObject> deckComposition;

    public List<GameObject> deck;


	// Use this for initialization
	void Start () {
        deck = new List<GameObject>(deckComposition);
        ShuffleDeck();
    }

    public GameObject DrawCardFromDeck()
    {
        GameObject card = new GameObject();
        
        if(deck.Count == 0)
        {
            deck = new List<GameObject>(deckComposition);
            ShuffleDeck();
        }

        card = Instantiate(deck[0]);
        deck.RemoveAt(0);

        return card;
    }

    private void ShuffleDeck()
    {
        int n = deck.Count;
        while(n > 1)
        {
            n--;
            int k = Random.Range(0, n+1);
            GameObject go = deck[k];
            deck[k] = deck[n];
            deck[n] = go;
        }
    }
	

}
