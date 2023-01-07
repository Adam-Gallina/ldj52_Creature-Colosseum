using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Charm : Card
{
    protected override void OnValidate()
    {
        base.OnValidate();

        CardType = CardType.Charm;
    }

    protected override void OnFed()
    {
        base.OnFed();

        UseCharm();
    }

    public abstract void UseCharm();
}
