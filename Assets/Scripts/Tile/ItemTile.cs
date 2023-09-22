using System.Collections;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class ItemTile : MonoBehaviour
{
    protected const float TILE_SIZE_X = 1.2f;
    protected const float TILE_SIZE_Y = 1.1f;

    public SpriteRenderer border;
    public SpriteRenderer bg;
    public SpriteRenderer icon;
    public SpriteRenderer shadow;
    public GameObject objGroup;

    public ItemTileCheckCollision itemTileCheckCollision;

    public int indexOnMap;

    [HideInInspector] public int floorIndex;
    public Vector2Int posTile;
    [HideInInspector] public ItemData itemData;
    protected float Pz;

    private Collider2D _touchCollider2D;
    private Config.ITEMTILE_STATE _itemTileState = Config.ITEMTILE_STATE.START;
    public Config.OBSTACLE_TYPE obstacleType = Config.OBSTACLE_TYPE.NONE;
    public CustomDictionary<Config.NEIGHBOR_TYPE, ItemTile> neighbors = new();
    public bool isShadow;
    public bool HasBee { get; set; }

    protected Transform TTransform
    {
        get
        {
            if (_tTransform == null) _tTransform = transform;
            return _tTransform;
        }
    }

    protected Transform OTransform
    {
        get
        {
            if (objGroup == null) return null;
            if (_oTransform == null) _oTransform = objGroup.transform;
            return _oTransform;
        }
    }

    public Config.ITEMTILE_STATE ItemTileState
    {
        get => _itemTileState;
        set => _itemTileState = value;
    }

    public bool IsTileTutorial
    {
        get { return _isTileTutorial; }
        set
        {
            _isTileTutorial = value;
            if (value == false)
            {
                SetOrderLayer_Floor();
                SetLayer_Floor();
            }
        }
    }

    private Transform _tTransform;
    private Transform _oTransform;

    private void Awake()
    {
        _touchCollider2D = GetComponent<Collider2D>();
        _touchCollider2D.enabled = true;
        ItemTileState = Config.ITEMTILE_STATE.START;
        itemTileCheckCollision.gameObject.SetActive(false);
    }

    private void Update()
    {
#if UNITY_EDITOR

#else
        if (Input.touchCount > 0) {
            isCheckTouchCount = true;
        }

        if (Input.touchCount == 0 && isCheckTouchCount) {
            isCheckTouchCount = false;
            // if (bg.sortingLayerName.Equals("Hover"))
            // {
            //     SetLayer_Floor();
            //     OTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear);
            // }
        }
#endif
    }

    public bool isCheckTouchCount = false;

    public virtual void InitTile(int indexShow, int _indexOnMap, int _floorIndex, Vector2Int _posTile,
        ItemData _itemData)
    {
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
        TTransform.localPosition =
            new Vector3(TILE_SIZE_X * posTile.x, TILE_SIZE_Y * posTile.y, 50 - floorIndex * 5);
        SetOrderLayer_Floor();
        SetLayer_Floor();
        ShowTile();
    }


    protected virtual void ShowTile()
    {
        ItemTileState = Config.ITEMTILE_STATE.START_TO_FLOOR;
        if (Config.listStartMoveType[floorIndex] == Config.START_MOVE_TYPE.BOTTOM)
        {
            TTransform.localPosition =
                new Vector3(TILE_SIZE_X * posTile.x, TILE_SIZE_Y * posTile.y - 20f, Pz);
        }
        else if (Config.listStartMoveType[floorIndex] == Config.START_MOVE_TYPE.TOP)
        {
            TTransform.localPosition =
                new Vector3(TILE_SIZE_X * posTile.x, TILE_SIZE_Y * posTile.y + 20f, Pz);
        }
        else if (Config.listStartMoveType[floorIndex] == Config.START_MOVE_TYPE.LEFT)
        {
            TTransform.localPosition =
                new Vector3(TILE_SIZE_X * posTile.x - 12f, TILE_SIZE_Y * posTile.y, Pz);
        }
        else if (Config.listStartMoveType[floorIndex] == Config.START_MOVE_TYPE.RIGHT)
        {
            TTransform.localPosition =
                new Vector3(TILE_SIZE_X * posTile.x + 12f, TILE_SIZE_Y * posTile.y, Pz);
        }

        TTransform.DOLocalMove(new Vector3(TILE_SIZE_X * posTile.x, TILE_SIZE_Y * posTile.y, Pz), 1f)
            .SetEase(Ease.InBack).SetDelay(0.04f * floorIndex).OnComplete(() =>
            {
                ItemTileState = Config.ITEMTILE_STATE.FLOOR;
                itemTileCheckCollision.gameObject.SetActive(true);
            });
    }


    public void SetTouch_Available(bool isTouchAvailable)
    {
        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
        _touchCollider2D.enabled = isTouchAvailable;
    }

    public void SetTouch_Enable()
    {
        StartCoroutine(SetTouch_Available_IEnumerator());
    }

    private IEnumerator SetTouch_Available_IEnumerator()
    {
        yield return new WaitForSeconds(0.1f);
        SetTouch_Available(true);
        SetShadow_Available(false);
    }


    public virtual void SetShadow_Available(bool isShadowAvailable)
    {
        shadow.gameObject.SetActive(true);
        var color = shadow.color;
        color.a = isShadowAvailable ? 0.6f : 0;
        shadow.DOColor(color, 0.3f);
        isShadow = isShadowAvailable;
    }

    public bool GetTouch_Available()
    {
        return _touchCollider2D.enabled;
    }


    public void OnMouseUpAsButton()
    {
        //SetTouchItemTile();
    }

    protected bool isMouseDown = false;

    public void OnMouseDown()
    {
        isMouseDown = true;
    }

    public virtual void OnMouseUp()
    {
        if (isMouseDown)
        {
            isMouseDown = false;
            SetTouchItemTile();
        }

        // if (bg.sortingLayerName.Equals("Hover"))
        // {
        //     OTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear);
        //     SetLayer_Floor();
        // }

        // if (IsTileTutorial)
        // {
        //     if (bg.sortingLayerName.Equals("TutHover"))
        //     {
        //         OTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear);
        //         SetLayer_Floor();
        //     }
        // }
    }


    protected virtual void SetTouchItemTile()
    {
        if (Config.gameState == Config.GAME_STATE.PLAYING)
        {
            if (ItemTileState == Config.ITEMTILE_STATE.FLOOR || ItemTileState == Config.ITEMTILE_STATE.RETURN_FLOOR)
            {
                if (CannotTouch()) return;

                if (!GameLevelManager.Instance.IsItemTileMoveToSlot())
                {
                    return;
                }

                SoundManager.Instance.PlaySound_BlockClick();
                itemTileCheckCollision.gameObject.SetActive(false);

                SetLayer_Move();
                SetTouch_Available(false);
                SetShadow_Available(false);
                NeighborsFinder();

                ItemTileState = ItemTileState == Config.ITEMTILE_STATE.RETURN_FLOOR
                    ? Config.ITEMTILE_STATE.MOVE_FROM_RETURN_FLOOR
                    : Config.ITEMTILE_STATE.MOVE_TO_SLOT;

                GameLevelManager.Instance.AddItemSlot(this);
            }
        }
    }

    protected virtual bool CannotTouch()
    {
        return (!IsTouch() || !HasBee);
    }

    protected virtual void SetLayer_Move()
    {
        bg.sortingLayerName = "Move";
        icon.sortingLayerName = "Move";
        shadow.sortingLayerName = "Move";
        border.sortingLayerName = "Move";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutHand";
            icon.sortingLayerName = "TutHand";
            shadow.sortingLayerName = "TutHand";
            border.sortingLayerName = "TutHand";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected virtual void SetLayer_Hover()
    {
        bg.sortingLayerName = "Hover";
        icon.sortingLayerName = "Hover";
        shadow.sortingLayerName = "Hover";
        border.sortingLayerName = "Hover";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutHover";
            icon.sortingLayerName = "TutHover";
            shadow.sortingLayerName = "TutHover";
            border.sortingLayerName = "TutHover";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected virtual void SetLayer_Floor()
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

        border.sortingLayerName = sortingLayerName;

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }


    protected virtual void SetOrderLayer_Move(int indexSlot)
    {
        var sortingOrder = 10 * (indexSlot + 1);
        if (IsTileTutorial)
        {
            sortingOrder = 9999;
        }

        bg.sortingOrder = sortingOrder;
        icon.sortingOrder = sortingOrder;
        shadow.sortingOrder = sortingOrder;

        border.sortingOrder = sortingOrder;
    }


    protected virtual void SetOrderLayer_Floor()
    {
        var sortingOrder = 400 - 20 * posTile.y + posTile.x;
        bg.sortingOrder = sortingOrder;
        icon.sortingOrder = sortingOrder;
        shadow.sortingOrder = sortingOrder;
        border.sortingOrder = sortingOrder;
    }

    #region MOVE

    protected Sequence moveToSlot_Sequence;

    public virtual void SetMoveToSlot(int indexSlot, bool match = true)
    {
        DOTween.Kill(TTransform);

        SetOrderLayer_Move(indexSlot);
        SetLayer_Move();

        moveToSlot_Sequence = DOTween.Sequence();

        // moveToSlot_Sequence.Insert(0f, OTransform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutQuad));
        moveToSlot_Sequence.Insert(0.1f, OTransform.DOScale(Vector3.one * 0.78f, 0.2f).SetEase(Ease.InQuad));

        moveToSlot_Sequence.InsertCallback(0.1f, () => { SoundManager.Instance.PlaySound_Wind(); });

        moveToSlot_Sequence.Insert(0f, TTransform.DOLocalMoveX(0, 0.4f).SetEase(Ease.OutCubic));
        moveToSlot_Sequence.Insert(0f, TTransform.DOLocalMoveY(0, 0.4f).SetEase(Ease.InCubic));

        moveToSlot_Sequence.OnComplete(() =>
        {
            OTransform.DOLocalMove(Vector2.zero, 0f);
            ItemTileState = Config.ITEMTILE_STATE.SLOT;
            IsTileTutorial = false;
            SetLayer_Move();

            SoundManager.Instance.PlaySound_BlockMoveFinish();
            GameLevelManager.Instance.SetMoveItemSlot_Finished();
        });
    }

    public void SetTileSlot(int indexSlot)
    {
        DOTween.Kill(TTransform);

        SetOrderLayer_Move(indexSlot);
        SetLayer_Move();
        TTransform.localPosition = Vector3.zero;
        TTransform.localScale = Vector3.one * 0.9f;

        ItemTileState = Config.ITEMTILE_STATE.SLOT;
        GameLevelManager.Instance.SetMoveItemSlot_Finished();
    }

    public void ResetPosSlot(int indexSlot)
    {
        SetOrderLayer_Move(indexSlot);
    }

    #endregion

    #region SLOT

    Sequence match_Sequence;

    public void SetItemTile_Match3(System.Action itemTileSlotMatch3CallBack, int index, bool isClear)
    {
        match_Sequence = DOTween.Sequence();
        var lightFade = isClear ? 0.8f : 0.9f;
        switch (index)
        {
            case 0 when isClear:
                match_Sequence.Insert(0f, OTransform.DOLocalMoveY(2f, 0.2f).SetEase(Ease.OutQuad));
                match_Sequence.Insert(0f,
                    OTransform.DOLocalRotate(new Vector3(0f, 0f, 360f), 0.2f, RotateMode.FastBeyond360));
                match_Sequence.Insert(0.2f, OTransform.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.OutQuad));
                match_Sequence.Insert(0.2f, OTransform.DOLocalMoveY(0f, 0.2f).SetEase(Ease.InOutQuart));
                break;
            case 1 when isClear:
                match_Sequence.Insert(0f, OTransform.DOLocalMoveX(1f, 0.2f).SetEase(Ease.OutQuad));
                match_Sequence.Insert(0.2f, OTransform.DOLocalMoveX(0f, 0.2f).SetEase(Ease.OutQuad));
                break;
            case 2 when isClear:
                match_Sequence.Insert(0f, OTransform.DOLocalMoveX(2f, 0.2f).SetEase(Ease.OutQuad));
                match_Sequence.Insert(0.2f, OTransform.DOLocalMoveX(0f, 0.2f).SetEase(Ease.OutQuad));
                break;
        }

        match_Sequence.Insert(0.0f, border.DOFade(lightFade, 0.1f));

        match_Sequence.Insert(0.0f,
            OTransform.DOScale(Vector3.one * 1.1f, isClear ? 0.2f : 0.1f).SetEase(Ease.InQuad));
        match_Sequence.Insert(isClear ? 0.4f : 0.2f, OTransform.DOScale(Vector3.zero, 0f).SetEase(Ease.InQuad));

        match_Sequence.OnComplete(() =>
        {
            if (OTransform.localPosition != Vector3.zero)
            {
                DOTween.Kill(OTransform);
                OTransform.DOScale(Vector3.zero, 0f).SetEase(Ease.InQuad).OnComplete(
                    () =>
                    {
                        itemTileSlotMatch3CallBack.Invoke();
                        GamePlayManager.Instance.SpawnParticle(TTransform.position);
                    }
                );
                return;
            }

            itemTileSlotMatch3CallBack.Invoke();
            GamePlayManager.Instance.SpawnParticle(TTransform.position);
        });
    }

    #endregion

    #region TILE_RETURN

    Sequence tileReturn_Sequence;

    public void SetItemTile_TileReturn()
    {
        DOTween.Kill(TTransform);

        SetOrderLayer_Move(0);
        SetLayer_Move();

        tileReturn_Sequence = DOTween.Sequence();
        tileReturn_Sequence.Insert(0f, OTransform.DOScale(Vector3.one * 1.1f, 0.05f).SetEase(Ease.OutQuad));
        tileReturn_Sequence.Insert(0f,
            OTransform.DOLocalRotate(new Vector3(0f, 0f, Random.Range(10f, 15f)), 0.05f));
        tileReturn_Sequence.Insert(0.05f, OTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InQuad));
        tileReturn_Sequence.Insert(0.05f, TTransform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.InQuad));
        tileReturn_Sequence.Insert(0.05f, OTransform.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.InQuad));
        tileReturn_Sequence.Insert(0f,
            TTransform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutQuad));
        tileReturn_Sequence.OnComplete(() =>
        {
            ItemTileState = Config.ITEMTILE_STATE.RETURN_FLOOR;
            var localPosition = TTransform.localPosition;
            localPosition =
                new Vector3(localPosition.x, localPosition.y, 50 - floorIndex * 5);
            TTransform.localPosition = localPosition;
            SetItemTile_Undo_Finished();
        });
    }

    #endregion

    #region UNDO

    Sequence moveUndo_Sequence;

    public void SetItemTile_Undo()
    {
        DOTween.Kill(TTransform);

        SetOrderLayer_Move(0);
        SetLayer_Move();

        moveUndo_Sequence = DOTween.Sequence();
        moveUndo_Sequence.Insert(0f, OTransform.DOScale(Vector3.one * 1.1f, 0.05f).SetEase(Ease.OutQuad));
        moveUndo_Sequence.Insert(0f,
            OTransform.DOLocalRotate(new Vector3(0f, 0f, Random.Range(10f, 15f)), 0.05f));
        moveUndo_Sequence.Insert(0.05f, OTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InQuad));
        moveUndo_Sequence.Insert(0.05f, OTransform.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.InQuad));
        moveUndo_Sequence.Insert(0f,
            TTransform.DOLocalMove(new Vector3(TILE_SIZE_X * posTile.x, TILE_SIZE_Y * posTile.y, Pz), 0.2f)
                .SetEase(Ease.OutQuad));
        moveUndo_Sequence.OnComplete(() =>
        {
            ItemTileState = Config.ITEMTILE_STATE.FLOOR;
            SetItemTile_Undo_Finished();
        });
    }

    public void SetItemTile_Undo_Now()
    {
        TTransform.localPosition = new Vector3(TILE_SIZE_X * posTile.x, TILE_SIZE_Y * posTile.y, Pz);
        OTransform.localScale = Vector3.one;
        ItemTileState = Config.ITEMTILE_STATE.FLOOR;
        SetItemTile_Undo_Finished();
    }

    private void SetItemTile_Undo_Finished()
    {
        SetTouch_Available(true);
        SetOrderLayer_Floor();
        SetLayer_Floor();
        itemTileCheckCollision.gameObject.SetActive(true);
        itemTileCheckCollision.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    #endregion


    #region SHUFFLE

    Sequence moveShuffle_Sequence;

    public void SetItemTile_Shuffle(ItemData _itemData)
    {
        itemData = _itemData;
        moveShuffle_Sequence = DOTween.Sequence();
        moveShuffle_Sequence.InsertCallback(.3f, SetItemTile_Shuffle_ChangeItemData);

        moveShuffle_Sequence.Insert(0f,
            OTransform.DOMove(Vector3.zero, 0.3f)
                .SetEase(Ease.OutCubic));

        moveShuffle_Sequence.Insert(0.3f, OTransform.DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.InCubic));
        moveShuffle_Sequence.OnComplete(() =>
        {
            SetLayer_Floor();
            itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);

            if (GetComponent<GlueTile>() != null) GetComponent<GlueTile>().RevertGlueTile();
        });
    }

    protected void SetItemTile_Shuffle_ChangeItemData()
    {
        icon.sprite = Resources.Load<Sprite>("Sprite/" + Config.currTheme.ToString() + "/" + (int)itemData.itemType);
    }

    #endregion

    #region SUGGEST

    public virtual void SetItemTileSuggest()
    {
        itemTileCheckCollision.gameObject.SetActive(false);
        SetLayer_Move();
        SetTouch_Available(false);
        SetShadow_Available(false);
        ItemTileState = Config.ITEMTILE_STATE.MOVE_TO_SLOT;

        GameLevelManager.Instance.AddItemSlot(this);
    }

    #endregion


    #region TUT

    protected bool _isTileTutorial = false;

    public virtual void SetItemTileTut()
    {
        IsTileTutorial = true;
        const string sortingLayerName = "TutHand";
        bg.sortingLayerName = sortingLayerName;
        icon.sortingLayerName = sortingLayerName;
        shadow.sortingLayerName = sortingLayerName;
    }

    #endregion

    protected virtual void NeighborsFinder()
    {
        neighbors.Clear();
        if (ItemTileState != Config.ITEMTILE_STATE.FLOOR)
            return;
        foreach (var itemTile in GameLevelManager.Instance.itemsTileMap[floorIndex])
        {
            if (itemTile.posTile == this.posTile + Vector2.right)
            {
                neighbors.Add(Config.NEIGHBOR_TYPE.RIGHT, itemTile);
            }

            if (itemTile.posTile == this.posTile + Vector2.left)
            {
                neighbors.Add(Config.NEIGHBOR_TYPE.LEFT, itemTile);
            }

            if (itemTile.posTile == this.posTile + Vector2.up)
            {
                neighbors.Add(Config.NEIGHBOR_TYPE.TOP, itemTile);
            }

            if (itemTile.posTile == this.posTile + Vector2.down)
            {
                neighbors.Add(Config.NEIGHBOR_TYPE.BOTTOM, itemTile);
            }
        }
    }

    protected virtual bool IsTouch()
    {
        return true;
    }

    protected virtual void BrakeObstacle()
    {
    }

    public void CheckObstacle()
    {
        foreach (var neighbor in neighbors.Where(neighbor => neighbor.Value != null))
        {
            CheckNeighborsObstacleByType(neighbor.Value);
        }

        CheckShadowObstacleByType();
    }

    private void CheckNeighborsObstacleByType(ItemTile neighbor)
    {
        switch (neighbor.obstacleType)
        {
            case Config.OBSTACLE_TYPE.CHAIN:
                (neighbor as ChainTile)?.BrakeObstacle();
                break;
            case Config.OBSTACLE_TYPE.GRASS:
                (neighbor as GrassTile)?.BrakeObstacle();
                break;
            case Config.OBSTACLE_TYPE.NONE:
            case Config.OBSTACLE_TYPE.BOMB:
            case Config.OBSTACLE_TYPE.ICE:
            case Config.OBSTACLE_TYPE.CLOCK:
            default:
                break;
        }
    }

    private void CheckShadowObstacleByType()
    {
        foreach (var (_, value) in GameLevelManager.Instance.itemsTileMap)
        {
            foreach (var itemTile in value)
            {
                if (itemTile == null) continue;
                if (itemTile.obstacleType == Config.OBSTACLE_TYPE.ICE)
                {
                    (itemTile as IceTile)?.BrakeObstacle();
                }

                if (itemTile.obstacleType == Config.OBSTACLE_TYPE.BOMB)
                {
                    (itemTile as BombTile)?.BrakeObstacle();
                }
            }
        }
    }
}