using UnityEngine;

[CreateAssetMenu(fileName = "BoosterDefinition",menuName ="ScriptableObjects/BoosterDefinition", order = 1)]
public class BoosterDefinition : ScriptableObject
{
    public string boosterName;
    public Config.ITEMHELP_TYPE itemHelpType;
    public int price;
    public int amount;
    public float frontSize;
    public string description;
    public Sprite sprite;
}
