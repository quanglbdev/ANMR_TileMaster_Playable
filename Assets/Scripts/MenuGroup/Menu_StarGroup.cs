using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu_StarGroup : MonoBehaviour
{
    public TextMeshProUGUI txtStar;
    public Button btnStar;
    private void Start()
    {
        Config.OnChangeStar += OnChangeStar;
        ShowStar();
    }

    private void OnDestroy()
    {
        Config.OnChangeStar -= OnChangeStar;
    }

    private void OnChangeStar(int starValue) {
        ShowStar();
    }

    private void ShowStar() {
        DOTween.Kill(txtStar.transform);
        
        txtStar.text = $"{Config.currStar}";
        txtStar.transform.localScale = Vector3.one;
        txtStar.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 10, 2f).SetEase(Ease.InOutBack).SetRelative(true).SetLoops(3,LoopType.Restart);
    }
}
