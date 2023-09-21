using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using AppsFlyerSDK;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class Config
{
    public enum THEME_TYPE
    {
        fruits,
        flower,
        flower_2,
        test
    }

    public static bool isShowStarterPack = true;
    public static THEME_TYPE currTheme = THEME_TYPE.flower_2;

    public enum ITEM_TYPE
    {
        ITEM_1 = 1,
        ITEM_2 = 2,
        ITEM_3 = 3,
        ITEM_4 = 4,
        ITEM_5 = 5,
        ITEM_6 = 6,
        ITEM_7 = 7,
        ITEM_8 = 8,
        ITEM_9 = 9,
        ITEM_10 = 10,
        ITEM_11 = 11,
        ITEM_12 = 12,
        ITEM_13 = 13,
        ITEM_14 = 14,
        ITEM_15 = 15,
        ITEM_16 = 16,
        ITEM_17 = 17,
        ITEM_18 = 18,
        ITEM_19 = 19,
        ITEM_20 = 20
    }

    public enum OBSTACLE_TYPE
    {
        NONE = 0,
        CHAIN = 1,
        ICE = 2,
        GRASS = 3,
        CLOCK = 4,
        BOMB = 5,
        BEE = 6,
        GLUE_RIGHT = 7,
        GLUE_LEFT = 8
    }

    public enum START_MOVE_TYPE
    {
        TOP,
        LEFT,
        RIGHT,
        BOTTOM
    }

    public enum NEIGHBOR_TYPE
    {
        TOP,
        LEFT,
        RIGHT,
        BOTTOM
    }

    public enum LEVEL_DIFFICULTY
    {
        EASY,
        MEDIUM,
        HARD
    }

    public enum ITEMTILE_STATE
    {
        START,
        START_TO_FLOOR,
        FLOOR,
        HOVER,
        MOVE_TO_SLOT,
        MOVE_FROM_RETURN_FLOOR,
        SLOT,
        RETURN_FLOOR
    }

    public static List<START_MOVE_TYPE> listStartMoveType = new List<START_MOVE_TYPE>();

    public enum GAME_STATE
    {
        START,
        PLAYING,
        SHOP,
        PAUSE,
        END,
        WIN,
        LOSE,
        REWARD,
        TUTORIAL,
        BOOM
    }

    public enum ITEM_UNLOCK
    {
        COMBO,
        BUILDING,
        UNDO,
        SUGGEST,
        SHUFFLE,
        EXTRA_SLOT,
        TILE_RETURN,
        STAR_CHEST,
        PROFILE,
        GLUE,
        CHAIN,
        ICE,
        GRASS,
        CLOCK,
        BOMB,
        BEE
    }

    public enum UNLOCK_SHOW
    {
        NONE,
        WIN,
        START
    }

    public static GAME_STATE gameState = Config.GAME_STATE.START;

    public enum ITEMHELP_TYPE
    {
        UNDO,
        SUGGEST,
        SHUFFLE,
        EXTRA_SLOT,
        TILE_RETURN
    }

    public enum LEADERBOARD_TYPE
    {
        top_player,
        weekly,
    }

    public enum REWARD_STATE
    {
        BUY,
        MAP_COMPLETED
    }

    public enum SPIN_STATE
    {
        IDLE,
        READY_SPIN
    }

    public const float TARGET_ASPECT = 946f / 2045f;

    public static void SetCount_ItemHelp(ITEMHELP_TYPE itemHelpType, int count)
    {
        PlayerPrefs.SetInt(itemHelpType.ToString(), count);
        PlayerPrefs.Save();
    }

    public static void UseItemHelp(ITEMHELP_TYPE itemHelpType, int useCount = 1)
    {
        if (GetCount_ItemHelpFree(itemHelpType) > 0)
        {
            UseItemHelpFree(itemHelpType);
            return;
        }
        SetCount_ItemHelp(itemHelpType, GetCount_ItemHelp(itemHelpType) - useCount);
    }

    public static int GetCount_ItemHelp(ITEMHELP_TYPE itemHelpType)
    {
        return itemHelpType switch
        {
            ITEMHELP_TYPE.UNDO => PlayerPrefs.GetInt(itemHelpType.ToString(), 3),
            ITEMHELP_TYPE.SHUFFLE => PlayerPrefs.GetInt(itemHelpType.ToString(), 3),
            ITEMHELP_TYPE.SUGGEST => PlayerPrefs.GetInt(itemHelpType.ToString(), 3),
            ITEMHELP_TYPE.EXTRA_SLOT => PlayerPrefs.GetInt(itemHelpType.ToString(), 3),
            ITEMHELP_TYPE.TILE_RETURN => PlayerPrefs.GetInt(itemHelpType.ToString(), 3),
            _ => PlayerPrefs.GetInt(itemHelpType.ToString(), 4)
        };
    }
    
    public static void SetCount_ItemHelpFree(ITEMHELP_TYPE itemHelpType, int count)
    {
        PlayerPrefs.SetInt(itemHelpType.ToString() + "FREE", count);
        PlayerPrefs.Save();
    }
    public static void UseItemHelpFree(ITEMHELP_TYPE itemHelpType, int useCount = 1)
    {
        SetCount_ItemHelpFree(itemHelpType, GetCount_ItemHelpFree(itemHelpType) - useCount);
    }
    public static int GetCount_ItemHelpFree(ITEMHELP_TYPE itemHelpType)
    {
        return itemHelpType switch
        {
            ITEMHELP_TYPE.UNDO => PlayerPrefs.GetInt(itemHelpType.ToString() + "FREE", 1),
            ITEMHELP_TYPE.SHUFFLE => PlayerPrefs.GetInt(itemHelpType.ToString() + "FREE", 1),
            ITEMHELP_TYPE.SUGGEST => PlayerPrefs.GetInt(itemHelpType.ToString() + "FREE", 1),
            ITEMHELP_TYPE.EXTRA_SLOT => PlayerPrefs.GetInt(itemHelpType.ToString() + "FREE", 1),
            ITEMHELP_TYPE.TILE_RETURN => PlayerPrefs.GetInt(itemHelpType.ToString() + "FREE", 1),
            _ => PlayerPrefs.GetInt(itemHelpType.ToString(), 1)
        };
    }

    #region Daily Reward

    private const string DAILY_REWARD_CYCLE_COUNT_KEY = "daily_reward_cycle_count";

    public static int DAILY_REWARD_CYCLE_COUNT
    {
        get { return PlayerPrefs.GetInt(DAILY_REWARD_CYCLE_COUNT_KEY, 0); }
        set { PlayerPrefs.SetInt(DAILY_REWARD_CYCLE_COUNT_KEY, value); }
    }

    private const string DAILY_REWARD_INDEX_KEY = "daily_reward_index";

    public static int DAILY_REWARD_INDEX
    {
        get { return PlayerPrefs.GetInt(DAILY_REWARD_INDEX_KEY, 0); }
        set
        {
            if (value == 7)
            {
                DAILY_REWARD_CYCLE_COUNT++;
                value = 0;
            }

            PlayerPrefs.SetInt(DAILY_REWARD_INDEX_KEY, value);
        }
    }

    private const string DAILY_REWARD_TIME_KEY = "daily_time";

    public static DateTime DAILY_REWARD_TIME
    {
        get
        {
            return DateTime.Parse(PlayerPrefs.GetString(DAILY_REWARD_TIME_KEY,
                GetDateTimeNow().AddDays(-1).ToString()));
        }
        set { PlayerPrefs.SetString(DAILY_REWARD_TIME_KEY, value.ToString()); }
    }

    #endregion

    #region Event

    public enum EVENT
    {
        WIN_STREAK
    }

    public static List<EVENT> Events;

    private const string EVENTS_INDEX_KEY = "events_index";

    public static bool IsEvent
    {
        get
        {
            if (Events == null) return false;
            if (Events.Count == 0) return false;

            return true;
        }
    }

    public static void GetCurrentEvent()
    {
        var strEvents = PlayerPrefs.GetString(EVENTS_INDEX_KEY);
        var resp = JsonConvert.DeserializeObject<List<EVENT>>(strEvents);
        Events = resp;
    }

    public static void SetCurrentEvent(List<EVENT> events)
    {
        Events = events;
        var jsonString = JsonConvert.SerializeObject(events);
        PlayerPrefs.SetString(EVENTS_INDEX_KEY, jsonString);
    }

    #endregion

    #region WinStreak

    private const string WIN_STREAK_OLD_INDEX_KEY = "win_streak_old_index";

    public static int WIN_STREAK_OLD_INDEX
    {
        get { return PlayerPrefs.GetInt(WIN_STREAK_OLD_INDEX_KEY, 0); }
        set { PlayerPrefs.SetInt(WIN_STREAK_OLD_INDEX_KEY, value); }
    }

    private const string WIN_STREAK_INDEX_KEY = "win_streak_index";

    public static int WIN_STREAK_INDEX
    {
        get { return PlayerPrefs.GetInt(WIN_STREAK_INDEX_KEY, 0); }
        set
        {
            if (value == 0)
            {
                WIN_STREAK_OLD_INDEX = 0;
                WIN_STREAK_NEXT_INDEX = 2;
            }

            PlayerPrefs.SetInt(WIN_STREAK_INDEX_KEY, value);
        }
    }

    private const string WIN_STREAK_NEXT_INDEX_KEY = "win_streak_next_index";

    public static int WIN_STREAK_NEXT_INDEX
    {
        get { return PlayerPrefs.GetInt(WIN_STREAK_NEXT_INDEX_KEY, 2); }
        set { PlayerPrefs.SetInt(WIN_STREAK_NEXT_INDEX_KEY, value); }
    }

    private const string WIN_STREAK_EVENT_DATE_KEY = "win_streak_event_date";

    public static DateTime WIN_STREAK_EVENT_DATE
    {
        get { return DateTime.Parse(PlayerPrefs.GetString(WIN_STREAK_EVENT_DATE_KEY, $"{new DateTime(2000, 1, 1)}")); }
        set { PlayerPrefs.SetString(WIN_STREAK_EVENT_DATE_KEY, value.ToString()); }
    }

    private const string WIN_STREAK_EVENT_DATE_NEXT_KEY = "win_streak_event_date_next";

    public static DateTime WIN_STREAK_EVENT_DATE_NEXT
    {
        get
        {
            return DateTime.Parse(PlayerPrefs.GetString(WIN_STREAK_EVENT_DATE_NEXT_KEY, $"{new DateTime(2000, 1, 1)}"));
        }
        set { PlayerPrefs.SetString(WIN_STREAK_EVENT_DATE_NEXT_KEY, value.ToString()); }
    }

    private const string REWARDS_CLAIMED_WIN_STREAK_KEY = "reward_claimed_win_streak";

    public static List<int> RewardsClaimedWinStreak;

    public static void GetRewardsClaimedWinStreak()
    {
        if (!PlayerPrefs.HasKey(REWARDS_CLAIMED_WIN_STREAK_KEY))
        {
            SetRewardsClaimedWinStreak(new List<int>());
        }

        var srtRewards = PlayerPrefs.GetString(REWARDS_CLAIMED_WIN_STREAK_KEY);
        var resp = JsonConvert.DeserializeObject<List<int>>(srtRewards);
        RewardsClaimedWinStreak = resp;
    }

    private static void SetRewardsClaimedWinStreak(List<int> srtRewards)
    {
        RewardsClaimedWinStreak = srtRewards;
        var jsonString = JsonConvert.SerializeObject(srtRewards);
        PlayerPrefs.SetString(REWARDS_CLAIMED_WIN_STREAK_KEY, jsonString);
    }

    public static void AddRewardClaimedWinStreak(int id)
    {
        RewardsClaimedWinStreak.Add(id);
        SetRewardsClaimedWinStreak(RewardsClaimedWinStreak);
    }

    public static bool HasClaimRewardClaimedWinStreak(int id)
    {
        if (RewardsClaimedWinStreak == null) GetRewardsClaimedWinStreak();
        return RewardsClaimedWinStreak.Contains(id);
    }
    

    private const string ShowWinStreak = "is_show_win_streak";

    public static void SetShowWinStreak(bool isShow)
    {
        PlayerPrefs.SetInt(ShowWinStreak, isShow ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool GetShowWinStreak()
    {
        var soundInt = PlayerPrefs.GetInt(ShowWinStreak, 0);
        return soundInt == 1;
    }

    #endregion

    #region COIN

    public const string COIN = "coin";
    public static event Action<int> OnChangeCoin = delegate(int _coin) { };
    public static int currCoin;

    public static void SetCoin(int coinValue)
    {
        PlayerPrefs.SetInt(COIN, coinValue);
        PlayerPrefs.Save();
        currCoin = coinValue;
        OnChangeCoin(coinValue);
    }

    public static int GetCoin()
    {
        return PlayerPrefs.GetInt(COIN, 100);
    }

    public static int coinRiviveRequired = 250;

    #endregion

    #region HEART

    public const string HEART_KEY = "heart";
    public const int MAX_HEART = 5;
    public const int TIME_TO_RELOAD_HEART = 30;
    public const int HEART_REQUIRE_TO_PLAY = 1;
    public static event Action<int> OnChangeHeart = delegate(int _heart) { };
    public static int currHeart;

    public static void SetHeart(int heartValue)
    {
        if (heartValue > MAX_HEART)
            heartValue = MAX_HEART;
        PlayerPrefs.SetInt(HEART_KEY, heartValue);
        PlayerPrefs.Save();
        if (heartValue < currHeart && currHeart == 5)
            HEART_LAST_ADDED_TIME = GetDateTimeNow();
        currHeart = heartValue;
        OnChangeHeart(heartValue);
    }

    public static int GetHeart()
    {
        return PlayerPrefs.GetInt(HEART_KEY, MAX_HEART);
    }

    private static string HEART_RESET_KEY = "heart_reset";

    public static DateTime HEART_RESET
    {
        get { return DateTime.Parse(PlayerPrefs.GetString(HEART_RESET_KEY, GetDateTimeNow().ToString())); }
        set { PlayerPrefs.SetString(HEART_RESET_KEY, value.ToString()); }
    }

    private const string HEART_LAST_ADDED_TIME_KEY = "heart_last_time_added";

    public static DateTime HEART_LAST_ADDED_TIME
    {
        get
        {
            if (!PlayerPrefs.HasKey(HEART_LAST_ADDED_TIME_KEY))
                HEART_LAST_ADDED_TIME = GetDateTimeNow();
            return DateTime.Parse(PlayerPrefs.GetString(HEART_LAST_ADDED_TIME_KEY, GetDateTimeNow().ToString()));
        }
        set { PlayerPrefs.SetString(HEART_LAST_ADDED_TIME_KEY, value.ToString()); }
    }

    private const string FREE_HEART_KEY = "free_heart";

    public static bool FREE_HEART => FREE_HEART_TIME > 0;

    private const string FREE_HEART_TIME_KEY = "free_heard_time";

    public static int FREE_HEART_TIME
    {
        get { return PlayerPrefs.GetInt(FREE_HEART_TIME_KEY, 0); }
        set { PlayerPrefs.SetInt(FREE_HEART_TIME_KEY, value); }
    }

    private static string FREE_HEART_ADD_KEY = "free_heart_add";

    public static DateTime FREE_HEART_DATE_ADD
    {
        get
        {
            if (!PlayerPrefs.HasKey(FREE_HEART_ADD_KEY))
                FREE_HEART_DATE_ADD = GetDateTimeNow();
            return DateTime.Parse(PlayerPrefs.GetString(FREE_HEART_ADD_KEY, GetDateTimeNow().ToString()));
        }
        set { PlayerPrefs.SetString(FREE_HEART_ADD_KEY, value.ToString()); }
    }

    #endregion

    #region Star

    public const string STAR_KEY = "star";
    public static event Action<int> OnChangeStar = delegate(int _star) { };
    public static int currStar;

    public static void SetStar(int starValue)
    {
        PlayerPrefs.SetInt(STAR_KEY, starValue);
        PlayerPrefs.Save();
        currStar = starValue;
        OnChangeStar(starValue);
    }

    public static void SetStar()
    {
        SetStar(currBuildingStar + currEventStar + GetCountStars());
    }

    public static int GetStar()
    {
        return PlayerPrefs.GetInt(STAR_KEY, 0);
    }

    #endregion

    #region StarWeekly

    private const string STAR_RECEIVED_BEFORE_KEY = "star_received_before";

    public static int STAR_RECEIVED_BEFORE
    {
        get { return PlayerPrefs.GetInt(STAR_RECEIVED_BEFORE_KEY, 0); }
        set { PlayerPrefs.SetInt(STAR_RECEIVED_BEFORE_KEY, value); }
    }

    public static int STAR_WEEKLY
    {
        get
        {
            var resp = currStar - STAR_RECEIVED_BEFORE;
            return resp > 0 ? resp : 0;
        }
    }

    #endregion

    public static bool IsShowRewardWhenHowHome = false;
    public static int CoinAdd = 0;
    public static int HammerAdd = 0;

    #region Hammer

    public const string HAMMER_KEY = "Hammer";
    public static event Action<int> OnChangeHammer = delegate(int _hammer) { };
    public static int currHammer;

    public static void SetHammer(int hammerValue)
    {
        PlayerPrefs.SetInt(HAMMER_KEY, hammerValue);
        PlayerPrefs.Save();
        currHammer = hammerValue;
        OnChangeHammer(hammerValue);
        EventDispatcher.Instance.PostEvent(EventID.UpdateHammer);
    }

    public static int GetHammer()
    {
        return PlayerPrefs.GetInt(HAMMER_KEY, 0);
    }

    #endregion

    #region BUILDING

    public enum BUILDING_TYPE
    {
        HOUSE = 1,
        BRIDGE = 2,
        VEHICLE = 3,
        HUMAN = 4,
        TEMPLE = 5
    }

    private const string MAP_INDEX_KEY = "map_index";

    public static int MAP_INDEX
    {
        get { return PlayerPrefs.GetInt(MAP_INDEX_KEY, 1); }
        set { PlayerPrefs.SetInt(MAP_INDEX_KEY, value); }
    }

    private const string MAP_SELECTED_KEY = "map_selected";

    public static int MAP_SELECTED
    {
        get { return PlayerPrefs.GetInt(MAP_SELECTED_KEY, 1); }
        set { PlayerPrefs.SetInt(MAP_SELECTED_KEY, value); }
    }

    private const string TILE_SELECTED_KEY = "tile_selected";

    public static int TILE_SELECTED
    {
        get { return PlayerPrefs.GetInt(TILE_SELECTED_KEY, 1); }
        set { PlayerPrefs.SetInt(TILE_SELECTED_KEY, value); }
    }

    private const string ELEMENT_LEVEL_KEY = "element_level_";

    public static int GetCurrentElement(int elementId)
    {
        return PlayerPrefs.GetInt($"{ELEMENT_LEVEL_KEY}{MAP_INDEX}_{elementId}", 0);
    }

    public static void SetCurrentElement(int elementId, int level)
    {
        PlayerPrefs.SetInt($"{ELEMENT_LEVEL_KEY}{MAP_INDEX}_{elementId}", level);
        SetBuildingStar(++currBuildingStar);
    }

    #endregion

    #region Profile

    private const string AVATAR_KEY = "avatar_id";
    private const string NAME_KEY = "proflie_name";

    public static int AVATAR_ID
    {
        get { return PlayerPrefs.GetInt(AVATAR_KEY, 1); }
        set { PlayerPrefs.SetInt(AVATAR_KEY, value); }
    }

    public static string PROFILE_NAME
    {
        get
        {
            if (!PlayerPrefs.HasKey(LEVEL_STAR))
            {
                PROFILE_NAME = GenerateName(5);
            }

            return PlayerPrefs.GetString(NAME_KEY);
        }
        set { PlayerPrefs.SetString(NAME_KEY, value); }
    }

    #endregion

    #region SHOP

    public enum SHOPITEM
    {
        COIN,
        UNDO,
        SUGGEST,
        SHUFFLE,
        TILE_RETURN,
        EXTRA_SLOT,
        FREE_HEART,
        HEART,
        COMBO,
        STAR
    }

    public static void BuySuccess_ItemShop(ConfigItemShopData itemShopData)
    {
        switch (itemShopData.shopItemType)
        {
            case SHOPITEM.UNDO:
                SetCount_ItemHelp(ITEMHELP_TYPE.UNDO, GetCount_ItemHelp(ITEMHELP_TYPE.UNDO) + itemShopData.countItem);
                break;
            case SHOPITEM.SHUFFLE:
                SetCount_ItemHelp(ITEMHELP_TYPE.SHUFFLE,
                    GetCount_ItemHelp(ITEMHELP_TYPE.SHUFFLE) + itemShopData.countItem);
                break;
            case SHOPITEM.SUGGEST:
                SetCount_ItemHelp(ITEMHELP_TYPE.SUGGEST,
                    GetCount_ItemHelp(ITEMHELP_TYPE.SUGGEST) + itemShopData.countItem);
                break;
            case SHOPITEM.COIN:
                MenuManager.instance.AddCoinAnim(itemShopData.countItem);
                break;
            case SHOPITEM.EXTRA_SLOT:
                SetCount_ItemHelp(ITEMHELP_TYPE.EXTRA_SLOT,
                    GetCount_ItemHelp(ITEMHELP_TYPE.EXTRA_SLOT) + itemShopData.countItem);
                break;
            case SHOPITEM.TILE_RETURN:
                SetCount_ItemHelp(ITEMHELP_TYPE.TILE_RETURN,
                    GetCount_ItemHelp(ITEMHELP_TYPE.TILE_RETURN) + itemShopData.countItem);
                break;
            case SHOPITEM.FREE_HEART:
                if (FREE_HEART_TIME == -1)
                {
                    FREE_HEART_DATE_ADD = GetDateTimeNow();
                    FREE_HEART_TIME = itemShopData.countItem;
                }
                else
                {
                    FREE_HEART_TIME += itemShopData.countItem;
                }

                break;
            case SHOPITEM.HEART:
                SetHeart(currHeart + 1);
                break;
            case SHOPITEM.STAR:
                SetStar(currStar + 1);
                break;
            case SHOPITEM.COMBO:
                SetCount_ItemHelp(ITEMHELP_TYPE.UNDO, GetCount_ItemHelp(ITEMHELP_TYPE.UNDO) + 1);
                SetCount_ItemHelp(ITEMHELP_TYPE.SHUFFLE, GetCount_ItemHelp(ITEMHELP_TYPE.SHUFFLE) + 1);
                SetCount_ItemHelp(ITEMHELP_TYPE.SUGGEST, GetCount_ItemHelp(ITEMHELP_TYPE.SUGGEST) + 1);
                break;
        }
    }


    public static SHOPITEM ConvertItemHelpTypeToShopItem(ITEMHELP_TYPE itemHelpType)
    {
        return itemHelpType switch
        {
            ITEMHELP_TYPE.UNDO => SHOPITEM.UNDO,
            ITEMHELP_TYPE.SUGGEST => SHOPITEM.SUGGEST,
            ITEMHELP_TYPE.SHUFFLE => SHOPITEM.SHUFFLE,
            ITEMHELP_TYPE.EXTRA_SLOT => SHOPITEM.EXTRA_SLOT,
            ITEMHELP_TYPE.TILE_RETURN => SHOPITEM.TILE_RETURN,
            _ => SHOPITEM.TILE_RETURN
        };
    }

    public static int GetCountItem_Pack_ItemType(SHOPITEM shopItemType, ConfigPackData configPackData)
    {
        for (int i = 0; i < configPackData.configItemShopDatas.Count; i++)
        {
            if (configPackData.configItemShopDatas[i].shopItemType == shopItemType)
            {
                return configPackData.configItemShopDatas[i].countItem;
            }
        }

        return 0;
    }

    #endregion

    #region SOUND

    public const string SOUND = "sound";
    public static bool isSound = true;

    public static void SetSound(bool _isSound)
    {
        isSound = _isSound;
        if (_isSound)
        {
            PlayerPrefs.SetInt(SOUND, 1);
        }
        else
        {
            PlayerPrefs.SetInt(SOUND, 0);
        }

        PlayerPrefs.Save();
    }

    public static void GetSound()
    {
        int soundInt = PlayerPrefs.GetInt(SOUND, 1);
        if (soundInt == 1)
        {
            isSound = true;
        }
        else
        {
            isSound = false;
        }
    }

    #endregion

    #region MUSIC

    public const string MUSIC = "music";
    public static bool isMusic = true;

    public static void SetMusic(bool _isMusic)
    {
        isMusic = _isMusic;
        if (_isMusic)
        {
            PlayerPrefs.SetInt(MUSIC, 1);
        }
        else
        {
            MusicManager.Instance.StopMusicBackground();
            PlayerPrefs.SetInt(MUSIC, 0);
        }

        PlayerPrefs.Save();
    }

    public static void GetMusic()
    {
        int musicInt = PlayerPrefs.GetInt(MUSIC, 1);
        if (musicInt == 1)
        {
            isMusic = true;
        }
        else
        {
            isMusic = false;
        }
    }

    #endregion

    #region VIBRATION

    private const string VIBRATION = "vibration";
    public static bool isVibration = true;

    public static void SetVibration(bool _isVibration)
    {
        isVibration = _isVibration;
        if (_isVibration)
        {
            PlayerPrefs.SetInt(VIBRATION, 1);
        }
        else
        {
            PlayerPrefs.SetInt(VIBRATION, 0);
        }

        PlayerPrefs.Save();
    }

    public static void GetVibration()
    {
        int musicInt = PlayerPrefs.GetInt(VIBRATION, 1);
        if (musicInt == 1)
        {
            isVibration = true;
        }
        else
        {
            isVibration = false;
        }
    }

    #endregion

    #region DATE

    private const string LOGIN_KEY = "date_login";

    public static string LOGIN_DATE
    {
        get { return PlayerPrefs.GetString(LOGIN_KEY, GetDateTimeNow().AddDays(-1).ToString()); }
        set
        {
            PlayerPrefs.SetString(LOGIN_KEY, value);

            EventDispatcher.Instance.PostEvent(EventID.RestartDaily);

            var loginDate = DateTime.Parse(value);
            if (loginDate.DayOfWeek == DayOfWeek.Monday && RESTART_WEEKY == false)
                EventDispatcher.Instance.PostEvent(EventID.RestartWeekly);

            if (loginDate.DayOfWeek == DayOfWeek.Tuesday)
                RESTART_WEEKY = false;
        }
    }

    private const string RESTART_WEEKY_KEY = "restart_weeky";

    public static bool RESTART_WEEKY
    {
        get { return PlayerPrefs.GetInt(RESTART_WEEKY_KEY, 0) == 1; }
        set { PlayerPrefs.SetInt(RESTART_WEEKY_KEY, value ? 1 : 0); }
    }

    public static void RestartDaily()
    {
        //update fake leaderboard
        foreach (var data in AssetManager.Instance.LeaderboardDefinitions_TOP.Where(data => data.id != 0))
        {
            data.score += Random.Range(0, 30);
        }

        foreach (var data in AssetManager.Instance.LeaderboardDefinitions_WEEK.Where(data => data.id != 0))
        {
            data.score += Random.Range(0, 20);
        }
    }

    public static void RestartWeekly()
    {
        STAR_RECEIVED_BEFORE = currStar;

        var positionWeekly = AssetManager.Instance.GetPositionWeekly();
        RESTART_WEEKY = true;
        if (positionWeekly == 1)
        {
            //TODO:
        }

        if (positionWeekly == 2)
        {
            //TODO:
        }

        if (positionWeekly == 3)
        {
            //TODO:
        }

        AssetManager.Instance.ResetDataWeekly();
    }

    private static TimeSpan timeDifference;

    public static DateTime GetNetworkTime()
    {
#if DEVELOPMENT
        return DateTime.Now;
#endif
        //default Windows time server
        const string ntpServer = "time.windows.com";

        // NTP message size - 16 bytes of the digest (RFC 2030)
        var ntpData = new byte[48];

        //Setting the Leap Indicator, Version Number and Mode values
        ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)
        try
        {
            var addresses = Dns.GetHostEntry(ntpServer).AddressList;


            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);

            //NTP uses UDP

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);

                //Stops code hang if NTP is blocked
                socket.ReceiveTimeout = 1000;

                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }


            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime =
                (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            timeDifference = networkDateTime.ToLocalTime() - DateTime.Now;
            return networkDateTime.ToLocalTime();
        }
        catch
        {
            return DateTime.Now;
        }
    }

    static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                      ((x & 0x0000ff00) << 8) +
                      ((x & 0x00ff0000) >> 8) +
                      ((x & 0xff000000) >> 24));
    }

    public static DateTime GetDateTimeNow()
    {
#if DEVELOPMENT
        return DateTime.Now;
#endif
        return DateTime.Now + timeDifference;
    }

    #endregion

    #region SPIN_LASTTIME

    public static int SPIN_PRICE = 150;

    public static long GetTimeStamp()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(2020, 1, 1))).TotalSeconds;
    }

    public const int TIME_SPIN = 12 * 60 * 60;
    public const string SPIN_LASTTIME = "spin_lastTime";
    public static event Action OnChangeSpin_LastTime = delegate() { };

    public static void SetSpin_LastTime()
    {
        long timeStamp = GetTimeStamp();
        PlayerPrefs.SetString(SPIN_LASTTIME, timeStamp + "");
        var addHours = GetDateTimeNow().AddHours(12);
        SPIN_LAST = $"{addHours}";
        PlayerPrefs.Save();

        OnChangeSpin_LastTime();
    }

    private static string SPIN_FREE_COUNT_KEY = "spin_free_count";

    public static int SPIN_FREE_COUNT
    {
        get => PlayerPrefs.GetInt(SPIN_FREE_COUNT_KEY, 1);
        set { PlayerPrefs.SetInt(SPIN_FREE_COUNT_KEY, value); }
    }


    private static string SPIN_LAST_TIME = "spin_last_time";

    public static string SPIN_LAST
    {
        get => PlayerPrefs.GetString(SPIN_LAST_TIME, GetDateTimeNow().AddDays(-999).ToString());
        set
        {
            PlayerPrefs.SetString(SPIN_LAST_TIME, value);
            OnChangeSpin_LastTime();
        }
    }

    public static long GetSpinLastTime()
    {
        string lastTime = PlayerPrefs.GetString(SPIN_LASTTIME, "0");
        return long.Parse(lastTime);
    }

    public static bool CheckSpinAvailable()
    {
        long currTimeLastTime = Config.GetSpinLastTime();
        if (currTimeLastTime + Config.TIME_SPIN < Config.GetTimeStamp())
        {
            return true;
        }

        return false;
    }

    #endregion

    #region COUNTDOWNTIME

    public enum COUNTDOWN_TIME_TYPE
    {
        END
    }

    #endregion

    #region PIGGYBANK

    public const string PIGGYBANK = "piggyBank";
    public static int currPiggyBankCoin = 0;
    public static int ADD_PIGGYBANK_COIN = 2;
    public static int PIGGYBANK_COIN_MAX = 1000;
    public static int PIGGYBANK_COIN_MIN = 500;
    public static int PIGGYBANK_COIN_xVALUE = 1;
    public static event Action OnChange_PiggyBank_Coin = delegate() { };

    public static void AddPiggyBank_Coin()
    {
        if (currPiggyBankCoin <= PIGGYBANK_COIN_MAX)
        {
            currPiggyBankCoin += ADD_PIGGYBANK_COIN;
            SetPiggyBank(currPiggyBankCoin);
        }
    }

    public static void SetPiggyBank(int piggyBankCoin)
    {
        currPiggyBankCoin = piggyBankCoin;
        PlayerPrefs.SetInt(PIGGYBANK, piggyBankCoin);
        PlayerPrefs.Save();

        OnChange_PiggyBank_Coin();
    }

    public static int GetPiggyBank()
    {
        return PlayerPrefs.GetInt(PIGGYBANK, 0);
    }

    #endregion

    #region STAR_CHEST

    private const string CHEST_STAR = "chest_countStar";

    public static void SetChestCountStar(int countStar)
    {
        PlayerPrefs.SetInt(CHEST_STAR, countStar);
        PlayerPrefs.Save();
    }

    public static int GetChestCountStar()
    {
        return PlayerPrefs.GetInt(CHEST_STAR, 0);
    }

    public const int STEP_STAR_CHEST = 18;

    private const string STEP_CLAIM_STAR_CHEST_KEY = "step_claim_star_chest";

    public static int STEP_CLAIM_STAR_CHEST
    {
        get { return PlayerPrefs.GetInt(STEP_CLAIM_STAR_CHEST_KEY, 1); }
        set { PlayerPrefs.SetInt(STEP_CLAIM_STAR_CHEST_KEY, value); }
    }

    #endregion

    #region EVENT_STAR

    private const string EVENT_STAR = "event_star";
    public static int currEventStar;

    public static void SetEventStar(int _countStar)
    {
        PlayerPrefs.SetInt(EVENT_STAR, _countStar);
        currEventStar = _countStar;
        PlayerPrefs.Save();
        SetStar();
    }

    public static int GetEventStar()
    {
        return PlayerPrefs.GetInt(EVENT_STAR, 0);
    }

    #endregion

    #region BUILDIING_STAR

    private const string BUILDING_STAR = "building_star";
    public static int currBuildingStar;

    public static void SetBuildingStar(int _countStar)
    {
        PlayerPrefs.SetInt(BUILDING_STAR, _countStar);
        currBuildingStar = _countStar;
        PlayerPrefs.Save();
        SetStar();
    }

    public static int GetBuildingStar()
    {
        return PlayerPrefs.GetInt(BUILDING_STAR, 0);
    }

    #endregion

    #region LEVEL_STAR

    public const string LEVEL_STAR = "levelStar";
    public static Dictionary<int, int> currDic_LevelStars = new Dictionary<int, int>();

    public static void SetLevelStar(int level, int star)
    {
        if (currDic_LevelStars.ContainsKey(level))
        {
            if (currDic_LevelStars[level] < star)
            {
                currDic_LevelStars[level] = star;
            }
        }
        else
        {
            currDic_LevelStars.Add(level, star);
        }

        List<string> listStrLevel = new List<string>();
        foreach (KeyValuePair<int, int> kvp in currDic_LevelStars)
        {
            listStrLevel.Add(kvp.Key + "_" + kvp.Value);
        }

        PlayerPrefs.SetString(LEVEL_STAR, JsonMapper.ToJson(listStrLevel));
        PlayerPrefs.Save();
        SetStar();
    }

    public static void GetLevelStar()
    {
        currDic_LevelStars.Clear();
        if (PlayerPrefs.HasKey(LEVEL_STAR))
        {
            string strlevelStar = PlayerPrefs.GetString(LEVEL_STAR);

            JsonData jsonData = JsonMapper.ToObject(strlevelStar);

            for (int i = 0; i < jsonData.Count; i++)
            {
                string str = jsonData[i].ToString();
                string[] strLevelStar = str.Split('_');
                currDic_LevelStars.Add(int.Parse(strLevelStar[0]), int.Parse(strLevelStar[1]));
            }
        }
    }

    public static int LevelStar(int level)
    {
        var stars = 0;
        if (!currDic_LevelStars.TryGetValue(level, out stars))
        {
            stars = 0;
        }

        return stars;
    }

    public static int GetCountStars()
    {
        return currDic_LevelStars.Sum(kvp => kvp.Value);
    }
    #endregion

    #region FailCount

    private const string FAIL_COUNT_KEY = "fail_count";
    public static Dictionary<int, int> CurrDicFailCount = new();

    public static void SetFailCount(int level)
    {
        if (CurrDicFailCount.ContainsKey(level))
            CurrDicFailCount[level]++;
        else
            CurrDicFailCount.Add(level, 1);

        var listStrLevel = new List<string>();
        foreach (var (key, value) in CurrDicFailCount)
        {
            listStrLevel.Add(key + "_" + value);
        }

        PlayerPrefs.SetString(FAIL_COUNT_KEY, JsonMapper.ToJson(listStrLevel));
        PlayerPrefs.Save();
        SetStar();
    }

    public static void GetFailCount()
    {
        CurrDicFailCount.Clear();
        if (PlayerPrefs.HasKey(FAIL_COUNT_KEY))
        {
            var strLevelFail = PlayerPrefs.GetString(FAIL_COUNT_KEY);

            var jsonData = JsonMapper.ToObject(strLevelFail);

            for (var i = 0; i < jsonData.Count; i++)
            {
                var str = jsonData[i].ToString();
                var strLevelStar = str.Split('_');
                CurrDicFailCount.Add(int.Parse(strLevelStar[0]), int.Parse(strLevelStar[1]));
            }
        }
    }
    public static int FailCount(int level)
    {
        if (!CurrDicFailCount.TryGetValue(level, out var count))
        {
            count = 0;
        }

        return count;
    }

    #endregion

    #region CURR_LEVEL

    public static bool isHackMode = false;
    public const int MAX_LEVEL = 100;
    public static bool isSelectLevel = false;
    public static int currSelectLevel = 1;
    public const string CURR_LEVEL = "CURR_LEVEL";

    public static int currLevel = 1;

    public static void SetCurrLevel(int _currLevel)
    {
        if (_currLevel > currLevel)
        {
            if (currLevel == 11)
            {
                var checkFirstPack = DateTime.Parse(FIRST_TIME_PACK_DATE);
                if (checkFirstPack == new DateTime(1, 1, 1))
                {
                    var dateDateNow = GetDateTimeNow();
                    var expiredFirstTimePack = dateDateNow.AddMinutes(720);
                    FIRST_TIME_PACK_DATE = $"{expiredFirstTimePack}";
                }
            }

            currLevel = _currLevel;
            PlayerPrefs.SetInt(CURR_LEVEL, _currLevel);
            PlayerPrefs.Save();
        }
    }

    public static void GetCurrLevel()
    {
        currLevel = PlayerPrefs.GetInt(CURR_LEVEL, 1);
    }

    #endregion

    #region TUTORIAL

    private const string TUT_MATCH_3 = "tut_match_3";

    public static void SetTut_ClickTile_Finished()
    {
        PlayerPrefs.SetInt(TUT_MATCH_3, 1);
        PlayerPrefs.Save();
    }

    public static bool GetTut_ClickTile_Finished()
    {
        if (PlayerPrefs.GetInt(TUT_MATCH_3, 0) == 1)
        {
            return true;
        }

        return false;
    }


    public static bool CheckTutorial_Match3()
    {
        if (GamePlayManager.Instance.level == LEVEL_UNLOCK_TAP && !GetTut_ClickTile_Finished())
        {
            return true;
        }

        return false;
    }


    public static bool IsShowTutUndo = false;
    private const string TUT_UNDO = "tut_undo";

    public static void SetTut_2_Undo_Finished()
    {
        PlayerPrefs.SetInt(TUT_UNDO, 1);
        PlayerPrefs.Save();
    }

    private static bool GetTut_Undo_Finished()
    {
        if (PlayerPrefs.GetInt(TUT_UNDO, 0) == 1)
        {
            return true;
        }

        return false;
    }


    public static bool CheckTutorial_Undo()
    {
        if (GamePlayManager.Instance.level == LEVEL_UNLOCK_UNDO && !GetTut_Undo_Finished())
        {
            return true;
        }

        return false;
    }

    //TUt3
    public static bool isShowTut_suggest = false;
    public const string TUT_SUGGEST = "tut_suggest";

    public static void SetTut_Suggest_Finished()
    {
        PlayerPrefs.SetInt(TUT_SUGGEST, 1);
        PlayerPrefs.Save();
    }

    public static bool GetTut_Suggest_Finished()
    {
        if (PlayerPrefs.GetInt(TUT_SUGGEST, 0) == 1)
        {
            return true;
        }

        return false;
    }


    public static bool CheckTutorial_Suggest()
    {
        if (GamePlayManager.Instance.level == LEVEL_UNLOCK_SUGGEST && !GetTut_Suggest_Finished())
        {
            return true;
        }

        return false;
    }


    //TUt4
    public static bool IsShowTutShuffle = false;
    private const string TUT_SHUFFLE = "tut_shuflle";

    public static void SetTut_Shuffle_Finished()
    {
        PlayerPrefs.SetInt(TUT_SHUFFLE, 1);
        PlayerPrefs.Save();
    }

    public static bool GetTut_Shuffle_Finished()
    {
        if (PlayerPrefs.GetInt(TUT_SHUFFLE, 0) == 1)
        {
            return true;
        }

        return false;
    }


    public static bool CheckTutorial_Shuffle()
    {
        if (GamePlayManager.Instance.level == LEVEL_UNLOCK_SHUFFLE && !GetTut_Shuffle_Finished())
        {
            return true;
        }

        return false;
    }

