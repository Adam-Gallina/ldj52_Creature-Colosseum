using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decay : Damage
{
    [SerializeField] protected Card summonedCard;

    protected override string GetDescription()
    {
        return base.GetDescription() + $" and summon a {summonedCard.BaseStrength},{summonedCard.BaseHealth} {summonedCard.name}";
    }

    public override void UseCharm(List<CropIcon> crops)
    {
        base.UseCharm(crops);

        PlayerBoard board = GameController.Instance.GetPlayer(Player).Board;
        if (board.CreatureZones[Lane].PlayedCards.Count == 0)
            board.CreatureZones[Lane].PlaceCard(Instantiate(summonedCard), Player);

    }


}
