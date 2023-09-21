using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuGamePopup : MonoBehaviour
{
    [Header("BBUIButton")] public BBUIButton btnClose;
    public BBUIButton btnProfile;
    public BBUIButton btnLeaderboard;
    public BBUIButton btnShop;
    public BBUIButton btnSetting;
    [Space(20)]
    public BBUIButton btnConnectFacebook;
    [Space(20)] public Image avatar;
    [Header("Popup")] public GameObject popup;
    public GameObject lockGroup;

    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        btnProfile.OnPointerClickCallBack_Start.AddListener(TouchProfile);
        btnLeaderboard.OnPointerClickCallBack_Start.AddListener(TouchLeaderboard);
        btnShop.OnPointerClickCallBack_Start.AddListener(TouchShop);
        btnSetting.OnPointerClickCallBack_Start.AddListener(TouchSetting);

        btnConnectFacebook.OnPointerClickCallBack_Start.AddListener(TouchConnectFacebook);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    private void TouchConnectFacebook()
    {
     
        NotificationPopup.instance.AddNotification("Coming soon!");
    }

    private void TouchSetting()
    {
        TouchClose();
        GameDisplay.Instance.OpenSettingPopup();
    }

    private void TouchShop()
    {
        TouchClose();
        GameDisplay.Instance.OpenShop();
    }

    private void TouchLeaderboard()
    {
        TouchClose();
        MenuManager.instance.OpenLeaderBoardPopup();
    }

    private void TouchProfile()
    {
        TouchClose();
        MenuManager.instance.OpenProfilePopup();
    }

    private void TouchClose()
    {
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    public void OpenGetMenuPopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        avatar.sprite = AssetManager.Instance.GetAvatarDefinition(Config.AVATAR_ID).avatarSprite;
        lockGroup.gameObject.SetActive(true);
        popup.gameObject.SetActive(false);

        btnProfile.gameObject.SetActive(false);
        btnLeaderboard.gameObject.SetActive(false);
        btnShop.gameObject.SetActive(false);
        btnSetting.gameObject.SetActive(false);

        btnConnectFacebook.gameObject.SetActive(false);
        if (Config.CheckTutorial_Profile())
        {
            TutorialManager.Instance.HideHandGuild();
        }
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

        sequenceShowView.InsertCallback(0.03f, () =>
        {
            btnProfile.gameObject.SetActive(true);
            btnProfile.GetComponent<BBUIView>().ShowView();

            btnLeaderboard.gameObject.SetActive(true);
            btnLeaderboard.GetComponent<BBUIView>().ShowView();

            btnShop.gameObject.SetActive(true);
            btnShop.GetComponent<BBUIView>().ShowView();

            btnSetting.gameObject.SetActive(true);
            btnSetting.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.05f, () =>
        {
            btnConnectFacebook.gameObject.SetActive(true);
            btnConnectFacebook.GetComponent<BBUIView>().ShowView();
            
        });
        
        sequenceShowView.InsertCallback(0.5f, () =>
        {
            lockGroup.gameObject.SetActive(false);
            
            if (Config.CheckTutorial_Profile())
            {
                TutorialManager.Instance.ShowTut_ClickProfile_HandGuild(btnProfile.transform);
            }
        });
    }

    private void PopupHideView_Finished()
    {
        gameObject.SetActive(false);
    }
}