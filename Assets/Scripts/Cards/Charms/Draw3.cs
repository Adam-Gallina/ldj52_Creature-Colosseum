using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw3 : Charm
{
    public override void UseCharm()
    {
        GameController.Instance.GetPlayer(Player).DrawCard(3);
    }
}
