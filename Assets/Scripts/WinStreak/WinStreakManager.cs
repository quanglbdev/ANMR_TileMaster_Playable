using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WinStreakManager : MonoBehaviour
{
    public static WinStreakManager Instance;

    [SerializeField] private TextMeshProUGUI countDownText;

    private bool active;

    public bool Active
    {
        get => active;
        set
        {
            var timeSpan = Config.WIN_STREAK_EVENT_DATE - Config.GetDateTimeNow();
            _timeStart = (int)timeSpan.TotalSeconds;
            StartCoroutine(YieldUpdateTime());

            active = value;
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private int _timeStart;

    private IEnumerator YieldUpdateTime()
    {
        while (true)
        {
            var time = Mathf.FloorToInt(_timeStart - Time.realtimeSinceStartup);
            if (time <= 0)
            {
                active = false;
                break;
            }

            countDownText.text = $"{Config.FormatTime(time)}";
            yield return new WaitForSecondsRealtime(1f);
        }
    }
}