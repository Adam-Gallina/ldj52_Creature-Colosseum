using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum CardType { Charm, Creature, Crop }
public abstract class Card : MonoBehaviour
{
    public PlayerNumber Player;

    [Header("Stats")]
    public CardType CardType;
    public string CardName;
    public CropClass[] CropClassCost;
    public string Description;
    public int BaseStrength;
    public int Strength;
    public int BaseHealth;
    public int Health;
    public int Lane;

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

    protected virtual void OnValidate()
    {
        if (nameText)
            SetName(CardName);
        if (tempHungerText)
            SetHunger(CropClassCost);
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
            SetHunger(CropClassCost);
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

    public virtual void BeforeHarvest()
    {

    }

    public virtual List<CropClass> CheckCrops(List<CropClass> crops)
    {
        List<CropClass> ret = new List<CropClass>(crops);

        foreach (CropClass t in CropClassCost)
        {
            foreach (CropClass c in ret)
            {
                if (c == t)
                {
                    ret.Remove(c);
                    break;
                }
            }
        }

        if (crops.Count - ret.Count != CropClassCost.Length)
            return crops;
    
        Fed = true;
        OnFed();
        return ret;
    }
    protected virtual void OnFed()
    {

    }
    public virtual List<CropClass> AbilityAfterEat(List<CropClass> crops)
    {
        return crops;
    }


    #region Attack
    public Coroutine Attack(BoardField board)
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
            GameController.Instance.GetOpponent(Player).Damage(Strength);
            OnAttack(null);
        }
        else
            OnAttack(target.AttackZone(Strength, this));

        endTime = Time.time + GameController.AttackAnimTime / 2;

        while (Time.time < endTime)
        {
            float t = 1 - (endTime - Time.time) / (GameController.AttackAnimTime / 2);
            transform.position = target.transform.position + (startPos - target.transform.position) * t;
            yield return new WaitForEndOfFrame();
        }
    }
    protected virtual bool CheckCreature(BoardField board)
    {
        return board.CreatureZones[Lane].PlayedCards.Count > 0;
    }
    protected virtual bool CheckCrop(BoardField board)
    {
        return board.CropZones[Lane].PlayedCards.Count > 0;
    }
    protected virtual bool CheckPlayer(BoardField board)
    {
        return true;
    }
    protected virtual void OnAttack(Card target)
    {

    }
    #endregion

    public void CheckDeath()
    {
        if (!Fed)
            DestroyCard();
        Fed = false;
    }

    // Return true if dead
    public virtual bool Damage(int amount, Card source, bool ignoreDeath=false)
    {
        Health -= amount;
        OnDamaged(amount, source);
        SetHealth(Health);

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
    protected void SetName(string n)
    {
        nameText.text = n;
    }
    protected void SetHunger(CropClass[] cClass)
    {
        string t = "";
        foreach (CropClass c in cClass)
        {
            t += c.ToString() + "\n";
        }
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

    #region Helper functions
    protected bool Surplus(List<CropClass> crops, CropClass crop, int count)
    {
        if (crops.FindAll((CropClass c) => c == crop).Count >= count)
        {
            for (int i = 0; i < count; i++)
                crops.Remove(crop);
            return true;
        }

        return false;
    }

    public bool Heal(int amount)
    {
        Health += amount;
        if (Health > BaseHealth)
            Health = BaseHealth;

        SetHealth(Health);

        return true;
    }
    #endregion
}
