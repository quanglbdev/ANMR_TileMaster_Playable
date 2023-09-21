using System.Collections.Generic;
using UnityEngine;

public class LevelPage : MonoBehaviour
{
    [SerializeField] private ItemLevel levelsGridItem;
    private List<ItemLevel> _levelsGridItems = new();
    private readonly List<InfoLevel> _levelsData = new();
    
    public void Init(List<InfoLevel> themesData)
    {
        _levelsData.AddRange(themesData);
        foreach (var data in themesData)
        {
            var gridItem = Instantiate(levelsGridItem, transform);
            gridItem.SetInitItemLevel(data);
            
            _levelsGridItems.Add(gridItem);
        }
    }
}
