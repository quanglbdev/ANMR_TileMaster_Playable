using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    bool firebaseInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        //Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        //    Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        //});
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                    "Error Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    void InitializeFirebase()
    {
        Debug.Log("Enabling data collection.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        firebaseInitialized = true;

        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Debug.Log("Firebase Messaging Initialized");


        // This will display the prompt to request permission to receive
        // notifications if the prompt has not already been displayed before. (If
        // the user already responded to the prompt, thier decision is cached by
        // the OS and can be changed in the OS settings).
        Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(
            task => { LogTaskCompletion(task, "RequestPermissionAsync"); }
        );
        // if (LoadSceneManager.instance.isActiveAndEnabled) {
        //     
        //     LoadSceneManager.instance.LoadMenuScene();
        // }
    }

    private string GetCurrentSceneName()
    {
        // Get the currently active scene
        var currentScene = SceneManager.GetActiveScene();
        return currentScene.name;
    }

    public void LogLevelStart(int level)
    {
        Debug.Log("Firebase level_start_" + level);

#if !DEVELOPMENT
        if (firebaseInitialized)
        {
            if (level <= 20)
                FirebaseAnalytics.LogEvent("level_start_" + level);
        }
#endif
    }

    public void LogLevelLose(int level, int failCount)
    {
        Debug.Log("Firebase level_fail_" + level + "_fail_count_" + failCount);

#if !DEVELOPMENT
        if (firebaseInitialized)
        {
            if (level <= 20)
                FirebaseAnalytics.LogEvent("level_fail_" + level + "_fail_count_" + failCount);
        }
#endif
    }

    public void LogLevelWin(int level, int timePlayed)
    {
        Debug.Log("Firebase level_" + level + "_complete_timePlayed_" + timePlayed + "s");
#if !DEVELOPMENT
        if (firebaseInitialized)
        {
            if (level <= 20)
                FirebaseAnalytics.LogEvent("level_complete_" + level + "_timePlayed_" + timePlayed +"s");
        }
#endif
    }

    public void LogEarnVirtualCoin(long value, string source)
    {
        Debug.Log("Firebase earn_virtual_coin_" + value + "_" + source);
#if !DEVELOPMENT
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent( "earn_virtual_coin_" + value + "_" +source);
        }
#endif
    }

    public void LogSpendVirtualCoin(long value, string source)
    {
        Debug.Log("Firebase spend_virtual_coin_" + value + "_" + source);
#if !DEVELOPMENT
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent( "spend_virtual_coin_" + value + "_" +source);
        }
#endif
    }

    public void LogLoadReward(string str = null)
    {
        Debug.Log("Firebase ads_reward_load");
#if !DEVELOPMENT
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ads_reward_load");
            Config.LogByAppsflyer("af_rewarded_show");
        }
#endif
    }

    public void LogShowRewardSuccess(string str = null)
    {
        Debug.Log("Firebase ads_reward_show_success");
#if !DEVELOPMENT
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ads_reward_show_success");
        }
        Config.LogByAppsflyer("af_rewarded_displayed");
#endif
    }

    public void LogShowRewardFail(string str = null)
    {
        Debug.Log("Firebase ads_reward_show_fail");
#if !DEVELOPMENT
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ads_reward_show_fail");
        }
#endif
    }

    public void LogRewarded(string str = null)
    {
        Debug.Log("Firebase ads_reward_complete");
#if !DEVELOPMENT
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ads_reward_complete");
        }
#endif
    }

    public void LogEligibleReward()
    {
#if !DEVELOPMENT
        Config.LogByAppsflyer("af_rewarded_show");
#endif
    }

    public void LogClickRewarded(string str = null)
    {
        Debug.Log("Firebase ads_reward_click");
#if !DEVELOPMENT
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ads_reward_click");
        }
#endif
    }

    public void LogLoadInterSuccess()
    {
#if !DEVELOPMENT
        Debug.Log("Firebase ad_inter_load_success");
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_inter_load_success");
        }
#endif
    }

    public void LogEligibleInter()
    {
        //if(Config.MAX_LEVEL) return;

        Debug.Log("Firebase LogEligibleInter");
#if !DEVELOPMENT
         Config.LogByAppsflyer("af_inter_show");
#endif
    }

    public void LogShowInterFail(string str = null)
    {
#if !DEVELOPMENT
        Debug.Log("Firebase ad_inter_load_fail");
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_inter_load_fail");
        }
#endif
    }

    public void LogShowInter()
    {
#if !DEVELOPMENT
        Debug.Log("Firebase ad_inter_show");
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_inter_show");
        }
        Config.LogByAppsflyer("af_inter_displayed");
#endif
    }

    public void LogClickInter()
    {
        Debug.Log("Firebase ad_inter_click");
#if !DEVELOPMENT
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_inter_click");
        }
#endif
    }

    public const string POWERUP_UNDO = "undo";
    public const string POWERUP_HINT = "magic_wand";
    public const string POWERUP_SHUFFLE = "shuffle";
    public const string POWERUP_TILE_RETURN = "tile return";
    public const string POWERUP_EXTRA_SLOT = "rxtra slot";

    public void LogUsePowerUp(string type, int currentLevel)
    {
        if (firebaseInitialized)
        {
            //FirebaseAnalytics.LogEvent("PowerUp_" + type, "CurrentLevel", currentLevel);
        }
    }


    #region FIREBASE MESSAGE

    public virtual void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message");
        var notification = e.Message.Notification;
        if (notification != null)
        {
            Debug.Log("title: " + notification.Title);
            Debug.Log("body: " + notification.Body);
            var android = notification.Android;
            if (android != null)
            {
                Debug.Log("android channel_id: " + android.ChannelId);
            }
        }

        if (e.Message.From.Length > 0)
            Debug.Log("from: " + e.Message.From);
        if (e.Message.Link != null)
        {
            Debug.Log("link: " + e.Message.Link.ToString());
        }

        if (e.Message.Data.Count > 0)
        {
            Debug.Log("data:");
            foreach (System.Collections.Generic.KeyValuePair<string, string> iter in
                     e.Message.Data)
            {
                Debug.Log("  " + iter.Key + ": " + iter.Value);
            }
        }
    }

    public virtual void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    protected bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            Debug.Log(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            Debug.Log(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string errorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    errorCode = String.Format("Error.{0}: ",
                        ((Firebase.Messaging.Error)firebaseEx.ErrorCode).ToString());
                }

                Debug.Log(errorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            Debug.Log(operation + " completed");
            complete = true;
        }

        return complete;
    }

    #endregion

    public enum RewardFor
    {
        X2Reward,
        Spin,
        X3RewardWin,
        AdsHeart,
        ClaimStarChest,
        Revive,
        DailyReward,
        ReviveTimeUp
    }
}