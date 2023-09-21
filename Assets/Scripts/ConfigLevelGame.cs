using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ConfigLevelGame
{
    [Header("TINH SAO")]
   
    public List<float> listScrore_Stars = new() { 50f, 70f, 85f };

    [Header("PHAN THUONG")]
    public List<int> listRewards_CoinValue = new() { 0, 10, 15, 20 };
}