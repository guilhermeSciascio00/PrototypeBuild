using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public int sceneTotalCoinsAmount;//feito
    public int collectedCoinsAmount;//feito
    public List<GameObject> collectedCoins = new List<GameObject>();//feito
    public Vector3 playerPos;//feito
    public DateTime savedTime;//feito
    public string inputActionMap;//feito

    public GameData()
    {
        collectedCoinsAmount = 0;
        sceneTotalCoinsAmount = 0;
        collectedCoins = new List<GameObject>();
        playerPos = new Vector3();
        savedTime = DateTime.Now;
    }
}
