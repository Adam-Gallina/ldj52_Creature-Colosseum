using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckLoader : MonoBehaviour
{
    public static DeckLoader Instance;

    [SerializeField] private SavedDeck[] savedDecks = new SavedDeck[0];
    [HideInInspector] public List<Deck> AiDecks = new List<Deck>();
    [HideInInspector] public List<Deck> Decks = new List<Deck>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Decks = new List<Deck>();
        AiDecks = new List<Deck>();
        foreach (SavedDeck d in savedDecks)
        {
            AiDecks.Add(d.deck);
            Deck de = new Deck();
            de.DeckName = d.deck.DeckName;
            de.Cards = new List<Card>(d.deck.Cards.ToArray());
            Decks.Add(de);
        }
    }

    public void PopulateDropdown(TMP_Dropdown dropdown, string firstOption)
    {
        dropdown.options.Clear();

        dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(firstOption));

        foreach (Deck d in DeckLoader.Instance.Decks)
            dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(d.DeckName));
    }

    public List<Card> GetAiDeck(int d = -1)
    {
        if (d == -1)
            d = Random.Range(0, AiDecks.Count);
        return new List<Card>(AiDecks[d].Cards.ToArray());
    }

    public List<Card> GetPlayerDeck(int d = -1)
    {
        if (d == -1)
            d = Random.Range(0, AiDecks.Count);
        return new List<Card>(Decks[d].Cards.ToArray());
    }
}
