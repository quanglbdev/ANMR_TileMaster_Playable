using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemoveAdsPopup : MonoBehaviour
{
    public ConfigPackData configPackData;
    public BBUIButton btnClose, btnBuy;
    public Button panelClose;
    public GameObject popup;
    public TextMeshProUGUI txtPrice;
    public GameObject lockGroup;

    void Start()
    {
        PurchaserManager.InitializeSucceeded += PurchaserManager_InitializeSucceeded;
        InitIAP();
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        panelClose.onClick.AddListener(TouchClose);
        btnBuy.OnPointerClickCallBack_Start.AddListener(TouchBuy);
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }

    private void PurchaserManager_InitializeSucceeded()
    {
        InitIAP();
    }

    private void InitIAP()
    {
        if (PurchaserManager.instance.IsInitialized())
        {
            txtPrice.text =
                PurchaserManager.instance.GetLocalizedPriceString(Config.IAP_ID.removead.ToString());
            btnBuy.Interactable = true;
        }
        else
        {
            txtPrice.text = "$500";
            btnBuy.Interactable = false;
        }
    }

    public void ShowRemoveAdsPopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void TouchClose()
    {
        SoundManager.Instance.PlaySound_Click();        
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void TouchBuy()
    {
        lockGroup.gameObject.SetActive(true);
#if UNITY_EDITOR
        btnBuy.Interactable = false;
        BuyRemoveAdsPackSuccess();
        return;
#endif
        PurchaserManager.instance.BuyConsumable(Config.IAP_ID.removead,
            (string _iapID, PurchaserManager.IAP_CALLBACK_STATE _state) =>
            {
                if (_state == PurchaserManager.IAP_CALLBACK_STATE.SUCCESS)
                {
                    lockGroup.gameObject.SetActive(false);
                    if (_iapID.Equals(Config.IAP_ID.removead.ToString()))
                    {
                        //Buy
                        btnBuy.Interactable = false;
                        BuyRemoveAdsPackSuccess();
                    }
                }
                else
                {
                    lockGroup.gameObject.SetActive(false);
                    NotificationPopup.instance.AddNotification("Buy Fail!");
                }
            });
    }

    private void BuyRemoveAdsPackSuccess()
    {
        Config.SetRemoveAd();
        Config.SetBuyIAP(configPackData.idPack);
        NotificationPopup.instance.AddNotification("Buy Success!");
        GameDisplay.Instance.OpenRewardPopup(configPackData.configItemShopDatas, false);
        foreach (var item in configPackData.configItemShopDatas)
        {
            if(item.shopItemType == Config.SHOPITEM.COIN)
                FirebaseManager.Instance.LogEarnVirtualCoin(item.countItem, "remove_ad_pack");
        }
        gameObject.SetActive(false);
    }

    private void HidePopup_Finished()
    {
        gameObject.SetActive(false);
    }

    private void InitViews()
    {
        SoundManager.Instance.PlaySound_Popup();
        lockGroup.gameObject.SetActive(false);
        popup.gameObject.SetActive(false);
        btnBuy.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);

        InitView_ShowView();
    }

    private void InitView_ShowView()
    {
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
        });
        sequenceShowView.InsertCallback(0.2f, () =>
        {
            btnBuy.gameObject.SetActive(false);
            btnBuy.gameObject.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            btnClose.gameObject.SetActive(false);
            btnClose.gameObject.GetComponent<BBUIView>().ShowView();
        });
    }
}