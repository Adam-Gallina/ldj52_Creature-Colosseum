using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAI : PlayerController
{
    [Header("Tutorial")]
    [SerializeField] private Card seedCrop;
    [SerializeField] private Card voleCreature;

    private int turn = 0;

    public override void TurnStart()
    {
        turn += 1;
        switch (turn)
        {
            case 1:
                StartCoroutine(TurnOne());
                break;
            case 2:
                StartCoroutine(TurnTwo());
                break;
            case 3:
                StartCoroutine(TurnThree());
                break;
        }
    }

    private int playerLane;
    private IEnumerator TurnOne()
    {
        for (int i = 0; i < 4; i++)
            if (GameController.Instance.GetPlayer(PlayerNumber.P1).Board.CreatureZones[i].PlayedCards.Count > 0)
            {
                playerLane = i;
                break;
            }

        yield return new WaitUntil(() => GameController.Instance.CanPlayCards);

        yield return new WaitForSeconds(0.25f);
        PlayCard(SpawnCard(seedCrop), Board.CropZones[playerLane]);
        yield return new WaitForSeconds(0.25f);
        PlayCard(SpawnCard(voleCreature), Board.CreatureZones[playerLane]);

        GameController.Instance.EndTurn();
    }

    private IEnumerator TurnTwo()
    {
        yield return new WaitUntil(() => GameController.Instance.CanPlayCards);

        yield return new WaitForSeconds(0.25f);
        PlayCard(SpawnCard(seedCrop), Board.CropZones[3 - playerLane]);
        yield return new WaitForSeconds(0.25f);
        PlayCard(SpawnCard(seedCrop), Board.CropZones[3 - playerLane]);
        yield return new WaitForSeconds(0.25f);
        PlayCard(SpawnCard(voleCreature), Board.CreatureZones[playerLane]);

        yield return ((TutorialGC)GameController.Instance).ShowText("Your Slug opposes your opponent's Vole, so it'll get attacked this turn. Luckily, its Surplus ability gave it enough health to take the attack");

        GameController.Instance.EndTurn();
    }
    private IEnumerator TurnThree()
    {
        yield return new WaitUntil(() => GameController.Instance.CanPlayCards);

        yield return new WaitForSeconds(0.25f);
        PlayCard(SpawnCard(seedCrop), Board.CropZones[3 - playerLane]);

        GameController.Instance.EndTurn();
    }
}
