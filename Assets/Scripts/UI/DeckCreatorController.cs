using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckCreatorController : MonoBehaviour
{
    public static DeckCreatorController Instance;

    [SerializeField] private string cardPath;
    [SerializeField] private CardWindow cropWindow;
    [SerializeField] private CardWindow creatureWindow;
    [SerializeField] private CardWindow charmWindow;

    [Header("Card Scroll Views")]
    public int cardsPerLine = 4;
    public float cardScale = 80;
    public float firstCardOffset = 10;
    public Vector2 cardMargin = new Vector2(10, 10);

    [Header("Active Deck")]
    [SerializeField] private DeckCardUI deckCardUIPrefab;
    [SerializeField] private RectTransform deckScrollView;
    [SerializeField] private float deckCardOffset = 5;
    [SerializeField] private float deckCardSize = 30;
    private Deck deck;
    private List<DeckCardUI> deckUI = new List<DeckCardUI>();

    [Header("Deck saving")]
    [SerializeField] private TMPro.TMP_InputField deckName;
    [SerializeField] private TMPro.TMP_Text cardCount;
    [SerializeField] private Color goodCardCount = Color.black;
    [SerializeField] private Color badCardCount = Color.red;
    [SerializeField] private Button saveBtn;
    [SerializeField] private TMPro.TMP_Dropdown savedDecks;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cropWindow.PopulateRect(cardPath + "/" + cropWindow.folder);
        creatureWindow.PopulateRect(cardPath + "/" + creatureWindow.folder);
        charmWindow.PopulateRect(cardPath + "/" + charmWindow.folder);

        deck = new Deck();

        DeckLoader.Instance.PopulateDropdown(savedDecks, "Load Saved deck");
    }

    public void AddCardToDeck(Card c)
    {
        deck.Cards.Add(c);
        UpdateDeckCardUI(c);
    }

    private void UpdateDeckCardUI(Card c)
    {
        if (!deckUI.Find((DeckCardUI d) => d.cardPrefab.CardName == c.CardName))
        {
            DeckCardUI d = Instantiate(deckCardUIPrefab, deckScrollView);
            d.cardPrefab = c;
            d.countText.text = "1";
            d.nameText.text = c.CardName;
            d.AddCard += IncreaseCard;
            d.RemCard += DecreaseCard;

            d.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -deckCardOffset * (deckUI.Count + 1) - deckCardSize * deckUI.Count);
            deckScrollView.sizeDelta = new Vector2(deckScrollView.sizeDelta.x, deckCardOffset * (deckUI.Count + 2) + deckCardSize * (deckUI.Count + 1));

            deckUI.Add(d);
        }

        deckUI.Find((DeckCardUI d) => d.cardPrefab.CardName == c.CardName).countText.text = deck.Cards.FindAll((Card card) => card.CardName == c.CardName).Count.ToString();
        UpdateSaveText();
    }

    private void UpdateDeckCardUIPositions() 
    {
        for (int i = 0; i < deckUI.Count; i++)
        {
            deckUI[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -deckCardOffset * (i + 1) - deckCardSize * i);
        }

        deckScrollView.sizeDelta = new Vector2(deckScrollView.sizeDelta.x, deckCardOffset * (deckUI.Count + 2) + deckCardSize * (deckUI.Count + 1));
        UpdateSaveText();
    }

    private void UpdateSaveText()
    {
        cardCount.text = "Cards:\n" + deck.Cards.Count + "/" + Constants.MaxCardCount;
        cardCount.color = deck.Cards.Count >= Constants.MinCardCount && deck.Cards.Count <= Constants.MaxCardCount ? goodCardCount : badCardCount;
        saveBtn.interactable = deck.Cards.Count >= Constants.MinCardCount && deck.Cards.Count <= Constants.MaxCardCount;
    }

    private void IncreaseCard(Card c)
    {
        deck.Cards.Add(c);
        UpdateDeckCardUI(c);
    }
    private void DecreaseCard(Card c)
    {
        deck.Cards.Remove(deck.Cards.Find((Card card) => card.CardName == c.CardName));

        if (!deck.Cards.Find((Card card) => card.CardName == c.CardName))
        {
            DeckCardUI dc = deckUI.Find((DeckCardUI d) => d.cardPrefab.CardName == c.CardName);
            deckUI.Remove(dc);
            Destroy(dc.gameObject);

            UpdateDeckCardUIPositions();
        }
        else
        {
            UpdateDeckCardUI(c);
        }
    }

    public void LoadDeck(int d)
    {
        if (d == 0)
            return;

        deck = new Deck();
        deck.Cards = new List<Card>(DeckLoader.Instance.Decks[d - 1].Cards.ToArray());
        deck.DeckName = DeckLoader.Instance.Decks[d - 1].DeckName;

        foreach (DeckCardUI de in deckUI)
            Destroy(de.gameObject);
        deckUI.Clear();

        foreach (Card c in deck.Cards)
            UpdateDeckCardUI(c);

        deckName.text = deck.DeckName;

        savedDecks.value = 0;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SaveDeck()
    {
        deck.DeckName = deckName.text;

        if (DeckLoader.Instance.Decks.Find((Deck d) => d.DeckName == deck.DeckName) != null)
        {
            DeckLoader.Instance.Decks.Find((Deck d) => d.DeckName == deck.DeckName).Cards = deck.Cards;
        }
        else
        {
            DeckLoader.Instance.Decks.Add(deck);
            DeckLoader.Instance.PopulateDropdown(savedDecks, "Load Saved deck");
        }

        Deck d = new Deck();
        d.DeckName = deck.DeckName;
        d.Cards = new List<Card>(deck.Cards.ToArray());
        deck = d;
    }

    public Card GetCardButton(Card c)
    {
        return Instantiate(c);
    }
}

