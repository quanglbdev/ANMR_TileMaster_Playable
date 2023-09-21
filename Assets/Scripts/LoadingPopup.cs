using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoadingPopup : MonoBehaviour
{
    public GameObject popup;
    public GameObject lockGroup;
    
    public TextMeshProUGUI loadingText;

    private void Start()
    {
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }
    
    public void OpenLoadingPopup()
    {
        gameObject.SetActive(true);
        InitViews();
        AnimateLoadingText();
    }

    public void Show()
    {
        if(popup !=null)
            popup.gameObject.SetActive(true);
        if(gameObject !=null)
            gameObject.SetActive(true);
        if(loadingText !=null)
            loadingText.gameObject.SetActive(false);
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);
        popup.gameObject.SetActive(false);
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
    }
    private float dotInterval = 0.3f;
    private Tween _dotTween;
    private readonly string _originalText = "Loading";

    private void AnimateLoadingText()
    {
        GameObject loadingGo;
        (loadingGo = loadingText.gameObject).SetActive(true);
        DOTween.Kill(loadingGo);
        loadingText.text = "Loading";
        _dotTween = DOTween.Sequence()
            .AppendCallback(() => loadingText.text = _originalText + ".")
            .AppendInterval(dotInterval)
            .AppendCallback(() => loadingText.text = _originalText + "..")
            .AppendInterval(dotInterval)
            .AppendCallback(() => loadingText.text = _originalText + "...")
            .AppendInterval(dotInterval)
            .SetLoops(-1);
    }

    public void Close()
    {
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void PopupHideView_Finished()
    {
        gameObject.SetActive(false);
    }
}