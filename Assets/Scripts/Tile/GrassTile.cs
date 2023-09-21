using DG.Tweening;
using UnityEngine;

public class GrassTile : ItemTile
{
    public SpriteRenderer grass, grass2;
    public SpriteRenderer grassShadow;
    [SerializeField] private ParticleSystem grassFx;

    public int lockTouch = 2;
    // [SerializeField] private GameObject grass1, grass2;

    private void Start()
    {
        grass.sprite =
            Resources.Load<Sprite>("Sprite/" + "obstacle/" + (int)obstacleType);
    }

    public override void InitTile(int _indexShow, int _indexOnMap, int _floorIndex, Vector2Int _posTile,
        ItemData _itemData)
    {
        ChangeSprite();
        obstacleType = Config.OBSTACLE_TYPE.GRASS;
        indexOnMap = _indexOnMap;
        itemData = _itemData;
        floorIndex = _floorIndex;
        posTile = _posTile;
        HasBee = true;

        Pz = 800 - indexOnMap;
        icon.sprite = AssetManager.Instance.GetTile($"{(int)itemData.itemType}");
        bg.sprite = AssetManager.Instance.GetTile("tile");
        shadow.sprite = AssetManager.Instance.GetTile("tile");
        border.sprite = AssetManager.Instance.GetTile("tile-light");
        
        grass.sprite =
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
        grass.sortingOrder = sortingOrder;
        grass2.sortingOrder = sortingOrder;
        shadow.sortingOrder = sortingOrder;
        grassShadow.sortingOrder = sortingOrder;

        border.sortingOrder = sortingOrder;
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
        shadow.sortingLayerName = sortingLayerName;
        grass.sortingLayerName = sortingLayerName;
        grass2.sortingLayerName = sortingLayerName;
        grassShadow.sortingLayerName = sortingLayerName;

        border.sortingLayerName = sortingLayerName;


        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetLayer_Hover()
    {
        bg.sortingLayerName = "Hover";
        icon.sortingLayerName = "Hover";
        grass.sortingLayerName = "Hover";
        grass2.sortingLayerName = "Hover";
        shadow.sortingLayerName = "Hover";
        grassShadow.sortingLayerName = "Hover";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutHover";
            icon.sortingLayerName = "TutHover";
            grass.sortingLayerName = "TutHover";
            grass2.sortingLayerName = "TutHover";
            shadow.sortingLayerName = "TutHover";
            grassShadow.sortingLayerName = "TutHover";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetLayer_Move()
    {
        bg.sortingLayerName = "Move";
        grass.sortingLayerName = "Move";
        grass2.sortingLayerName = "Move";
        icon.sortingLayerName = "Move";
        shadow.sortingLayerName = "Move";
        grassShadow.sortingLayerName = "Move";

        border.sortingLayerName = "Move";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutHand";
            icon.sortingLayerName = "TutHand";
            grass.sortingLayerName = "TutHand";
            grass2.sortingLayerName = "TutHand";
            shadow.sortingLayerName = "TutHand";
            grassShadow.sortingLayerName = "TutHand";

            border.sortingLayerName = "TutHand";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetOrderLayer_Move(int indexSlot)
    {
        int sortingOrder = 10 * indexSlot;
        bg.sortingOrder = sortingOrder;
        icon.sortingOrder = sortingOrder;
        grass.sortingOrder = sortingOrder;
        grass2.sortingOrder = sortingOrder;
        shadow.sortingOrder = sortingOrder;
        grassShadow.sortingOrder = sortingOrder;

        border.sortingOrder = sortingOrder;
    }
    public override void SetShadow_Available(bool isShadowAvailable)
    {
        grassShadow.gameObject.SetActive(true);
        shadow.gameObject.SetActive(true);
        var color = shadow.color;
        color.a = isShadowAvailable ? 0.6f : 0;
        shadow.DOColor(color, 0.3f);
        grassShadow.DOColor(color, 0.3f);
        isShadow = isShadowAvailable;
    }
    protected override bool IsTouch()
    {
        return lockTouch <= 0;
    }

    protected override void BrakeObstacle()
    {
        lockTouch--;
        ChangeSprite();
        if (IsTouch())
        {
            grassShadow.gameObject.SetActive(false);
            shadow.gameObject.SetActive(false);
            obstacleType = Config.OBSTACLE_TYPE.NONE;
            grass.transform.DOScale(0f, 0.3f);
            grass2.transform.DOScale(0f, 0.3f);
        }
    }

    private void ChangeSprite()
    {
        if (lockTouch < 0)
            return;
        grassFx.Play();
        grass.gameObject.SetActive(false);
        grass2.gameObject.SetActive(false);


        grass.gameObject.SetActive(lockTouch == 2);
        grass2.gameObject.SetActive(lockTouch == 1);
    }
    
    public override void SetItemTileTut()
    {
        IsTileTutorial = true;
        const string sortingLayerName = "TutHand";
        
        bg.sortingLayerName = sortingLayerName;
        icon.sortingLayerName = sortingLayerName;
        shadow.sortingLayerName = sortingLayerName;
        border.sortingLayerName = sortingLayerName;
        grassShadow.sortingLayerName = sortingLayerName;
        
        grass.sortingLayerName = sortingLayerName;
        grass2.sortingLayerName = sortingLayerName;
    }
}