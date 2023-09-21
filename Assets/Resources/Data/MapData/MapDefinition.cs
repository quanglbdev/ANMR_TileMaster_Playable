using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDefinition", menuName = "ScriptableObjects/MapDefinition", order = 1)]
public class MapDefinition : ScriptableObject
{
    public List<Map> maps = new();
    [System.Serializable]
    public class Map
    {
        public int mapId;
        public string mapName;
        public Sprite background;
        public List<ElementInMap> elements;
        public List<ConfigItemShopData> rewards;
    }
}



[System.Serializable]
public class ElementInMap
{
    public Config.BUILDING_TYPE buildingType;
    public List<int> price;
}