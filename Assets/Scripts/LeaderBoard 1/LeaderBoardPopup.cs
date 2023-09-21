using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LeaderBoardPopup : MonoBehaviour
{
    [Header("Popup")] public BBUIButton btnClose;
    //public BBUIButton btnWeekly, btnTopPlayer; 
    public Button btnWeekly, btnTopPlayer;
    public GameObject popup;
    public GameObject lockGroup;
    [Header("Sprite")] public Sprite btn_On, btn_Off;
    [Header("Text")] public GameObject txtWeek_On, txtWeek_Off, txtTopPlayer_On, txtTopPlayer_Off;

    [Header("Tab")] public GameObject tab_TOP;
    public GameObject tab_WEEK;


    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        btnWeekly.onClick.AddListener(TouchWeekly);
        btnTopPlayer.onClick.AddListener(TouchTopPlayer);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    private void TouchTopPlayer()
    {
        btnWeekly.interactable = true;
        btnTopPlayer.interactable = false;

        tab_TOP.SetActive(false);
        tab_WEEK.SetActive(true);

        btnTopPlayer.GetComponent<Image>().sprite = btn_Off;
        btnWeekly.GetComponent<Image>().sprite = btn_On;

        txtWeek_On.SetActive(false);
        txtWeek_Off.SetActive(true);

        txtTopPlayer_On.SetActive(true);
        txtTopPlayer_Off.SetActive(false);
    }

    private void TouchWeekly()
    {
        btnWeekly.interactable = false;
        btnTopPlayer.interactable = true;

        tab_TOP.SetActive(true);
        tab_WEEK.SetActive(false);

        btnTopPlayer.GetComponent<Image>().sprite = btn_On;
        btnWeekly.GetComponent<Image>().sprite = btn_Off;

        txtWeek_On.SetActive(true);
        txtWeek_Off.SetActive(false);

        txtTopPlayer_On.SetActive(false);
        txtTopPlayer_Off.SetActive(true);
    }

    private void TouchClose()
    {
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }
    
    public void Hide()
    {
        PopupHideView_Finished();
    }

    public void OpenLeaderBoardPopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);

        popup.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        btnWeekly.gameObject.SetActive(false);
        btnTopPlayer.gameObject.SetActive(false);

        InitViews_ShowView();
    }

    private void InitViews_ShowView()
    {
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
            TouchTopPlayer();
        });

        sequenceShowView.InsertCallback(0.2f, () =>
        {
            btnWeekly.gameObject.SetActive(true);
            btnWeekly.GetComponent<BBUIView>().ShowView();

            btnTopPlayer.gameObject.SetActive(true);
            btnTopPlayer.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            btnClose.gameObject.SetActive(true);
            if (btnClose.gameObject.activeSelf == false)
                btnClose.GetComponent<BBUIView>().ShowView();
        });
    }

    private void PopupHideView_Finished()
    {
        gameObject.SetActive(false);
    }
}