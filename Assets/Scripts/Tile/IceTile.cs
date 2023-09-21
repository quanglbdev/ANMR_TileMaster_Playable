using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class IceTile : ItemTile
{
    public SpriteRenderer ice;
    public int lockTouch = 3;

    [FormerlySerializedAs("_icies")] [SerializeField]
    private List<GameObject> _ices;

    [SerializeField] private ParticleSystem iceFx;

    private void Start()
    {
        ice.sprite =
            Resources.Load<Sprite>("Sprite/" + "obstacle/" + (int)obstacleType);
    }

    public override void InitTile(int _indexShow, int _indexOnMap, int _floorIndex, Vector2Int _posTile,
        ItemData _itemData)
    {
        ChangeSprite();
        obstacleType = Config.OBSTACLE_TYPE.ICE;
        indexOnMap = _indexOnMap;
        itemData = _itemData;
        floorIndex = _floorIndex;
        posTile = _posTile;
        HasBee = true;

        Pz = 800 - indexOnMap;
        icon.sprite = AssetManager.Instance.GetTile($"{(int)itemData.itemType}");
        bg.sprite = AssetManager.Instance.GetTile("tile");
        //shadow.sprite = AssetManager.Instance.GetTile("tile");
        border.sprite = AssetManager.Instance.GetTile("tile-light");
        
        ice.sprite =
            Resources.Load<Sprite>("Sprite/" + "obstacle/" + (int)obstacleType);
        
        gameObject.transform.localPosition =
            new Vector3(TILE_SIZE_X * posTile.x, TILE_SIZE_Y * posTile.y, 50 - floorIndex * 5);

        SetOrderLayer_Floor();
        SetLayer_Floor();
        ShowTile();
    }

    protected override void SetOrderLayer_Floor()
    {
        int sortingOrder = 400 - 20 * posTile.y + posTile.x;
        bg.sortingOrder = sortingOrder;
        icon.sortingOrder = sortingOrder;

        border.sortingOrder = sortingOrder;
        foreach (var _ice in _ices)
        {
            _ice.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        }

        shadow.sortingOrder = sortingOrder;
    }

    protected override void SetLayer_Floor()
    {
        isMouseDown = false;
        string sortingLayerName = "Floor" + (floorIndex + 1);
        if (IsTileTutorial)
        {
            sortingLayerName = "Tut";
        }

        bg.sortingLayerName = sortingLayerName;
        icon.sortingLayerName = sortingLayerName;

        border.sortingLayerName = sortingLayerName;

        foreach (var _ice in _ices)
        {
            _ice.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
        }

        shadow.sortingLayerName = sortingLayerName;

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetLayer_Hover()
    {
        bg.sortingLayerName = "Hover";
        icon.sortingLayerName = "Hover";
        foreach (var _ice in _ices)
        {
            _ice.GetComponent<SpriteRenderer>().sortingLayerName = "Hover";
        }

        shadow.sortingLayerName = "Hover";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutHover";
            icon.sortingLayerName = "TutHover";
            foreach (var _ice in _ices)
            {
                _ice.GetComponent<SpriteRenderer>().sortingLayerName = "TutHover";
            }

            shadow.sortingLayerName = "TutHover";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetLayer_Move()
    {
        bg.sortingLayerName = "Move";
        icon.sortingLayerName = "Move";
        foreach (var _ice in _ices)
        {
            _ice.GetComponent<SpriteRenderer>().sortingLayerName = "Move";
        }

        shadow.sortingLayerName = "Move";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutMove";
            icon.sortingLayerName = "TutMove";
            foreach (var _ice in _ices)
            {
                _ice.GetComponent<SpriteRenderer>().sortingLayerName = "TutMove";
            }

            shadow.sortingLayerName = "TutMove";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetOrderLayer_Move(int indexSlot)
    {
        int sortingOrder = 10 * indexSlot;
        bg.sortingOrder = sortingOrder;
        icon.sortingOrder = sortingOrder;

        border.sortingOrder = sortingOrder;
        foreach (var _ice in _ices)
        {
            _ice.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        }

        shadow.sortingOrder = sortingOrder;
    }

    protected override bool IsTouch()
    {
        return lockTouch <= 0;
    }

    protected override void BrakeObstacle()
    {
        if (isShadow) return;
        lockTouch--;
        ChangeSprite();

        if (IsTouch())
        {
            obstacleType = Config.OBSTACLE_TYPE.NONE;
            foreach (var _ice in _ices)
            {
                _ice.transform.DOScale(0f, 0.3f);
            }
        }
    }

    private void ChangeSprite()
    {
        if (lockTouch < 0)
            return;

        foreach (var _ice in _ices)
        {
            _ice.SetActive(false);
        }
        iceFx.Play();
        _ices[lockTouch].SetActive(true);
    }

    public override void SetItemTileTut()
    {
        IsTileTutorial = true;
        const string sortingLayerName = "TutHand";

        bg.sortingLayerName = sortingLayerName;
        icon.sortingLayerName = sortingLayerName;
        shadow.sortingLayerName = sortingLayerName;
        border.sortingLayerName = sortingLayerName;

        foreach (var iceFrag in _ices)
        {
            iceFrag.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
        }
    }
}