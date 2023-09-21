using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopIAPGroup : MonoBehaviour
{
    public List<ItemShopIAP> listItemShopIAPs;
    public ItemShopIAP itemShopRemoveAd;
    public ItemShopIAP itemShopRemoveAd_And_Combo10;

    public void InitShopIAP() {
        if (Config.GetRemoveAd()) {
            itemShopRemoveAd.gameObject.SetActive(false);
            itemShopRemoveAd_And_Combo10.gameObject.SetActive(false);
        }

        for (int i=0; i< listItemShopIAPs.Count;i++) {
            listItemShopIAPs[i].InitIAP();
        }
    }
}
