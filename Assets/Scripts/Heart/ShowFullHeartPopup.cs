using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShowFullHeartPopup : MonoBehaviour
{
    [Header("Popup")] public BBUIButton btnClose;
    public GameObject popup;
    public GameObject lockGroup;
    
    public GameObject heart;
    public TextMeshProUGUI text;
    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);

    }
    public void OpenShowFullHeartPopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);

        popup.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        heart.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
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
            heart.SetActive(true);
            heart.GetComponent<BBUIView>().ShowView();
            
            text.gameObject.SetActive(true);
            text.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();
        });
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
