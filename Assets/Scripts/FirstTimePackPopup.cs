using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirstTimePackPopup : MonoBehaviour
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
        // InitPacks();
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        panelClose.onClick.AddListener(TouchClose);
        btnBuy.OnPointerClickCallBack_Start.AddListener(TouchBuy);
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }


    private void OnDestroy()
    {
        PurchaserManager.InitializeSucceeded -= PurchaserManager_InitializeSucceeded;
    }


    private void PurchaserManager_InitializeSucceeded()
    {
        InitIAP();
    }

    private void InitIAP()
    {
        if (PurchaserManager.instance.IsInitialized())
        {
            txtPrice.text = PurchaserManager.instance.GetLocalizedPriceString(Config.IAP_ID.first_time_pack.ToString());
            btnBuy.Interactable = true;
        }
        else
        {
            txtPrice.text = "";
            btnBuy.Interactable = false;
        }
    }


    [Button]
    public void ShowFirstTimePack()
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
        BuyStartPackSuccess();
        return;
#endif
        PurchaserManager.instance.BuyConsumable(Config.IAP_ID.first_time_pack,
            (string _iapID, PurchaserManager.IAP_CALLBACK_STATE _state) =>
            {
                if (_state == PurchaserManager.IAP_CALLBACK_STATE.SUCCESS)
                {
                    lockGroup.gameObject.SetActive(false);
                    if (_iapID.Equals(Config.IAP_ID.first_time_pack.ToString()))
                    {
                        //Buy
                        btnBuy.Interactable = false;
                        BuyStartPackSuccess();
                    }
                }
                else
                {
                    lockGroup.gameObject.SetActive(false);
                    NotificationPopup.instance.AddNotification("Buy Fail!");
                }
            });
    }

    private void BuyStartPackSuccess()
    {
        Config.SetBuyIAP(configPackData.idPack);
        NotificationPopup.instance.AddNotification("Buy Success!");

        if (MenuManager.instance != null && MenuManager.instance.isActiveAndEnabled)
        {
            MenuManager.instance.SetBuyStarterPackSuccess();
            GameDisplay.Instance.OpenRewardPopup(configPackData.configItemShopDatas, false);

            foreach (var item in configPackData.configItemShopDatas)
            {
                if(item.shopItemType == Config.SHOPITEM.COIN)
                    FirebaseManager.Instance.LogEarnVirtualCoin(item.countItem, "fist_time_pack");
            }
        }

        gameObject.SetActive(false);
    }

    public void HidePopup_Finished()
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
        Sequence sequenceShowView = DOTween.Sequence();
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