using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ItemNotification : MonoBehaviour
{
    public Image bg;
    public TextMeshProUGUI txtContent;


    public void ShowNotification(string content) {
        txtContent.text = $"{content}";
        AutoHideView();
    }

    private void AutoHideView() {
        var sequence = DOTween.Sequence();
        sequence.Insert(0.1f, bg.DOFade(0f, 0.5f).SetEase(Ease.InExpo));
        sequence.Insert(0.1f, bg.GetComponent<RectTransform>().DOAnchorPosY(300f,0.5f).SetEase(Ease.OutQuad).SetRelative(true));
        sequence.Insert(0.1f, txtContent.DOFade(0f, 0.5f).SetEase(Ease.InExpo));
        sequence.OnComplete(()=> {
            Destroy(gameObject);
        });
    }
}
