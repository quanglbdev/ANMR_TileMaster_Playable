using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class GlueTile : ItemTile
{
    [Header("Skeleton- glue")] [SerializeField]
    private SkeletonAnimation glue;

    [SerializeField] private AnimationReferenceAsset split;


    [Header("Skeleton- tile R")] [SerializeField]
    private SkeletonAnimation tileR;

    [SerializeField] private AnimationReferenceAsset tileSplitR;
    [SerializeField] private AnimationReferenceAsset outroSplitR;

    [Header("Skeleton- tile L")] [SerializeField]
    private SkeletonAnimation tileL;

    [SerializeField] private AnimationReferenceAsset tileSplitL;
    [SerializeField] private AnimationReferenceAsset outroSplitL;

    [SerializeField] private SpriteRenderer glueShadow;
    public GlueTile itemDual;

    [Header("SpriteRenderer icon")] [SerializeField]
    private SpriteRenderer iconL, iconR;

    [Header("Particle")] [SerializeField] private ParticleSystem fx;

    private bool _checkMatch;

    private Config.NEIGHBOR_TYPE _selfGlue;

    public Config.NEIGHBOR_TYPE SelfGlue
    {
        set
        {
            if (obstacleType == Config.OBSTACLE_TYPE.NONE) return;
            _selfGlue = value;
            glue.gameObject.SetActive(_selfGlue == Config.NEIGHBOR_TYPE.RIGHT);
            glueShadow.gameObject.SetActive(false);
        }
    }

    public override void InitTile(int _indexShow, int _indexOnMap, int _floorIndex, Vector2Int _posTile,
        ItemData _itemData)
    {
        indexOnMap = _indexOnMap;
        itemData = _itemData;
        floorIndex = _floorIndex;
        posTile = _posTile;
        HasBee = true;

        Pz = 800 - indexOnMap;

        var sprite = AssetManager.Instance.GetTile($"{(int)itemData.itemType}");
        bg.sprite = AssetManager.Instance.GetTile("tile");
        shadow.sprite = AssetManager.Instance.GetTile("tile");
        border.sprite = AssetManager.Instance.GetTile("tile-light");

        icon.sprite = sprite;
        iconL.sprite = sprite;
        iconR.sprite = sprite;

        gameObject.transform.localPosition =
            new Vector3(TILE_SIZE_X * posTile.x, TILE_SIZE_Y * posTile.y, 50 - floorIndex * 5);

        tileR.gameObject.SetActive(false);
        tileL.gameObject.SetActive(false);
        bg.gameObject.SetActive(true);
        icon.gameObject.SetActive(true);

        iconL.gameObject.SetActive(false);
        iconR.gameObject.SetActive(false);

        tileR.timeScale = 0;
        tileL.timeScale = 0;
        glue.timeScale = 0;

        SetOrderLayer_Floor();
        SetLayer_Floor();
        ShowTile();
    }

    protected override void SetOrderLayer_Floor()
    {
        if (obstacleType == Config.OBSTACLE_TYPE.NONE)
        {
            base.SetOrderLayer_Floor();
            return;
        }

        int sortingOrder = 400 - 20 * posTile.y + posTile.x;
        bg.sortingOrder = sortingOrder;
        icon.sortingOrder = sortingOrder;
        iconL.sortingOrder = sortingOrder;
        iconR.sortingOrder = sortingOrder;
        glue.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        tileR.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        tileL.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        shadow.sortingOrder = sortingOrder;
        glueShadow.sortingOrder = sortingOrder;

        border.sortingOrder = sortingOrder;
    }

    protected override void SetLayer_Floor()
    {
        if (obstacleType == Config.OBSTACLE_TYPE.NONE)
        {
            base.SetLayer_Floor();
            return;
        }

        isMouseDown = false;
        string sortingLayerName = "Floor" + (floorIndex + 1);
        if (IsTileTutorial)
        {
            sortingLayerName = "Tut";
        }

        bg.sortingLayerName = sortingLayerName;
        icon.sortingLayerName = sortingLayerName;
        iconL.sortingLayerName = sortingLayerName;
        iconR.sortingLayerName = sortingLayerName;
        shadow.sortingLayerName = sortingLayerName;
        glueShadow.sortingLayerName = sortingLayerName;
        glue.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        tileR.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        tileL.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;

        border.sortingLayerName = sortingLayerName;


        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetLayer_Hover()
    {
        if (obstacleType == Config.OBSTACLE_TYPE.NONE)
        {
            base.SetLayer_Hover();
            return;
        }

        bg.sortingLayerName = "Hover";
        icon.sortingLayerName = "Hover";
        iconL.sortingLayerName = "Hover";
        iconR.sortingLayerName = "Hover";
        glue.GetComponent<MeshRenderer>().sortingLayerName = "Hover";
        tileR.GetComponent<MeshRenderer>().sortingLayerName = "Hover";
        tileL.GetComponent<MeshRenderer>().sortingLayerName = "Hover";
        shadow.sortingLayerName = "Hover";
        glueShadow.sortingLayerName = "Hover";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutHand";
            icon.sortingLayerName = "TutHand";
            iconL.sortingLayerName = "TutHand";
            iconR.sortingLayerName = "TutHand";
            glue.GetComponent<MeshRenderer>().sortingLayerName = "TutHand";
            tileR.GetComponent<MeshRenderer>().sortingLayerName = "TutHand";
            tileL.GetComponent<MeshRenderer>().sortingLayerName = "TutHand";
            shadow.sortingLayerName = "TutHand";
            glueShadow.sortingLayerName = "TutHand";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetLayer_Move()
    {
        if (obstacleType == Config.OBSTACLE_TYPE.NONE)
        {
            base.SetLayer_Move();
            return;
        }

        bg.sortingLayerName = "Move";
        glue.GetComponent<MeshRenderer>().sortingLayerName = "TutMove";
        tileR.GetComponent<MeshRenderer>().sortingLayerName = "Move";
        tileL.GetComponent<MeshRenderer>().sortingLayerName = "Move";
        icon.sortingLayerName = "Move";
        iconL.sortingLayerName = "Move";
        iconR.sortingLayerName = "Move";
        shadow.sortingLayerName = "Move";
        glueShadow.sortingLayerName = "Move";

        border.sortingLayerName = "Move";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutHand";
            icon.sortingLayerName = "TutHand";
            iconL.sortingLayerName = "TutHand";
            iconR.sortingLayerName = "TutHand";
            glue.GetComponent<MeshRenderer>().sortingLayerName = "TutHand";
            tileR.GetComponent<MeshRenderer>().sortingLayerName = "TutHand";
            tileL.GetComponent<MeshRenderer>().sortingLayerName = "TutHand";
            shadow.sortingLayerName = "TutHand";
            glueShadow.sortingLayerName = "TutHand";

            border.sortingLayerName = "TutHand";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetOrderLayer_Move(int indexSlot)
    {
        if (obstacleType == Config.OBSTACLE_TYPE.NONE)
        {
            base.SetOrderLayer_Move(indexSlot);
            return;
        }

        int sortingOrder = 10 * indexSlot;
        bg.sortingOrder = sortingOrder;
        icon.sortingOrder = sortingOrder;
        iconL.sortingOrder = sortingOrder;
        iconR.sortingOrder = sortingOrder;
        tileR.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        tileL.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        glue.GetComponent<MeshRenderer>().sortingOrder = sortingOrder + 10;
        shadow.sortingOrder = sortingOrder;
        glueShadow.sortingOrder = sortingOrder;

        border.sortingOrder = sortingOrder;
    }

    public override void SetShadow_Available(bool isShadowAvailable)
    {
        if (obstacleType == Config.OBSTACLE_TYPE.NONE)
        {
            base.SetShadow_Available(isShadowAvailable);
            return;
        }

        isShadow = isShadowAvailable;

        shadow.gameObject.SetActive(true);
        glueShadow.gameObject.SetActive(_selfGlue == Config.NEIGHBOR_TYPE.RIGHT);

        var color = shadow.color;
        if (itemDual != null)
            color.a = isShadow || itemDual.isShadow ? 0.6f : 0f;
        else
            color.a = isShadow ? 0.6f : 0f;

        shadow.DOColor(color, 0.3f);
        glueShadow.DOColor(color, 0.3f);


        if (itemDual != null)
        {
            itemDual.shadow.gameObject.SetActive(true);
            itemDual.glueShadow.gameObject.SetActive(itemDual._selfGlue == Config.NEIGHBOR_TYPE.RIGHT);
            itemDual.shadow.DOColor(color, 0.3f);
            itemDual.glueShadow.DOColor(color, 0.3f);
        }
    }

    private void HideGlue()
    {
        if (glue == null) return;
        glue.gameObject.SetActive(false);
        if (tileR == null) return;
        tileR.gameObject.SetActive(false);
        if (tileL == null) return;
        tileL.gameObject.SetActive(false);
        if (iconL == null) return;
        iconL.gameObject.SetActive(false);
        if (iconR == null) return;
        iconR.gameObject.SetActive(false);

        if (bg == null) return;
        bg.gameObject.SetActive(true);
    }

    protected override bool CannotTouch()
    {
        if (obstacleType == Config.OBSTACLE_TYPE.NONE)
            return base.CannotTouch();

        if (itemDual != null)
            return (!IsTouch() || !HasBee || !itemDual.IsTouch() || !itemDual.HasBee || isShadow || itemDual.isShadow);

        return !IsTouch() || !HasBee;
    }

    public override void OnMouseUp()
    {
        if (obstacleType == Config.OBSTACLE_TYPE.NONE)
        {
            base.OnMouseUp();
            return;
        }

        if (isMouseDown)
        {
            isMouseDown = false;
            StartCoroutine(YieldSetTouchItemTile());
        }
    }

    private IEnumerator YieldSetTouchItemTile()
    {
        if (!GameLevelManager.Instance.IsItemTileMoveToSlot())
            yield break;
        if (_selfGlue == Config.NEIGHBOR_TYPE.LEFT)
        {
            _checkMatch = false;
            SetTouchItemTile();
            if (itemDual != null)
            {
                itemDual.SetTouchItemTile();
                itemDual._checkMatch = true;
            }
            else
            {
                Debug.Log("itemDual null ");
            }
        }
        else
        {
            if (itemDual != null)
            {
                itemDual._checkMatch = false;
                itemDual.SetTouchItemTile();
            }
            else
            {
                Debug.Log("itemDual null");
            }

            _checkMatch = true;
            SetTouchItemTile();
        }

        yield return null;
    }

    protected override async void SetTouchItemTile()
    {
        if (obstacleType == Config.OBSTACLE_TYPE.NONE)
        {
            base.SetTouchItemTile();
            return;
        }

        if (Config.gameState == Config.GAME_STATE.PLAYING)
        {
            if (ItemTileState == Config.ITEMTILE_STATE.FLOOR)
            {
                if (CannotTouch()) return;
                SoundManager.Instance.PlaySound_BlockClick();
                NeighborsFinder();

                ItemTileState = ItemTileState == Config.ITEMTILE_STATE.RETURN_FLOOR
                    ? Config.ITEMTILE_STATE.MOVE_FROM_RETURN_FLOOR
                    : Config.ITEMTILE_STATE.MOVE_TO_SLOT;

                SetLayer_Move();
                SetTouch_Available(false);
                SetShadow_Available(false);
                itemTileCheckCollision.gameObject.SetActive(false);
                await OnAnimSplit();

                obstacleType = Config.OBSTACLE_TYPE.NONE;
                GameLevelManager.Instance.AddItemSlot(this);
            }
        }
    }

    private async UniTask OnAnimSplit()
    {
        var timeScale = 5;
        glue.timeScale = timeScale;

        if (_selfGlue == Config.NEIGHBOR_TYPE.RIGHT)
        {
            tileR.timeScale = timeScale;
            tileR.gameObject.SetActive(true);
            iconR.gameObject.SetActive(true);
            tileR.AnimationState.SetAnimation(0, tileSplitR, false);

            itemDual.tileL.gameObject.SetActive(true);
            itemDual.tileL.AnimationState.SetAnimation(0, tileSplitL, false);
        }

        if (_selfGlue == Config.NEIGHBOR_TYPE.LEFT)
        {
            tileL.timeScale = timeScale;
            tileL.gameObject.SetActive(true);
            iconL.gameObject.SetActive(true);

            tileL.AnimationState.SetAnimation(0, tileSplitL, false);

            itemDual.tileR.gameObject.SetActive(true);
            itemDual.tileR.AnimationState.SetAnimation(0, tileSplitR, false);
        }

        bg.gameObject.SetActive(false);
        itemDual.bg.gameObject.SetActive(false);

        glue.AnimationState.SetAnimation(0, split, false);

        await UniTask.Delay((int)(split.Animation.Duration / glue.timeScale * 1000));
        if (glue == null) return;
        glue.gameObject.SetActive(false);
        var fxSpawn = GameLevelManager.Instance.poolObj.Spawn(fx.transform, glue.transform.position,
            Quaternion.identity);

        DOVirtual.DelayedCall(1.1f, () => { GameLevelManager.Instance.poolObj.Despawn(fxSpawn); });
        //OnAnimSplitOutro();
    }

    private void OnAnimSplitOutro()
    {
        if (_selfGlue == Config.NEIGHBOR_TYPE.RIGHT)
        {
            if (tileR == null) return;
            if (itemDual == null) return;
            if (tileR.AnimationState == null) return;
            if (itemDual.tileL.AnimationState == null) return;
            tileR.AnimationState.SetAnimation(0, outroSplitR, false);
            itemDual.tileL.AnimationState.SetAnimation(0, outroSplitL, false);

            DOVirtual.DelayedCall(0.2f, () =>
            {
                var fxR = GameLevelManager.Instance.poolObj.Spawn(fx.transform, tileR.transform.position,
                    Quaternion.identity);
                var fxL = GameLevelManager.Instance.poolObj.Spawn(fx.transform, itemDual.tileL.transform.position,
                    Quaternion.identity);

                DOVirtual.DelayedCall(1.1f, () =>
                {
                    GameLevelManager.Instance.poolObj.Despawn(fxR);
                    GameLevelManager.Instance.poolObj.Despawn(fxL);
                });
            });
        }

        if (_selfGlue == Config.NEIGHBOR_TYPE.LEFT)
        {
            if (tileL == null) return;
            if (itemDual == null) return;
            if (tileL.AnimationState == null) return;
            if (itemDual.tileR.AnimationState == null) return;
            tileL.AnimationState.SetAnimation(0, outroSplitL, false);
            itemDual.tileR.AnimationState.SetAnimation(0, outroSplitR, false);

            DOVirtual.DelayedCall(0.2f, () =>
            {
                var fxL = GameLevelManager.Instance.poolObj.Spawn(fx.transform, tileL.transform.position,
                    Quaternion.identity);
                var fxR = GameLevelManager.Instance.poolObj.Spawn(fx.transform, itemDual.tileR.transform.position,
                    Quaternion.identity);
                DOVirtual.DelayedCall(1f, () =>
                {
                    GameLevelManager.Instance.poolObj.Despawn(fxR);
                    GameLevelManager.Instance.poolObj.Despawn(fxL);
                });
            });
        }
    }

    public override void SetMoveToSlot(int indexSlot, bool match = true)
    {
        DOTween.Kill(TTransform);

        SetOrderLayer_Move(indexSlot);
        SetLayer_Move();

        moveToSlot_Sequence = DOTween.Sequence();


        //moveToSlot_Sequence.Insert(0f, OTransform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutQuad));
        moveToSlot_Sequence.Insert(0.05f, OTransform.DOScale(Vector3.one * 0.78f, 0.05f).SetEase(Ease.InQuad));

        moveToSlot_Sequence.InsertCallback(0.1f, () => { SoundManager.Instance.PlaySound_Wind(); });

        moveToSlot_Sequence.Insert(0f, TTransform.DOLocalMoveX(0, 0.3f).SetEase(Ease.OutCubic));
        moveToSlot_Sequence.Insert(0f, TTransform.DOLocalMoveY(0, 0.3f).SetEase(Ease.InCubic));

        moveToSlot_Sequence.OnComplete(() =>
        {
            ItemTileState = Config.ITEMTILE_STATE.SLOT;
            if (itemDual != null)
                itemDual.HideGlue();
            
            if (OTransform != null)
                OTransform.DOLocalMove(Vector2.zero, 0f);

            if (IsTileTutorial)
                IsTileTutorial = false;
            
            SetLayer_Move();
            SoundManager.Instance.PlaySound_BlockMoveFinish();
            GameLevelManager.Instance.SetMoveItemSlot_Finished(_checkMatch);
        });
    }

    public override void SetItemTileSuggest()
    {
        itemTileCheckCollision.gameObject.SetActive(false);
        SetTouch_Available(false);
        SetShadow_Available(false);
        DisableGlue();
        if (itemDual != null)
            itemDual.DisableGlue();
        SetLayer_Move();

        ItemTileState = Config.ITEMTILE_STATE.MOVE_TO_SLOT;
        GameLevelManager.Instance.AddItemSlot(this);
    }

    private void DisableGlue()
    {
        if (glue == null) return;
        if (glue.gameObject.activeSelf)
        {
            glue.transform.DOScale(0f, 0.1f).OnComplete(() => { glue.gameObject.SetActive(false); });
        }

        obstacleType = Config.OBSTACLE_TYPE.NONE;
    }

    public override void SetItemTileTut()
    {
        IsTileTutorial = true;
        const string sortingLayerName = "TutHand";

        bg.sortingLayerName = sortingLayerName;
        icon.sortingLayerName = sortingLayerName;
        iconL.sortingLayerName = sortingLayerName;
        iconR.sortingLayerName = sortingLayerName;
        glue.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        tileR.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        tileL.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        shadow.sortingLayerName = sortingLayerName;
        glueShadow.sortingLayerName = sortingLayerName;
        border.sortingLayerName = sortingLayerName;

        itemDual.IsTileTutorial = true;

        itemDual.bg.sortingLayerName = sortingLayerName;
        itemDual.icon.sortingLayerName = sortingLayerName;
        itemDual.iconL.sortingLayerName = sortingLayerName;
        itemDual.iconR.sortingLayerName = sortingLayerName;
        itemDual.glue.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        itemDual.tileR.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        itemDual.tileL.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        itemDual.shadow.sortingLayerName = sortingLayerName;
        itemDual.glueShadow.sortingLayerName = sortingLayerName;
        itemDual.border.sortingLayerName = sortingLayerName;
    }

    private bool _prevActiveGlue, _prevActiveGlueShadow;

    public void DisableGlueTile()
    {
        _prevActiveGlue = glue.gameObject.activeSelf;
        _prevActiveGlueShadow = glueShadow.gameObject.activeSelf;

        glue.gameObject.SetActive(false);
        glueShadow.gameObject.SetActive(false);
    }

    public void RevertGlueTile()
    {
        glue.gameObject.SetActive(_prevActiveGlue);
        glueShadow.gameObject.SetActive(_prevActiveGlueShadow);
    }
}