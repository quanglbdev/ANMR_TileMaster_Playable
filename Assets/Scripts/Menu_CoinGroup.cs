using DG.Tweening;
using TMPro;
using UnityEngine;
public class Menu_CoinGroup : MonoBehaviour
{
    public BBUIButton btnAddCoin;
    public TextMeshProUGUI txtCoin;
    void Start()
    {
        if(btnAddCoin != null)
            btnAddCoin.OnPointerClickCallBack_Start.AddListener(TouchAddCoin);
        Config.OnChangeCoin += OnChangeCoin;
        ShowCoin();
    }

    private void OnDestroy()
    {
        Config.OnChangeCoin -= OnChangeCoin;
    }

    public void OnChangeCoin(int coinValue) {
        ShowCoin();
    }

    public void ShowCoin() {
        DOTween.Kill(txtCoin.transform);
        
        txtCoin.text = $"{Config.currCoin}";
        txtCoin.transform.localScale = Vector3.one;
        txtCoin.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 10, 2f).SetEase(Ease.InOutBack).SetRelative(true).SetLoops(3,LoopType.Restart);
    }

    public void TouchAddCoin() {
        MenuManager.instance.OpenShopCoin();
    }
}
