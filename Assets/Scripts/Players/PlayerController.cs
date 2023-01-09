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

    protected List<Card> queuedCards = new List<Card>();

    private void Start()
    {
        DrawPileObj.SpawnCards(deck.Count);
    }

    protected virtual void Update()
    {
        ArrangeHand();

        if (selectedCard)
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, 1 << Constants.CardZoneLayer);
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

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public virtual void TurnStart()
    {
        
    }
    public virtual void TurnEnd()
    {
        queuedCards.Clear();
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
            //PlayCard(hoveredCard, hoveredZone);
            QueueCard(hoveredCard, hoveredZone);
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
            c.transform.forward = Camera.main.transform.forward;
            Vector3 cardPos = new Vector3(i, 0, 0);
            Vector3 cardScale = new Vector3(1, 1, 1);

            if (selectedCard && c == hoveredCard && hoveredZone && hoveredZone.CanPlaceCard(c, Player) && GameController.Instance.CurrPlayer == Player && GameController.Instance.CanPlayCards)
            {
                foreach (Transform g in c.GetComponentsInChildren<Transform>())
                    g.gameObject.layer = Constants.DefaultLayer;
                c.transform.localScale = cardScale;
                c.transform.SetParent(hoveredZone.transform, false);
                c.transform.localPosition = hoveredZone.PlayedCards.Count * hoveredZone.CardOffset;
                c.transform.forward = hoveredZone.transform.forward;
                continue;
            }
            
            c.transform.SetParent(handParent);
            foreach (Transform g in c.GetComponentsInChildren<Transform>())
                g.gameObject.layer = Constants.PlayerHandLayer;
            

            if (c == hoveredCard)
                cardPos += c.transform.up * hoveredCardOffset;

            if (selectedCard && c == hoveredCard)
            {
                cardScale = new Vector3(selectedCardScale, selectedCardScale, selectedCardScale);
                cardPos += c.transform.up * selectedCardScale / 2;
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

            Card c = SpawnCard(deck[0]);
            c.OnHover += HoverCard;
            c.OnClick += SelectCard;
            c.OnHoverEnd += EndHoverCard;

            hand.Add(c);
            deck.RemoveAt(0);
        }
    }

    protected Card SpawnCard(Card prefab)
    {
        Card c = Instantiate(prefab, handParent);
        c.Player = Player;
        return c;
    }

    public void UnqueueLastCard()
    {
        if (queuedCards.Count == 0)
            return;

        Card c = queuedCards[0];
        queuedCards.Remove(c);
        c.GetComponentInParent<CardPlacementZone>().UnQueueCard(c);
        hand.Add(c);
        c.OnHover += HoverCard;
        c.OnClick += SelectCard;
        c.OnHoverEnd += EndHoverCard;
    }

    protected void QueueCard(Card c, CardPlacementZone zone)
    {
        if (!GameController.Instance.CanPlayCards)
            return;

        if (zone.QueueCard(c, Player))
        {
            queuedCards.Insert(0, c);
            hand.Remove(c);
            c.OnHover -= HoverCard;
            c.OnClick -= SelectCard;
            c.OnHoverEnd -= EndHoverCard;
        }
    }

    protected void PlayCard(Card c, CardPlacementZone zone)
    {
        if (!GameController.Instance.CanPlayCards)
            return;

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
            Invoke(nameof(OnLose), Constants.WinAnimTime);
    }
    protected virtual void OnLose()
    {
        if (Player == PlayerNumber.P1)
        {
            GameUI.Instance.ShowLoseScreen();
        }
        else
        {
            GameUI.Instance.ShowWinScreen();
        }
    }
}
