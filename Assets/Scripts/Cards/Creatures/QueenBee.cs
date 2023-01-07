using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenBee : Creature
{
    [SerializeField] protected int boostMod;

    public override void BeforeHarvest()
    {
        for (int i = 0; i < 4; i++)
            foreach (Card c in GameController.Instance.GetPlayer(Player).Board.CropZones[i].PlayedCards)
                ((Crop)c).BoostCrop(boostMod);
    }
}
