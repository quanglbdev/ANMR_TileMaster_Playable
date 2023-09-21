using System;
using TMPro;
using UnityEngine;

public class RewardItem : MonoBehaviour
{
    public TextMeshProUGUI txtCountItem;
    private ConfigItemShopData _configItemShopData;
    public BBUIView view;

    public void InitSpinItem(ConfigItemShopData data)
    {
        _configItemShopData = data;
        switch (_configItemShopData.shopItemType)
        {
            case Config.SHOPITEM.COIN:
                txtCountItem.text = $"{_configItemShopData.countItem}";
                break;
            case Config.SHOPITEM.UNDO:
            case Config.SHOPITEM.SUGGEST:
            case Config.SHOPITEM.SHUFFLE:
            case Config.SHOPITEM.TILE_RETURN:
            case Config.SHOPITEM.EXTRA_SLOT:
            case Config.SHOPITEM.STAR:
                txtCountItem.text = $"x{_configItemShopData.countItem}";
                break;
            case Config.SHOPITEM.FREE_HEART:
                txtCountItem.text = $"{_configItemShopData.countItem}m";
                break;
            case Config.SHOPITEM.HEART:
                txtCountItem.text = $"+{_configItemShopData.countItem}";
                break;
            case Config.SHOPITEM.COMBO:
                txtCountItem.text = $"{_configItemShopData.countItem}";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        view.gameObject.SetActive(false);
    }

    public void ShowView()
    {
        view.gameObject.SetActive(true);
        view.ShowView();
    }
}