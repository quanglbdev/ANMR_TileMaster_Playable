using System;
using UnityEngine;

public class HeartManager : Singleton<HeartManager>
{
    private void Start()
    {
        if (Config.GetDateTimeNow().Date > Config.HEART_RESET.Date)
        {
            // Config.ENERGY_ADS_COUNT = 0;
            // Config.ENERGY_GEM_COUNT = 0;
        }

        Config.HEART_RESET = Config.GetDateTimeNow();
    }

    private void Update()
    {
        if (Config.FREE_HEART)
        {
            CountDownFreeHeart();
            return;
        }
        
        if (Config.currHeart < Config.MAX_HEART)
        {
            RegenerateHeart();
        }
    }

    private float _countDownFreeHeart;
    private void CountDownFreeHeart()
    {
        var timeDifference = Config.GetDateTimeNow() - Config.FREE_HEART_DATE_ADD;
        if (timeDifference.TotalMinutes >= Config.FREE_HEART_TIME)
        {
            Config.FREE_HEART_TIME = -1;
        }
        else
        {
            EventDispatcher.Instance.PostEvent(EventID.FreeHeartCountdown, timeDifference);
        }
    }
    private void RegenerateHeart()
    {
        var timeDifference = (Config.GetDateTimeNow() - Config.HEART_LAST_ADDED_TIME);
        if (timeDifference.TotalMinutes >= Config.TIME_TO_RELOAD_HEART)
        {
            for (var i = 0;
                 i < Mathf.FloorToInt((float)(timeDifference.TotalMinutes / Config.TIME_TO_RELOAD_HEART));
                 i++)
            {
                Config.SetHeart(Config.currHeart + 1);
                timeDifference += new TimeSpan(0, -Config.TIME_TO_RELOAD_HEART, 0);
                if (Config.currHeart == Config.MAX_HEART)
                {
                    timeDifference = new TimeSpan(0);
                    EventDispatcher.Instance.PostEvent(EventID.HeartCountdown, timeDifference);
                    break;
                }
            }

            Config.HEART_LAST_ADDED_TIME = Config.GetDateTimeNow().Add(-timeDifference);
        }
        else
        {
            EventDispatcher.Instance.PostEvent(EventID.HeartCountdown, timeDifference);
        }
    }
}