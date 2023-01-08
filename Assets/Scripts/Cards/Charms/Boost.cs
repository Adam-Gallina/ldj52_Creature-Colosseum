using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : Charm
{
    [SerializeField] protected int strengthBoost;
    [SerializeField] protected int healthBoost;

    protected override string GetDescription()
    {
        return $"Add +{strengthBoost},+{healthBoost} to target creature";
    }

    public override void UseCharm(List<CropIcon> crops)
    {
        PlayerBoard board = GameController.Instance.GetPlayer(Player).Board;
        
        if (board.CreatureZones[Lane].PlayedCards.Count == 1) 
        {
            board.CreatureZones[Lane].PlayedCards[0].Boost(strengthBoost, healthBoost);
        }
    }
}
