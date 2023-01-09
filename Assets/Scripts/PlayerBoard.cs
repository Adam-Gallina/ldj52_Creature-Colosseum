using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoard : MonoBehaviour
{
    public PlayerNumber Player;
    public CardPlacementZone[] CharmZones = new CardPlacementZone[0];
    public CardPlacementZone[] CreatureZones = new CardPlacementZone[0];
    public CardPlacementZone[] CropZones = new CardPlacementZone[0];

    public List<CropIcon> cropSurplus = new List<CropIcon>();

    private void OnValidate()
    {
        for (int i = 0; i < CharmZones.Length; i++)
        {
            CharmZones[i].Player = Player;
            CharmZones[i].Lane = i;
            CharmZones[i].ZoneType = CardType.Charm;
            CharmZones[i].MaxCards = 1;
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

    public void OnTurnStart()
    {
        foreach (CardPlacementZone zone in CharmZones)
        {
            if (zone.PlayedCards.Count == 1)
                zone.PlayedCards[0].OnTurnBegin();
        }
        foreach (CardPlacementZone zone in CreatureZones)
        {
            if (zone.PlayedCards.Count == 1)
                zone.PlayedCards[0].OnTurnBegin();
        }
        foreach (CardPlacementZone zone in CropZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                c.OnTurnBegin();
            }
        }
    }

    public void PlayAllQueuedCards()
    {
        foreach (CardPlacementZone zone in CharmZones)
        {
            if (zone.QueuedCards.Count > 0)
                zone.PlayQueuedCards();
        }
        foreach (CardPlacementZone zone in CreatureZones)
        {
            if (zone.QueuedCards.Count > 0)
                zone.PlayQueuedCards();
        }
        foreach (CardPlacementZone zone in CropZones)
        {
            if (zone.QueuedCards.Count > 0)
                zone.PlayQueuedCards();
        }
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
        foreach (CardPlacementZone zone in CharmZones)
        {
            if (zone.PlayedCards.Count == 1)
            {
                zone.PlayedCards[0].CheckCrops(cropSurplus);
            }
        }

        foreach (CardPlacementZone zone in CharmZones)
        {
            if (zone.PlayedCards.Count == 1)
            {
                zone.PlayedCards[0].AbilityAfterEat(cropSurplus);
            }
        }
    }

    public void DoHunger()
    {
        foreach (CardPlacementZone zone in CreatureZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                c.CheckCrops(cropSurplus);
            }
        }
        foreach (CardPlacementZone zone in CropZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                c.CheckCrops(cropSurplus);
            }
        }

        // Abilities
        foreach (CardPlacementZone zone in CreatureZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                c.AbilityAfterEat(cropSurplus);
            }
        }
        foreach (CardPlacementZone zone in CropZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                c.AbilityAfterEat(cropSurplus);
            }
        }
    }

    public Coroutine Combat()
    {
        return StartCoroutine(DoCombat());
    }
    private IEnumerator DoCombat()
    {
        PlayerBoard opponent = GameController.Instance.GetOpponent(Player).Board;

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
        foreach (CropIcon c in cropSurplus)
            Destroy(c.gameObject);
        cropSurplus.Clear();
    }

    public void OnTurnEnd()
    {
        foreach (CardPlacementZone zone in CharmZones)
        {
            if (zone.PlayedCards.Count == 1)
                zone.PlayedCards[0].OnTurnEnd();
        }
        foreach (CardPlacementZone zone in CreatureZones)
        {
            if (zone.PlayedCards.Count == 1)
                zone.PlayedCards[0].OnTurnEnd();
        }
        foreach (CardPlacementZone zone in CropZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                c.OnTurnEnd();
            }
        }
    }
}
