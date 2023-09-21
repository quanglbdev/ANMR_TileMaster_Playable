using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonItemLockManager : MonoBehaviour
{
    public List<GameObject> listObj_Opens;
    public List<GameObject> listObj_Locks;

    public Sprite iconSprite, iconLockSprite;
    public Image icon;
    
    public GameObject free;

    public void ShowButtonItem_Lock(bool isLock)
    {
        icon.sprite = isLock ? iconLockSprite : iconSprite;
        icon.SetNativeSize();

        foreach (var obj in listObj_Locks)
        {
            obj.gameObject.SetActive(isLock);
        }

        foreach (var obj in listObj_Opens)
        {
            obj.gameObject.SetActive(!isLock);
        }
        free.gameObject.SetActive(false);
    }
    
    public void ShowButtonItemLockWithoutSprite(bool isLock)
    {
        icon.SetNativeSize();

        foreach (var obj in listObj_Locks)
        {
            obj.gameObject.SetActive(isLock);
        }

        foreach (var obj in listObj_Opens)
        {
            obj.gameObject.SetActive(!isLock);
        }
        free.gameObject.SetActive(false);

    }
    
    public void SetSpriteIcon(bool isLock)
    {
        icon.sprite = isLock ? iconLockSprite : iconSprite;
        icon.SetNativeSize();
    }

    public void SetFree()
    {
        free.gameObject.SetActive(true);
        foreach (var obj in listObj_Opens)
        {
            obj.gameObject.SetActive(false);
        }
    }
}