using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBoost : Creature
{
    [Header("Kill Boost")]
    [SerializeField] protected int strengthBoost;
    [SerializeField] protected int healthBoost;

    protected override void OnValidate()
    {
        Description = $"Gain +{strengthBoost},+{healthBoost} on kill";

        base.OnValidate();
    }

    protected override void OnAttack(Card target)
    {
        if (target.Health <= 0)
        {
            BaseStrength += strengthBoost;
            Strength += strengthBoost;
            BaseHealth += healthBoost;
            Health += healthBoost;

            UpdateStats();
        }
    }
}
