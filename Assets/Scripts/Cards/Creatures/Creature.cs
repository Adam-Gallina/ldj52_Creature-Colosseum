using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Card
{
    protected override void OnValidate()
    {
        base.OnValidate();

        CardType = CardType.Creature;
    }
}
