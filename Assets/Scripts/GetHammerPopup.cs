using DG.Tweening;
using UnityEngine;

public class GetHammerPopup : MonoBehaviour
{
    [Header("Popup")] public BBUIButton btnClose;
    public BBUIButton btnPlay;
    public GameObject popup;
    public GameObject lockGroup;

    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        btnPlay.OnPointerClickCallBack_Start.AddListener(TouchPlay);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    public void OpenGetHammerPopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);

        popup.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        btnPlay.gameObject.SetActive(false);
        InitViews_ShowView();
    }

    private void InitViews_ShowView()
    {
        Sequence sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.2f, () =>
        {
            btnPlay.gameObject.SetActive(true);
            btnPlay.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();
        });
    }

    private void TouchPlay()
    {
        TouchClose();
        MenuManager.instance.TouchPlay();
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