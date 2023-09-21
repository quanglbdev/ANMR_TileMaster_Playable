using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDefinition", menuName = "ScriptableObjects/TileDefinition")]
public class TileDefinition : ScriptableObject
{
    public int id;
    public Config.THEME_TYPE themeType;
    public Sprite sprite;
}
