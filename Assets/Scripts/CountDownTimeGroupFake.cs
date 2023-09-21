using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CountDownTimeGroupFake : MonoBehaviour
{
    public TextMeshProUGUI txtTime;

    private int TIME_START;

    private void OnEnable()
    {
        var expiredFirstTimePack = DateTime.Parse(Config.FIRST_TIME_PACK_DATE);
        var checkFirstPack = expiredFirstTimePack - Config.GetDateTimeNow();
        TIME_START = (int)checkFirstPack.TotalSeconds;
        StartCoroutine(UpdateTime_IEnumerator());
    }

    private IEnumerator UpdateTime_IEnumerator()
    {
        while (true)
        {
            int time = Mathf.FloorToInt(TIME_START - Time.realtimeSinceStartup);
            if (time <= 0)
            {
                Config.IS_SHOW_FIRST_TIME_PACK = false;
                EventDispatcher.Instance.PostEvent(EventID.ExpiredFirstTimePack);
                yield break;
            }

            txtTime.text = $"{Config.FormatTime(time)}";
            yield return new WaitForSecondsRealtime(1f);
        }
    }
}