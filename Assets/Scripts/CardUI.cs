using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("Card text")]
    [SerializeField] protected TMP_Text nameText;
    [SerializeField] protected TMP_Text descText;
    [SerializeField] protected TMP_Text strengthText;
    [SerializeField] protected TMP_Text healthText;

    [Header("Hunger Indicator")]
    [SerializeField] protected Transform hungerParent;
    [SerializeField] protected float iconSpacing;
    [SerializeField] protected float iconWidth;
    [SerializeField] protected int maxIcons;
    [SerializeField] protected float iconAreaWidth;

    [Header("Icon Images")]
    [SerializeField] protected GameObject uiVegetableIconPrefab;
    [SerializeField] protected GameObject uiFruitIconPrefab;
    [SerializeField] protected GameObject uiSeedIconPrefab;
    [SerializeField] protected GameObject uiMeatIconPrefab;

    private GameObject GetIconPrefab(CropClass crop)
    {
        switch (crop) 
        {
            case CropClass.Vegetable:
                return uiVegetableIconPrefab;
            case CropClass.Fruit:
                return uiFruitIconPrefab;
            case CropClass.Seed:
                return uiSeedIconPrefab;
            case CropClass.Meat:
                return uiMeatIconPrefab;
        }

        Debug.LogError("Don't have a card icon for " + crop);
        return null;
    }

    public void SetName(string n)
    {
        if (!nameText)
            return;

        nameText.text = n;
    }
    public void SetHunger(CropClass[] cClass, bool regen=false)
    {
        if (!hungerParent)
            return;
        
        if (!regen && cClass.Length == hungerParent.childCount)
            return;

        for (int _ = hungerParent.childCount; _ > 0 ; _--)
            DestroyImmediate(hungerParent.GetChild(0).gameObject);

        float i = cClass.Length <= maxIcons ? -((cClass.Length - 1) * (iconSpacing + iconWidth)) / 2 : -iconAreaWidth / 2 + iconWidth / 2;
        float di = cClass.Length <= maxIcons ? iconSpacing + iconWidth : (iconAreaWidth - iconWidth / 2) / (cClass.Length);

        foreach (CropClass c in cClass)
        {
            GameObject icon = Instantiate(GetIconPrefab(c), hungerParent);
            icon.transform.localPosition = new Vector3(i, 0, 0);
            i += di;
        }
    }

    public void SetDescription(string d)
    {
        if (!descText)
            return;

        descText.text = d;
    }
    public void SetStrength(int s)
    {
        if (!strengthText)
            return;

        strengthText.text = s.ToString();
    }
    public void SetHealth(int h)
    {
        if (!healthText)
            return;

        healthText.text = h.ToString();
    }

    public void SetUnderCard(bool under)
    {
        descText.gameObject.SetActive(!under);
        hungerParent.gameObject.SetActive(!under);
        strengthText.gameObject.SetActive(!under);
        healthText.gameObject.SetActive(!under);
    }
}
