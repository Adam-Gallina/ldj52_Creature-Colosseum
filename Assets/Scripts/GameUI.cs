using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [SerializeField] private TMP_Text P1HealthText;
    [SerializeField] private TMP_Text P2HealthText;
    
    [SerializeField] private TMP_Text BannerText;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        P1HealthText.text = "P1: " + GameController.Instance.P1.CurrLife;
        P2HealthText.text = "P2: " + GameController.Instance.P2.CurrLife;
    }

    public void PressEndTurn()
    {
        GameController.Instance.EndTurn();
    }

    public Coroutine SetBannerText(string text, float time)
    {
        return StartCoroutine(BannerTextAnim(text, time));
    }

    private IEnumerator BannerTextAnim(string text, float time)
    {
        BannerText.text = text;
        yield return new WaitForSeconds(time);
    }
}
