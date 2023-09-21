using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class CircleScrollRectItemBase : MonoBehaviour
{
    [NonSerialized]
    public CircleScrollRectItemBase prevItem;
    [NonSerialized]
    public CircleScrollRectItemBase nextItem;
    [NonSerialized]
    public int currPosIndex; //index in position list
    [NonSerialized]
    public int contentListIndex; //index in content list

    private int itemListIndex;   //index in ui item list     
    private int contentListLength; //content list length

    //Refresh item data
    public abstract void RefreshItemContent(object data);
    //Get content list item
    public abstract object GetContentListItem(int index);
    //Get content list length
    public abstract int GetContentListLength();

    /// <summary>
    /// Init config
    /// </summary>
    /// <param name="id"></param>
    /// <param name="nItem"></param>
    /// <param name="pItem"></param>
    public void SetItemConfig(int id, CircleScrollRectItemBase nItem, CircleScrollRectItemBase pItem)
    {
        this.itemListIndex = id;
        this.prevItem = pItem;
        this.nextItem = nItem;
    }

    /// <summary>
    /// Calculate content list index
    /// </summary>
    /// <param name="itemListLen"></param>
    public void RefreshContentListLength(int itemListLen)
    {
        contentListLength = GetContentListLength();

        //fixed：advoid endless loop
        if (contentListLength <= 0)
        {
            contentListIndex = 0;
            return;
        }

        //start from center
        int centerIndex = itemListLen / 2;
        if (itemListIndex == centerIndex)
        {
            contentListIndex = 0;
        }
        else if (itemListIndex < centerIndex)
        {
            contentListIndex = contentListLength - (centerIndex - itemListIndex);
        }
        else
        {
            contentListIndex = itemListIndex - centerIndex;
        }
        //circular
        while (contentListIndex < 0)
        {
            contentListIndex += contentListLength;
        }
        //mod
        contentListIndex = contentListIndex % contentListLength;

        UpdateToCurrentContent();
    }

    /// <summary>
    /// Refresh content to current one
    /// </summary>
    public void UpdateToCurrentContent()
    {
        RefreshItemContent(GetContentListItem(contentListIndex));
    }

    /// <summary>
    /// Refresh content to backward direction
    /// </summary>
    /// <param name="shiftStep">shift step</param>
    public void UpdateToPrevContent(int shiftStep = 1)
    {
        contentListIndex = nextItem.contentListIndex - shiftStep;
        contentListIndex = (contentListIndex < 0) ? contentListLength - 1 : contentListIndex;
        RefreshItemContent(GetContentListItem(contentListIndex));
    }

    /// <summary>
    /// Refresh content to forward direction
    /// </summary>
    /// <param name="shiftStep">shift step</param>
    public void UpdateToNextContent(int shiftStep = 1)
    {
        contentListIndex = prevItem.contentListIndex + shiftStep;
        contentListIndex = (contentListIndex == contentListLength) ? 0 : contentListIndex;
        RefreshItemContent(GetContentListItem(contentListIndex));
    }
}
