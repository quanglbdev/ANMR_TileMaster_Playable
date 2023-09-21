using System;

[Serializable]
public class ConfigItemShopData
{
    public Config.SHOPITEM shopItemType;
    public int countItem;
    public int price;

    public ConfigItemShopData()
    {
    }

    public ConfigItemShopData(Config.SHOPITEM shopItemType, int countItem)
    {
        this.shopItemType = shopItemType;
        this.countItem = countItem;
        price = 0;
    }
}