using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopIAP : MonoBehaviour
{
    public ConfigPackData configPackData;

    public BBUIButton btnBuy;
    public Button btnBuy2;
    public TextMeshProUGUI txtPrice;
    
    public virtual void Start()
    {
        if(btnBuy != null)
            btnBuy.OnPointerClickCallBack_Start.AddListener(TouchBuy);
        
        if(btnBuy2 != null)
            btnBuy2.onClick.AddListener(TouchBuy);
    }

    private void OnDestroy()
    {
        if(btnBuy != null)
            btnBuy.OnPointerClickCallBack_Start.RemoveAllListeners();
        
        if(btnBuy2 != null)
            btnBuy2.onClick.RemoveAllListeners();
    }

    private void TouchBuy()
    {
        if (ShopPopup2.instance != null && ShopPopup2.instance.isActiveAndEnabled)
        {
            ShopPopup2.instance.TouchBuy_ShopItem(configPackData);
        }
        else if(WinPopup.Instance != null && WinPopup.Instance.isActiveAndEnabled)
        {
            WinPopup.Instance.BuyRemoveAd(configPackData);
        }
       
    }

    public void InitIAP() {
        if (PurchaserManager.instance.IsInitialized())
        {
            txtPrice.text = PurchaserManager.instance.GetLocalizedPriceString(configPackData.idPack.ToString());
            if(btnBuy != null)
                btnBuy.Interactable = true;
            
            if(btnBuy2 != null)
                btnBuy2.interactable = true;
        }
        else {
            txtPrice.text = "";
            if(btnBuy != null)
                btnBuy.Interactable = false;
            
            if(btnBuy2 != null)
                btnBuy2.interactable = false;
        }
    }
}
