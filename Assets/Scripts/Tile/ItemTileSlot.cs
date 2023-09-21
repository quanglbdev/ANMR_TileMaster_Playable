using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ItemTileSlot : MonoBehaviour
{
    [HideInInspector] public ItemTile itemTile;
    public List<Transform> slotsPosition;

    public void SetItemTile(ItemTile tile) {
        itemTile = tile;
    }

    public void SetItemSlot_Match3(int index, bool isClear) {
        // if (itemTile.GetComponent<GlueTile>() != null)
        // {
        //     itemTile.GetComponent<GlueTile>().SetItemTile_Match3(ItemTileSlot_Match3_CallBack, index, isClear);
        //     return;
        // }

        itemTile.SetItemTile_Match3(ItemTileSlot_Match3_CallBack, index, isClear);
    }

    private void ItemTileSlot_Match3_CallBack() {
        Destroy(gameObject);
    }

    private Sequence _moveResetPosSlotSequence;
    public void ResetPosSlot(int indexSlot)
    {
        itemTile.ResetPosSlot(indexSlot);
        DOTween.Kill(gameObject.transform);
        if(gameObject == null) return;
        _moveResetPosSlotSequence = DOTween.Sequence();
        _moveResetPosSlotSequence.Insert(0f, gameObject.transform.DOMove(slotsPosition[indexSlot].position, 0.1f).SetEase(Ease.OutQuad));
    }
}
