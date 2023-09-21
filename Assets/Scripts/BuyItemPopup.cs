using System;
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
        switch (configItemShop.shopItemType)
        {
            case Config.SHOPITEM.UNDO:
                Config.SetCount_ItemHelp(Config.ITEMHELP_TYPE.UNDO, configItemShop.countItem);
                break;
            case Config.SHOPITEM.SUGGEST:
                Config.SetCount_ItemHelp(Config.ITEMHELP_TYPE.SUGGEST, configItemShop.countItem);
                break;
            case Config.SHOPITEM.SHUFFLE:
                Config.SetCount_ItemHelp(Config.ITEMHELP_TYPE.SHUFFLE, configItemShop.countItem);
                break;
            case Config.SHOPITEM.TILE_RETURN:
                Config.SetCount_ItemHelp(Config.ITEMHELP_TYPE.TILE_RETURN, configItemShop.countItem);
                break;
            case Config.SHOPITEM.EXTRA_SLOT:
                Config.SetCount_ItemHelp(Config.ITEMHELP_TYPE.EXTRA_SLOT, configItemShop.countItem);
                break;
        }
        GamePlayManager.Instance.SetBuyItem_Success();
        NotificationPopup.instance.AddNotification("Buy Success!");
        TouchClose();
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