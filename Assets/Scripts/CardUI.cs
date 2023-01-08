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

    [Header("Card Art")]
    [SerializeField] protected Transform figureParent;

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
        descText?.gameObject.SetActive(!under);
        hungerParent?.gameObject.SetActive(!under);
        strengthText?.gameObject.SetActive(!under);
        healthText?.gameObject.SetActive(!under);
    }


    public Transform GetArtFigure()
    {
        return figureParent;
    }

    public void SetArtStand(float rot, float scale)
    {
        figureParent.localEulerAngles = new Vector3(rot, 0, 0);
        figureParent.localScale = new Vector3(scale, scale, scale);
    }

    public Coroutine ArtStandAnim(float targetRot, float targetScale, float duration)
    {
        if (!figureParent)
            return null;

        return StartCoroutine(DoArtStandAnim(targetRot, targetScale, duration));
    }

    private IEnumerator DoArtStandAnim(float targetRot, float targetScale, float duration)
    {
        float end = Time.time + duration;
        float startRot = figureParent.eulerAngles.x;
        if (targetRot > 180)
            targetRot -= 360;
        float startScale = figureParent.localScale.x;

        while (Time.time < end)
        {
            float t = 1 - (end - Time.time) / duration;
            figureParent.eulerAngles = new Vector3(startRot + (targetRot - startRot) * t, 0, 0);
            float s = startScale + (targetScale - startScale) * t;
            figureParent.localScale = new Vector3(s, Mathf.Abs(s), Mathf.Abs(s));
            yield return new WaitForEndOfFrame();
        }

        figureParent.eulerAngles = new Vector3(targetRot, 0, 0);
    }
}
