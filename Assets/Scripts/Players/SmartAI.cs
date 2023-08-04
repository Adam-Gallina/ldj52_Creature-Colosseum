using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartAI : AiBase
{
    [Header("Chances\n(Calculated by [VAL] / [CARD TYPE COUNT])")]
    [SerializeField] private float maxChance = .5f;
    [SerializeField] private float cropPlayChance = 2;
    [SerializeField] private float creaturePlayChance = 2;
    [SerializeField] private float charmPlayChance = 1;

    protected override IEnumerator AIenumerator()
    {
        yield return new WaitForSeconds(0.25f);

        List<CropIcon> created = new List<CropIcon>();
        List<CropIcon> cropMin = new List<CropIcon>();
        
        // Estimate crop production
        foreach (CardPlacementZone zone in Board.CropZones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                foreach (CropIcon i in ((Crop)c).producedCrops)
                {
                    CropIcon n = Instantiate(i);
                    cropMin.Add(n);
                    created.Add(n);
                }
            }
        }
        foreach (CardPlacementZone zone in Board.CreatureZones)
        {
            if (zone.PlayedCards.Count == 1)
            {
                List<CropIcon> consumed = zone.PlayedCards[0].CheckCrops(cropMin);

                if (consumed.Count == zone.PlayedCards[0].CropClassCost.Length)
                {
                    foreach (CropIcon icon in consumed)
                        cropMin.Remove(icon);
                }
            }
        }

        // Play Random crops
        List<Card> crops = hand.FindAll((Card c) => c.CardType == CardType.Crop);
        List<Card> newCards = PlayCards(cropMin, crops, 1, true);
        foreach (Card c in newCards)
            foreach (CropIcon i in ((Crop)c).producedCrops)
            {
                CropIcon n = Instantiate(i);
                cropMin.Add(n);
                created.Add(n);
            }

        // Creatures
        List<Card> creatures = hand.FindAll((Card c) => c.CardType == CardType.Creature);
        float creatureChance = creaturePlayChance / creatures.Count;
        newCards.AddRange(PlayCards(cropMin, creatures, creatureChance));
        
        // Charms
        List<Card> charms = hand.FindAll((Card c) => c.CardType == CardType.Charm);
        float charmChance = charmPlayChance / charms.Count;
        newCards.AddRange(PlayCards(cropMin, charms, charmChance));

        for (int i = 0; i < newCards.Count; i++)
        {
            Card temp = newCards[i];
            int randomIndex = Random.Range(i, newCards.Count);
            newCards[i] = newCards[randomIndex];
            newCards[randomIndex] = temp;
        }

        foreach (CropIcon i in created)
            Destroy(i.gameObject);

        foreach (Card c in newCards)
        {
            // this is stupid and not a great fix
            Card x = c.name.Contains("Clone") ? c : SpawnCard(c);

            CardPlacementZone zone = GetPlacementZone(c);
            if (zone)
                PlayCard(x, zone);
            hand.Remove(c);
            yield return new WaitForSeconds(0.25f);
        }

    }

    private List<Card> PlayCards(List<CropIcon> crops, List<Card> cards, float chance, bool ignoreMaxChance=false)
    {
        List<Card> played = new List<Card>();

        if (!ignoreMaxChance && chance > maxChance)
            chance = maxChance;

        foreach (Card c in cards)
        {
            List<CropIcon> consumed = c.CheckCrops(crops);
            
            if (consumed.Count == c.CropClassCost.Length && Random.Range(0, 1000) / 1000f < chance)
            {
                foreach (CropIcon icon in consumed)
                    crops.Remove(icon);

                played.Add(c);
            }
        }

        return played;
    }
}
