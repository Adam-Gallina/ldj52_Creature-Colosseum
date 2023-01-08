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

    [SerializeField] private float hoveredCardOffset;
    [SerializeField] private float hoveredCardScale;
    [SerializeField] private float selectedCardScale;
    protected Card hoveredCard;
    protected bool selectedCard;

    public List<Card> PlayedCards = new List<Card>();

    private void ArrangeCards()
    {
        Vector3 currOffset = Vector3.zero;

        foreach (Card c in PlayedCards)
        {
            Vector3 scale = new Vector3(1, 1, 1);
            if (c == hoveredCard && selectedCard)
                scale = new Vector3(selectedCardScale, selectedCardScale, selectedCardScale);
            else if (c == hoveredCard)
                scale = new Vector3(hoveredCardScale, hoveredCardScale, hoveredCardScale);

            c.transform.localScale = scale;
            c.transform.localPosition = c == hoveredCard ? currOffset + new Vector3(0, 0, hoveredCardOffset) : currOffset;
            c.CardUI.SetUnderCard(PlayedCards.IndexOf(c) != PlayedCards.Count - 1);
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

        c.transform.parent = transform;

        PlayedCards.Add(c);
        c.OnHover += HoverCard;
        c.OnClick += SelectCard;
        c.OnHoverEnd += EndHoverCard;
        c.OnDeath += CardDeath;
        c.Lane = Lane;
        ArrangeCards();
        return true;
    }

    private void HoverCard(Card card)
    {
        if (GameController.Instance.GetPlayer(card.Player).hoveredCard)
            return;

        if (!selectedCard)
            hoveredCard = card;
        ArrangeCards();
    }

    private void SelectCard(Card card)
    {
        if (GameController.Instance.GetPlayer(card.Player).hoveredCard)
            return;

        if (selectedCard)
        {
            if (card == hoveredCard)
                selectedCard = false;
            else
                hoveredCard = card;
        }
        else
            selectedCard = true;
        ArrangeCards();
    }

    private void EndHoverCard(Card card)
    {
        if (!selectedCard)
            hoveredCard = null;
        ArrangeCards();
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
            if (c.Damage(damage, source, true))
            {
                kills += 1;
                damage -= h;
            }
        }

        Card ret = PlayedCards[0];
        for (int i = 0; i < kills; i++)
        {
            PlayedCards[0].DestroyCard();
        }
        ArrangeCards();
        return ret;
    }
}
