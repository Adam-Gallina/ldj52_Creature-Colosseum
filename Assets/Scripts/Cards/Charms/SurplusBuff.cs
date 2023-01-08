using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurplusBuff : Charm
{
    [SerializeField] protected CropClass surplusType;
    [SerializeField] protected int surplusAmount;

    public override void UseCharm(List<CropIcon> crops)
    {
        if (Surplus(crops, surplusType, surplusAmount))
        {

        }
    }
}
