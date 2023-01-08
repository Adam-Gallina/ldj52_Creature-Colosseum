using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : Charm
{
    [SerializeField] protected int damageAmount;
    protected override string GetDescription()
    {
        return $"Deal {damageAmount} damage to opposing creature";
    }

    public override void UseCharm(List<CropIcon> crops)
    {
        PlayerBoard board = GameController.Instance.GetOpponent(Player).Board;

        if (board.CreatureZones[Lane].PlayedCards.Count == 1)
            board.CreatureZones[Lane].PlayedCards[0].Damage(damageAmount, this);
    }

}
