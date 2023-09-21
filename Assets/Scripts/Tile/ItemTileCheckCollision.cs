using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemTileCheckCollision : MonoBehaviour
{
    public ItemTile itemTile;
    public int countCollision;

    private void Start()
    {
        ResetCollisionCount();
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("TileCheckCollision")) return;
        if (itemTile.ItemTileState != Config.ITEMTILE_STATE.FLOOR && itemTile.ItemTileState != Config.ITEMTILE_STATE.RETURN_FLOOR) return;
        if (collision.GetComponent<ItemTileCheckCollision>().itemTile.floorIndex <= itemTile.floorIndex) return;
        
        countCollision++;

        if (itemTile.GetComponent<GlueTile>() != null)
        {
            itemTile.GetComponent<GlueTile>().SetTouch_Available(false);
            itemTile.GetComponent<GlueTile>().SetShadow_Available(true);
            return;
        }
        
        itemTile.SetTouch_Available(false);
        itemTile.SetShadow_Available(true);
    }


    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("TileCheckCollision")) return;
        if (itemTile.ItemTileState != Config.ITEMTILE_STATE.FLOOR && itemTile.ItemTileState != Config.ITEMTILE_STATE.RETURN_FLOOR) return;
        if (collision.GetComponent<ItemTileCheckCollision>().itemTile.floorIndex > itemTile.floorIndex)
        {
            countCollision--;
        }

        if (countCollision <= 0)
        {
            if (itemTile.GetComponent<GlueTile>() != null)
            {
                itemTile.GetComponent<GlueTile>().SetTouch_Enable();
                return;
            }
            
            itemTile.SetTouch_Enable();
        }
    }

    private void ResetCollisionCount()
    {
        countCollision = 0;
    }
}