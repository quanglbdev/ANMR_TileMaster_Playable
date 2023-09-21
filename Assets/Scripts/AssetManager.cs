using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AssetManager : Singleton<AssetManager>
{
    [Header("Tile - Prefab")] [SerializeField]
    public ItemTile itemTile;

    public ChainTile chainTile;
    public IceTile iceTile;
    public GrassTile grassTile;
    public ItemTile bombTile;
    public BeeTile beeTile;
    public GlueTile glueTile;

    [Header("Data")] public ObstacleDefinition obstacleDefinition;
    public MapDefinition mapDefinition;

    public TileDataDefinition tileDataDefinition;


    public List<BoosterDefinition> boosterDefinitions;

    public List<TileDefinition> tilesDefinition;

    [Header("Tile - SPRITE ATLAS")] [SerializeField]
    private SpriteAtlas flower, fruits, flower2;

    private SpriteAtlas _currSpriteAtlas;


    #region Building

    public int GetPriceElement(Config.BUILDING_TYPE type, int mapIndex, int level)
    {
        if (level > 2)
            level = 2;
        var currentMap = mapDefinition.maps.Find(x => x.mapId == mapIndex);
        if (currentMap == null)
            return 1;
        var element = currentMap.elements.Find(x => x.buildingType == type);
        if (element == null)
            return 1;
        return element.price[level];
    }

    public MapDefinition.Map GetMapDefinition(int mapIndex)
    {
        return mapDefinition.maps.Find(x => x.mapId == mapIndex);
    }

    public int GetMapCount()
    {
        return mapDefinition.maps.Count;
    }

    #endregion

    public BoosterDefinition GetBoosterDefinition(Config.ITEMHELP_TYPE type)
    {
        return boosterDefinitions.Find(x => x.itemHelpType == type);
    }

    public List<ItemData> GetTileDataDefinition()
    {
        return tileDataDefinition.tilesData;
    }


    public override void Awake()
    {
        base.Awake();


        var loginDate = DateTime.Parse(Config.LOGIN_DATE);
        if (loginDate.Date < Config.GetDateTimeNow().Date)
        {
            Config.LOGIN_DATE = $"{Config.GetDateTimeNow()}";
        }


#if FREE_HEART
        Config.FREE_HEART_TIME = 30;
        Config.FREE_HEART_DATE_ADD = Config.GetDateTimeNow();
#endif
    }

    private SpriteAtlas GetTile(Config.THEME_TYPE type)
    {
        return type switch
        {
            Config.THEME_TYPE.fruits => fruits,
            Config.THEME_TYPE.flower => flower,
            Config.THEME_TYPE.flower_2 => flower2,
        };
    }

    public void SetTile(Config.THEME_TYPE type)
    {
        _currSpriteAtlas = GetTile(type);
    }

    public Sprite GetTile(string tileName)
    {
        return _currSpriteAtlas.GetSprite(tileName);
    }
}