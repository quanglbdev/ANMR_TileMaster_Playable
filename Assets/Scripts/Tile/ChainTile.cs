using System.Collections;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class ChainTile : ItemTile
{
    [Header("SkeletonAnimation")]
    public SkeletonAnimation chain;
    public AnimationReferenceAsset chainIdle, chainIntro, chainOutro;
    public int lockTouch = 1;
    [SerializeField] private ParticleSystem vineFx;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        chain.AnimationState.SetAnimation(0, chainIntro, false);
        yield return new WaitForSeconds(chainIntro.Animation.Duration);
        chain.AnimationState.SetAnimation(0, chainIdle, true);
    }

    public override void InitTile(int _indexShow, int _indexOnMap, int _floorIndex, Vector2Int _posTile,
        ItemData _itemData)
    {
        obstacleType = Config.OBSTACLE_TYPE.CHAIN;
        indexOnMap = _indexOnMap;
        //indexShowTile = _indexShow;
        itemData = _itemData;
        floorIndex = _floorIndex;
        posTile = _posTile;
        HasBee = true;

        Pz = 800 - indexOnMap;
        
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
        chain.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        bg.sortingOrder = sortingOrder;
        icon.sortingOrder = sortingOrder;
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
        chain.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetLayer_Hover()
    {
        bg.sortingLayerName = "Hover";
        icon.sortingLayerName = "Hover";
        chain.GetComponent<MeshRenderer>().sortingLayerName = "Hover";
        shadow.sortingLayerName = "Hover";

        border.sortingLayerName = "Move";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutHand";
            icon.sortingLayerName = "TutHand";
            chain.GetComponent<MeshRenderer>().sortingLayerName = "TutHand";
            shadow.sortingLayerName = "TutHand";

            border.sortingLayerName = "TutHand";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetLayer_Move()
    {
        bg.sortingLayerName = "Move";
        chain.GetComponent<MeshRenderer>().sortingLayerName = "Move";
        icon.sortingLayerName = "Move";
        shadow.sortingLayerName = "Move";
        if (IsTileTutorial)
        {
            bg.sortingLayerName = "TutMove";
            icon.sortingLayerName = "TutMove";
            chain.GetComponent<MeshRenderer>().sortingLayerName = "TutMove";
            shadow.sortingLayerName = "TutMove";
        }

        itemTileCheckCollision.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    protected override void SetOrderLayer_Move(int indexSlot)
    {
        var sortingOrder = 10 * (indexSlot + 1);
        bg.sortingOrder = sortingOrder;
        icon.sortingOrder = sortingOrder;
        chain.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        shadow.sortingOrder = sortingOrder;

        border.sortingOrder = sortingOrder;
    }

    protected override bool IsTouch()
    {
        return lockTouch <= 0;
    }

    protected override void BrakeObstacle()
    {
        lockTouch--;
        if (!IsTouch())
        {
            if (chain.gameObject.activeSelf)
            {
                AnimChain();
            }
            return;
        }

        AnimChain();
    }

    private void AnimChain()
    {
        if(obstacleType == Config.OBSTACLE_TYPE.NONE) return;
        vineFx.Play();
        obstacleType = Config.OBSTACLE_TYPE.NONE;
        chain.AnimationState.SetAnimation(0, chainOutro, false);
        DOVirtual.DelayedCall(chainOutro.Animation.Duration, () =>
        {
            chain.gameObject.SetActive(false);
        });
    }
    
    public override void SetItemTileTut()
    {
        IsTileTutorial = true;
        const string sortingLayerName = "TutHand";
        
        chain.GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
        bg.sortingLayerName = sortingLayerName;
        icon.sortingLayerName = sortingLayerName;
        shadow.sortingLayerName = sortingLayerName;
        border.sortingLayerName = sortingLayerName;
    }
}