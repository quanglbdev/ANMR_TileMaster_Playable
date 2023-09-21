using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelDefinition", menuName = "ScriptableObjects/LevelDefinition", order = 1)]
public class LevelDefinition : ScriptableObject
{
    public Config.LEVEL_DIFFICULTY difficulty;
    public List<DataTileInFloor> dataTileInFloor = new();
    public int dataAmountOnMap;
    public int secondsRequired;

}

[Serializable]
public class DataTileInFloor
{
    public int floor;
    public List<ItemTileData> data;

        public DataTileInFloor(int floor, List<ItemTileData> data)
    {
        this.floor = floor;
        this.data = data;
    }
}