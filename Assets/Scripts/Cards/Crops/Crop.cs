using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CropClass { None, Vegetable, Fruit, Seed, Meat }
public class Crop : Card
{
    [Header("Crop Stats")]
    public List<CropIcon> producedCrops = new List<CropIcon>();
    protected int cropBoost = 1;

    [Header("Harvest anim")]
    [SerializeField] protected Transform cropSpawnPoint;
    [SerializeField] protected float minYDist;
    [SerializeField] protected float maxYDist;
    [SerializeField] protected float maxXDelta;

    protected override void OnValidate()
    {
        base.OnValidate();

        CardType = CardType.Crop;
    }

    protected override string GetDescription()
    {
        string prod = "";
        foreach (CropIcon c in producedCrops)
            if (c)
                prod += c.CropType + " ";

        return $"Produces {producedCrops.Count} {prod} every turn";
    }

    public virtual CropIcon[] HarvestCrop()
    {
        CropIcon[] ret = new CropIcon[producedCrops.Count * cropBoost];

        for (int i = 0; i < ret.Length; i++)
        {
            CropIcon c = Instantiate(producedCrops[i % producedCrops.Count], cropSpawnPoint.position, Quaternion.identity);
            c.MoveIcon(transform.position + new Vector3(Random.Range(-maxXDelta, maxXDelta), Random.Range(minYDist, maxYDist), 0), Constants.CropHarvestTime);
            ret[i] = c;
        }

        cropBoost = 1;
        return ret;
    }

    public void BoostCrop(int modifier)
    {
        cropBoost *= modifier;
    }
}