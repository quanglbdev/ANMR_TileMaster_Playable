using UnityEngine;

public class BeeTile : ItemTile
{
    public Bee bee;
    
    public override void InitTile(int _indexShow,int _indexOnMap, int _floorIndex, Vector2Int _posTile, ItemData _itemData) {

        indexOnMap = _indexOnMap;
        //indexShowTile = _indexShow;
        itemData = _itemData;
        floorIndex = _floorIndex;
        posTile = _posTile;
        HasBee = false;
        //pZ = 800 - floorIndex * 100f + posTile.y * 2 + 2 - posTile.x * 0.1f; 
        Pz = 800 - indexOnMap;
        
        icon.sprite = AssetManager.Instance.GetTile($"{(int)itemData.itemType}");
        bg.sprite = AssetManager.Instance.GetTile("tile");
        shadow.sprite = AssetManager.Instance.GetTile("tile");
        border.sprite = AssetManager.Instance.GetTile("tile-light");
        
        gameObject.transform.localPosition = new Vector3(TILE_SIZE_X * posTile.x, TILE_SIZE_Y * posTile.y, 50 - floorIndex * 5);
        SetOrderLayer_Floor();
        SetLayer_Floor();
        ShowTile();
    }
    
    public override void SetItemTileTut()
    {
        IsTileTutorial = true;
        const string sortingLayerName = "TutHand";
        
        bee.skeleton.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        bee.skeleton.GetComponent<MeshRenderer>().sortingOrder = icon.sortingOrder + 1;
        bg.sortingLayerName = sortingLayerName;
        icon.sortingLayerName = sortingLayerName;
        shadow.sortingLayerName = sortingLayerName;
        border.sortingLayerName = sortingLayerName;
    }
}
