using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ConfigLevelGame
{
    [Header("TINH SAO")]
    [InfoBox("Ở ngưỡng <50% (của điểm tối đa theo màn chơi) người chơi sẽ được 0 sao")]
    [InfoBox("Ở ngưỡng >=50% người chơi sẽ được 1 sao")]
    [InfoBox("Ở ngưỡng >=650% người chơi sẽ được 2 sao")]
    [InfoBox("Ở ngưỡng >=80% người chơi sẽ được 3 sao")]
    public List<float> listScrore_Stars = new() { 50f, 70f, 85f };

    [Header("PHAN THUONG")] [InfoBox("1sao - 10vang,2sao-15vang,3sao-20vang")]
    public List<int> listRewards_CoinValue = new() { 0, 10, 15, 20 };
}