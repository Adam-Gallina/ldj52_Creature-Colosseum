using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedDeck : MonoBehaviour
{
    [SerializeField] public Deck deck;
}

[System.Serializable]
public class Deck
{
    public string DeckName = "Unnamed Deck";
    public List<Card> Cards = new List<Card>();
}
