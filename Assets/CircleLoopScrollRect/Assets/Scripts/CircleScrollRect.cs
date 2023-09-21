using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[SerializeField]
public class CircleScrollRect : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //UI Item List
    //[SerializeField] //Change to serializefield if you want to add item from inspector
    [NonSerialized]
    public CircleScrollRectItemBase[] listItems;
    //Slide Direction
    [SerializeField]
    public bool horizontal = false;
    //Sliding threshold
    [NonSerialized]
    public float oneShiftThreshold = 90.0f;
    //UI Item Positin List
    private Vector3[] itemPostions;
    //Start position of draging
    private Vector2 dragStartPostion;
    //if the list need to adjust after draging
    private bool needAdjust = false;
    //Drag to dest position event
    public class ReachDestinationEvent : UnityEvent<int> { }
    //Event
    private ReachDestinationEvent onDragToTargetPosition = new ReachDestinationEvent();
    public ReachDestinationEvent OnDragToTargetPosition
    {
        get { return onDragToTargetPosition; }
        set { onDragToTargetPosition = value; }
    }
    //Target position index
    public int targetPositionIndex = -1;
    //Target content list index
    private int targetContentListIndex = -1;
    /// <summary>
    /// Comparision of position y
    /// </summary>
    private Comparison<CircleScrollRectItemBase> ComparisionY =
        delegate (CircleScrollRectItemBase itemA, CircleScrollRectItemBase itemB)
        {
            if (itemA.transform.localPosition.y == itemB.transform.localPosition.y) return 0;
            return (itemA.transform.localPosition.y > itemB.transform.localPosition.y) ? -1 : 1;
        };

    /// <summary>
    ///Comparision of position x
    /// </summary>
    private Comparison<CircleScrollRectItemBase> ComparisionX =
        delegate (CircleScrollRectItemBase itemA, CircleScrollRectItemBase itemB)
        {
            if (itemA.transform.localPosition.x == itemB.transform.localPosition.x) return 0;
            return (itemA.transform.localPosition.x > itemB.transform.localPosition.x) ? -1 : 1;
        };

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        //Get child items
        listItems = new CircleScrollRectItemBase[gameObject.transform.childCount];
        int index = 0;
        foreach (Transform trans in gameObject.transform)
        {
            //Debug.Log("child" + trans.name);
            CircleScrollRectItemBase item = trans.GetComponent<CircleScrollRectItemBase>();
            listItems[index++] = item;
        }
        //Check
        if (null == listItems || listItems.Length == 0)
        {
            return;
        }
        //Sort UI item list by postion
        if (horizontal)
        {
            //ascending order
            Array.Sort(listItems, ComparisionX);
        }
        else
        {
            // descending order
            Array.Sort(listItems, ComparisionY);
        }
        
        //element relationship and positions
        itemPostions = new Vector3[listItems.Length];
        for (int i = 0; i < listItems.Length; ++i)
        {
            listItems[i].SetItemConfig(i, //set id
                                       listItems[(i + 1) % listItems.Length], //next item
                                       listItems[(i - 1 + listItems.Length) % listItems.Length]); //previous item

            itemPostions[i] = new Vector3(listItems[i].transform.localPosition.x,
                                            listItems[i].transform.localPosition.y,
                                              listItems[i].transform.localPosition.z);
            listItems[i].currPosIndex = i;
        }
        RefreshContentListLength();
        TriggerDragToTargetPostionEvent();
        targetContentListIndex = -1;
        if (targetPositionIndex < 0)
        {
            targetPositionIndex = listItems.Length / 2;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        listItems = null;
    }

    private void Update()
    {

    }
    
    /// <summary>
    /// Begin Dragging
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        needAdjust = false;
        dragStartPostion = eventData.position;
    }

    /// <summary>
    /// Update postion when dragging
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (horizontal)
        {
            if(ShiftListHorizontal(eventData.position - dragStartPostion))
            {
                dragStartPostion = eventData.position; //Update the start draging postion
            }
        }
        else
        {
            if (ShiftListVertical(eventData.position - dragStartPostion))
            {
                dragStartPostion = eventData.position; //Update the start draging postion
            }
        }

    }

    /// <summary>
    /// End Drag
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if(needAdjust)
        {
            if (horizontal)
            {
                AdjustLocationX();
            }
            else
            {
                AdjustLocationY();
            }       
        }
        TriggerDragToTargetPostionEvent();
    }

    /// <summary>
    /// Refresh When the conent list changes
    /// </summary>
    public void RefreshContentListLength()
    {
        for (int i = 0; i < listItems.Length; ++i)
        {
            listItems[i].RefreshContentListLength(listItems.Length);
        }
    }

    /// <summary>
    /// Shift the list when dragging vertical
    /// </summary>
    /// <param name="delta"></param>
    /// <returns>need update initial dragging postion</returns>
    public bool ShiftListVertical(Vector2 delta)
    {
        if (null == listItems || listItems.Length < 2 || delta.y == 0)
        {
            return false;
        }
        needAdjust = true;
        bool downSlide = (delta.y < 0); 
        for (int i = 0; i < listItems.Length; ++i)
        {
            int curIndex = listItems[i].currPosIndex;
            int targetIndex = downSlide ? curIndex + 1 : curIndex - 1;

            if (targetIndex >= 0 && targetIndex < listItems.Length)
            {
                //Lerp ratio
                float ratio = Mathf.Abs(delta.y) / oneShiftThreshold;
                //Lerp
                if (ratio <= 1)
                {
                    listItems[i].transform.localPosition = Vector3.Lerp(listItems[i].transform.localPosition,
                                                                            itemPostions[targetIndex], ratio);
                }
                //If reach the target postion
                if (ratio >= 1 || Mathf.Abs(listItems[i].transform.localPosition.y - itemPostions[targetIndex].y) <= 1.0f)
                {                       
                    ShiftOneTime(!downSlide);
                    return true;
                }
            }
        }          
        
        return false;
    }

    /// <summary>
    /// Shift the list when dragging horizontal
    /// </summary>
    /// <param name="delta"></param>
    /// <returns>need update initial dragging postion</returns>
    public bool ShiftListHorizontal(Vector2 delta)
    {
        if (null == listItems || listItems.Length < 2 || delta.x == 0)
        {
            return false;
        }
        needAdjust = true;
        bool leftSlide = (delta.x < 0);
        for (int i = 0; i < listItems.Length; ++i)
        {
            int curIndex = listItems[i].currPosIndex;
            int targetIndex = leftSlide ? curIndex + 1 : curIndex - 1;

            if (targetIndex >= 0 && targetIndex < listItems.Length)
            {              
                float ratio = Mathf.Abs(delta.x) / oneShiftThreshold;

                if (ratio <= 1)
                {
                    listItems[i].transform.localPosition = Vector3.Lerp(listItems[i].transform.localPosition,
                                                                            itemPostions[targetIndex], ratio);
                }

                if (ratio >= 1 || Mathf.Abs(listItems[i].transform.localPosition.x - itemPostions[targetIndex].x) <= 1.0f)
                {
                    ShiftOneTime(!leftSlide);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Do one circular shift one time
    /// </summary>
    /// <param name="reverse">reverse shift</param>
    private void ShiftOneTime(bool reverse, int shiftStep = 1)
    {
        needAdjust = false; 
        for (int i = 0; i < listItems.Length; ++i)
        {
            int curIndex = listItems[i].currPosIndex;
            if (reverse) 
            {
                listItems[i].transform.localPosition = itemPostions[(curIndex - shiftStep + listItems.Length) % listItems.Length];
                listItems[i].currPosIndex = (curIndex - shiftStep + listItems.Length) % listItems.Length;
                if(curIndex - shiftStep < 0)
                {
                    listItems[i].UpdateToNextContent();
                }         
            }
            else
            {
                listItems[i].transform.localPosition = itemPostions[(curIndex + shiftStep) % itemPostions.Length];
                listItems[i].currPosIndex = (curIndex + shiftStep) % itemPostions.Length;
                if(curIndex + shiftStep >= itemPostions.Length)
                {
                    
                    listItems[i].UpdateToPrevContent();
                }
            }

        }
    }
    private void SlideItemPosition(bool reverse, int closestIndex, int targetIndex)
    {
        for (int i = 0; i < listItems.Length; ++i)
        {
            int curIndex = listItems[i + closestIndex].currPosIndex;
            if(i == 0)
            {

                continue;
            }
            if (reverse)
            {
                listItems[i].transform.localPosition = itemPostions[(curIndex - 1 + listItems.Length) % listItems.Length];
                listItems[i].currPosIndex = (curIndex - 1 + listItems.Length) % listItems.Length;
                if (curIndex - 1 < 0)
                {
                    listItems[i].UpdateToNextContent();
                }
            }
            else
            {
                listItems[i].transform.localPosition = itemPostions[(curIndex + 1) % itemPostions.Length];
                listItems[i].currPosIndex = (curIndex + 1) % itemPostions.Length;
                if (curIndex + 1 >= itemPostions.Length)
                {

                    listItems[i].UpdateToPrevContent();
                }
            }

        }
    }
    /// <summary>
    /// Adjust position y
    /// </summary>
    public void AdjustLocationY()
    {
        int shiftStep = 1;
        int halfPosIndex = itemPostions.Length / 2;
        //Attach to target position
        if(targetPositionIndex < itemPostions.Length)
        {
            halfPosIndex = targetPositionIndex;
        }
        //find the item closest to target poistion
        bool reverse = false;
        float disMin = float.MaxValue;
        for (int i = 0; i < listItems.Length; ++i)
        {
            float dis = listItems[i].transform.localPosition.y - itemPostions[halfPosIndex].y;
            if (Mathf.Abs(dis) < disMin)
            {
                disMin = Mathf.Abs(dis);
                reverse = (dis < 0) ? true : false; //upside is the reverse direction
                shiftStep = Math.Abs(listItems[i].currPosIndex - halfPosIndex);
            }
        }
        //var item = (CircleItemContent)listItems[targetIndex].GetContentListItem(listItems[targetIndex].contentListIndex);
        //Debug.LogError("Direction: " + reverse.ToString() + "; step: " + shiftStep + "; Target: " + item.name);
        ShiftOneTime(reverse, shiftStep);        
    }
    /// <summary>
    /// Adjust position x
    /// </summary>
    public void AdjustLocationX()
    {
        int shiftStep = 1;
        int halfPosIndex = itemPostions.Length / 2;
        //Attach to target position
        if (targetPositionIndex < itemPostions.Length)
        {
            halfPosIndex = targetPositionIndex;
        }
        //find the item closest to target poistion
        bool reverse = false;
        float disMin = float.MaxValue;
        for (int i = 0; i < listItems.Length; ++i)
        {
            float dis = listItems[i].transform.localPosition.x - itemPostions[halfPosIndex].x;
            if (Mathf.Abs(dis) < disMin)
            {
                disMin = Mathf.Abs(dis);
                reverse = (dis < 0) ? true : false; //upside is the reverse direction
                shiftStep = Math.Abs(listItems[i].currPosIndex - halfPosIndex);
            }
        }
        ShiftOneTime(reverse, shiftStep);
    }

    /// <summary>
    /// Trigger event while dragging to target position
    /// </summary>
    private void TriggerDragToTargetPostionEvent()
    {
        if(targetPositionIndex < listItems.Length)
        {
            int targetIndex = 0;
            for(int i = 0; i < listItems.Length; ++i)
            {
                if(listItems[i].currPosIndex == targetPositionIndex)
                {
                    targetIndex = listItems[i].contentListIndex;
                    break;
                }
            }
            if(targetContentListIndex != targetIndex)
            {
                targetContentListIndex = targetIndex;
                //Debug.Log(targetIndex);
                if (null != onDragToTargetPosition)
                {                    
                    onDragToTargetPosition.Invoke(targetContentListIndex);
                }
            }
        }
    }
}
