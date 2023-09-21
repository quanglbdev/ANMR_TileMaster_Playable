using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMono : MonoBehaviour {

    public CircleScrollRect scrollRect;
    public Button btn;
    public Text descText;

    [SerializeField]
    public Sprite[] contentSpritePool;

    public static TestMono ins;

    List<CircleItemContent> content = new List<CircleItemContent>();

    // Use this for initialization
    private void Awake () {
        ins = this;
        RandomizeContentList();

        if (null != btn)
        {
            btn.onClick.AddListener(ChangeContentListLength);
        }
        if(null != scrollRect)
        {
            scrollRect.OnDragToTargetPosition.AddListener(OnDragToTargetPosition);
        }

    }

    private void OnDestroy()
    {
        if (null != btn)
        {
            btn.onClick.RemoveListener(ChangeContentListLength);
        }
        if (null != scrollRect)
        {
            scrollRect.OnDragToTargetPosition.RemoveListener(OnDragToTargetPosition);
        }
    }

    public CircleItemContent GetItem(int index)
    {
        return content[index];
    }
    public int GetItemLength()
    {
        return content.Count;
    }

    public void OnDragToTargetPosition(int targetContentListIndex)
    {
        if(null != descText)
        {
            descText.text = "The selected content list item: \n {index:name} = {" + targetContentListIndex.ToString() + ": " + content[targetContentListIndex].name.ToString() + "}";
        }
    }

    private void RandomizeContentList()
    {
        int len = UnityEngine.Random.Range(5, contentSpritePool.Length);
        content.Clear();
        for (int i = 0; i < len; ++i)
        {
            CircleItemContent item = new CircleItemContent();
            item.icon = contentSpritePool[i];
            item.name = "item" + i.ToString();//contentNamePool[i];
            content.Add(item);
        }
        if (null != descText)
        {
            descText.text = "The length of content list changed: " + len.ToString();
        }
    }

    public void ChangeContentListLength()
    {
        RandomizeContentList();

        //When you change content list, you should call .RefreshContentListLength()
        if (null != scrollRect)
        {
            scrollRect.RefreshContentListLength();
        }
    }

}
