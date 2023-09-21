using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoinGroup : MonoBehaviour
{
    public TextMeshProUGUI txtCoin;
    void Start()
    {
        Config.OnChangeCoin += OnChangeCoin;
        ShowCoin();
    }

    private void OnDestroy()
    {
        Config.OnChangeCoin -= OnChangeCoin;
    }

    private void OnChangeCoin(int coinValue)
    {
        ShowCoin();
    }

    private void ShowCoin()
    {
        if(txtCoin == null) return;
        DOTween.Kill(txtCoin.transform);
        
        txtCoin.text = $"{Config.currCoin}";
        txtCoin.transform.localScale = Vector3.one;
        txtCoin.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 10, 2f).SetEase(Ease.InOutBack).SetRelative(true).SetLoops(3,LoopType.Restart);
    }
}
