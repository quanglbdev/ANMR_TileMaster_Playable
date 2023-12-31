﻿using DG.Tweening;
using TMPro;
using UnityEngine;
using Your.Namespace.Here.UniqueStringHereToAvoidNamespaceConflicts.Grids;

public class LevelPopup : MonoBehaviour
{
    public static LevelPopup instance;
    public BBUIButton btnClose;
    public GameObject popup;
    public GameObject lockGroup;
    public TextMeshProUGUI txtCountStar;
    
    public Manager manager;


    public LevelGridAdapter osa;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    enum LEVELPOPUP_ACTION_TYPE
    {
        SelectLevel,
        Close
    }

    LEVELPOPUP_ACTION_TYPE _typeAction = LEVELPOPUP_ACTION_TYPE.Close;

    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }


    public void ShowLevelPopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void TouchClose()
    {
        _typeAction = LEVELPOPUP_ACTION_TYPE.Close;
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private InfoLevel infoLevelSelect;

    public void SelectLevel(InfoLevel infoLevel)
    {
        infoLevelSelect = infoLevel;
        _typeAction = LEVELPOPUP_ACTION_TYPE.SelectLevel;
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void HidePopup_Finished()
    {
        if (_typeAction == LEVELPOPUP_ACTION_TYPE.SelectLevel)
        {
            Config.currSelectLevel = infoLevelSelect.level;

            manager.DisableCanvas();
            manager.SelectLevel(infoLevelSelect.level);
            gameObject.SetActive(false);
            return;
        }

        if (GamePlayManager.Instance.canvas.enabled)
        {
            manager.DisableCanvas();
            gameObject.SetActive(false);
            Config.gameState = Config.GAME_STATE.PLAYING;
        }
        else
        {
            manager.btnPlay.gameObject.SetActive(true);
        }
        gameObject.SetActive(false);
    }


    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);
        popup.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        txtCountStar.text = $"{Config.GetCountStars()}/{Config.MAX_LEVEL * 3}";
        InitView_ShowView();
    }


    private void InitLevels()
    {
        osa.Restart();
    }

    private void InitView_ShowView()
    {
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            InitLevels();
            popup.GetComponent<BBUIView>().ShowView();
        });


        sequenceShowView.InsertCallback(0.2f, () =>
        {
            btnClose.gameObject.SetActive(true);
            if (btnClose.gameObject.activeSelf)
                btnClose.gameObject.GetComponent<BBUIView>().ShowView();
        });
    }
}