using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

public class IconNotification : MonoBehaviour
{
    private Image _iconNotify;

    private void Awake()
    {
        _iconNotify = GetComponent<Image>();
    }

    private void Start()
    {
        ShowAnimation();
    }


    private Sequence _sequence;
    private void ShowAnimation()
    {
        if (_sequence != null)
        {
            _sequence.Kill();
        }
        _sequence = DOTween.Sequence();
        var timeDelayStart = Random.Range(0.2f, 2f);
        var timePunchScale = Random.Range(1f, 1.5f);
        var timeNormalScale = 0.2f;
        _sequence.Insert(timeDelayStart,_iconNotify.transform.DOPunchScale(Vector3.one * 0.2f, timePunchScale, 10, 2f).SetRelative(true));
        _sequence.Insert(timeDelayStart + timePunchScale,
            _iconNotify.transform.DOScale(Vector3.one * 0.2f, timeNormalScale).SetEase(Ease.Linear).SetRelative(true)
                .SetLoops(Random.Range(0,6), LoopType.Yoyo));
        _sequence.SetLoops(-1, LoopType.Restart);
    }
}
