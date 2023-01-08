using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : Creature
{
    protected override bool CheckCreature(PlayerBoard board)
    {
        return false;
    }
}
