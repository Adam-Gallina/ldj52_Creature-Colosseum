using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : Boost
{
    protected override string GetDescription()
    {
        return $"Add +{strengthBoost},+{healthBoost} to target creature for 1 turn";
    }

    public override void UseCharm(List<CropIcon> crops)
    {
        PlayerBoard board = GameController.Instance.GetPlayer(Player).Board;
        if (board.CreatureZones[Lane].PlayedCards.Count == 1)
            board.CreatureZones[Lane].PlayedCards[0].Buff(strengthBoost, healthBoost);
    }
}
