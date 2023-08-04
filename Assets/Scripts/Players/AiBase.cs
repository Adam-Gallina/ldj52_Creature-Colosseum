using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiBase : PlayerController
{

    public override void TurnStart()
    {
        StartCoroutine(WaitForPlay());
    }

    private IEnumerator WaitForPlay()
    {
        yield return new WaitUntil(() => GameController.Instance.CanPlayCards);

        yield return StartCoroutine(AIenumerator());

        GameController.Instance.EndTurn();
    }

    protected abstract IEnumerator AIenumerator();

    protected CardPlacementZone GetPlacementZone(Card c, int lane = -1)
    {
        switch (c.CardType)
        {
            case CardType.Charm:
                return GetZoneLane(Board.CharmZones, lane);
            case CardType.Creature:
                return GetZoneLane(Board.CreatureZones, lane);
            case CardType.Crop:
                return GetZoneLane(Board.CropZones, lane);
        }

        Debug.Log("Not sure what to return for " + c.CardType + " (" + c.CardName + ")");
        return null;
    }

    protected CardPlacementZone GetZoneLane(CardPlacementZone[] zones, int lane = -1)
    {
        if (lane != -1)
            return zones[lane];

        List<CardPlacementZone> validZones = new List<CardPlacementZone>();
        foreach (CardPlacementZone z in zones)
            if (z.PlayedCards.Count < z.MaxCards)
                validZones.Add(z);
        
        if (validZones.Count == 0)
            return null;
        return validZones[Random.Range(0, validZones.Count)];
    }
}
