
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleItemContent
{
    public string name;
    public Sprite icon;
}

public class CircleTestItem : CircleScrollRectItemBase
{
    public Text nameTxt;
    public Image iconImg;

    public override void RefreshItemContent(object data)
    {
        var contentItem = data as CircleItemContent;
        if(null != contentItem)
        {
            if(null != nameTxt)
            {
                nameTxt.text = contentItem.name;
            }
            if(null != iconImg)
            {
                iconImg.sprite = contentItem.icon;
            }          
        }
        
    }

    public override object GetContentListItem(int index)
    {
        return TestMono.ins.GetItem(index);
    }

    public override int GetContentListLength()
    {
        return TestMono.ins.GetItemLength();
    }

}
