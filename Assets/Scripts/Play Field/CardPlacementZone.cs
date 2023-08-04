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

    [SerializeField] private float hoveredCardScale;
    [SerializeField] private float selectedCardScale;
    protected Card hoveredCard;
    protected GameObject selectedCard;

    public List<Card> PlayedCards = new List<Card>();
    public List<Card> QueuedCards = new List<Card>();

    private void Update()
    {
        if (selectedCard && !hoveredCard && Input.GetKeyDown(KeyCode.Mouse0))
            Destroy(selectedCard);
    }

    private void ArrangeCards()
    {
        Vector3 currOffset = Vector3.zero;

        foreach (Card c in PlayedCards)
        {
            Vector3 scale = new Vector3(1, 1, 1);
            if (c == hoveredCard)
                scale = new Vector3(hoveredCardScale, hoveredCardScale, hoveredCardScale);

            c.transform.localScale = scale;
            c.transform.localPosition = currOffset;
            currOffset += CardOffset;
        }
        foreach (Card c in QueuedCards)
        {
            Vector3 scale = new Vector3(1, 1, 1);
            if (c == hoveredCard)
                scale = new Vector3(hoveredCardScale, hoveredCardScale, hoveredCardScale);

            c.transform.localScale = scale;
            c.transform.localPosition = currOffset;
            currOffset += CardOffset;
        }
    }

    public bool CanPlaceCard(Card c, PlayerNumber p)
    {
        if (p != Player)
            return false;

        if (MaxCards != 0 && (PlayedCards.Count + QueuedCards.Count) >= MaxCards)
            return false;

        return ZoneType == c.CardType;
    }

    public void UnQueueCard(Card c)
    {
        if (QueuedCards.Remove(c))
        {
            c.OnHover -= HoverCard;
            c.OnClick -= SelectCard;
            c.OnHoverEnd -= EndHoverCard;
            c.OnDeath -= CardDeath;
            ArrangeCards();
            c.GetComponentInChildren<CardUI>().SetArtStand(0, 1);
        }
    }
    public bool QueueCard(Card c, PlayerNumber p)
    {
        if (!CanPlaceCard(c, p))
            return false;

        c.transform.SetParent(transform, false);

        QueuedCards.Add(c);
        c.OnHover += HoverCard;
        c.OnClick += SelectCard;
        c.OnHoverEnd += EndHoverCard;
        c.OnDeath += CardDeath;
        c.Lane = Lane;
        ArrangeCards();
        c.OnPlayCard();

        return true;
    }
    public void PlayQueuedCards()
    {
        for (int i = QueuedCards.Count; i > 0; i--)
        {
            Card c = QueuedCards[0];
            PlayedCards.Add(c);
            QueuedCards.Remove(c);
        }
        ArrangeCards();
    }
    public bool PlaceCard(Card c, PlayerNumber p)
    {
        if (!CanPlaceCard(c, p))
            return false;

        c.transform.SetParent(transform, false);
        c.transform.localPosition = Vector3.zero;
        c.transform.localRotation = Quaternion.identity;

        PlayedCards.Add(c);
        c.OnHover += HoverCard;
        c.OnClick += SelectCard;
        c.OnHoverEnd += EndHoverCard;
        c.OnDeath += CardDeath;
        c.Lane = Lane;
        ArrangeCards();
        c.OnPlayCard();

        return true;
    }

    private void HoverCard(Card card)
    {
        if (GameController.Instance.GetPlayer(card.Player).hoveredCard)
            return;

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
                Destroy(selectedCard);
            else
                hoveredCard = card;
        }
        else
        {
            selectedCard = Instantiate(card.gameObject, GameObject.Find("Card Inspector").transform);
            selectedCard.transform.localScale = new Vector3(selectedCardScale, selectedCardScale, selectedCardScale);
            foreach (Transform t in selectedCard.GetComponentsInChildren<Transform>())
                t.gameObject.layer = Constants.PlayerHandLayer;
            selectedCard.GetComponentInChildren<CardUI>().SetArtStand(0, 1);
        }
        ArrangeCards();
    }

    private void EndHoverCard(Card card)
    {
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
