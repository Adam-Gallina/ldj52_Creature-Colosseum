using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Charm : Card
{
    [SerializeField] protected bool destroyOnUse = true;

    protected override void OnValidate()
    {
        base.OnValidate();

        CardType = CardType.Charm;
    }

    public override void AbilityAfterEat(List<CropIcon> crops)
    {
        if (!Fed)
        {
            DestroyCard();
            return;
        }

        UseCharm(crops);

        if (destroyOnUse)
            DestroyCard();

    }

    public abstract void UseCharm(List<CropIcon> crops);
}
