using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAi : AiBase
{
    protected override IEnumerator AIenumerator()
    {
        yield return new WaitForSeconds(0.25f);

        float chanceToPlay = 2f / hand.Count;

        for (int i = hand.Count; i > 0; i--)
        {            
            if (Random.Range(0, 1000) / 1000f <= chanceToPlay)
            {
                i -= 1;
                PlayCard(hand[i], GetPlacementZone(hand[i]));
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}
