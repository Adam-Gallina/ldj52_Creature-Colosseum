using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : Charm
{
    [SerializeField] protected int drawCount = 3;

    protected override string GetDescription()
    {
        return $"Draw {drawCount} cards";
    }

    public override void UseCharm(List<CropIcon> crops)
    {
        GameController.Instance.GetPlayer(Player).DrawCard(drawCount);
    }
}
