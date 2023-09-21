using DG.Tweening;
using TMPro;
using UnityEngine;

public class TimeUpPopup : MonoBehaviour
{
    [Header("Popup")] public GameObject popup;

    [Header("ADD TIME COIN")] public BBUIButton btnRevive;
    public TextMeshProUGUI timeAddCoinTxt;

    [Header("ADD TIME ADS")] public BBUIButton btnReviveAds;
    public TextMeshProUGUI timeAddAdsTxt;

    [Space(10)] public BBUIButton btnGiveUp;
    public GameObject lockGroup;

    private void Start()
    {
        btnRevive.OnPointerClickCallBack_Start.AddListener(TouchRevive);
        btnReviveAds.OnPointerClickCallBack_Start.AddListener(TouchAdsRevive);
        btnGiveUp.OnPointerClickCallBack_Start.AddListener(TouchGiveUp);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }

    #region Revive

    private void TouchRevive()
    {
        lockGroup.gameObject.SetActive(true);
        Config.SetHeart(Config.currHeart + 1);
        Config.SetCoin(Config.currCoin - Config.coinRiviveRequired);
        Revive_Finished();
    }

    private void TouchAdsRevive()
    {
        lockGroup.gameObject.SetActive(false);
        //WatchAds
        ReviveSuccess();
    }

    private void ReviveSuccess()
    {
        lockGroup.gameObject.SetActive(true);
        Revive_Finished();
    }

    #endregion

    private void Revive_Finished()
    {
        popup.GetComponent<BBUIView>().HideView();
        GamePlayManager.Instance.EnableClock(Vector3.zero, _timeAdd);
        GamePlayManager.Instance.SetRevive_Success();
    }

    private void TouchGiveUp()
    {
        GamePlayManager.Instance.OpenTryAgainPopup();
        HidePopup_Finished();
    }

    private int _timeAdd;

    public void ShowTimeUpPopup(int level)
    {
        _timeAdd = GameLevelManager.Instance.secondsRequired / 3;
        timeAddCoinTxt.text = $"+{_timeAdd}s";
        timeAddAdsTxt.text = $"+{_timeAdd}s";
        SoundManager.Instance.PlaySound_GameOver();

        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(true);
        popup.gameObject.SetActive(false);

        btnRevive.gameObject.SetActive(false);

        btnGiveUp.gameObject.SetActive(false);
        InitViews_ShowView();
    }

    private void InitViews_ShowView()
    {
        Sequence sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.2f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            btnRevive.gameObject.SetActive(true);
            btnRevive.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.6f, () =>
        {
            btnGiveUp.gameObject.SetActive(true);
            btnGiveUp.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.OnComplete(() => { lockGroup.gameObject.SetActive(false); });
    }

    private void HidePopup_Finished()
    {
        gameObject.SetActive(false);
    }
}