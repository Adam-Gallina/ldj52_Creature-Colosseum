using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CropClass { None, Vegetable, Fruit, Seed, Meat }
public class Crop : Card
{
    [Header("Crop Stats")]
    public List<CropClass> producedCrops = new List<CropClass>();
    protected int cropBoost = 1;

    protected override void OnValidate()
    {
        base.OnValidate();

        CardType = CardType.Crop;
    }

    public virtual CropClass[] HarvestCrop()
    {
        CropClass[] ret = new CropClass[producedCrops.Count * cropBoost];

        for (int i = 0; i < cropBoost; i++)
            producedCrops.CopyTo(ret, producedCrops.Count * i);

        cropBoost = 1;
        return ret;
    }

    public void BoostCrop(int modifier)
    {
        cropBoost *= modifier;
    }
}