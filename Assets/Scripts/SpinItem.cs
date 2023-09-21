using System;
using TMPro;
using UnityEngine;
public class SpinItem : MonoBehaviour
{
    public TextMeshProUGUI txtCountItem;
    private ConfigItemShopData configItemShopData;
    
    public void InitSpinItem(ConfigItemShopData _configItemShopData) {
        configItemShopData = _configItemShopData;
        switch (configItemShopData.shopItemType)
        {
            case Config.SHOPITEM.COIN:
                txtCountItem.text = $"{configItemShopData.countItem}";
                break;
            case Config.SHOPITEM.UNDO:
                txtCountItem.text = $"x{configItemShopData.countItem}";
                break;
            case Config.SHOPITEM.SUGGEST:
                txtCountItem.text = $"x{configItemShopData.countItem}";
                break;
            case Config.SHOPITEM.SHUFFLE:
                txtCountItem.text = $"x{configItemShopData.countItem}";
                break;
            case Config.SHOPITEM.TILE_RETURN:
                txtCountItem.text = $"x{configItemShopData.countItem}";
                break;
            case Config.SHOPITEM.EXTRA_SLOT:
                txtCountItem.text = $"x{configItemShopData.countItem}";
                break;
            case Config.SHOPITEM.FREE_HEART:
                txtCountItem.text = $"{configItemShopData.countItem}m";
                break;
            case Config.SHOPITEM.HEART:
                txtCountItem.text = $"x{configItemShopData.countItem}";
                break;
            case Config.SHOPITEM.COMBO:
                txtCountItem.text = $"x{configItemShopData.countItem}";
                break;
        }
      
    }
}
