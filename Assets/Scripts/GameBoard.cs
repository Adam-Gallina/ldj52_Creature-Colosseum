using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance;

    public BoardField P1Field;
    public BoardField P2Field;

    private void Awake()
    {
        Instance = this;
    }

    public BoardField GetPlayerField(PlayerNumber p)
    {
        return p == PlayerNumber.P1 ? P1Field : P2Field;
    }

    public BoardField GetOpposingField(PlayerNumber p)
    {
        return p == PlayerNumber.P1 ? P2Field : P1Field;
    }
}
