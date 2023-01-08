using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum CardType { Charm, Creature, Crop }
public abstract class Card : MonoBehaviour
{
    [HideInInspector] public PlayerNumber Player;

    [Header("Stats")]
    public CardType CardType;
    public string CardName;
    public CropClass[] CropClassCost;
    public string Description;
    public int BaseStrength;
    [HideInInspector] public int Strength;
    [HideInInspector] public int TempStrength;
    public int BaseHealth;
    [HideInInspector] public int Health;
    [HideInInspector] public int TempHealth;
    [HideInInspector] public int Lane;

    [HideInInspector] public bool Fed = false;

    [Header("Image")]
    public CardUI CardUI;

    public event Action<Card> OnHover;
    public event Action<Card> OnClick;
    public event Action<Card> OnHoverEnd;
    public event Action<Card> OnDeath;

    protected virtual void OnValidate()
    {
        string desc = GetDescription();
        Description = string.IsNullOrEmpty(desc) ? Description : desc;

        if (!CardUI)
            return;

        CardUI.SetName(CardName);
        //cardUI.SetHunger(CropClassCost);
        CardUI.SetDescription(Description);
        CardUI.SetStrength(BaseStrength);
        CardUI.SetHealth(BaseHealth);
    }
    [MenuItem("Card Generation/Update Hunger Bar")]
    static void GenerateHungerBar()
    {
        Card c = Selection.activeGameObject?.GetComponentInParent<Card>();
        if (c)
            c.GetComponentInChildren<CardUI>().SetHunger(c.CropClassCost, true);
    }
    protected virtual string GetDescription()
    {
        return "";
    }

    private void Awake()
    {
        Strength = BaseStrength;
        Health = BaseHealth;

        CardUI.SetName(CardName);
        CardUI.SetDescription(Description);
        UpdateStats();
    }

    private void Start()
    {
        CardUI.SetHunger(CropClassCost);
    }

    private void OnMouseEnter()
    {
        OnHover?.Invoke(this);
    }

    private void OnMouseDown()
    {
        OnClick?.Invoke(this);
    }

    private void OnMouseExit()
    {
        OnHoverEnd?.Invoke(this);
    }

    public virtual void OnTurnBegin()
    {
        TempStrength = 0;
        TempHealth = 0;
        UpdateStats();
    }

    public virtual void BeforeHarvest()
    {

    }

    public virtual void CheckCrops(List<CropIcon> crops)
    {
        List<CropIcon> consuming = new List<CropIcon>();
        foreach (CropClass t in CropClassCost)
        {
            foreach (CropIcon c in crops)
            {
                if (c.CropType == t && !c.consumed && !consuming.Contains(c))
                {
                    consuming.Add(c);
                    break;
                }
            }
        }

        if (consuming.Count != CropClassCost.Length)
            return;
    
        foreach (CropIcon c in consuming)
            c.MoveIcon(transform, Constants.CropConsumeTime, true);

        Fed = true;
        OnFed();
    }
    protected virtual void OnFed()
    {

    }
    public virtual void AbilityAfterEat(List<CropIcon> crops)
    {
    }


    #region Attack
    public Coroutine Attack(PlayerBoard board)
    {
        if (CheckCreature(board))
            return StartCoroutine(AttackAnim(board.CreatureZones[Lane]));

        if (CheckCrop(board))
            return StartCoroutine(AttackAnim(board.CropZones[Lane]));

        if (CheckPlayer(board))
            return StartCoroutine(AttackAnim(board.CropZones[Lane], true));

        return null;
    }
    protected virtual IEnumerator AttackAnim(CardPlacementZone target, bool attackPlayer = false)
    {
        Vector3 startPos = transform.position;
        float endTime = Time.time + GameController.AttackAnimTime / 2;

        while (Time.time < endTime)
        {
            float t = 1 - (endTime - Time.time) / (GameController.AttackAnimTime / 2);
            transform.position = startPos + (target.transform.position - startPos) * t;
            yield return new WaitForEndOfFrame();
        }

        if (attackPlayer)
        {
            GameController.Instance.GetOpponent(Player).Damage(Strength + TempStrength);
            OnAttack(null);
        }
        else
            OnAttack(target.AttackZone(Strength + TempStrength, this));

        endTime = Time.time + GameController.AttackAnimTime / 2;

        while (Time.time < endTime)
        {
            float t = 1 - (endTime - Time.time) / (GameController.AttackAnimTime / 2);
            transform.position = target.transform.position + (startPos - target.transform.position) * t;
            yield return new WaitForEndOfFrame();
        }
    }
    protected virtual bool CheckCreature(PlayerBoard board)
    {
        return board.CreatureZones[Lane].PlayedCards.Count > 0;
    }
    protected virtual bool CheckCrop(PlayerBoard board)
    {
        return board.CropZones[Lane].PlayedCards.Count > 0;
    }
    protected virtual bool CheckPlayer(PlayerBoard board)
    {
        return true;
    }
    protected virtual void OnAttack(Card target)
    {

    }
    #endregion

    public virtual void OnTurnEnd()
    {
    }

    public void CheckDeath()
    {
        if (!Fed)
            DestroyCard();
        Fed = false;
    }

    // Return true if dead
    public virtual bool Damage(int amount, Card source, bool ignoreDeath=false)
    {
        if (TempHealth > 0)
        {
            TempHealth -= amount;
            if (TempHealth < 0)
            {
                amount = -TempHealth;
                TempHealth = 0;
            }
            else
                amount = 0;
        }
        Health -= amount;
        OnDamaged(amount, source);
        UpdateStats();

        if (Health <= 0)
        {
            if (!ignoreDeath)
                DestroyCard();
            return true;
        }
        return false;
    }
    protected virtual void OnDamaged(int amount, Card source)
    {

    }

    public virtual void DestroyCard()
    {
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    #region Card Text

    public void UpdateStats()
    {
        CardUI.SetStrength(Strength + TempStrength);
        CardUI.SetHealth(Health + TempHealth);
    }
    #endregion

    #region Helper functions
    protected bool Surplus(List<CropIcon> crops, CropClass crop, int count)
    {
        if (crops.FindAll((CropIcon c) => c.CropType == crop && !c.consumed).Count >= count)
        {
            foreach (CropIcon c in crops)
            {
                if (c.CropType == crop && !c.consumed)
                {
                    c.MoveIcon(transform, Constants.CropConsumeTime, true);
                    count -= 1;
                }
                if (count == 0)
                    return true;
            }
        }

        return false;
    }

    public void Boost(int strength, int health)
    {
        Strength += strength;
        BaseStrength += strength;
        Health += health;
        BaseHealth += health;

        UpdateStats();
    }

    public void Buff(int tempStrength, int tempHealth)
    {
        TempStrength += tempStrength;
        TempHealth += tempHealth;

        UpdateStats();
    }

    public bool Heal(int amount)
    {
        Health += amount;
        if (Health > BaseHealth)
            Health = BaseHealth;

        UpdateStats();

        return true;
    }
    #endregion
}
