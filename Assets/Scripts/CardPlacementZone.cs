using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPlacementZone : MonoBehaviour
{
    public PlayerNumber Player;
    public CardType ZoneType;
    public int Lane;
    public int MaxCards;
    public Vector3 CardOffset;

    public List<Card> PlayedCards = new List<Card>();

    private void ArrangeCards()
    {
        Vector3 currOffset = Vector3.zero;

        foreach (Card c in PlayedCards)
        {
            c.transform.localPosition = currOffset;
            currOffset += CardOffset;
        }
    }

    public bool CanPlaceCard(Card c, PlayerNumber p)
    {
        if (p != Player)
            return false;

        if (MaxCards != 0 && PlayedCards.Count >= MaxCards)
            return false;

        return ZoneType == c.CardType;
    }

    public bool PlaceCard(Card c, PlayerNumber p)
    {
        if (!CanPlaceCard(c, p))
            return false;

        PlayedCards.Add(c);
        c.SetPlacedZone(this);
        c.OnDeath += CardDeath;
        ArrangeCards();
        return true;
    }

    private void CardDeath(Card c)
    {
        PlayedCards.Remove(c);
        ArrangeCards();
    }

    public Card AttackZone(int damage, Card source)
    {
        if (PlayedCards.Count == 0)
            return null;

        int kills = 0;
        foreach (Card c in PlayedCards)
        {
            int h = c.Health;
            if (c.Damage(damage, source))
            {
                kills += 1;
                damage -= h;
            }
        }

        Card ret = PlayedCards[0];
        for (int i = 0; i < kills; i++)
        {
            PlayedCards.RemoveAt(0);
        }
        ArrangeCards();
        return ret;
    }
}
