using DG.Tweening;
using TMPro;
using UnityEngine;

public class OutGamePopup : MonoBehaviour
{
    [Header("Popup")] public GameObject popup;
    public BBUIButton btnLeave, btnClose;

    public GameObject content1, content2;
    public GameObject trophy;

    public TextMeshProUGUI winStreak;

    public GameObject lockGroup;

    void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        btnLeave.OnPointerClickCallBack_Start.AddListener(TouchLeave);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }

    private void TouchLeave()
    {
        HidePopup_Finished();
        GamePlayManager.Instance.HideView();
        Config.WIN_STREAK_INDEX = 0;
    }

    private void TouchClose()
    {
        HidePopup_Finished();
    }

    public void ShowOutGamePopup()
    {
        SoundManager.Instance.PlaySound_GameOver();
        transform.DOScale(1, 0.1f);
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);
        popup.gameObject.SetActive(false);

        btnClose.gameObject.SetActive(false);

        btnLeave.gameObject.SetActive(false);

        if (WinStreakManager.Instance.Active && Config.WIN_STREAK_INDEX > 0)
        {
            content1.SetActive(false);
            content2.SetActive(true);
            trophy.SetActive(true);

            content2.GetComponent<TextMeshProUGUI>().text =
                $"  You lose a life and {Config.WIN_STREAK_INDEX} win streak!";
            
            winStreak.text = $"{Config.WIN_STREAK_INDEX}";
        }
        else
        {
            content1.SetActive(true);
            content2.SetActive(false);
            trophy.SetActive(false);
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

        sequenceShowView.InsertCallback(0.2f, () =>
        {
            btnLeave.gameObject.SetActive(true);
            btnLeave.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();
        });
    }

    private void HidePopup_Finished()
    {
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().HideView();
        });

        sequenceShowView.InsertCallback(0.1f, () =>
        {
            btnLeave.gameObject.SetActive(true);
            btnLeave.GetComponent<BBUIView>().HideView();
        });

        sequenceShowView.InsertCallback(0.3f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().HideView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            gameObject.SetActive(false);
            Config.gameState = Config.GAME_STATE.PLAYING;
            GamePlayManager.Instance.SortingLayerCanvasPlaying();
        });
    }
}