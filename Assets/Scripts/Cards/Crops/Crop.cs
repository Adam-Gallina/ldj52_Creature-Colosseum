using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CropClass { None, Vegetable }//, Any }
public enum CropType { None, Carrot }
public class Crop : Card
{
    [Header("Crop Stats")]
    [SerializeField] public List<ProducedCrop> producedCrops = new List<ProducedCrop>();

    protected override void OnValidate()
    {
        base.OnValidate();

        CardType = CardType.Crop;
    }

    public virtual ProducedCrop[] HarvestCrop()
    {
        return producedCrops.ToArray();
    }
}

[System.Serializable]
public class ProducedCrop
{
    public CropType cropType;
    public CropClass cropClass;
}