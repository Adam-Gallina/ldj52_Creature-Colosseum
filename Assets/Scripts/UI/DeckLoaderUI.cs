using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckLoaderUI : MonoBehaviour
{
    public TMP_Dropdown p1Deck;
    public TMP_Dropdown p2Deck;

    private void Start()
    {
        DeckLoader.Instance.PopulateDropdown(p1Deck, "P1's Deck");
        if (p2Deck)
            DeckLoader.Instance.PopulateDropdown(p2Deck, "P2's Deck");
    }
}
