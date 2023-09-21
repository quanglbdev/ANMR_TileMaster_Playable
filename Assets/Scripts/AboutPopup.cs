using DG.Tweening;
using TMPro;
using UnityEngine;

public class AboutPopup : MonoBehaviour
{
    [Header("Popup")] public GameObject popup;
    [Header("LockGroup")] public GameObject lockGroup;

    [Header("Button top")] public BBUIButton btnPolicy;
    public BBUIButton btnService;
    public BBUIButton btnClose;

    [Header("Version")] public TextMeshProUGUI version;

    private void Start()
    {
        btnPolicy.OnPointerClickCallBack_Start.AddListener(TouchPolicy);
        btnService.OnPointerClickCallBack_Start.AddListener(TouchService);
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    public void OpenAboutPopup()
    {
        gameObject.SetActive(true);
        version.text = $"App Version: {Config.PROJECT_VERSION} \n" +
                       $"Player ID: {Config.GetShortUUID(Config.USER_ID)}";
        InitViews();
    }

    private void InitViews()
    {
        SoundManager.Instance.PlaySound_Popup();
        lockGroup.gameObject.SetActive(true);

        popup.gameObject.SetActive(false);

        btnPolicy.gameObject.SetActive(true);
        btnService.gameObject.SetActive(true);
        btnClose.gameObject.SetActive(true);
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
            btnPolicy.gameObject.SetActive(true);
            btnPolicy.GetComponent<BBUIView>().ShowView();

            btnService.gameObject.SetActive(true);
            btnService.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.3f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();
            lockGroup.gameObject.SetActive(false);
        });
    }

    private void TouchPolicy()
    {
        NotificationPopup.instance.AddNotification("Privacy Policy");
    }

    private void TouchService()
    {
        NotificationPopup.instance.AddNotification("Terms Of Service");
    }

    private void TouchClose()
    {
        popup.GetComponent<BBUIView>().HideView();
    }

    private void PopupHideView_Finished()
    {
        gameObject.SetActive(false);
        lockGroup.gameObject.SetActive(true);
    }
}