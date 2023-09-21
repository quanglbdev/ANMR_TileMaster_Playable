using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDataDefinition", menuName = "ScriptableObjects/ItemData/TileDataDefinition", order = 1)]

public class TileDataDefinition : ScriptableObject
{
    public List<ItemData> tilesData;
}
