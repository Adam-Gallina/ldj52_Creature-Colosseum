using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialGC : GameController
{
    [SerializeField] private GameObject tutTextBkgd;
    [SerializeField] private TMP_Text tutText;

    protected override IEnumerator PreGameLoop()
    {
        GameUI.Instance.SetEndTurnBtn(false);

        yield return ShowText("Welcome to the game!");
        yield return ShowText("At the bottom of the screen is your hand");
        yield return ShowText("At the beginning of your turn, you'll draw a card");
        yield return GetCurrPlayer().DrawCard(1);

        CanPlayCards = true;

        yield return ShowText("You drew a Creature card! You play Creatures on the middle row to attack your opponent. Go ahead and play it now!");
        yield return new WaitUntil(() => CreatureInLane(PlayerNumber.P1));

        yield return ShowText("Your Slug will need something to eat, or it'll starve at the end of your turn. Select your Slug to see what it eats");
        yield return new WaitUntil(() => GameObject.Find("Card Inspector").transform.childCount == 1);
        yield return ShowText("The symbols under a card's name tell you what kind of Crop it eats. Your Slug needs 1 vegetable every turn.\nThe number on the left of a card shows how much Damage a card can do.\nThe number on the right shows how much Health it has");

        yield return GetCurrPlayer().DrawCard(1);

        yield return ShowText("Crops generate food during the Harvest phase for your Creatures. Play your Vegetable on the back row.");
        yield return ShowText("You can play up to 3 Crops in one space");
        yield return new WaitUntil(() => CropInLane(PlayerNumber.P1));

        yield return ShowText("Looks like you're out of cards, click End Turn to enter your Harvest phase. Creature's don't attack on the first turn");
        yield return StartRound(CurrPlayer, false, false);

        // AI Turn
        CurrPlayer = PlayerNumber.P2;
        yield return StartRound(PlayerNumber.P2, false, true);

        CurrPlayer = PlayerNumber.P1;
        CanPlayCards = true;
        yield return GetCurrPlayer().DrawCard(1);

        yield return ShowText("Looks like the programmer was a fool and didn't play enough Crops to support his Vole, so it starved");
        yield return ShowText("You drew another Vegetable, go ahead and play it");
        yield return new WaitUntil(() => PlacedCrops(PlayerNumber.P1, 2));

        yield return ShowText("Your Slug has a 'Vegetable Surplus 1' ability. If you Harvest an extra Vegetable, the Slug can eat it and increase it's health by 1.");
        yield return ShowText("Your opponent's Vole died, so your Slug can attack their Crop directly when you end your turn.");
        yield return StartRound(PlayerNumber.P1, true, false);

        // AI Turn
        CurrPlayer = PlayerNumber.P2;
        yield return StartRound(PlayerNumber.P2, true, true);

        CurrPlayer = PlayerNumber.P1;
        CanPlayCards = true;
        yield return GetCurrPlayer().DrawCard(1);

        yield return ShowText("You drew a Charm! Charms have single-use abilities, and are used before your attack. Play Charms in the middle row.");
        yield return ShowText("This Charm will increase your Slug's stats for one turn, and allow it to kill your opponent's Vole.");

        int playerLane = -1;
        for (int i = 0; i < 4; i++)
            if (GetPlayer(PlayerNumber.P1).Board.CreatureZones[i].PlayedCards.Count > 0)
            {
                playerLane = i;
                break;
            }

        yield return new WaitUntil(() => CharmInLane(PlayerNumber.P1));

        if (!CharmInLane(PlayerNumber.P1, lane: playerLane))
        {
            yield return ShowText("You don't have a Creature in that lane, so the Charm won't have any effect. Use the undo button to place it in front of your Slug.");
        }

        yield return new WaitUntil(() => CharmInLane(PlayerNumber.P1, lane: playerLane));

        yield return ShowText("Charms consume Crops before Creatures, so have a free Vegetable to keep your Slug from starving.");
        yield return GetCurrPlayer().DrawCard(1);
        yield return StartRound(CurrPlayer, true, false);

        // AI Turn
        CurrPlayer = PlayerNumber.P2;
        yield return StartRound(PlayerNumber.P2, true, true);

        CurrPlayer = PlayerNumber.P1;
        CanPlayCards = true;
        yield return GetCurrPlayer().DrawCard(1);

        yield return ShowText("Your Slug is unopposed, so you can attack your opponent directly and win!");

        yield return StartRound(CurrPlayer, true, false);
    }

    public Coroutine ShowText(string text)
    {
        return StartCoroutine(DisplayText(text));
    }
    private IEnumerator DisplayText(string text)
    {
        tutTextBkgd.SetActive(true);
        tutText.text = text;
        yield return StartCoroutine(WaitForClick());
        tutTextBkgd.SetActive(false);
    }

    protected IEnumerator WaitForClick()
    {
        next = false;
        yield return new WaitUntil(() => next);
        yield return new WaitForEndOfFrame();
    }

    #region Board helpers
    private bool CharmInLane(PlayerNumber p, Charm c = null, int lane = -1)
    {
        return CardInLane(GetPlayer(p).Board.CharmZones, c, lane);
    }
    private bool CreatureInLane(PlayerNumber p, Creature c = null, int lane = -1)
    {
        return CardInLane(GetPlayer(p).Board.CreatureZones, c, lane);
    }
    private bool CropInLane(PlayerNumber p, Crop c = null, int lane = -1)
    {
        return CardInLane(GetPlayer(p).Board.CropZones, c, lane);
    }

    private bool CardInLane(CardPlacementZone[] zones, Card c = null, int lane = -1)
    {
        if (lane != -1)
        {
            return CardExists(zones[lane], c);
        }

        for (int i = 0; i < 4; i++)
        {
            if (CardExists(zones[i], c))
                return true;
        }

        return false;
    }

    private bool PlacedCharms(PlayerNumber p, int count, Charm c = null)
    {
        return PlacedCards(GetPlayer(p).Board.CharmZones, count, c);
    }
    private bool PlacedCreatures(PlayerNumber p, int count, Creature c = null)
    {
        return PlacedCards(GetPlayer(p).Board.CreatureZones, count, c);
    }
    private bool PlacedCrops(PlayerNumber p, int count, Crop c = null)
    {
        return PlacedCards(GetPlayer(p).Board.CropZones, count, c);
    }
    private bool PlacedCards(CardPlacementZone[] zones, int count, Card card = null)
    {
        foreach (CardPlacementZone zone in zones)
        {
            foreach (Card c in zone.PlayedCards)
            {
                if (card == null)
                    count--;
                else if (c.CardName == card.CardName)
                    count--;
            }
            foreach (Card c in zone.QueuedCards)
            {
                if (card == null)
                    count--;
                else if (c.CardName == card.CardName)
                    count--;
            }
        }

        return count <= 0;
    }

    private bool CardExists(CardPlacementZone zone, Card card)
    {
        if (card == null)
            return zone.PlayedCards.Count > 0 || zone.QueuedCards.Count > 0;

        foreach (Card c in zone.PlayedCards)
        {
            if (c.CardName == card.CardName)
                return true;
        }
        foreach (Card c in zone.QueuedCards)
        {
            if (c.CardName == card.CardName)
                return true;
        }

        return false;
    }
    #endregion

    private bool next = false;
    public void NextText()
    {
        next = true;
    }
}
