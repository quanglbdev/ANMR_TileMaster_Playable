using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePageController : MonoBehaviour
{
    [SerializeField] private TileGridItem themeGridItem;
    private List<TileGridItem> _themeGridItems = new();
    private readonly List<TileGirdData> _tilesData = new();
    public void Init(List<TileGirdData> tilesData)
    {
        _tilesData.AddRange(tilesData);
        foreach (var data in tilesData)
        {
            var gridItem = Instantiate(themeGridItem, transform);
            gridItem.Init(data.index, data.sprite,data.themeType ,this);
            
            _themeGridItems.Add(gridItem);
        }
    }

    public void Restart()
    {
        for (var i = 0; i < _tilesData.Count; i++)
        {
            var data = _tilesData[i];
            _themeGridItems[i].Init(data.index, data.sprite,data.themeType ,this);
        }
    }
}
public class TileGirdData
{
    public int index;
    public Sprite sprite;
    public Config.THEME_TYPE themeType;

    public TileGirdData(int index, Sprite sprite, Config.THEME_TYPE themeType)
    {
        this.index = index;
        this.sprite = sprite;
        this.themeType = themeType;
    }
}