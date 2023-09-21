using System;
using System.Collections;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BombTile : ItemTile
{
    public TextMeshProUGUI count;
    public Canvas canvas;
    [Header("SkeletonAnimation - Boom")] public SkeletonAnimation bomb;
    public AnimationReferenceAsset bombActive, bombIdle, bombLastStep, bombCountDown;

    [Header("SkeletonAnimation - BrakeTile")]
    public SkeletonAnimation breakTileSkeleton;

    public AnimationReferenceAsset tileBreak;
    [SerializeField] private ParticleSystem bombFx;

    private int _lockTouch;
    private bool _isSlot = false;

    public int LockTouch
    {
        get { return _lockTouch; }
        set
        {
            _lockTouch = value;
            count.text = $"{_lockTouch}";
        }
    }

    private void Start()
    {
        bomb.AnimationState.AddAnimation(0, bombIdle, true, 0);
        breakTileSkeleton.gameObject.SetActive(false);
        bg.gameObject.SetActive(true);
    }

    public override void InitTile(int _indexShow, int _indexOnMap, int _floorIndex, Vector2Int _posTile,
        ItemData _itemData)
    {
        LockTouch = Random.Range(4, 7);
        obstacleType = Config.OBSTACLE_TYPE.BOMB;
        indexOnMap = _indexOnMap;
        itemData = _itemData;
        floorIndex = _floorIndex;
        posTile = _posTile;
        HasBee = true;

        icon.sprite = AssetManager.Instance.GetTile($"{(int)itemData.itemType}");
        bg.sprite = AssetManager.Instance.GetTile("tile");
        shadow.sprite = AssetManager.Instance.GetTile("tile");
        border.sprite = AssetManager.Instance.GetTile("tile-light");

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
        bomb.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        canvas.sortingOrder = sortingOrder;
        shadow.sortingOrder = sortingOrder;

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
        bomb.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        canvas.sortingLayerName = sortingLayerName;

        border.sortingLayerName = sortingLayerName;

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetLayer_Hover()
    {
        bg.sortingLayerName = "Hover";
        icon.sortingLayerName = "Hover";
        bomb.GetComponent<MeshRenderer>().sortingLayerName = "Hover";
        breakTileSkeleton.GetComponent<MeshRenderer>().sortingLayerName = "Hover";
        canvas.sortingLayerName = "Hover";
        shadow.sortingLayerName = "Hover";

        border.sortingLayerName = "Move";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutHover";
            icon.sortingLayerName = "TutHover";
            bomb.GetComponent<MeshRenderer>().sortingLayerName = "TutHover";
            breakTileSkeleton.GetComponent<MeshRenderer>().sortingLayerName = "TutHover";
            canvas.sortingLayerName = "TutHover";
            shadow.sortingLayerName = "TutHover";

            border.sortingLayerName = "TutHover";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetLayer_Move()
    {
        bg.sortingLayerName = "Move";
        bomb.GetComponent<MeshRenderer>().sortingLayerName = "Move";
        breakTileSkeleton.GetComponent<MeshRenderer>().sortingLayerName = "Move";
        canvas.sortingLayerName = "Move";
        icon.sortingLayerName = "Move";
        shadow.sortingLayerName = "Move";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutHand";
            icon.sortingLayerName = "TutHand";
            bomb.GetComponent<MeshRenderer>().sortingLayerName = "TutHand";
            breakTileSkeleton.GetComponent<MeshRenderer>().sortingLayerName = "TutHand";
            canvas.sortingLayerName = "TutHand";
            shadow.sortingLayerName = "TutHand";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetTouchItemTile()
    {
        if (Config.gameState == Config.GAME_STATE.PLAYING)
        {
            if (ItemTileState == Config.ITEMTILE_STATE.FLOOR)
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

                bomb.transform.DOScale(0f, 0.3f);
                obstacleType = Config.OBSTACLE_TYPE.NONE;
                canvas.enabled = false;
                _isSlot = true;
                GameLevelManager.Instance.AddItemSlot(this);
            }
        }
    }

    protected override void SetOrderLayer_Move(int indexSlot)
    {
        var sortingOrder = 10 * indexSlot;
        bg.sortingOrder = sortingOrder;
        icon.sortingOrder = sortingOrder;
        bomb.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        breakTileSkeleton.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        canvas.sortingOrder = sortingOrder;
        shadow.sortingOrder = sortingOrder;

        border.sortingOrder = sortingOrder;
    }

    protected override void BrakeObstacle()
    {
        if (isShadow || ItemTileState != Config.ITEMTILE_STATE.FLOOR || _isSlot) return;

        LockTouch--;
        if (LockTouch == 0)
        {
            Config.gameState = Config.GAME_STATE.BOOM;
            obstacleType = Config.OBSTACLE_TYPE.NONE;
            SetOverGame();
            return;
        }

        if (LockTouch == 1)
        {
            bomb.AnimationState.SetAnimation(0, bombCountDown, false);
            bomb.AnimationState.AddAnimation(0, bombLastStep, true, 0);
            return;
        }

        bomb.AnimationState.SetAnimation(0, bombCountDown, false);
        bomb.AnimationState.AddAnimation(0, bombIdle, true, 0);
    }

    private void SetOverGame()
    {
        StartCoroutine(YieldLose());
    }

    private IEnumerator YieldLose()
    {
        bomb.AnimationState.SetAnimation(0, bombActive, false);
        yield return new WaitForSeconds(bombActive.Animation.Duration);
        bombFx.Play();
        bomb.transform.DOScale(0f, 0.1f);
        breakTileSkeleton.gameObject.SetActive(true);
        breakTileSkeleton.GetComponent<MeshRenderer>().sortingLayerName = "Move";
        bg.gameObject.SetActive(false);
        icon.transform.DOScale(0f, 0.3f);

        breakTileSkeleton.AnimationState.SetAnimation(0, tileBreak, false);

        yield return new WaitForSeconds(0.5f);
        GameLevelManager.Instance.SetGameOver();
    }

    public override void SetItemTileTut()
    {
        IsTileTutorial = true;
        const string sortingLayerName = "TutHand";

        bomb.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        bg.sortingLayerName = sortingLayerName;
        icon.sortingLayerName = sortingLayerName;
        shadow.sortingLayerName = sortingLayerName;
        border.sortingLayerName = sortingLayerName;
        canvas.sortingLayerName = sortingLayerName;
    }
}