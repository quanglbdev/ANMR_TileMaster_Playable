using DG.Tweening;
using TMPro;
using UnityEngine;
using static System.String;

public class FeedBackPopup : MonoBehaviour
{
    [Header("Popup")] public GameObject popup;
    [Header("LockGroup")] public GameObject lockGroup;

    [Header("Button top")] public BBUIButton btnSubmit;
    public BBUIButton btnClose;
    
    [Header("Button top")] public TMP_InputField emailField;
    [Header("Button top")] public TMP_InputField problemField;

    enum STATE_FEEDBACK
    {
        CLOSE,
        SUBMIT
    }
    private STATE_FEEDBACK _stateClosePopup = STATE_FEEDBACK.CLOSE;

    private void Start()
    {
        btnSubmit.OnPointerClickCallBack_Start.AddListener(TouchSubmit);
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    public void OpenFeedBackPopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        SoundManager.Instance.PlaySound_Popup();
        lockGroup.gameObject.SetActive(true);

        popup.gameObject.SetActive(false);

        btnSubmit.gameObject.SetActive(true);
        btnClose.gameObject.SetActive(true);
        emailField.text = Empty;
        problemField.text = Empty;
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

            btnSubmit.gameObject.SetActive(true);
            btnSubmit.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.3f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();
            lockGroup.gameObject.SetActive(false);
        });
    }
    private void TouchClose()
    {
        _stateClosePopup = STATE_FEEDBACK.CLOSE;
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void TouchSubmit()
    {
        lockGroup.SetActive(true);
        var email = emailField.text;
        var feedback = problemField.text;
        Debug.Log($"Email: {email}");
        Debug.Log($"Feedback: {feedback}");
        _stateClosePopup = STATE_FEEDBACK.SUBMIT;
        popup.GetComponent<BBUIView>().HideView();
    }

    private void PopupHideView_Finished()
    {
        gameObject.SetActive(false);

        if (_stateClosePopup == STATE_FEEDBACK.SUBMIT)
        {
            NotificationPopup.instance.AddNotification("Submit!");
        }
    }
}