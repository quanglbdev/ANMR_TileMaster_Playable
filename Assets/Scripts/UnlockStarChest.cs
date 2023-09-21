using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockStarChest : MonoBehaviour
{
    [Header("Popup")] public GameObject popup;
    public GameObject lockGroup;
    [Header("Element")] [SerializeField] private Button hideBtn;
    [SerializeField] private Transform starChestGroup;
    [SerializeField] private Image process;
    [SerializeField] private TextMeshProUGUI processTxt;

    private void Start()
    {
        hideBtn.onClick.AddListener(Hide);
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }

    public void Show()
    {
        hideBtn.onClick.RemoveAllListeners();
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(true);
        popup.gameObject.SetActive(false);
        starChestGroup.localScale = Vector3.zero;

        process.fillAmount = 0f / Config.STEP_STAR_CHEST;
        processTxt.text = $"{0}/{Config.STEP_STAR_CHEST}";
        InitViews_ShowView();
    }

    private Sequence _sequenceShowView;

    private void InitViews_ShowView()
    {
        _sequenceShowView = DOTween.Sequence();
        _sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
        });
        _sequenceShowView.InsertCallback(0.3f, () =>
        {
            starChestGroup.DOScale(1f, 0.5f).OnComplete(() =>
            {
                process.DOFillAmount(Config.GetChestCountStar() * 1f / Config.STEP_STAR_CHEST, 0.3f);
                processTxt.text = $"{Config.GetChestCountStar()}/{Config.STEP_STAR_CHEST}";
                lockGroup.gameObject.SetActive(false);
            });
        });
    }

    private void Hide()
    {
        SoundManager.Instance.PlaySound_Click();
        popup.GetComponent<BBUIView>().HideView();
    }

    private void HidePopup_Finished()
    {
        gameObject.SetActive(false);
        WinPopup.Instance.HideFinished();
    }
}