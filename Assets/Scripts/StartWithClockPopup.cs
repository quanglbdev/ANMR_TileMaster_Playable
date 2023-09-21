using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartWithClockPopup : MonoBehaviour
{
    [Header("Popup")] public GameObject popup;
    public GameObject lockGroup;

    [Header("Element")] [SerializeField] private Button hideBtn;
    [SerializeField] private Transform clock;
    [SerializeField] private TextMeshProUGUI content;

    private string _content;
    private int _seconds;

    private void Start()
    {
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }

    public void Show(int second)
    {
        _content = Config.ConvertSecondsToMinutesAndSeconds(second);
        _seconds = second;
        hideBtn.onClick.RemoveAllListeners();
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(true);
        popup.gameObject.SetActive(false);
        clock.localScale = Vector3.zero;
        content.transform.localScale = Vector3.zero;

        content.text = $"Completed the level in \n <color=#2CFF10><size= 80>{_content}</size></color>";
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
            clock.DOScale(1f, 0.5f).OnComplete(() =>
            {
                content.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
                lockGroup.gameObject.SetActive(false);
                hideBtn.onClick.AddListener(Hide);
            }).SetEase(Ease.OutBack);
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
        GamePlayManager.Instance.EnableClock(clock.position, _seconds);
        Config.gameState = Config.GAME_STATE.PLAYING;
    }
}