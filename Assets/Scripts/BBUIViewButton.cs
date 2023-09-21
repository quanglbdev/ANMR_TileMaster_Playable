using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BBUIViewButton : BBUIView
{
    [SerializeField] private Image icon;
    [SerializeField] private Transform starPosition;

    public void ShowViewAndAddWinStreak()
    {
        ShowView();
        DOVirtual.DelayedCall(0.3f, () =>
        {
            starPosition.position = starPosition.position;
            icon.gameObject.SetActive(true);
            icon.DOFade(0.5f, 0.8f);
            icon.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() =>
            {
                icon.gameObject.SetActive(false);
                var nextStep = Config.WIN_STREAK_INDEX <= 2 ? 2 : 3;
                if (Config.WIN_STREAK_INDEX >= Config.WIN_STREAK_OLD_INDEX + nextStep)
                {
                    GameDisplay.Instance.OpenWinStreakPopup();
                    MenuManager.instance.SetActiveLockGroup(false);
                }
            });
        });
    }

    public void ShowViewAndOpenPopup()
    {
        ShowView();
        DOVirtual.DelayedCall(0.8f, () =>
        {
            starPosition.position = starPosition.position;
            icon.gameObject.SetActive(false);
            icon.DOFade(0.5f, 0.3f);
            icon.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() =>
            {
                icon.gameObject.SetActive(false);
                GameDisplay.Instance.OpenWinStreakPopup();
                MenuManager.instance.SetActiveLockGroup(false);
            });
        });
    }
}