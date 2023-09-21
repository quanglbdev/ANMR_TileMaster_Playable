using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShowFreeHeartPopup : MonoBehaviour
{
    [Header("Popup")] public BBUIButton btnClose;
    public GameObject popup;
    public GameObject lockGroup;

    public GameObject heart;
    public TextMeshProUGUI countDownTxt;

    private Action<object> _updateFreeHeartCountDown;

    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    private void UpdateFreeHeartCountdown(TimeSpan countdown)
    {
        if (countdown.TotalSeconds != 0)
        {
            TimeSpan countDownFreeHeartTime = new(0, Config.FREE_HEART_TIME, 0);
            countdown = countDownFreeHeartTime - countdown;

            countDownTxt.gameObject.SetActive(true);

            countDownTxt.text = countdown.Hours > 0
                ? $"{countdown.Hours:0}:{countdown.Minutes:00}:{countdown.Seconds:00}"
                : $"{countdown.Minutes:00}:{countdown.Seconds:00}";
        }
        else
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.FreeHeartCountdown, _updateFreeHeartCountDown);
    }

    private void OnEnable()
    {
        _updateFreeHeartCountDown = (param) => UpdateFreeHeartCountdown((TimeSpan)param);
        EventDispatcher.Instance.RegisterListener(EventID.FreeHeartCountdown, _updateFreeHeartCountDown);
    }

    public void OpenShowFreeHeartPopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);

        popup.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        heart.gameObject.SetActive(false);
        countDownTxt.gameObject.SetActive(false);
        InitViews_ShowView();
    }

    private void InitViews_ShowView()
    {
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.2f, () =>
        {
            heart.SetActive(true);
            heart.GetComponent<BBUIView>().ShowView();

            countDownTxt.gameObject.SetActive(true);
            countDownTxt.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();
        });
    }

    private void TouchClose()
    {
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void PopupHideView_Finished()
    {
        gameObject.SetActive(false);
    }
}