using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
public class BuyItemPopup : MonoBehaviour
{
    public BBUIButton btnClose, btnWatchAds;
    public GameObject popup;
    public GameObject lockGroup;
    public Image icon;
    public TextMeshProUGUI txtCount, txtDescription, txtPrice, txtTitle;

    void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        btnWatchAds.OnPointerClickCallBack_Start.AddListener(TouchBuyItem);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }

    private ConfigItemShopData configItemShop;
    public void OpenBuyItemPopup(Config.ITEMHELP_TYPE itemType)
    {
        var boosterDefinition = AssetManager.Instance.GetBoosterDefinition(itemType);
        
        configItemShop = new ConfigItemShopData
        {
            shopItemType = Config.ConvertItemHelpTypeToShopItem(itemType),
            countItem = boosterDefinition.amount,
            price = boosterDefinition.price
        };

        icon.sprite = boosterDefinition.sprite;
        icon.SetNativeSize();
        txtTitle.text = $"{boosterDefinition.boosterName}";
        txtCount.text = $"+{boosterDefinition.amount}";
        txtDescription.text = $"{boosterDefinition.description}";
        txtDescription.fontSize = boosterDefinition.frontSize;
        txtPrice.text = $"{boosterDefinition.price}";
        gameObject.SetActive(true);
        InitViews();
    }

    private void TouchClose()
    {
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();

    }
    private void TouchBuyItem()
    {
        if (Config.currCoin >= configItemShop.price)
        {
            Config.SetCoin(Config.currCoin - configItemShop.price);
            FirebaseManager.Instance.LogSpendVirtualCoin(configItemShop.price, "buy_" + configItemShop.shopItemType + "_in_game");
            Config.BuySuccess_ItemShop(configItemShop);
            GamePlayManager.Instance.SetBuyItem_Success();

            NotificationPopup.instance.AddNotification("Buy Success!");
            TouchClose();
        }
        else {
            NotificationPopup.instance.AddNotification("Not enough Coin!");
            GamePlayManager.Instance.OpenShopPopup();
        }

    }
    private void HidePopup_Finished()
    {
        GamePlayManager.Instance.SetUnPause();
        gameObject.SetActive(false);
    }

    private void InitViews()
    {
        SoundManager.Instance.PlaySound_Popup();
        lockGroup.gameObject.SetActive(false);
        popup.gameObject.SetActive(false);
        btnWatchAds.gameObject.SetActive(false);
        icon.gameObject.SetActive(false);
        txtDescription.gameObject.SetActive(false);
        txtCount.transform.parent.gameObject.SetActive(false);

        if (AdsManager.Instance.isRewardAds_Available)
        {
            btnWatchAds.Interactable = true;
        }
        else
        {
            btnWatchAds.Interactable = false;
        }
        
        btnClose.gameObject.SetActive(false);
        InitView_ShowView();
    }

    private void InitView_ShowView()
    {
        Sequence sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
        });
        
        sequenceShowView.InsertCallback(0.03f, () =>
        {
            btnWatchAds.gameObject.SetActive(false);
            btnWatchAds.gameObject.GetComponent<BBUIView>().ShowView();
            
            txtDescription.gameObject.SetActive(true);
            txtDescription.gameObject.GetComponent<BBUIView>().ShowView(); 
        });


        sequenceShowView.InsertCallback(0.5f, () =>
        {
            txtCount.transform.parent.gameObject.SetActive(true);
            txtCount.GetComponentInParent<BBUIView>().ShowView();
            
            icon.gameObject.SetActive(true);
            icon.gameObject.GetComponent<BBUIView>().ShowView();
            
            btnClose.gameObject.SetActive(false);
            btnClose.gameObject.GetComponent<BBUIView>().ShowView();
        });
    }
}
