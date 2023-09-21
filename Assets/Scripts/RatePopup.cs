using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class RatePopup : MonoBehaviour
{
    [Header("LINK RATE")] private string rateLinkAndorid = "market://details?id=com.cla.tileworld";
    private string rateLinkIOS = "";

    [Header("Popup")] public GameObject popup;
    public BBUIButton btnClose, btnClose2;
    public BBUIButton btnLike;

    [Header("LockGroup")] public GameObject lockGroup;

    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(() => { TouchClose(); });
        btnClose2.OnPointerClickCallBack_Start.AddListener(() => { TouchClose(); });
        btnLike.OnPointerClickCallBack_Start.AddListener(TouchLike);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }


    public void OpenRatePopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);

        popup.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        btnLike.gameObject.SetActive(false);
        // AdmobManager.instance.HideBannerAd();
        InitViews_ShowView();
    }

    public void InitViews_ShowView()
    {
        Sequence sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.5f, () =>
        {
            btnLike.gameObject.SetActive(true);
            btnLike.GetComponent<BBUIView>().ShowView();
        });
    }

    private async UniTask TouchClose()
    {
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();

        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
    }

    private void TouchLike()
    {
        Config.SetRate();
        NotificationPopup.instance.AddNotification("Coming Soon!");
#if UNITY_ANDROID
        //Application.OpenURL(rateLinkAndorid);
#else
        //Application.OpenURL(rateLinkIOS);
#endif
        PopupHideView_Finished();
    }

    private void PopupHideView_Finished()
    {
        gameObject.SetActive(false);
        if (Config.gameState == Config.GAME_STATE.PAUSE)
        {
            Config.gameState = Config.GAME_STATE.PLAYING;
            GamePlayManager.Instance.SortingLayerCanvasPlaying();
        }
    }
}