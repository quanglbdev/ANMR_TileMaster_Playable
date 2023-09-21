using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockNewItemData", menuName = ("ScriptableObjects/UnlockNewItemData"))]
public class UnlockNewItemData : ScriptableObject
{
    public int unlockLevel;
    public Sprite sprite;
    public Config.ITEM_UNLOCK itemUnlock;
    public Config.UNLOCK_SHOW unlockShow;
    public bool isSpecialTile;
}
