using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ThemePopup : MonoBehaviour
{
    [Header("Popup")]
    public BBUIButton btnClose;
    public GameObject popup;
    public GameObject lockGroup;

    [Header("Button Themes")] public GameObject themeBtn_Enable;
    public Button themeBtn_Disable;
    public GameObject themeContent;
    
    
    [Header("Button Tiles")] public GameObject tileBtn_Enable;
    public Button tileBtn_Disable;
    public GameObject tileContent;

    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        
        themeBtn_Disable.onClick.AddListener(TouchTheme);
        tileBtn_Disable.onClick.AddListener(TouchTile);
        
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    private void TouchTile()
    {
        SoundManager.Instance.PlaySound_Click();
        themeBtn_Disable.gameObject.SetActive(true);
        themeBtn_Enable.SetActive(false);
        
        tileBtn_Disable.gameObject.SetActive(false);
        tileBtn_Enable.SetActive(true);
        
        themeContent.gameObject.SetActive(false);
        tileContent.gameObject.SetActive(true);
    }

    private void TouchTheme()
    {
        SoundManager.Instance.PlaySound_Click();
        tileBtn_Disable.gameObject.SetActive(true);
        tileBtn_Enable.SetActive(false);
        
        themeBtn_Disable.gameObject.SetActive(false);
        themeBtn_Enable.SetActive(true);
        
        themeContent.gameObject.SetActive(true);
        tileContent.gameObject.SetActive(false);
    }


    public void OpenThemePopup() {
        TouchTile();
        gameObject.SetActive(true);
        InitViews();
    }
    
    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);

        popup.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
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
