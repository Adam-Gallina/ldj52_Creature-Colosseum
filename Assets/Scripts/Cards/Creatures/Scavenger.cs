using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scavenger : Creature
{
    protected override void OnFed()
    {
        Fed = false;
    }

    protected override void OnAttack(Card target)
    {
        if (target)
        {
            Fed = true;
        }
    }
}
