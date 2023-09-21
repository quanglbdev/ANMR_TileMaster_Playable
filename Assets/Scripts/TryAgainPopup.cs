using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TryAgainPopup : MonoBehaviour
{
    [Header("Popup")] public GameObject popup;
    public BBUIButton btnTryAgain, btnClose;
    public TextMeshProUGUI _levelTxt;

    public GameObject lockGroup;

    private enum HideState
    {
        Replay,
        Hide
    }

    private HideState _hideState;
    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        btnTryAgain.OnPointerClickCallBack_Start.AddListener(TouchTryAgain);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }

    private void TouchTryAgain()
    {
        lockGroup.gameObject.SetActive(true);
        _hideState = HideState.Replay;
        popup.GetComponent<BBUIView>().HideView();
        Config.currSelectLevel = _level;
        GamePlayManager.Instance.SetReplayGame();
    }

    private void TouchClose()
    {
        _hideState = HideState.Hide;
        popup.GetComponent<BBUIView>().HideView();
    }

    private int _level;

    public void ShowTryAgainPopup(int level)
    {
        SoundManager.Instance.PlaySound_GameOver();
        _level = level;
        transform.DOScale(1, 0.1f);
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);
        popup.gameObject.SetActive(false);

        btnClose.gameObject.SetActive(false);

        btnTryAgain.gameObject.SetActive(false);
        _levelTxt.text = $"Level {_level}";
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
            btnTryAgain.gameObject.SetActive(true);
            btnTryAgain.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.3f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();
        });
    }

    private void HidePopup_Finished()
    {
        switch (_hideState)
        {
            case HideState.Replay:
                gameObject.SetActive(false);
                break;
            case HideState.Hide:
                gameObject.SetActive(false);
                GamePlayManager.Instance.HideView();
                break;
        }
    }
}