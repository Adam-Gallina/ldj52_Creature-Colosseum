using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wildfire : Charm
{
    public override void UseCharm(List<CropIcon> crops)
    {
        DestroyCrops(GameController.Instance.GetPlayer(Player).Board);
        DestroyCrops(GameController.Instance.GetOpponent(Player).Board);
    }

    protected void DestroyCrops(PlayerBoard target)
    {
        for (int i = target.CropZones[Lane].PlayedCards.Count; i > 0; i--)
        {
            target.CropZones[Lane].PlayedCards[0].DestroyCard();
        }
    }
}
