using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum CardType { Charm, Creature, Crop }
public abstract class Card : MonoBehaviour
{
    [Header("Stats")]
    public CardType CardType;
    public string CardName;
    public CropClass[] CropClassCost;
    //public CropType[] CropTypeCost;
    public string Description;
    public int BaseStrength;
    public int Strength;
    public int BaseHealth;
    public int Health;

    public bool Fed = false;

    [Header("Image")]
    [SerializeField] protected TMP_Text nameText;
    [SerializeField] protected TMP_Text tempHungerText;
    [SerializeField] protected TMP_Text descText;
    [SerializeField] protected TMP_Text strengthText;
    [SerializeField] protected TMP_Text healthText;

    public event Action<Card> OnHover;
    public event Action<Card> OnClick;
    public event Action<Card> OnHoverEnd;
    public event Action<Card> OnDeath;

    protected CardPlacementZone placedZone;

    protected virtual void OnValidate()
    {
        if (nameText)
            SetName(CardName);
        if (tempHungerText)
            SetHunger(CropClassCost);//, CropTypeCost);
        if (descText)
            SetDescription(Description);
        if (strengthText)
            SetStrength(BaseStrength);
        if (healthText)
            SetHealth(BaseHealth);
    }

    private void Awake()
    {
        Strength = BaseStrength;
        Health = BaseHealth;

        if (nameText)
            SetName(CardName);
        if (tempHungerText)
            SetHunger(CropClassCost);//, CropTypeCost);
        if (descText)
            SetDescription(Description);
        if (strengthText)
            SetStrength(Strength);
        if (healthText)
            SetHealth(Health);
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

    public virtual void SetPlacedZone(CardPlacementZone zone)
    {
        placedZone = zone;
    }

    public virtual List<ProducedCrop> CheckCrops(List<ProducedCrop> crops)
    {
        List<ProducedCrop> ret = new List<ProducedCrop>(crops);

        /*foreach (CropType t in CropTypeCost)
        {
            foreach (ProducedCrop c in ret)
            {
                if (c.cropType == t)
                {
                    ret.Remove(c);
                    break;
                }
            }
        }*/
        foreach (CropClass t in CropClassCost)
        {
            foreach (ProducedCrop c in ret)
            {
                if (c.cropClass == t)
                {
                    ret.Remove(c);
                    break;
                }
            }
        }

        if (crops.Count - ret.Count != /*CropTypeCost.Length + */CropClassCost.Length)
            return crops;
    
        Fed = true;
        OnFed();
        return ret;
    }

    public void CheckDeath()
    {
        if (!Fed)
            DestroyCard();
        Fed = false;
    }

    protected virtual void OnFed()
    {

    }

    public void DoAttack(int lane, BoardField board)
    {
        Card attackedCard = CheckCreature(lane, board);
        if (attackedCard)
        {
            OnAttack(attackedCard);
            return;
        }
        attackedCard = CheckCrop(lane, board);
        if (attackedCard)
        {
            OnAttack(attackedCard);
            return;
        }
        CheckPlayer(board);
    }
    protected virtual Card CheckCreature(int lane, BoardField board)
    {
        if (board.CreatureZones[lane].PlayedCards.Count == 0)
            return null;

        return board.CreatureZones[lane].AttackZone(Strength, this);
    }
    protected virtual Card CheckCrop(int lane, BoardField board)
    {
        if (board.CropZones[lane].PlayedCards.Count == 0)
            return null;

        return board.CropZones[lane].AttackZone(Strength, this);
    }
    protected virtual void CheckPlayer(BoardField board)
    {
        board.DamagePlayer(Strength);
    }
    protected virtual void OnAttack(Card target)
    {

    }

    // Return true if dead
    public bool Damage(int amount, Card source)
    {
        Health -= amount;
        OnDamaged(amount, source);
        SetHealth(Health);

        if (Health <= 0)
        {
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
    protected void SetName(string n)
    {
        nameText.text = n;
    }
    protected void SetHunger(CropClass[] cClass)//, CropType[] cType)
    {
        string t = "";
        foreach (CropClass c in cClass)
        {
            t += c.ToString() + "\n";
        }
        /*foreach (CropType c in cType)
        {
            t += c.ToString() + "\n";
        }*/
        tempHungerText.text = t;
    }
    protected void SetDescription(string d)
    {
        descText.text = d;
    }
    protected void SetStrength(int s)
    {
        strengthText.text = s.ToString();
    }
    protected void SetHealth(int h)
    {
        healthText.text = h.ToString();
    }
    #endregion
}
