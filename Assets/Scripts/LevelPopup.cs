using DG.Tweening;
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


    public LevelGridAdapter osa;

    //public ScrollRect scrollRect;
    //public Transform content;
    //public ItemLevel itemLevelPrefab;
    private void Awake()
    {
        instance = this;
    }

    enum LEVELPOPUP_ACTION_TYPE
    {
        SelectLevel,
        Close
    }

    LEVELPOPUP_ACTION_TYPE _typeAction = LEVELPOPUP_ACTION_TYPE.Close;

    // Start is called before the first frame update
    void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }


    public void ShowLevelPopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    public void TouchClose()
    {
        _typeAction = LEVELPOPUP_ACTION_TYPE.Close;
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
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
            Config.isSelectLevel = true;
            Config.currSelectLevel = infoLevelSelect.level;

            AdsManager.Instance.ShowInterstitialAd();
            MenuManager.instance.TouchPlay();
        }

        gameObject.SetActive(false);
    }


    //bool isShowView = false;
    private void InitViews()
    {
        //isShowView = true;
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