[System.Serializable]
class CardWindow
{
    public string folder;
    public RectTransform scrollViewContent;

    private List<Card> prefabs = new List<Card>();

    public void PopulateRect(string path)
    {
        DeckCreatorController dc = DeckCreatorController.Instance;

        foreach (Card c in Resources.LoadAll<Card>(path))
        {
            if (!c.ShowInDeckBuilder)
                continue;

            Card card = dc.GetCardButton(c);
            card.OnHover += HoverCard;
            card.OnClick += SelectCard;
            card.OnHoverEnd += ExitCard;
            
            RectTransform rt = card.gameObject.AddComponent<RectTransform>();
            rt.SetParent(scrollViewContent, false);

            card.transform.localScale = new Vector3(dc.cardScale, dc.cardScale, dc.cardScale);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);

            int x = prefabs.Count % dc.cardsPerLine;
            int y = prefabs.Count / dc.cardsPerLine;
            
            rt.anchoredPosition = new Vector2(dc.firstCardOffset + dc.cardScale / 2 + (dc.cardScale + dc.cardMargin.x) * x,
                                              -dc.cardScale - (dc.cardScale * 1.5f + dc.cardMargin.y) * y);

            prefabs.Add(c);
        }
        
        scrollViewContent.sizeDelta = new Vector2(scrollViewContent.sizeDelta.x, (prefabs.Count / dc.cardsPerLine + 1) * (dc.cardScale * 1.5f + dc.cardMargin.y) + dc.cardScale);
    }

    private void HoverCard(Card card)
    {
        float s = DeckCreatorController.Instance.cardScale + 5;
        card.GetComponent<RectTransform>().localScale = new Vector3(s, s, s);
    }

    private void ExitCard(Card card)
    {
        float s = DeckCreatorController.Instance.cardScale;
        card.GetComponent<RectTransform>().localScale = new Vector3(s, s, s);
    }

    private void SelectCard(Card card)
    {
        DeckCreatorController.Instance.AddCardToDeck(prefabs.Find((Card c) => c.CardName == card.CardName));
    }
}