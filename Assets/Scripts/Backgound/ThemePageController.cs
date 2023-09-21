using System.Collections.Generic;
using UnityEngine;

public class ThemePageController : MonoBehaviour
{
    [SerializeField] private ThemeGridItem themeGridItem;
    private List<ThemeGridItem> _themeGridItems = new();
    private readonly List<ThemeGirdData> _themesData = new();
    public void Init(List<ThemeGirdData> themesData)
    {
        _themesData.AddRange(themesData);
        foreach (var data in themesData)
        {
            var gridItem = Instantiate(themeGridItem, transform);
            gridItem.Init(data.index,data.mapName, data.sprite, this);
            
            _themeGridItems.Add(gridItem);
        }
    }
    public void Restart()
    {
        for (var i = 0; i < _themesData.Count; i++)
        {
            var data = _themesData[i];
            _themeGridItems[i].Init(data.index,data.mapName, data.sprite, this);
        }
    }
}


public class ThemeGirdData
{
    public int index;
    public string mapName;
    public Sprite sprite;

    public ThemeGirdData(int index, string mapName, Sprite sprite)
    {
        this.index = index;
        this.mapName = mapName;
        this.sprite = sprite;
    }
}