using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerNumber Player;
    public PlayerBoard Board;
    public List<Card> hand = new List<Card>();
    public List<Card> deck = new List<Card>();
    public int CurrLife;

    [Header("Hand")]
    [SerializeField] private Transform handParent;
    [SerializeField] private float handCardOffset;
    [SerializeField] private float handCardWidth;
    [SerializeField] private float hoveredCardOffset;
    [SerializeField] private float selectedCardScale;
    [HideInInspector] public Card hoveredCard;
    protected bool selectedCard;
    protected CardPlacementZone hoveredZone;

    [Header("Draw Pile")]
    public DrawPile DrawPileObj;

    private void Start()
    {
        DrawPileObj.SpawnCards(deck.Count);
    }

    protected virtual void Update()
    {
        ArrangeHand();

        if (selectedCard)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouse, mouse - Camera.main.transform.position, 10, 1 << Constants.CardZoneLayer);

            hoveredZone = hit.transform ? hit.transform.GetComponent<CardPlacementZone>() : null;
        }
    }

    private void OnEnable()
    {
        foreach (Card c in hand)
        {
            c.OnHover += HoverCard;
            c.OnClick += SelectCard;
            c.OnHoverEnd += EndHoverCard;
        }
    }
    private void OnDisable()
    {
        foreach (Card c in hand)
        {
            c.OnHover -= HoverCard;
            c.OnClick -= SelectCard;
            c.OnHoverEnd -= EndHoverCard;
        }
    }

    private void HoverCard(Card card)
    {
        if (!selectedCard)
            hoveredCard = card;
    }

    private void SelectCard(Card card)
    {
        if (selectedCard && hoveredZone && card == hoveredCard && GameController.Instance.CurrPlayer == Player)
        {
            PlayCard(hoveredCard, hoveredZone);
            selectedCard = false;
            hoveredCard = null;
            hoveredZone = null;
            return;
        }

        if (selectedCard) 
        {
            if (card == hoveredCard)
                selectedCard = false;
            else
                hoveredCard = card;
        }
        else
            selectedCard = true;
    }

    private void EndHoverCard(Card card)
    {
        if (!selectedCard)
            hoveredCard = null;
    }

    protected void ArrangeHand()
    {
        float i = -((hand.Count - 1) * (handCardWidth + handCardOffset)) / 2;
        foreach (Card c in hand)
        {
            Vector3 cardPos = new Vector3(i, 0, 0);
            Vector3 cardScale = new Vector3(1, 1, 1);

            if (selectedCard && c == hoveredCard && hoveredZone && hoveredZone.CanPlaceCard(c, Player) && GameController.Instance.CurrPlayer == Player)
            {
                c.transform.localScale = cardScale;
                c.transform.SetParent(hoveredZone.transform, false);
                c.transform.localPosition = hoveredZone.PlayedCards.Count * hoveredZone.CardOffset;
                continue;
            }
            else
                c.transform.SetParent(handParent);

            if (c == hoveredCard)
                cardPos.y = hoveredCardOffset;

            if (selectedCard && c == hoveredCard)
            {
                cardScale = new Vector3(selectedCardScale, selectedCardScale, selectedCardScale);
                cardPos.y += selectedCardScale / 2;
                cardPos.z += -0.1f;
            }

            c.transform.localScale = cardScale;
            c.transform.localPosition = cardPos;
            i += handCardOffset + handCardWidth;
        }
    }

    public Coroutine DrawCard(int count, bool doAnim=true)
    {
        return StartCoroutine(DrawCardWithAnim(count, doAnim));
    }
    private IEnumerator DrawCardWithAnim(int count, bool doAnim)
    {
        if (doAnim)
            yield return DrawPileObj.DoDrawCardAnim(count, handParent, 0.5f);

        for (int i = 0; i < count; i++)
        {
            if (deck.Count == 0)
                break;

            Card c = Instantiate(deck[0], handParent);
            c.Player = Player;
            c.OnHover += HoverCard;
            c.OnClick += SelectCard;
            c.OnHoverEnd += EndHoverCard;

            hand.Add(c);
            deck.RemoveAt(0);
        }
    }

    private void PlayCard(Card c, CardPlacementZone zone)
    {
        if (zone.PlaceCard(c, Player))
        {
            hand.Remove(c);
            c.OnHover -= HoverCard;
            c.OnClick -= SelectCard;
            c.OnHoverEnd -= EndHoverCard;
        }
    }

    public void Damage(int amount)
    {
        CurrLife -= amount;

        if (CurrLife <= 0)
            Debug.Log(Player + " lost!");
    }
}
