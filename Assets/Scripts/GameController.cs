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
    public static int MaxLife = 10;
    public static int StartCards = 5;
    public static int MaxCropPerZone = 3;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartGame();
    }

    public PlayerController GetPlayer(PlayerNumber p)
    {
        return p == PlayerNumber.P1 ? P1 : P2;
    }

    public void StartGame()
    {
        P1.CurrLife = MaxLife;
        P2.CurrLife = MaxLife;

        P1.DrawCard(StartCards);
        P2.DrawCard(StartCards);
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

    private IEnumerator GameLoop()
    {
        int round = 0;
        while (!CheckWin())
        {
            BoardField currField = GameBoard.Instance.GetPlayerField(CurrPlayer);
            //currField.DoTurnStart();

            yield return GameUI.Instance.SetBannerText(CurrPlayer + "'s Turn", 0.5f);

            GetCurrPlayer().DrawCard(1);
            CanPlayCards = true;
            turnEnded = false;

            yield return new WaitUntil(() => turnEnded == true);

            CanPlayCards = false;

            yield return GameUI.Instance.SetBannerText(CurrPlayer + "'s Harvest", 0.5f);

            currField.DoHarvest();

            yield return GameUI.Instance.SetBannerText(CurrPlayer + "'s Feast", 0.5f);

            // Cast Charms
            currField.DoCharms();

            // Feed Creatures
            currField.DoHunger();

            yield return GameUI.Instance.SetBannerText(CurrPlayer + "'s Attack", 0.5f);

            // Do Attacks
            if (round != 0)// || CurrPlayer != PlayerNumber.P1)
            {
                currField.DoCombat();
            }

            currField.CheckStarve();

            currField.RemoveSurplus();

            //currField.OnTurnEnd();

            CurrPlayer = CurrPlayer == PlayerNumber.P1 ? PlayerNumber.P2 : PlayerNumber.P1;
            if (CurrPlayer == startPlayer)
                round += 1;
        }
    }
}
