using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class ShopPopup2 : MonoBehaviour
{
    public static ShopPopup2 instance;

    public BBUIView popup;

    public BBUIButton btnClose;
    public GameObject lockGroup;
    public ScrollRect scrollView;

    public Image blink;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        PurchaserManager.InitializeSucceeded += PurchaserManager_InitializeSucceeded;
        popup.ShowBehavior.onCallback_Completed.AddListener(ShowView_Finished);
        popup.HideBehavior.onCallback_Completed.AddListener(HideView_Finished);

        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        btnLoadMores.OnPointerClickCallBack_Start.AddListener(TouchLoadMore);
        StartBlink(blink);
        InitIAP();
    }

    private void OnDestroy()
    {
        PurchaserManager.InitializeSucceeded -= PurchaserManager_InitializeSucceeded;
        popup.ShowBehavior.onCallback_Completed.RemoveAllListeners();
        popup.HideBehavior.onCallback_Completed.RemoveAllListeners();

        btnClose.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnLoadMores.OnPointerClickCallBack_Start.RemoveAllListeners();
    }

    private void StartBlink(Image image)
    {
        // Fade In
        image.DOFade(1f, 1.5f).OnComplete(() =>
        {
            // Fade Out
            image.DOFade(0f, 1.5f).OnComplete(() =>
            {
                // Start a new blink cycle
                StartBlink(image);
            });
        });
    }

    [Button("OpenPopup")]
    public void OpenPopup()
    {
        lockGroup.SetActive(true);
        gameObject.SetActive(true);
        InitLoadMores();
        TouchLoadMore();

        scrollView.verticalNormalizedPosition = 1f;
        ShowViews();
    }

    private void ShowViews()
    {
        SoundManager.Instance.PlaySound_Popup();
        lockGroup.SetActive(true);

        StartCoroutine(ShowViews_IEnumerator());
    }

    private IEnumerator ShowViews_IEnumerator()
    {
        popup.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);
        popup.gameObject.SetActive(true);
        popup.ShowView();

        yield return new WaitForSeconds(0.1f);
        btnClose.gameObject.SetActive(true);
        btnClose.GetComponent<BBUIView>().ShowView();
    }

    private void ShowView_Finished()
    {
        lockGroup.SetActive(false);
    }

    private void HideView_Finished()
    {
        gameObject.SetActive(false);
    }

    private void TouchClose()
    {
        TouchCloseDelay();
    }

    private async UniTask TouchCloseDelay()
    {
        lockGroup.SetActive(true);
        popup.HideView();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        //canvas.enabled = false;
    }


    #region LOADMORES

    [Header("LOAD MORES")] public BBUIButton btnLoadMores;

    public GameObject loadMoreObj;
    public List<GameObject> listLoadMore_ItemShops = new();

    private void TouchLoadMore()
    {
        for (int i = 0; i < listLoadMore_ItemShops.Count; i++)
        {
            listLoadMore_ItemShops[i].SetActive(true);
        }

        loadMoreObj.SetActive(false);
        InitIAP_RemoveAd();
    }

    private void InitLoadMores()
    {
        loadMoreObj.SetActive(true);
        for (int i = 0; i < listLoadMore_ItemShops.Count; i++)
        {
            listLoadMore_ItemShops[i].SetActive(false);
        }
    }

    #endregion

    #region REMOVE_AD

    public ItemShopIAP iapRemoveAd;

    private void InitIAP_RemoveAd()
    {
        if (Config.GetRemoveAd())
        {
            iapRemoveAd.gameObject.SetActive(false);
        }
        else
        {
            iapRemoveAd.gameObject.SetActive(true);
        }
    }

    #endregion

    #region IAP

    public List<ItemShopIAP> listItemShopIAPs = new List<ItemShopIAP>();

    public void TouchBuy_ShopItem(ConfigPackData configPackData)
    {
        lockGroup.gameObject.SetActive(true);

        PurchaserManager.instance.BuyConsumable(configPackData.idPack,
            (string _iapID, PurchaserManager.IAP_CALLBACK_STATE _state) =>
            {
                if (_state == PurchaserManager.IAP_CALLBACK_STATE.SUCCESS)
                {
                    lockGroup.gameObject.SetActive(false);
                    if (_iapID.Equals(Config.IAP_ID.removead.ToString()))
                    {
                        Config.SetRemoveAd();
                        Config.SetBuyIAP(configPackData.idPack);
                        EventDispatcher.Instance.PostEvent(EventID.BuyNoAdsPack);
                        NotificationPopup.instance.AddNotification("RemoveAd Success!");
                        BuyPackSuccess(configPackData);
                        InitIAP_RemoveAd();
                    }
                    else
                    {
                        BuyPackSuccess(configPackData);
                    }
                }
                else
                {
                    lockGroup.gameObject.SetActive(false);
                    NotificationPopup.instance.AddNotification("Buy Fail!");
                }
            });
    }

    private void BuyPackSuccess(ConfigPackData configPackData)
    {
        foreach (var item in configPackData.configItemShopDatas)
        {
            // switch (item.shopItemType)
            // {
            //     case Config.SHOPITEM.COIN:
            //         PlayerPrefs.SetInt(Config.COIN, Config.currCoin + item.countItem);
            //         PlayerPrefs.Save();
            //         break;
            //     case Config.SHOPITEM.UNDO:
            //         PlayerPrefs.SetInt(Config.ITEMHELP_TYPE.UNDO.ToString(),
            //             item.countItem + Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.UNDO));
            //         PlayerPrefs.Save();
            //         break;
            //     case Config.SHOPITEM.SUGGEST:
            //         PlayerPrefs.SetInt(Config.ITEMHELP_TYPE.SUGGEST.ToString(),
            //             item.countItem + Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SUGGEST));
            //         PlayerPrefs.Save();
            //         break;
            //     case Config.SHOPITEM.SHUFFLE:
            //         PlayerPrefs.SetInt(Config.ITEMHELP_TYPE.SHUFFLE.ToString(),
            //             item.countItem + Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SHUFFLE));
            //         PlayerPrefs.Save();
            //         break;
            //     case Config.SHOPITEM.TILE_RETURN:
            //         PlayerPrefs.SetInt(Config.ITEMHELP_TYPE.TILE_RETURN.ToString(),
            //             item.countItem + Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.TILE_RETURN));
            //         PlayerPrefs.Save();
            //         break;
            //     case Config.SHOPITEM.EXTRA_SLOT:
            //         PlayerPrefs.SetInt(Config.ITEMHELP_TYPE.EXTRA_SLOT.ToString(),
            //             item.countItem + Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.EXTRA_SLOT));
            //         PlayerPrefs.Save();
            //         break;
            //     case Config.SHOPITEM.FREE_HEART:
            //         // if (Config.FREE_HEART_TIME == -1)
            //         // {
            //         //     Config.FREE_HEART_DATE_ADD = Config.GetDateTimeNow();
            //         //     Config.FREE_HEART_TIME = item.countItem;
            //         // }
            //         // else
            //         // {
            //         //     Config.FREE_HEART_TIME += item.countItem;
            //         // }
            //         // break;
            //     case Config.SHOPITEM.HEART:
            //         var heartValue = item.countItem + Config.currHeart;
            //         if (heartValue > Config.MAX_HEART)
            //             heartValue = Config.MAX_HEART;
            //         PlayerPrefs.SetInt(Config.HEART_KEY, heartValue);
            //         PlayerPrefs.Save();
            //         break;
            //     case Config.SHOPITEM.COMBO:
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
        }

        GameDisplay.Instance.OpenRewardPopup(configPackData.configItemShopDatas, false);
        foreach (var data in configPackData.configItemShopDatas)
        {
            if (data.shopItemType == Config.SHOPITEM.COIN)
            {
                FirebaseManager.Instance.LogEarnVirtualCoin(data.countItem, "shop");
            }
        }
        lockGroup.gameObject.SetActive(false);

        //gameObject.SetActive(false);
    }


    private void PurchaserManager_InitializeSucceeded()
    {
        InitIAP();
    }

    private void InitIAP()
    {
        for (int i = 0; i < listItemShopIAPs.Count; i++)
        {
            listItemShopIAPs[i].InitIAP();
        }
    }

    #endregion
}