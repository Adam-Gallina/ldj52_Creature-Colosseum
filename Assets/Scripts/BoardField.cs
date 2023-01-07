using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardField : MonoBehaviour
{
    public PlayerNumber Player;
    public CardPlacementZone CharmZone;
    public CardPlacementZone[] CreatureZones = new CardPlacementZone[0];
    public CardPlacementZone[] CropZones = new CardPlacementZone[0];

    public List<CropClass> cropSurplus = new List<CropClass>();

    private void OnValidate()
    {
        if (CharmZone)
        {
            CharmZone.Player = Player;
            CharmZone.ZoneType = CardType.Charm;
            CharmZone.MaxCards = 0;
        }
        for (int i = 0; i < CreatureZones.Length; i++)
        {
            CreatureZones[i].Player = Player;
            CreatureZones[i].Lane = i;
            CreatureZones[i].ZoneType = CardType.Creature;
            CreatureZones[i].MaxCards = 1;
        }
        for (int i = 0; i < CropZones.Length; i++)
        {
            CropZones[i].Player = Player;
            CropZones[i].Lane = i;
            CropZones[i].ZoneType = CardType.Crop;
            CropZones[i].MaxCards = GameController.MaxCropPerZone;
        }
    }

    public void DoTurnStart()
    {

    }

    public void DoHarvest()
    {
        foreach (CardPlacementZone zone in CreatureZones)
        {
            if (zone.PlayedCards.Count == 1)
                zone.PlayedCards[0].BeforeHarvest();
        }

        foreach (CardPlacementZone zone in CropZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                cropSurplus.AddRange(((Crop)c).HarvestCrop());
            }
        }
    }

    public void DoCharms()
    {
        for (int i = CharmZone.PlayedCards.Count - 1; i >= 0; i--)
        {
            cropSurplus = CharmZone.PlayedCards[0].CheckCrops(cropSurplus);
            CharmZone.PlayedCards[0].DestroyCard();
        }

        CharmZone.PlayedCards.Clear();
    }

    public void DoHunger()
    {
        foreach (CardPlacementZone zone in CreatureZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                cropSurplus = c.CheckCrops(cropSurplus);
            }
        }
        foreach (CardPlacementZone zone in CropZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                cropSurplus = c.CheckCrops(cropSurplus);
            }
        }

        // Abilities
        foreach (CardPlacementZone zone in CreatureZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                cropSurplus = c.AbilityAfterEat(cropSurplus);
            }
        }
        foreach (CardPlacementZone zone in CropZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                cropSurplus = c.AbilityAfterEat(cropSurplus);
            }
        }
    }

    public Coroutine Combat()
    {
        return StartCoroutine(DoCombat());
    }
    private IEnumerator DoCombat()
    {
        BoardField opponent = GameController.Instance.GetOpponent(Player).Board;

        for (int lane = 0; lane < CreatureZones.Length; lane++)
        {
            if (CreatureZones[lane].PlayedCards.Count == 1)
            {
                yield return CreatureZones[lane].PlayedCards[0].Attack(opponent);
            }
        }
    }

    public void CheckStarve()
    {
        foreach (CardPlacementZone zone in CreatureZones)
        {
            if (zone.PlayedCards.Count == 1)
                zone.PlayedCards[0].CheckDeath();
        }
        foreach (CardPlacementZone zone in CropZones)
        {
            for (int i = zone.PlayedCards.Count - 1; i >= 0; i--)
            {
                zone.PlayedCards[i].CheckDeath();
            }
        }
    }

    public void RemoveSurplus()
    {
        cropSurplus.Clear();
    }

    public void OnTurnEnd()
    {

    }
}
