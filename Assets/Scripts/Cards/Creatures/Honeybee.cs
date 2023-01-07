using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honeybee : Creature
{
    [SerializeField] protected int boostMod;

    public override void BeforeHarvest()
    {
        foreach (Card c in GameController.Instance.GetPlayer(Player).Board.CropZones[Lane].PlayedCards)
            ((Crop)c).BoostCrop(boostMod);
    }
}
