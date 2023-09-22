using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
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

    private static TimeSpan timeDifference;

    public static DateTime GetNetworkTime()
    {
        return DateTime.Now;
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
        return DateTime.Now;
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

    #region CURR_LEVEL

    public const int MAX_LEVEL = 20;
    public static int currSelectLevel = 1;
    public const string CURR_LEVEL = "CURR_LEVEL";

    public static int currLevel = 1;

    public static void SetCurrLevel(int _currLevel)
    {
        if (_currLevel > currLevel)
        {
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

    public static int GetCurrDayTime()
    {
        int currDayTime = DateTime.Now.DayOfYear + DateTime.Now.Year * 10000;
        return currDayTime;
    }

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
    public static bool isHackMode;

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
}