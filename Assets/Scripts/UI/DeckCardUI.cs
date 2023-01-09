using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCardUI : MonoBehaviour
{
    public Card cardPrefab;

    public TMPro.TMP_Text nameText;
    public TMPro.TMP_Text countText;

    public event Action<Card> AddCard;
    public event Action<Card> RemCard;

    public void AddCount()
    {
        AddCard?.Invoke(cardPrefab);
    }
    public void ReduceCount()
    {
        RemCard?.Invoke(cardPrefab);
    }
}