//TUt5
    public static bool isShowTut5 = false;
    public const string TUT_EXTRA_SLOT = "tut_extra_slot";

    public static void SetTut_ExtraSlot_Finished()
    {
        PlayerPrefs.SetInt(TUT_EXTRA_SLOT, 1);
        PlayerPrefs.Save();
    }

    public static bool GetTut_ExtraSlot_Finished()
    {
        if (PlayerPrefs.GetInt(TUT_EXTRA_SLOT, 0) == 1)
        {
            return true;
        }

        return false;
    }


    public static bool CheckTutorial_ExtraSlot()
    {
        if (GamePlayManager.Instance.level == LEVEL_UNLOCK_EXTRA_SLOT && !GetTut_ExtraSlot_Finished())
        {
            return true;
        }

        return false;
    }

    //TUt6
    public static bool isShowTut_TileReturn = false;
    public const string TUT_TILE_RETURN = "tut_tile_return";

    public static void SetTut_TileReturn_Finished()
    {
        PlayerPrefs.SetInt(TUT_TILE_RETURN, 1);
        PlayerPrefs.Save();
    }

    public static bool GetTut_TileReturn_Finished()
    {
        if (PlayerPrefs.GetInt(TUT_TILE_RETURN, 0) == 1)
        {
            return true;
        }

        return false;
    }

    public static bool CheckTutorial_TileReturn()
    {
        if (GamePlayManager.Instance.level == LEVEL_UNLOCK_TILE_RETURN && !GetTut_TileReturn_Finished())
        {
            return true;
        }

        return false;
    }

    //Tut7
    private const string TUT_BUILDING = "tut_building";

    public static void SetTut_ClickTown_Finished()
    {
        PlayerPrefs.SetInt(TUT_BUILDING, 1);
        PlayerPrefs.Save();
    }

    private static bool GetTut_Building_Finished()
    {
        if (PlayerPrefs.GetInt(TUT_BUILDING, 0) == 1)
        {
            return true;
        }

        return false;
    }

    public static bool CheckTutorial_Building()
    {
        if (currLevel == LEVEL_SHOW_TUT_BUILDING && !GetTut_Building_Finished()) // lv3 unlock, lv4 shot tut
        {
            return true;
        }

        return false;
    }

    //Tut8
    private const string TUT_COMBO = "tut_combo";

    public static void SetTut_Combo_Finished()
    {
        PlayerPrefs.SetInt(TUT_COMBO, 1);
        PlayerPrefs.Save();
    }

    private static bool GetTut_Combo_Finished()
    {
        if (PlayerPrefs.GetInt(TUT_COMBO, 0) == 1)
        {
            return true;
        }

        return false;
    }

    public static bool CheckTutorial_Combo() //Combo
    {
        if (GamePlayManager.Instance.level == LEVEL_SHOW_TUT_COMBO && !GetTut_Combo_Finished())
        {
            return true;
        }

        return false;
    }

    //Tut_profile
    private const string TUT_PROFILE = "tut_profile";

    public static void SetTut_Profile_Finished()
    {
        PlayerPrefs.SetInt(TUT_PROFILE, 1);
        PlayerPrefs.Save();
    }

    private static bool GetTut_Profile_Finished()
    {
        if (PlayerPrefs.GetInt(TUT_PROFILE, 0) == 1)
        {
            return true;
        }

        return false;
    }

    public static bool CheckTutorial_Profile()
    {
        if (currLevel == LEVEL_SHOW_TUT_PROFILE && !GetTut_Profile_Finished())
        {
            return true;
        }

        return false;
    }

    //Tut_leaderboard
    private const string TUT_LEADERBOARD = "tut_leaderboard";

    public static void SetTut_Leaderboard_Finished()
    {
        PlayerPrefs.SetInt(TUT_LEADERBOARD, 1);
        PlayerPrefs.Save();
    }

    private static bool GetTut_Leaderboard_Finished()
    {
        if (PlayerPrefs.GetInt(TUT_LEADERBOARD, 0) == 1)
        {
            return true;
        }

        return false;
    }

    public static bool CheckTutorial_Leaderboard()
    {
        if (currLevel == LEVEL_SHOW_TUT_LEADERBOARD && !GetTut_Leaderboard_Finished())
        {
            return true;
        }

        return false;
    }

    //Tut_spin
    private const string TUT_SPIN = "tut_Spin";

    public static void SetTut_Spin_Finished()
    {
        PlayerPrefs.SetInt(TUT_SPIN, 1);
        PlayerPrefs.Save();
    }

    private static bool GetTut_Spin_Finished()
    {
        if (PlayerPrefs.GetInt(TUT_SPIN, 0) == 1)
        {
            return true;
        }

        return false;
    }

    public static bool CheckTutorial_Spin()
    {
        if (currLevel == LEVEL_SHOW_TUT_SPIN && !GetTut_Spin_Finished())
        {
            return true;
        }

        return false;
    }

    //
    private const string SHOW_SPECIAL_TILE = "show_special_tile_";

    public static void SetShowItemUnlockFinished(ITEM_UNLOCK itemUnlock)
    {
        PlayerPrefs.SetInt(SHOW_SPECIAL_TILE + itemUnlock, 1);
        PlayerPrefs.Save();
    }

    private static bool GetShowItemUnlockFinished(ITEM_UNLOCK itemUnlock)
    {
        return PlayerPrefs.GetInt(SHOW_SPECIAL_TILE + itemUnlock, 0) == 1;
    }

    public static bool CheckShowItemUnlockFinished(ITEM_UNLOCK itemUnlock)
    {
        return GamePlayManager.Instance.level == GetLevelUnlock(itemUnlock) && !GetShowItemUnlockFinished(itemUnlock);
    }

    private static int GetLevelUnlock(ITEM_UNLOCK itemUnlock)
    {
        return itemUnlock switch
        {
            ITEM_UNLOCK.GLUE => LEVEL_UNLOCK_GLUE,
            ITEM_UNLOCK.CHAIN => LEVEL_UNLOCK_CHAIN,
            ITEM_UNLOCK.ICE => LEVEL_UNLOCK_ICE,
            ITEM_UNLOCK.GRASS => LEVEL_UNLOCK_GLASS,
            ITEM_UNLOCK.CLOCK => LEVEL_UNLOCK_CLOCK,
            ITEM_UNLOCK.BOMB => LEVEL_UNLOCK_BOMB,
            ITEM_UNLOCK.BEE => LEVEL_UNLOCK_BEE
        };
    }

    public enum TUT
    {
        TUT_NEXTLEVEL_LEVEL1,
        TUT_TOUCHSPIN_LEVEL5
    }


    public static void SetTut_Finished(TUT tut)
    {
        PlayerPrefs.SetInt(tut.ToString(), 1);
        PlayerPrefs.Save();
    }

    public static bool GetTut_Finished(TUT tut)
    {
        if (PlayerPrefs.GetInt(tut.ToString(), 0) == 1)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region ADS_INTERSTITIAL

    public const int interstitialAd_levelShowAd = 2;
    public const int interstitialAd_SHOW_WIN_INTERVAL = 2;
    public const int interstitialAd_SHOW_LOSE_INTERVAL = 2;
    public static int interstitialAd_countWin = 0;
    public static int interstitialAd_countLose = 0;
    public static int interstitialAd_countRestart = 0;
    public const int interstitialAd_SHOW_PAUSE_INTERVAL = 1;
    public static int interstitialAd_countPause = 0;

    #endregion

    #region IAP

    public enum IAP_ID
    {
        first_time_pack,
        piggy_bank,
        removead,
        SeedPack,
        GrowthPack,
        FlowerBudPack,
        BloomPack,
        BeautifulPack,
        WonderfulPack,

        Coin1,
        Coin2,
        Coin3,
        Coin4,
        Coin5,
        Coin6,
    }

    #region IAP

    public const string IAP = "iap_";

    public static void SetBuyIAP(IAP_ID idPack)
    {
        PlayerPrefs.SetInt(IAP + idPack.ToString(), 1);
        PlayerPrefs.Save();
    }

    public static bool GetBuyIAP(IAP_ID idPack)
    {
        int buyed = PlayerPrefs.GetInt(IAP + idPack.ToString(), 0);
        if (buyed == 1) return true;
        return false;
    }

    #endregion

    #endregion

    #region RATE

    public const string RATE = "rate";

    public static void SetRate()
    {
        PlayerPrefs.SetInt(RATE, 1);
        PlayerPrefs.Save();
    }

    public static bool GetRate()
    {
        if (PlayerPrefs.GetInt(RATE, 0) == 1) return true;
        return false;
    }

    #endregion

    #region REMOVE_AD

    public const string REMOVE_AD = "remove_Ad";

    public static void SetRemoveAd()
    {
        PlayerPrefs.SetInt(REMOVE_AD, 1);
        PlayerPrefs.Save();
    }

    public static bool GetRemoveAd()
    {
        if (PlayerPrefs.GetInt(REMOVE_AD, 0) == 1) return true;
        return false;
    }

    #endregion

    #region AUTO

    public static float AUTO_TIME_TO_SUGGEST_SHUFFLE = 10f;

    #endregion

    public static string FormatTime(int time)
    {
        int hour = time / 3600;
        int minus = (time - hour * 3600) / 60;
        int second = time - hour * 3600 - minus * 60;
        if (hour == 0)
        {
            return $"{minus:00}:{second:00}";
        }

        return $"{hour:00}:{minus:00}:{second:00}";
    }

    #region UNLOCK_ITEM_BY_LEVEL

    private const string UNLOCK_ITEM = "unlock_item";

    public static void SetUnLockLevel(int level)
    {
        PlayerPrefs.SetInt(UNLOCK_ITEM, level);
        PlayerPrefs.Save();
    }

    public static int GetUnLockLevel()
    {
        return PlayerPrefs.GetInt(UNLOCK_ITEM, LEVEL_UNLOCK_BUILDING);
    }

    #endregion

    #region DAILY_FREECOIN

    public const string DAILY_FREE_COIN = "daily_freecoin";

    public static void SetDaily_FreeCoin()
    {
        PlayerPrefs.SetInt(DAILY_FREE_COIN, Config.GetCurrDayTime());
        PlayerPrefs.Save();
    }

    public static int GetDaily_FreeCoin()
    {
        return PlayerPrefs.GetInt(DAILY_FREE_COIN, 0);
    }

    public static bool CheckDaily_FreeCoin()
    {
        return Config.GetCurrDayTime() > GetDaily_FreeCoin();
    }

    #endregion

    #region DAILY_VIDEOCOIN

    public const string DAILY_VIDEO_COIN = "daily_videocoin";
    public const string DAILY_VIDEO_COIN_COUNT = "daily_videocoin_count";
    public const int DAILY_VIDEO_COIN_MAX = 10;

    public static void SetDaily_VideoCoin()
    {
        PlayerPrefs.SetInt(DAILY_VIDEO_COIN, Config.GetCurrDayTime());
        PlayerPrefs.Save();
    }

    public static int GetDaily_VideoCoin()
    {
        return PlayerPrefs.GetInt(DAILY_VIDEO_COIN, 0);
    }

    public static bool CheckDaily_VideoCoin()
    {
        return Config.GetCurrDayTime() > GetDaily_VideoCoin();
    }

    public static void SetDaily_VideoCoin_AddCount()
    {
        int currCount = GetDaily_VideoCoin_Count();
        if (currCount > 0)
        {
            SetDaily_VideoCoin_Count(currCount - 1);
        }
    }

    public static void SetDaily_VideoCoin_Count(int count)
    {
        PlayerPrefs.SetInt(DAILY_VIDEO_COIN_COUNT, count);
        PlayerPrefs.Save();
    }

    public static int GetDaily_VideoCoin_Count()
    {
        return PlayerPrefs.GetInt(DAILY_VIDEO_COIN_COUNT, DAILY_VIDEO_COIN_MAX);
    }

    #endregion


    public static int GetCurrDayTime()
    {
        int currDayTime = DateTime.Now.DayOfYear + DateTime.Now.Year * 10000;
        return currDayTime;
    }


    public const int PRICE_COIN_REVIVE = 100;

    #region DAILY_FREEREVIVE

    public const string DAILY_FREE_REVIVE = "daily_freerevive";

    public static void SetDaily_FreeRevive()
    {
        PlayerPrefs.SetInt(DAILY_FREE_REVIVE, Config.GetCurrDayTime());
        PlayerPrefs.Save();
    }

    public static int GetDaily_FreeRevive()
    {
        return PlayerPrefs.GetInt(DAILY_FREE_REVIVE, 0);
    }

    public static bool CheckDaily_FreeRevive()
    {
        // return Config.GetCurrDayTime() > GetDaily_FreeRevive();
        return true;
    }

    #endregion

    #region FirstTimePack

    private const string FIRST_TIME_PACK_DATE_KEY = "first_time_pack_time";

    public static string FIRST_TIME_PACK_DATE
    {
        get
        {
            if (!PlayerPrefs.HasKey(FIRST_TIME_PACK_DATE_KEY))
            {
                var time = new DateTime(1, 1, 1);
                PlayerPrefs.SetString(FIRST_TIME_PACK_DATE_KEY, time.ToString());
                return time.ToString();
            }

            return PlayerPrefs.GetString(FIRST_TIME_PACK_DATE_KEY, GetDateTimeNow().AddDays(-1).ToString());
        }
        set
        {
            PlayerPrefs.SetString(FIRST_TIME_PACK_DATE_KEY, value);
            PlayerPrefs.Save();
        }
    }

    private const string IS_SHOW_FIRST_TIME_PACK_KEY = "is_show_first_time_pack";

    public static bool IS_SHOW_FIRST_TIME_PACK
    {
        get { return PlayerPrefs.GetInt(IS_SHOW_FIRST_TIME_PACK_KEY, 1) == 1; }
        set { PlayerPrefs.SetInt(IS_SHOW_FIRST_TIME_PACK_KEY, value ? 1 : 0); }
    }

    private const string IS_SHOW_FIRST_TIME_KEY = "is_show_first_time";

    public static bool IS_SHOW_FIRST_TIME
    {
        get { return PlayerPrefs.GetInt(IS_SHOW_FIRST_TIME_KEY, 1) == 1; }
        set { PlayerPrefs.SetInt(IS_SHOW_FIRST_TIME_KEY, value ? 1 : 0); }
    }

    #endregion

    #region LevelUnlock

    //unlock at level
    public const int LEVEL_UNLOCK_TAP = 1;
    public const int LEVEL_UNLOCK_COMBO = 2;
    public const int LEVEL_UNLOCK_BUILDING = 3;
    public const int LEVEL_UNLOCK_UNDO = 5;
    public const int LEVEL_UNLOCK_SHUFFLE = 6;
    public const int LEVEL_UNLOCK_TILE_RETURN = 7;
    public const int LEVEL_UNLOCK_SUGGEST = 8;
    public const int LEVEL_UNLOCK_EXTRA_SLOT = 9;
    public const int LEVEL_UNLOCK_STAR_CHEST = 10;

    public const int LEVEL_UNLOCK_GLUE = 4;
    public const int LEVEL_UNLOCK_CHAIN = 14;
    public const int LEVEL_UNLOCK_ICE = 24;
    public const int LEVEL_UNLOCK_GLASS = 34;
    public const int LEVEL_UNLOCK_CLOCK = 44;
    public const int LEVEL_UNLOCK_BOMB = 64;
    public const int LEVEL_UNLOCK_BEE = 84;

    public const int LEVEL_UNLOCK_EVENT = 20;


    //show tut at level
    public static int LEVEL_SHOW_TUT_COMBO => LEVEL_UNLOCK_COMBO;
    public static int LEVEL_SHOW_TUT_BUILDING => LEVEL_UNLOCK_BUILDING + 1;
    public static int LEVEL_SHOW_TUT_SPIN = 10;
    public const int LEVEL_SHOW_TUT_PROFILE = 11;
    public static int LEVEL_SHOW_TUT_LEADERBOARD = 15;
    public const int LEVEL_CAN_AI_SUGGEST = 11;

    #endregion

    public static string GenerateName(int len)
    {
        var r = new System.Random();
        string[] consonants =
        {
            "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w",
            "x"
        };
        string[] vowels = { "a", "e", "i", "o", "u", "y" };
        string Name = "";
        Name += consonants[r.Next(consonants.Length)].ToUpper();
        Name += vowels[r.Next(vowels.Length)];
        int
            b = 2;
        while (b < len)
        {
            Name += consonants[r.Next(consonants.Length)];
            b++;
            Name += vowels[r.Next(vowels.Length)];
            b++;
        }

        return Name;
    }

    public static string GenerateUUID()
    {
        return Guid.NewGuid().ToString();
    }

    public static string GetShortUUID(string UUID)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(UUID);
        var hashBytes = md5.ComputeHash(inputBytes);
        var sb = new StringBuilder();

        foreach (var hashByte in hashBytes)
        {
            sb.Append(hashByte.ToString("x2"));
        }

        return sb.ToString().Substring(0, 10);
    }

    public static string ConvertSecondsToMinutesAndSeconds(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;

        // Format the time as a string in the "m:ss" format
        string formattedTime = $"{minutes}:{remainingSeconds:D2}";

        return formattedTime;
    }

    #region Version

    private static string PROJECT_VERSION_KEY = "project_version";

    public static string PROJECT_VERSION
    {
        get { return PlayerPrefs.GetString(PROJECT_VERSION_KEY); }
        set { PlayerPrefs.SetString(PROJECT_VERSION_KEY, value); }
    }

    private static string USER_ID_KEY = "user_ID";

    public static string USER_ID
    {
        get
        {
            if (PlayerPrefs.GetString(USER_ID_KEY).Length < 1)
            {
                PlayerPrefs.SetString(USER_ID_KEY, GenerateUUID());
            }

            return PlayerPrefs.GetString(USER_ID_KEY);
        }
    }

    #endregion
    
    public static void LogByAppsflyer(string eventName)
    {
#if !DEVELOPMENT
        //if (MAX_LEVEL) return;
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        AppsFlyer.sendEvent(eventName, eventValues);
#endif
    }

    public static void LogByAppsflyer(string eventName, Dictionary<string, string> parameter)
    {
#if !DEVELOPMENT
        //if (MAX_LEVEL) return;
        Dictionary<string, string> eventValues = parameter;
        AppsFlyer.sendEvent(eventName, eventValues);
#endif
    }
}