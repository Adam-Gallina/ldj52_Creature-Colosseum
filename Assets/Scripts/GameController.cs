using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerNumber { P1, P2 }
public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public PlayerNumber CurrPlayer;
    private PlayerNumber startPlayer = PlayerNumber.P1;
    [HideInInspector] public bool CanPlayCards = false;
    private bool turnEnded = true;
    public PlayerController P1;
    public PlayerController P2;

    [Header("Rules")]
    public int MaxLife = 10;
    public int StartCards = 5;
    public static int MaxCropPerZone = 3;
    public bool ShuffleOnStart = true;

    private void Awake()
    {
        Instance = this;
    }

    protected virtual void Start()
    {
        GameObject.Find("Card Inspector").transform.forward = Camera.main.transform.forward;
    }

    public PlayerController GetPlayer(PlayerNumber p)
    {
        return p == PlayerNumber.P1 ? P1 : P2;
    }
    public PlayerController GetOpponent(PlayerNumber p)
    {
        return p == PlayerNumber.P1 ? P2 : P1;
    }

    public void StartGame()
    {
        P1.CurrLife = MaxLife;
        P2.CurrLife = MaxLife;

        if (ShuffleOnStart)
        {
            P1.ShuffleDeck();
            P2.ShuffleDeck();
        }

        P1.DrawCard(StartCards, false);
        P2.DrawCard(StartCards, false);
        CurrPlayer = PlayerNumber.P1;

        StartCoroutine(GameLoop());
    }

    public void EndTurn()
    {
        turnEnded = true;
    }

    public PlayerController GetCurrPlayer()
    {
        return CurrPlayer == PlayerNumber.P1 ? P1 : P2;
    }

    private bool CheckWin()
    {
        return false;
    }

    protected virtual IEnumerator PreGameLoop()
    {
        yield return null;
    }

    private IEnumerator GameLoop()
    {
        yield return PreGameLoop();

        int round = 0;
        while (!CheckWin())
        {
            yield return StartRound(CurrPlayer, round != 0);

            CurrPlayer = CurrPlayer == PlayerNumber.P1 ? PlayerNumber.P2 : PlayerNumber.P1;
            if (CurrPlayer == startPlayer)
                round += 1;
        }
    }

    protected Coroutine StartRound(PlayerNumber player, bool doAttacks, bool draw=true)
    {
        return StartCoroutine(DoRound(player, doAttacks, draw));
    }
    private IEnumerator DoRound(PlayerNumber player, bool doAttacks, bool draw=true)
    {
        GameUI.Instance.SetEndTurnBtn(false);
        PlayerBoard currField = GetPlayer(player).Board;
        GetPlayer(player).TurnStart();
        currField.OnTurnStart();

        yield return GameUI.Instance.SetBannerText(player + "'s Turn", 0.5f);

        if (draw)
            yield return GetPlayer(player).DrawCard(1);
        CanPlayCards = true;
        turnEnded = false;
        GameUI.Instance.SetEndTurnBtn(true);

        yield return new WaitUntil(() => turnEnded == true);
        currField.PlayAllQueuedCards();

        GameUI.Instance.SetEndTurnBtn(false);
        CanPlayCards = false;

        yield return GameUI.Instance.SetBannerText(player + "'s Harvest", 0.5f);
        currField.DoHarvest();
        yield return new WaitForSeconds(Constants.CropHarvestTime);

        yield return GameUI.Instance.SetBannerText(player + "'s Feast", 0.5f);
        // Cast Charms
        currField.DoCharms();

        // Feed Creatures
        currField.DoHunger();

        yield return new WaitForSeconds(Constants.CropConsumeTime);

        // Do Attacks
        if (doAttacks)
        {
            yield return GameUI.Instance.SetBannerText(player + "'s Attack", 0.5f);
            yield return currField.Combat();
        }

        yield return GameUI.Instance.SetBannerText(player + "'s Turn", 0);
        currField.CheckStarve();

        currField.RemoveSurplus();

        GetPlayer(player).TurnEnd();
        currField.OnTurnEnd();
    }
}
