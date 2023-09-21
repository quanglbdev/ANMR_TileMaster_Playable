using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WinStreakDefinition", menuName = "ScriptableObjects/WinStreakDefinition", order = 1)]
public class WinStreakDefinition : ScriptableObject
{
    public int index;
    public int winStreak;
    public List<Reward> rewards;
}

[System.Serializable]
public class Reward
{
    public Config.SHOPITEM shopItemType;
    public int countItem;

    public Reward(Config.SHOPITEM shopItemType, int countItem)
    {
        this.shopItemType = shopItemType;
        this.countItem = countItem;
    }
}