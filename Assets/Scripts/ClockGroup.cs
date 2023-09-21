using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockGroup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownTxt;
    [SerializeField] private Image imgProgress;

    [SerializeField] private Transform clock;

    private float _time;

    private void OnEnable()
    {
        clock.gameObject.SetActive(false);
        countDownTxt.text = string.Empty;
        imgProgress.fillAmount = 1f;
    }

    public void Enable(Vector3 starPosition, int seconds)
    {
        clock.gameObject.SetActive(true);
        clock.position = starPosition;
        _timeStart = seconds + 1;
        clock.DOScale(Vector3.one, .3f);
        clock.DOLocalMove(Vector3.zero, .3f).OnComplete(() => { StartCoroutine(YieldUpdateTime()); });
        _time = 0;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Config.gameState != Config.GAME_STATE.PLAYING) return;
        _time += Time.deltaTime;
    }

    private int _timeStart;

    private IEnumerator YieldUpdateTime()
    {
        while (true)
        {
            var time = Mathf.FloorToInt(_timeStart - _time);

            if (time <= 0)
            {
                imgProgress.DOFillAmount(0, 0.5f);
                countDownTxt.text = $"{Config.FormatTime(time)}";
                SetLose();
                break;
            }

            countDownTxt.text = $"{Config.FormatTime(time)}";
            imgProgress.DOFillAmount(1f - _time / _timeStart, 1f);
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private Sequence _moveSequence;

    private void SetLose()
    {
        Config.gameState = Config.GAME_STATE.LOSE;
        GamePlayManager.Instance.SortingLayerCanvasUI();

        _moveSequence = DOTween.Sequence();

        _moveSequence.Insert(0f, clock.DOScale(Vector3.one * 3f, 1f).SetEase(Ease.OutQuad));
        _moveSequence.Insert(0f, clock.DOMove(Vector3.zero, 1f).SetEase(Ease.OutQuad));

        _moveSequence.OnComplete(() =>
        {
            GamePlayManager.Instance.SetFinishedGame();
            GamePlayManager.Instance.SetGameTimeUp();
        });
    }
}