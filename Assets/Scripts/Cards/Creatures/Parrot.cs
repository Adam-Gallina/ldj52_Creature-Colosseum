using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parrot : Creature
{
    [Header("Surplus Ability")]
    [SerializeField] protected CropClass surplusType;
    [SerializeField] protected int surplusCount;
    [SerializeField] protected int healAmount;

    protected override void OnValidate()
    {
        Description = $"{surplusType} Surplus {surplusCount}:\nHeal adjacent Creatures for {healAmount}";

        base.OnValidate();
    }

    public override List<CropClass> AbilityAfterEat(List<CropClass> crops)
    {
        if (Surplus(crops, surplusType, surplusCount))
        {
            BoardField b = GameController.Instance.GetPlayer(Player).Board;
            if (Lane > 0 && b.CreatureZones[Lane - 1].PlayedCards.Count == 1)
                b.CreatureZones[Lane - 1].PlayedCards[0].Heal(healAmount);

            if (Lane < 3 && b.CreatureZones[Lane + 1].PlayedCards.Count == 1)
                b.CreatureZones[Lane + 1].PlayedCards[0].Heal(healAmount);
        }

        return crops;
    }
}
