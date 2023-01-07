using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurplusBoost : Creature
{
    [Header("Surplus Ability")]
    [SerializeField] protected CropClass surplusType;
    [SerializeField] protected int surplusCount;
    [SerializeField] protected int strengthBoost;
    [SerializeField] protected int healthBoost;

    protected override void OnValidate()
    {
        Description = $"{surplusType} Surplus {surplusCount}:\nGain +{strengthBoost}, +{healthBoost}";

        base.OnValidate();
    }

    public override List<CropClass> AbilityAfterEat(List<CropClass> crops)
    {
        if (Surplus(crops, surplusType, surplusCount))
        {
            BaseStrength += strengthBoost;
            Strength += strengthBoost;
            BaseHealth += healthBoost;
            Health += healthBoost;
        }

        SetStrength(Strength);
        SetHealth(Health);
        return crops;
    }
}
