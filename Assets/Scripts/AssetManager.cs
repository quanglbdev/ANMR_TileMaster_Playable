using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

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

    public List<UnlockNewItemData> unlockNewItemData;
    public TileDataDefinition tileDataDefinition;

    public List<WinStreakDefinition> winStreakDefinitions;

    public List<BoosterDefinition> boosterDefinitions;
    public List<AvatarDefinition> avatarDefinitions;
    public LeaderboardDummyData leaderboardDummyData_WEEK;
    public LeaderboardDummyData leaderboardDummyData_TOP;

    public List<TileDefinition> tilesDefinition;

    [Header("Tile - SPRITE ATLAS")] [SerializeField]
    private SpriteAtlas flower, fruits, flower2;

    private SpriteAtlas _currSpriteAtlas;

    #region Leaderboard

    public List<UserRankSO> LeaderboardDefinitions_WEEK
    {
        get
        {
            var myData = leaderboardDummyData_WEEK.leaderboardDefinitions.Find(x => x.id == 0);
            if (myData == null)
            {
                myData = new UserRankSO(0, Config.PROFILE_NAME, Config.STAR_WEEKLY, Config.AVATAR_ID);
                leaderboardDummyData_WEEK.leaderboardDefinitions.Add(myData);
            }
            else
            {
                foreach (var definition in leaderboardDummyData_WEEK.leaderboardDefinitions.Where(definition =>
                             definition.id == 0))
                {
                    definition.score = Config.STAR_WEEKLY;
                    definition.playerName = Config.PROFILE_NAME;
                    definition.avatarId = Config.AVATAR_ID;
                }
            }

            return leaderboardDummyData_WEEK.leaderboardDefinitions;
        }
    }

    public List<UserRankSO> LeaderboardDefinitions_TOP
    {
        get
        {
            var myData = leaderboardDummyData_TOP.leaderboardDefinitions.Find(x => x.id == 0);
            if (myData == null)
            {
                myData = new UserRankSO(0, Config.PROFILE_NAME, Config.GetStar(), Config.AVATAR_ID);
                leaderboardDummyData_TOP.leaderboardDefinitions.Add(myData);
            }
            else
            {
                foreach (var definition in leaderboardDummyData_TOP.leaderboardDefinitions.Where(definition =>
                             definition.id == 0))
                {
                    definition.score = Config.GetStar();
                    definition.playerName = Config.PROFILE_NAME;
                    definition.avatarId = Config.AVATAR_ID;
                }
            }

            return leaderboardDummyData_TOP.leaderboardDefinitions;
        }
    }


    public void ResetDataWeekly()
    {
        leaderboardDummyData_WEEK.Clear();
        for (var i = 1; i < 101; i++)
        {
            leaderboardDummyData_WEEK.RecruitData(new($"{i}", Config.GenerateName(Random.Range(3, 6)),
                Random.Range(5, 100)));
        }

        leaderboardDummyData_TOP.SetDirty();
    }

    public void ResetDataTopPlayer()
    {
        leaderboardDummyData_TOP.Clear();
        for (var i = 1; i < 101; i++)
        {
            leaderboardDummyData_TOP.RecruitData(new($"{i}", Config.GenerateName(Random.Range(3, 6)),
                Random.Range(2000, 5000)));
        }

        leaderboardDummyData_TOP.SetDirty();
    }

    public int GetPositionWeekly()
    {
        var sortingList = LeaderboardDefinitions_WEEK;
        var temp = sortingList.OrderByDescending(x => x.score).ToList();

        for (var i = 0; i < temp.Count; i++)
        {
            if (temp[i].id == 0)
                return i + 1;
        }

        return 0;
    }

    [Header("Sprite")] [SerializeField] private Sprite trophy1, trophy2, trophy3;
    [SerializeField] private Sprite gift1, gift2, gift3;

    public Sprite GetSpriteTrophy(int index)
    {
        return index switch
        {
            0 => trophy1,
            1 => trophy2,
            2 => trophy3,
            _ => null
        };
    }

    public Sprite GetSpriteGiftBox(int index)
    {
        return index switch
        {
            0 => gift1,
            1 => gift2,
            2 => gift3,
            _ => null
        };
    }

    #endregion

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

    public AvatarDefinition GetAvatarDefinition(int avatarId)
    {
        return avatarDefinitions.Find(x => x.avatarId == avatarId);
    }

    public int GetLastItemUnlock()
    {
        return unlockNewItemData.LastOrDefault()!.unlockLevel;
    }

    public UnlockNewItemData GetItemUnlockDefinition(int level)
    {
        return unlockNewItemData.Find(x => x.unlockLevel == level);
    }

    public UnlockNewItemData GetItemUnlockDefinition(Config.ITEM_UNLOCK itemUnlock)
    {
        return unlockNewItemData.Find(x => x.itemUnlock == itemUnlock);
    }

    public UnlockNewItemData GetCurrentItemUnlock()
    {
        var currentLevel = unlockNewItemData.Find(x => x.unlockLevel == Config.GetUnLockLevel());
        var lastIndexOf = unlockNewItemData.LastIndexOf(currentLevel);
        if (lastIndexOf == unlockNewItemData.Count - 1)
            return null;

        return unlockNewItemData[lastIndexOf + 1];
    }

    public List<ItemData> GetTileDataDefinition()
    {
        return tileDataDefinition.tilesData;
    }

    public WinStreakDefinition GetWinStreakDefinition(int winStreak)
    {
        var index = winStreak;
        WinStreakDefinition resp;
        while (true)
        {
            resp = winStreakDefinitions.Find(x => x.winStreak == index);
            if (resp != null) return resp;
            index--;
        }
    }

    public List<WinStreakDefinition> GetWinStreakDefinitions()
    {
        return winStreakDefinitions;
    }

    public int GetWinStreakDefinitionsCount()
    {
        return winStreakDefinitions.Count;
    }


    public override void Awake()
    {
        base.Awake();

        if (leaderboardDummyData_WEEK.leaderboardDefinitions.Count == 0)
        {
            ResetDataWeekly();
        }

        if (leaderboardDummyData_TOP.leaderboardDefinitions.Count == 0)
        {
            ResetDataTopPlayer();
        }

        EventDispatcher.Instance.RegisterListener(EventID.RestartDaily, (param) => Config.RestartDaily());
        EventDispatcher.Instance.RegisterListener(EventID.RestartWeekly, (param) => Config.RestartWeekly());

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

    [Button]
    public void TestWeekly()
    {
        Config.RestartWeekly();
    }
}