using DG.Tweening;
using UnityEngine;

public class LeaderBoardInfoPopup : MonoBehaviour
{
    
    [Header("Popup")]
    public BBUIButton btnClose;
    public BBUIButton btnLeaderBoard;
    public GameObject popup;
    public GameObject lockGroup;
    public GameObject textGroup;
    public GameObject cardGroup;
    void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        btnLeaderBoard.OnPointerClickCallBack_Start.AddListener(TouchLeaderboard);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    private void TouchLeaderboard()
    {
        TouchClose();
        MenuManager.instance.OpenLeaderBoardPopup();
    }

    private void TouchClose()
    {
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }
    
    public void OpenLeaderBoardInfoPopup()
    {
        SoundManager.Instance.PlaySound_Click();
        gameObject.SetActive(true);
        InitViews();
    }
    
    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);

        popup.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        btnLeaderBoard.gameObject.SetActive(false);
        textGroup.SetActive(false);
        cardGroup.SetActive(false);
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
            textGroup.gameObject.SetActive(true);
            textGroup.GetComponent<BBUIView>().ShowView();
            
            cardGroup.gameObject.SetActive(true);
            cardGroup.GetComponent<BBUIView>().ShowView();
            
            btnLeaderBoard.gameObject.SetActive(true);
            btnLeaderBoard.GetComponent<BBUIView>().ShowView();
        });
        
        sequenceShowView.InsertCallback(0.3f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();
        });
    }
    
    private void PopupHideView_Finished()
    {
        gameObject.SetActive(false);
    }
}
