using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [SerializeField] private Button endTurnBtn;
    [SerializeField] public Button undoBtn;

    [SerializeField] private TMP_Text P1HealthText;
    [SerializeField] private TMP_Text P2HealthText;
    
    [SerializeField] private TMP_Text BannerText;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    public TMP_Dropdown p1Deck;
    public TMP_Dropdown p2Deck;
    public Button startBtn;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (p1Deck)
            DeckLoader.Instance.PopulateDropdown(p1Deck, "P1's Deck");
        if (p2Deck)
            DeckLoader.Instance.PopulateDropdown(p2Deck, "P2's Deck");
        else
            GameController.Instance.GetPlayer(PlayerNumber.P2).deck = DeckLoader.Instance.GetPlayerDeck();
        startBtn.interactable = !(p1Deck || p2Deck);
    }

    public void SelectP1Deck(int d)
    {
        if (d == 0)
            return;
        GameController.Instance.GetPlayer(PlayerNumber.P1).deck = DeckLoader.Instance.GetPlayerDeck(d - 1);

        if (p2Deck)
            startBtn.interactable = p2Deck.value != 0;
        else
            startBtn.interactable = true;
    }
    public void SelectP2Deck(int d)
    {
        if (d == 0)
            return;
        GameController.Instance.GetPlayer(PlayerNumber.P2).deck = DeckLoader.Instance.GetPlayerDeck(d - 1);

        startBtn.interactable = p1Deck.value != 0;
    }

    private void Update()
    {
        P1HealthText.text = "P1: " + GameController.Instance.P1.CurrLife;
        P2HealthText.text = "P2: " + GameController.Instance.P2.CurrLife;
    }

    public void PressStart()
    {
        GameController.Instance.StartGame();
    }

    public void PressEndTurn()
    {
        GameController.Instance.EndTurn();
    }

    public Coroutine SetBannerText(string text, float time)
    {
        return StartCoroutine(BannerTextAnim(text, time));
    }

    private IEnumerator BannerTextAnim(string text, float time)
    {
        BannerText.text = text;
        yield return new WaitForSeconds(time);
    }

    public void SetEndTurnBtn(bool enabled)
    {
        endTurnBtn.interactable = enabled;
        undoBtn.interactable = enabled;
    }

    public void UndoButton()
    {
        GameController.Instance.GetCurrPlayer().UnqueueLastCard();
    }

    public void ShowWinScreen()
    {
        SetEndTurnBtn(false);
        winScreen.SetActive(true);
    }

    public void ShowLoseScreen()
    {
        SetEndTurnBtn(false);
        loseScreen.SetActive(true);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
