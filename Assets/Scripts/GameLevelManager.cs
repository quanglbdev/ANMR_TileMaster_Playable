using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using HellTap.PoolKit;
using TMPro;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance;

    [FormerlySerializedAs("_score")] [Header("Score")]
    public TextMeshProUGUI score;

    public StarGroup starGroup;
    [Title("CONFIG LEVEL")] public ConfigLevelGame configLevelGame;

    [Space(20)] [InfoBox("Các item hiện trên map")]
    public List<ItemData> listItemDataOnMaps = new();

    public List<ItemData> listItemDataTemp = new();

    [Space(20)] [InfoBox("Số lượng data type trong map này")]
    public int dataAmountOnMap = 0;

    [Space(20)]
    [InfoBox("Thời gian hoàn thành game")]
    [Space(20)]
    [InfoBox("Nếu bằng set = 0 => màn chơi không cần thời gian hoàn thành")]
    public int secondsRequired = 0;

    [Header("TileMaps")] public List<Tilemap> listTileMaps = new();
    [Space(10)] public List<Transform> listFloors = new();
    [Space(10)] public Transform floor;
    [Space(10)] [Header("ItemsTile")] public CustomDictionary<int, List<ItemTile>> itemsTileMap = new();

    [Header("Score")] public Transform scorePrefab;
    public Transform startPoint, endPoint;
    [Header("Pool")] public Pool poolObj;
    [Header("HandGuide")] [SerializeField] private GameObject handGuide, handGuide2;
    [Header("ExtraSlot")] [SerializeField] private GameObject extraSlotIcon;

    [Header("FX EXTRA SLOT")] public ParticleSystem extraSlotFX;
    public GameObject slotBG;
    private List<ItemTileData> listItemTileDatas = new();
    private List<ItemTile> _tilesCheck = new();
    private Vector3 _originalScaleOfSlotBG;

    private Vector3 _originalScaleOfFloor;

    private readonly List<Bee> _listBee = new();

    private void Awake()
    {
        _originalScaleOfSlotBG = slotBG.transform.localScale;
        slotBG.transform.localScale = Vector3.zero;
        _originalScaleOfFloor = floor.localScale;
        Instance = this;
    }

    public void InitSlotPosition(Vector3 pos)
    {
        slotBG.transform.parent.position = new Vector3(0f, pos.y, 0);
    }

    public void StartGame()
    {
        if (Config.CheckTutorial_Match3())
        {
            //slotBG.GetComponent<SpriteRenderer>().sortingLayerName = "TutUI";
        }

        Slot = 7;
        extraSlotIcon.SetActive(true);
        extraSlotIcon.transform.localScale = Vector3.one;
        AssetManager.Instance.SetTile(Config.currTheme);
        InitListMoveType();
        ReadDataTileMap();

        var maxScore = CalculateMaxPoints(_totalTilesInLevel);
        CalculateConfigLevel();
        starGroup.StartGame(maxScore, _levelDifficulty);
        RestartScore();
        StartCoroutine(ShowSlotBg());
        // GamePlayManager.instance.UpdateTileReturnStatus(CheckTileReturnAvailable());
        // GamePlayManager.instance.SetInteractableUndoButton(CheckUndoAvailable());
        listTileReturn_ItemTiles.Clear();
        _floorTileReturn = 1;
    }

    private static int CalculateMaxPoints(int totalTiles)
    {
        var n = totalTiles / 3;
        var resp = BASE_SCORE_MATCH
                   + (int)(BASE_SCORE_MATCH * 1.1f)
                   + (int)(BASE_SCORE_MATCH * 1.3f)
                   + (int)(BASE_SCORE_MATCH * 1.6f)
                   + (int)(BASE_SCORE_MATCH * 2f)
                   + (n - 5) * 600;
        return resp;
    }

    private void CalculateConfigLevel()
    {
        configLevelGame.listScrore_Stars.Clear();
        switch (_levelDifficulty)
        {
            case Config.LEVEL_DIFFICULTY.EASY:
                configLevelGame.listScrore_Stars.Add(15);
                configLevelGame.listScrore_Stars.Add(35);
                configLevelGame.listScrore_Stars.Add(55);
                break;
            case Config.LEVEL_DIFFICULTY.MEDIUM:
                configLevelGame.listScrore_Stars.Add(25);
                configLevelGame.listScrore_Stars.Add(45);
                configLevelGame.listScrore_Stars.Add(65);
                break;
            case Config.LEVEL_DIFFICULTY.HARD:
                configLevelGame.listScrore_Stars.Add(35);
                configLevelGame.listScrore_Stars.Add(55);
                configLevelGame.listScrore_Stars.Add(75);
                break;
        }

        starGroup.InitStarGroup(configLevelGame);
    }

    private Config.LEVEL_DIFFICULTY _levelDifficulty;

    private void InitListMoveType()
    {
        Config.listStartMoveType.Clear();
        var listTempStartMoveType = new List<Config.START_MOVE_TYPE>();
        listTempStartMoveType.Clear();
        listTempStartMoveType.Add(Config.START_MOVE_TYPE.TOP);
        listTempStartMoveType.Add(Config.START_MOVE_TYPE.BOTTOM);
        listTempStartMoveType.Add(Config.START_MOVE_TYPE.RIGHT);
        listTempStartMoveType.Add(Config.START_MOVE_TYPE.LEFT);


        var listTemp2StartMoveType = new List<Config.START_MOVE_TYPE>();
        listTemp2StartMoveType.Clear();
        for (var i = 0; i < 4; i++)
        {
            var moveType = listTempStartMoveType[Random.Range(0, listTempStartMoveType.Count)];
            listTemp2StartMoveType.Add(moveType);
            listTempStartMoveType.Remove(moveType);
        }

        for (var i = 0; i < listFloors.Count; i++)
        {
            Config.listStartMoveType.Add(listTemp2StartMoveType[i % 4]);
        }
    }

    private int _totalTilesInLevel;

    private void ReadDataTileMap()
    {
        listItemTileDatas.Clear();
        itemsTileMap.Clear();

        var levelDefinition = Resources.Load("Data/LevelData/Lv" + GamePlayManager.Instance.level) as LevelDefinition;
        if (levelDefinition == null)
        {
            Debug.LogError($"Data of level {GamePlayManager.Instance.level} IS NULL");
            return;
        }

        _levelDifficulty = levelDefinition.difficulty;
        secondsRequired = levelDefinition.secondsRequired;
        dataAmountOnMap = levelDefinition.dataAmountOnMap;
        var dataMap = levelDefinition.dataTileInFloor;
        if (listItemDataTemp.Count < 20)
            listItemDataTemp = new List<ItemData>(AssetManager.Instance.GetTileDataDefinition());

        var value = new List<ItemData>(listItemDataTemp);
        var selectedValues = SelectRandomValues(value, dataAmountOnMap == 0 ? 5 : dataAmountOnMap);
        listItemDataOnMaps = new List<ItemData>(selectedValues);
        foreach (var data in dataMap)
        {
            foreach (var tileData in data.data)
            {
                listItemTileDatas.Add(tileData);
            }

            itemsTileMap.Add(data.floor, new List<ItemTile>());
        }

        _totalTilesInLevel = listItemTileDatas.Count;
        GenerateItemData();
    }

    static List<T> SelectRandomValues<T>(List<T> values, int n)
    {
        if (n > values.Count)
        {
            throw new ArgumentException("n không thể lớn hơn số lượng giá trị có sẵn.");
        }

        var selectedValues = new List<T>();
        var random = new System.Random();

        while (selectedValues.Count < n)
        {
            var randomIndex = random.Next(values.Count);
            var selectedValue = values[randomIndex];
            selectedValues.Add(selectedValue);
            values.RemoveAt(randomIndex);
        }

        return selectedValues;
    }

    private void GenerateItemData()
    {
        var listTempItemDatas = new List<ItemData>();
        listTempItemDatas.Clear();
        if (listItemTileDatas.Count % 3 != 0)
        {
            return;
        }

        var listItemDataOnMapsShuffle = new List<ItemData>();
        var listItemDataOnMapsTemp = new List<ItemData>(listItemDataOnMaps);

        for (var i = 0; i < listItemDataOnMaps.Count; i++)
        {
            var itemData = listItemDataOnMapsTemp[Random.Range(0, listItemDataOnMapsTemp.Count)];

            listItemDataOnMapsShuffle.Add(itemData);
            listItemDataOnMapsTemp.Remove(itemData);
        }

        for (var i = 0; i < listItemTileDatas.Count / 3; i++)
        {
            var itemData = listItemDataOnMapsShuffle[i % listItemDataOnMapsShuffle.Count];

            //Add 3 phan tu
            listTempItemDatas.Add(itemData);
            listTempItemDatas.Add(itemData);
            listTempItemDatas.Add(itemData);
        }

        foreach (var dât in listItemTileDatas)
        {
            var itemData = listTempItemDatas[Random.Range(0, listTempItemDatas.Count)];
            dât.itemData = itemData;
            listTempItemDatas.Remove(itemData);
        }

        if (GamePlayManager.Instance.level == 1)
        {
            //Tut
            var indexItemData = Random.Range(0, listItemDataOnMaps.Count);

            var itemTileData1 = new ItemTileData(2, 100, new Vector2Int(1, 1));
            listItemTileDatas.Add(itemTileData1);
            itemTileData1.itemData = listItemDataOnMaps[indexItemData];

            var itemTileData2 = new ItemTileData(2, 101, new Vector2Int(0, 1));
            listItemTileDatas.Add(itemTileData2);
            itemTileData2.itemData = listItemDataOnMaps[indexItemData];


            var itemTileData3 = new ItemTileData(2, 102, new Vector2Int(-1, 1));
            listItemTileDatas.Add(itemTileData3);
            itemTileData3.itemData = listItemDataOnMaps[indexItemData];


            listItemTile_Tutorials.Clear();
        }

        FloorGenerateItemTileDatas();
    }

    private void FloorGenerateItemTileDatas()
    {
        GlueTile lastLeftGlueTile = null;
        _tilesCheck.Clear();
        _listBee.Clear();
        listItemTile_Tutorials.Clear();
        for (var i = 0; i < listItemTileDatas.Count; i++)
        {
            var itemTile = new ItemTile();
            switch (listItemTileDatas[i].obstacleType)
            {
                case Config.OBSTACLE_TYPE.NONE:
                    itemTile = Instantiate(AssetManager.Instance.itemTile, listFloors[listItemTileDatas[i].floorIndex]);
                    break;
                case Config.OBSTACLE_TYPE.CHAIN:
                    itemTile = Instantiate(AssetManager.Instance.chainTile,
                        listFloors[listItemTileDatas[i].floorIndex]);
                    break;
                case Config.OBSTACLE_TYPE.ICE:
                    itemTile = Instantiate(AssetManager.Instance.iceTile, listFloors[listItemTileDatas[i].floorIndex]);

                    break;
                case Config.OBSTACLE_TYPE.GRASS:
                    itemTile = Instantiate(AssetManager.Instance.grassTile,
                        listFloors[listItemTileDatas[i].floorIndex]);
                    break;
                case Config.OBSTACLE_TYPE.CLOCK:
                    break;
                case Config.OBSTACLE_TYPE.BEE:
                    itemTile = Instantiate(AssetManager.Instance.beeTile, listFloors[listItemTileDatas[i].floorIndex]);
                    var bee = ((BeeTile)itemTile)?.bee;
                    if (bee != null)
                    {
                        _listBee.Add(bee);
                    }

                    break;
                case Config.OBSTACLE_TYPE.BOMB:
                    itemTile = Instantiate(AssetManager.Instance.bombTile, listFloors[listItemTileDatas[i].floorIndex]);
                    break;
                case Config.OBSTACLE_TYPE.GLUE_RIGHT:
                    itemTile = Instantiate(AssetManager.Instance.glueTile, listFloors[listItemTileDatas[i].floorIndex]);
                    ((GlueTile)itemTile).obstacleType = Config.OBSTACLE_TYPE.GLUE_RIGHT;
                    ((GlueTile)itemTile).SelfGlue = Config.NEIGHBOR_TYPE.RIGHT;
                    ((GlueTile)itemTile).itemDual = lastLeftGlueTile;
                    if (lastLeftGlueTile != null) lastLeftGlueTile.itemDual = (GlueTile)itemTile;

                    break;
                case Config.OBSTACLE_TYPE.GLUE_LEFT:
                    itemTile = Instantiate(AssetManager.Instance.glueTile, listFloors[listItemTileDatas[i].floorIndex]);
                    ((GlueTile)itemTile).obstacleType = Config.OBSTACLE_TYPE.GLUE_LEFT;
                    ((GlueTile)itemTile).SelfGlue = Config.NEIGHBOR_TYPE.LEFT;
                    lastLeftGlueTile = ((GlueTile)itemTile);
                    break;
            }

            itemTile.InitTile(i, listItemTileDatas[i].indexOnMap, listItemTileDatas[i].floorIndex,
                listItemTileDatas[i].posTile, listItemTileDatas[i].itemData);
            itemTile.name = "" + i;
            _tilesCheck.Add(itemTile);
            StartCoroutine(PlaySound_MoveBoardStart(listItemTileDatas[i].floorIndex));

            if (Config.CheckTutorial_Match3())
            {
                if (i >= listItemTileDatas.Count - 3)
                {
                    listItemTile_Tutorials.Add(itemTile);
                    itemTile.SetItemTileTut();
                }
            }

            if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.GLUE))
            {
                if (i >= listItemTileDatas.Count - 1)
                {
                    listItemTile_Tutorials.Add(itemTile);
                }
            }

            if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.CHAIN))
            {
                // if (listItemTileDatas[i].floorIndex == 4)
                // {
                //     listItemTile_Tutorials.Add(itemTile);
                // }

                if (i is 42 or 43 or 41)
                {
                    listItemTile_Tutorials.Add(itemTile);
                }
            }

            if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.ICE))
            {
                if (listItemTileDatas[i].floorIndex == 3 &&
                    listItemTileDatas[i].obstacleType != Config.OBSTACLE_TYPE.NONE)
                {
                    listItemTile_Tutorials.Add(itemTile);
                }
            }

            if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.GRASS))
            {
                if (listItemTileDatas[i].floorIndex == 4)
                {
                    listItemTile_Tutorials.Add(itemTile);
                }
            }

            if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.BOMB))
            {
                if (i is 33 or 34)
                {
                    listItemTile_Tutorials.Add(itemTile);
                }
            }

            if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.BEE))
            {
                if (listItemTileDatas[i].floorIndex == 4)
                {
                    listItemTile_Tutorials.Add(itemTile);
                }
            }


            AddTileToMap(itemTile);
        }

        SetStartLevelGame();
    }

    public void FinishTutorial()
    {
        foreach (var tile in _tilesCheck)
        {
            if (tile.IsTileTutorial)
            {
                tile.IsTileTutorial = false;
            }
        }
    }

    private void SetStartLevelGame()
    {
        StartCoroutine(SetStartLevelGameYield());
        StartCoroutine(SetupTutTileReturnYield());
    }

    private IEnumerator SetStartLevelGameYield()
    {
        yield return new WaitForSeconds(listFloors.Count * 0.04f + 1.05f);
        GamePlayManager.Instance.SetStartPlayingGame();
    }

    private IEnumerator SetupTutTileReturnYield()
    {
        if (!Config.CheckTutorial_TileReturn()) yield break;
        yield return new WaitForSeconds(1f);
        var tiles = GetListTutorialTileReturn();
        foreach (var tile in tiles)
        {
            SetItemSlot(tile);
        }
    }

    public void RemoveTileInFloor()
    {
        ClearUndoSlot();
        slotBG.transform.DOScale(0, 0.3f);
        foreach (var floor1 in listFloors)
        {
            while (floor1.childCount > 0)
            {
                var child = floor1.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }

        foreach (var floor1 in listPointTileReturn)
        {
            while (floor1.childCount > 0)
            {
                var child = floor1.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }

        foreach (var floor2 in listPointTileReturn2)
        {
            while (floor2.childCount > 0)
            {
                var child = floor2.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }

        foreach (var floor3 in listPointTileReturn3)
        {
            while (floor3.childCount > 0)
            {
                var child = floor3.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }

        foreach (var floor4 in listPointTileReturn4)
        {
            while (floor4.childCount > 0)
            {
                var child = floor4.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }

        foreach (var floor5 in listPointTileReturn5)
        {
            while (floor5.childCount > 0)
            {
                var child = floor5.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }
    }


    private int _indexMoveBoardStart = -1;

    private IEnumerator PlaySound_MoveBoardStart(int floorIndex)
    {
        yield return new WaitForSeconds(0.2f * floorIndex + 0.18f);
        if (floorIndex > _indexMoveBoardStart)
        {
            _indexMoveBoardStart = floorIndex;
            SoundManager.Instance.PlaySound_ShowBoard();
        }
    }

    #region Bee

    private void BeeAction()
    {
        if (Config.currSelectLevel < Config.LEVEL_UNLOCK_BEE) return;
        foreach (var bee in _listBee)
        {
            bee.JumpToTile();
        }
    }

    #endregion

    #region TileInMap

    private void AddTileToMap(ItemTile tile)
    {
        itemsTileMap[tile.floorIndex].Add(tile);
    }

    private void RemoveTileToMap(ItemTile tile)
    {
        if (!itemsTileMap.ContainsKey(tile.floorIndex)) return;
        if (itemsTileMap[tile.floorIndex].Contains(tile))
            itemsTileMap[tile.floorIndex].Remove(tile);
    }

    public bool CheckTileCount()
    {
        var count = 0;
        foreach (var (_, value) in itemsTileMap)
        {
            count += value.Count;
            foreach (var tile in value)
            {
                if (tile.obstacleType != Config.OBSTACLE_TYPE.NONE)
                    count--;
            }
        }

        return count <= _listBee.Count;
    }

    public ItemTile GetRandomTileCanTouch()
    {
        var tiles = new List<ItemTile>();
        foreach (var (_, value) in itemsTileMap)
        {
            foreach (var tile in value)
            {
                if (!tile.isShadow && tile.HasBee && tile.obstacleType == Config.OBSTACLE_TYPE.NONE)
                    tiles.Add(tile);
            }
        }

        return tiles.Count == 0 ? null : tiles[Random.Range(0, tiles.Count)];
    }

    #endregion


    #region SLOT

    //Chua cac itemTile
    [Header("Slot")] public ItemTileSlot itemTileSlotPrefab;
    public List<ItemTileSlot> listItemSlots = new();
    public List<Transform> listPointSlots;
    public Transform slotParentTranform;
    private int _slot;

    public int GetItemSlotCount()
    {
        return listItemSlots.Count;
    }

    private int _glueCount;

    public void AddItemSlot(ItemTile itemTile, float delay = 0)
    {
        if (listItemSlots.Count < Slot || itemTile.GetComponent<GlueTile>() != null)
        {
            if (itemTile.ItemTileState == Config.ITEMTILE_STATE.MOVE_FROM_RETURN_FLOOR)
                listTileReturn_ItemTiles.Remove(itemTile);

            itemTile.CheckObstacle();
            RemoveTileToMap(itemTile);
            BeeAction();
            var indexNewSlot = FindIndexAddItemTileSlot(itemTile);
            var itemTileSlot = Instantiate(itemTileSlotPrefab, slotParentTranform);
            itemTileSlot.slotsPosition = listPointSlots;

            listItemSlots.Insert(indexNewSlot, itemTileSlot);
            listCheckUndo_ItemTileSlots.Add(itemTileSlot);

            var tileSlotTransform = itemTileSlot.transform;
            if (indexNewSlot > listPointSlots.Count - 1) return;
            tileSlotTransform.position = listPointSlots[indexNewSlot].position;
            itemTile.transform.parent = tileSlotTransform;
            itemTileSlot.SetItemTile(itemTile);
            var checkMatch = true;
            // if (itemTile.GetComponent<GlueTile>() != null && itemTile.GetComponent<GlueTile>().itemDual != null)
            // {
            //     _glueCount++;
            // }
            // Debug.Log("_glueCount " + _glueCount);
            // if (_glueCount % 2 == 1) checkMatch = false;
            // else
            // {
            //     _glueCount = 0;
            // }
            DOVirtual.DelayedCall(delay, () =>
            {
                if (itemTile.GetComponent<GlueTile>())
                    itemTile.GetComponent<GlueTile>().SetMoveToSlot(indexNewSlot);
                else itemTile.SetMoveToSlot(indexNewSlot);

                //Find indexAdd newItemSlot
                StartCoroutine(SetListItemSlot_ResetPosition_Now());
            });
        }

        if (Config.CheckTutorial_Undo())
        {
            TutorialManager.Instance.ShowTut_Undo();
        }
    }

    private void SetItemSlot(ItemTile itemTile)
    {
        if (listItemSlots.Count < Slot)
        {
            if (itemTile.ItemTileState == Config.ITEMTILE_STATE.MOVE_FROM_RETURN_FLOOR)
                listTileReturn_ItemTiles.Remove(itemTile);

            itemTile.CheckObstacle();
            RemoveTileToMap(itemTile);
            var indexNewSlot = FindIndexAddItemTileSlot(itemTile);
            var itemTileSlot = Instantiate(itemTileSlotPrefab, slotParentTranform);
            itemTileSlot.slotsPosition = listPointSlots;

            var tileSlotTransform = itemTileSlot.transform;
            tileSlotTransform.position = listPointSlots[indexNewSlot].position;
            itemTile.transform.parent = tileSlotTransform;
            itemTileSlot.SetItemTile(itemTile);
            itemTile.SetTileSlot(indexNewSlot);
            listCheckUndo_ItemTileSlots.Add(itemTileSlot);

            //Find indexAdd newItemSlot
            listItemSlots.Insert(indexNewSlot, itemTileSlot);
            StartCoroutine(SetListItemSlot_ResetPosition_Now());
        }
    }

    private List<ItemTile> GetListTutorialTileReturn()
    {
        var resp = new List<ItemTile>();
        var listTile = new List<ItemTile>();
        listTile.AddRange(itemsTileMap[0]);
        listTile.AddRange(itemsTileMap[1]);
        listTile.AddRange(itemsTileMap[2]);
        var temp = itemsTileMap[1][0];
        foreach (var tile in listTile.Where(tile =>
                     temp != null && temp.itemData != tile.itemData && tile.GetComponent<GlueTile>() == null))
        {
            resp.Add(tile);
            temp = tile;
            if (resp.Count == 3)
            {
                return resp;
            }
        }

        return null;
    }

    private int FindIndexAddItemTileSlot(ItemTile itemTile)
    {
        var indexSlot = listItemSlots.Count;
        for (var i = listItemSlots.Count - 1; i >= 0; i--)
        {
            if (listItemSlots[i].itemTile.itemData.itemType == itemTile.itemData.itemType)
            {
                return i + 1;
            }
        }

        return indexSlot;
    }


    public void SetMoveItemSlot_Finished(bool match = true)
    {
        if (!match) return;
        var listCheckMatch3Slots = FindMatch3_ItemTiles_Slots();
        foreach (var listCheck in listCheckMatch3Slots)
        {
            if (listCheck.Count == 3)
            {
                SoundManager.Instance.PlaySound_ComboMatch(starGroup.CurrentCombo);

                var isClear = listItemSlots.Count == 3;
                for (var i = 0; i < 3; i++)
                {
                    if (listCheck[i] != null)
                    {
                        listItemSlots.Remove(listCheck[i]);
                        //Undo
                        listCheckUndo_ItemTileSlots.Remove(listCheck[i]);
                        listCheck[i].SetItemSlot_Match3(i, isClear);
                        GamePlayManager.Instance.SetStatusUndoButton(listCheckUndo_ItemTileSlots.Count == 0);
                    }

                    if (i == 1)
                    {
                        UpdateScore(BASE_SCORE_MATCH);
                    }
                }

                StartCoroutine(SetListItemSlot_ResetPosition());
                StartCoroutine(CheckGameWin());
                SoundManager.Instance.PlaySound_FreeBlock();
            }
        }

        StartCoroutine(CheckGameOver_IEnumerator());
    }


    private IEnumerator CheckGameOver_IEnumerator()
    {
        if (!IsItemTileMoveToSlot()) SoundManager.Instance.PlaySound_NoMoreMove();
        yield return new WaitForSeconds(0.1f);
        if (CheckGameOver())
        {
            SetGameOver();
        }
    }

    private IEnumerator SetListItemSlot_ResetPosition()
    {
        //0.3 la thoi gian itemTile Efx Destroy
        yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < listItemSlots.Count; i++)
        {
            if (listItemSlots[i] != null)
                listItemSlots[i].ResetPosSlot(i);
        }

        //GamePlayManager.Instance.UpdateTileReturnStatus(!CheckTileReturnAvailable());
        GamePlayManager.Instance.SetStatusUndoButton(listCheckUndo_ItemTileSlots.Count == 0);
    }

    private IEnumerator SetListItemSlot_ResetPosition_Now()
    {
        TutorialManager.Instance.HideHandGuild();
        yield return new WaitForSeconds(0.01f);
        for (int i = 0; i < listItemSlots.Count; i++)
        {
            if (listItemSlots[i] == null) yield break;
            listItemSlots[i].ResetPosSlot(i);
        }

        yield return new WaitForSeconds(0.19f);
        GamePlayManager.Instance.UpdateTileReturnStatus(!CheckTileReturnAvailable());
        GamePlayManager.Instance.SetStatusUndoButton(listCheckUndo_ItemTileSlots.Count == 0);
    }

    //Tim bo 3 voi itemTile vua dc chuyen den slot
    private List<ItemTileSlot> FindMatch3_ItemTile_Slots(ItemTile itemTile)
    {
        var listCheckMatch3ItemTileSlot = new List<ItemTileSlot>();
        foreach (var slot in listItemSlots.Where(slot => slot.itemTile.itemData.itemType == itemTile.itemData.itemType))
        {
            listCheckMatch3ItemTileSlot.Add(slot);

            if (listCheckMatch3ItemTileSlot.Count == 3)
            {
                return listCheckMatch3ItemTileSlot;
            }
        }

        return listCheckMatch3ItemTileSlot;
    }

    private Dictionary<Config.ITEM_TYPE, List<ItemTileSlot>> _slotDataDict;

    private List<List<ItemTileSlot>> FindMatch3_ItemTiles_Slots()
    {
        var listCheckMatch3ItemTileSlot = new List<List<ItemTileSlot>>();
        var slotDataDict = new Dictionary<Config.ITEM_TYPE, List<ItemTileSlot>>();

        foreach (var slot in listItemSlots)
        {
            var slotData = slot.itemTile.itemData.itemType;
            if (!slotDataDict.ContainsKey(slotData))
            {
                slotDataDict[slotData] = new List<ItemTileSlot>();
            }

            slotDataDict[slotData].Add(slot);
        }

        foreach (var kvp in slotDataDict)
        {
            if (kvp.Value.Count >= 3)
            {
                listCheckMatch3ItemTileSlot.Add(kvp.Value);
            }
        }

        return listCheckMatch3ItemTileSlot;
    }

    #endregion

    private IEnumerator CheckGameWin()
    {
        var isGameWin = CheckGameFinished();
        if (isGameWin)
        {
            SoundManager.Instance.PlaySound_Clear();
        }

        yield return new WaitForSeconds(0.5f);
        if (isGameWin)
        {
            SetGameWin();
        }
    }

    private bool CheckGameFinished()
    {
        foreach (var t in listFloors)
        {
            if (t.transform.childCount > 0)
            {
                return false;
            }
        }

        foreach (var floor1 in listPointTileReturn)
        {
            while (floor1.childCount > 0)
            {
                return false;
            }
        }

        foreach (var floor2 in listPointTileReturn2)
        {
            while (floor2.childCount > 0)
            {
                return false;
            }
        }

        foreach (var floor3 in listPointTileReturn3)
        {
            while (floor3.childCount > 0)
            {
                return false;
            }
        }

        foreach (var floor4 in listPointTileReturn4)
        {
            while (floor4.childCount > 0)
            {
                return false;
            }
        }

        foreach (var floor5 in listPointTileReturn5)
        {
            while (floor5.childCount > 0)
            {
                return false;
            }
        }

        return true;
    }

    private void SetGameWin()
    {
        GamePlayManager.Instance.SetGameWin();
    }

    private bool CheckGameOver()
    {
        var listCheckMatch3Slots = FindMatch3_ItemTiles_Slots();
        var match = false;
        foreach (var listCheck in listCheckMatch3Slots)
        {
            if (listCheck.Count >= 3)
            {
                match = true;
                SoundManager.Instance.PlaySound_ComboMatch(starGroup.CurrentCombo);

                var isClear = listItemSlots.Count == 3;
                for (var i = 0; i < 3; i++)
                {
                    if (listCheck[i] != null)
                    {
                        listItemSlots.Remove(listCheck[i]);
                        //Undo
                        listCheckUndo_ItemTileSlots.Remove(listCheck[i]);
                        listCheck[i].SetItemSlot_Match3(i, isClear);
                        GamePlayManager.Instance.SetStatusUndoButton(listCheckUndo_ItemTileSlots.Count == 0);
                    }

                    if (i == 1)
                    {
                        UpdateScore(BASE_SCORE_MATCH);
                    }
                }

                StartCoroutine(SetListItemSlot_ResetPosition());
                StartCoroutine(CheckGameWin());
                SoundManager.Instance.PlaySound_FreeBlock();
            }
        }
        
        if (listItemSlots.Count < Slot)
        {
            return false;
        }

        return !match;
    }

    public bool IsItemTileMoveToSlot()
    {
        if (listItemSlots.Count >= Slot)
        {
            return false;
        }

        return true;
    }


    public void SetGameOver()
    {
        GamePlayManager.Instance.SetGameLose();
    }


    private IEnumerator ShowSlotBg()
    {
        //SoundManager.instance.PlaySound_ShowBoard();
        slotBG.transform.DOLocalMoveY(-5f, 0f);
        yield return new WaitForSeconds(0.5f);
        slotBG.transform.DOScale(_originalScaleOfSlotBG, 0.3f);
        slotBG.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutQuart);
    }

    #region Score

    private int CurrentScore { get; set; }

    public int Slot
    {
        get => _slot;
        set => _slot = value;
    }

    private const int BASE_SCORE_MATCH = 300;

    private void UpdateScore(int scoreAdditional)
    {
        var temp = CurrentScore;
        CurrentScore += Mathf.CeilToInt(scoreAdditional * starGroup.coefficientCombo);
        starGroup.Score = CurrentScore;
        starGroup.IncreaseCombo();

        DOTween.Kill(this.score.transform);
        var countAdd = CurrentScore / 100;
        if (countAdd > 5)
        {
            countAdd = 5;
        }

        for (var i = 0; i < countAdd; i++)
        {
            DOVirtual.DelayedCall(0.05f * i, () =>
            {
                score.text = $"{temp}";
                var str = temp;
                DOTween.To(() => str, x => str = x, CurrentScore, 1f).OnUpdate(
                    () => { score.text = $"{str}"; }
                );
            });
        }
    }

    private void RestartScore()
    {
        //score.transform.localScale = Vector3.one;
        //score.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 10, 2f).SetEase(Ease.InOutBack).SetRelative(true)
        //    .SetLoops(3, LoopType.Restart);
        CurrentScore = 0;
        starGroup.Score = CurrentScore;
        //score.text = $"Score: {CurrentScore}";
    }

    public void OnAnimScore()
    {
        var score = poolObj.Spawn(scorePrefab, startPoint.position, Quaternion.identity);
        var position = endPoint.position;
        score.DOMove(position, 0.3f).SetEase(Ease.OutBack).OnComplete(() => { poolObj.Despawn(score); });
    }

    #endregion


    #region UNDO

    [Header("UNDO")] public List<ItemTileSlot> listCheckUndo_ItemTileSlots = new();

    public bool CheckUndoAvailable()
    {
        return Config.currLevel >= Config.LEVEL_UNLOCK_UNDO;
    }

    public bool CheckMoveUndo()
    {
        return listCheckUndo_ItemTileSlots.Count == 0;
    }

    public bool SetUndo()
    {
        if (listCheckUndo_ItemTileSlots.Count > 0)
        {
            var itemTileSlotUndo = listCheckUndo_ItemTileSlots[^1];

            var glueTile = itemTileSlotUndo.itemTile.GetComponent<GlueTile>();
            if (glueTile != null)
            {
                var glueDual = glueTile.itemDual;
                if (glueDual == null) return false; //1 in 2 glue had match. do not undo

                var glueDualSlot = glueDual.GetComponentInParent<ItemTileSlot>();

                glueDualSlot.itemTile.transform.parent = listFloors[glueDualSlot.itemTile.floorIndex];
                glueDualSlot.itemTile.SetItemTile_Undo();
                ((GlueTile)glueDualSlot.itemTile).itemDual = null;
                glueTile.itemDual = null;
                AddTileToMap(glueDualSlot.itemTile);
                listCheckUndo_ItemTileSlots.Remove(glueDualSlot);
                listItemSlots.Remove(glueDualSlot);
                Destroy(glueDualSlot.gameObject);
                StartCoroutine(SetListItemSlot_ResetPosition_Now());

                return true;
            }

            itemTileSlotUndo.itemTile.transform.parent = listFloors[itemTileSlotUndo.itemTile.floorIndex];
            itemTileSlotUndo.itemTile.SetItemTile_Undo();
            AddTileToMap(itemTileSlotUndo.itemTile);
            listCheckUndo_ItemTileSlots.Remove(itemTileSlotUndo);
            listItemSlots.Remove(itemTileSlotUndo);
            Destroy(itemTileSlotUndo.gameObject);

            StartCoroutine(SetListItemSlot_ResetPosition_Now());

            return true;
        }

        return false;
    }

    private void SetUndoAll()
    {
        for (int i = listCheckUndo_ItemTileSlots.Count - 1; i >= 0; i--)
        {
            ItemTileSlot itemTileSlot_Undo = listCheckUndo_ItemTileSlots[listCheckUndo_ItemTileSlots.Count - 1];
            itemTileSlot_Undo.itemTile.transform.parent = listFloors[itemTileSlot_Undo.itemTile.floorIndex];
            itemTileSlot_Undo.itemTile.SetItemTile_Undo_Now();
            AddTileToMap(itemTileSlot_Undo.itemTile);
            listCheckUndo_ItemTileSlots.Remove(itemTileSlot_Undo);
            listItemSlots.Remove(itemTileSlot_Undo);
            Destroy(itemTileSlot_Undo.gameObject);
        }
    }

    private void ClearUndoSlot()
    {
        for (int i = listCheckUndo_ItemTileSlots.Count - 1; i >= 0; i--)
        {
            ItemTileSlot itemTileSlot_Undo = listCheckUndo_ItemTileSlots[listCheckUndo_ItemTileSlots.Count - 1];
            listCheckUndo_ItemTileSlots.Remove(itemTileSlot_Undo);
            listItemSlots.Remove(itemTileSlot_Undo);
            DestroyImmediate(itemTileSlot_Undo.gameObject);
        }
    }

    #endregion

    #region TILE_RETURN

    [Header("TILE_RETURN")] public List<Transform> listPointTileReturn;
    [Header("TILE_RETURN")] public List<Transform> listPointTileReturn2;
    [Header("TILE_RETURN")] public List<Transform> listPointTileReturn3;
    [Header("TILE_RETURN")] public List<Transform> listPointTileReturn4;
    [Header("TILE_RETURN")] public List<Transform> listPointTileReturn5;
    public List<ItemTile> listTileReturn_ItemTiles = new();

    public bool CheckTileReturnAvailable()
    {
        return listCheckUndo_ItemTileSlots.Count >= 1 && _floorTileReturn <= 5;
    }

    private int _floorTileReturn;

    public bool SetTileReturn()
    {
        if (Config.CheckTutorial_TileReturn() && Config.isShowTut_TileReturn)
        {
            TutorialManager.Instance.HideTut_TileReturn();
        }

        var parent = _floorTileReturn switch
        {
            1 => listPointTileReturn,
            2 => listPointTileReturn2,
            3 => listPointTileReturn3,
            4 => listPointTileReturn4,
            5 => listPointTileReturn5
        };


        if (!CheckTileReturnAvailable()) return false;
        for (var i = 0; i < 3; i++)
        {
            if (listCheckUndo_ItemTileSlots.Count == 0)
                continue;
            var itemTileSlotTileReturn = listCheckUndo_ItemTileSlots[^1];

            itemTileSlotTileReturn.itemTile.transform.parent = parent[i].transform;
            itemTileSlotTileReturn.itemTile.SetItemTile_TileReturn();
            AddTileToMap(itemTileSlotTileReturn.itemTile);
            itemTileSlotTileReturn.itemTile.floorIndex = _floorTileReturn;
            listTileReturn_ItemTiles.Add(itemTileSlotTileReturn.itemTile);
            listCheckUndo_ItemTileSlots.Remove(itemTileSlotTileReturn);
            listItemSlots.Remove(itemTileSlotTileReturn);
            Destroy(itemTileSlotTileReturn.gameObject);
            StartCoroutine(SetListItemSlot_ResetPosition_Now());
        }

        _floorTileReturn++;
        return true;
    }

    #endregion

    #region SHUFFLE

    [Header("SHUFFLE")] public List<ItemTile> listShuffle_ItemTiles = new List<ItemTile>();

    public void SetShuffle()
    {
        listShuffle_ItemTiles.Clear();

        for (int i = 0; i < listFloors.Count; i++)
        {
            foreach (Transform child in listFloors[i])
            {
                ItemTile itemTile = child.GetComponent<ItemTile>();
                if (itemTile.GetComponent<GlueTile>() != null) itemTile.GetComponent<GlueTile>().DisableGlueTile();
                listShuffle_ItemTiles.Add(itemTile);
            }
        }

        var listShuffleItemTilesTemp = new List<ItemTile>(listShuffle_ItemTiles);
        var listShuffleItemDatas = new List<ItemData>();
        for (var i = 0; i < listShuffle_ItemTiles.Count; i++)
        {
            var index = Random.Range(0, listShuffleItemTilesTemp.Count);
            listShuffleItemDatas.Add(listShuffleItemTilesTemp[index].itemData);
            listShuffleItemTilesTemp.RemoveAt(index);
        }

        for (var i = 0; i < listShuffle_ItemTiles.Count; i++)
        {
            listShuffle_ItemTiles[i].SetItemTile_Shuffle(listShuffleItemDatas[i]);
        }

        var moveShuffleSequence = DOTween.Sequence();
        moveShuffleSequence.Insert(0f, floor.DOScale(_originalScaleOfFloor * 1.3f, 0.3f).SetEase(Ease.OutQuad));
        moveShuffleSequence.Insert(0.3f, floor.transform.DOScale(_originalScaleOfFloor, 0.3f).SetEase(Ease.InQuad));
    }

    #endregion

    #region SUGGEST

    [Header("SUGGEST")] public List<ItemTile> listSuggest_ItemTiles = new();

    [ShowInInspector] public Dictionary<Config.ITEM_TYPE, List<ItemTile>> dicItemTiles = new();

    //Bộ dic này là cả sáng cả mờ
    private Dictionary<Config.ITEM_TYPE, List<ItemTile>> _dicItemTilesAll = new();

    public void SetSuggest()
    {
        listSuggest_ItemTiles.Clear();

        for (int i = 0; i < listFloors.Count; i++)
        {
            foreach (Transform child in listFloors[i])
            {
                ItemTile itemTile = child.GetComponent<ItemTile>();
                listSuggest_ItemTiles.Add(itemTile);
            }
        }

        for (var i = 0; i < listPointTileReturn.Count; i++)
        {
            foreach (Transform child in listPointTileReturn[i])
            {
                var itemTile = child.GetComponent<ItemTile>();
                listSuggest_ItemTiles.Add(itemTile);
            }
        }

        for (var i = 0; i < listPointTileReturn2.Count; i++)
        {
            foreach (Transform child in listPointTileReturn2[i])
            {
                var itemTile = child.GetComponent<ItemTile>();
                listSuggest_ItemTiles.Add(itemTile);
            }
        }

        for (var i = 0; i < listPointTileReturn3.Count; i++)
        {
            foreach (Transform child in listPointTileReturn3[i])
            {
                var itemTile = child.GetComponent<ItemTile>();
                listSuggest_ItemTiles.Add(itemTile);
            }
        }

        for (var i = 0; i < listPointTileReturn4.Count; i++)
        {
            foreach (Transform child in listPointTileReturn4[i])
            {
                var itemTile = child.GetComponent<ItemTile>();
                listSuggest_ItemTiles.Add(itemTile);
            }
        }

        for (var i = 0; i < listPointTileReturn5.Count; i++)
        {
            foreach (Transform child in listPointTileReturn5[i])
            {
                var itemTile = child.GetComponent<ItemTile>();
                listSuggest_ItemTiles.Add(itemTile);
            }
        }

        Split_ListSuggest_ItemTiles();

        List<ItemTile> listItemTileSuggests = AI_Suggest();

        if (listItemTileSuggests.Count > 0)
        {
            GamePlayManager.Instance.SetSuggestSuccess();
            StartCoroutine(SetSuggest_IEnumerator(listItemTileSuggests));
        }
    }


    public bool CheckSuggestAvailable()
    {
        listSuggest_ItemTiles.Clear();

        for (int i = 0; i < listFloors.Count; i++)
        {
            foreach (Transform child in listFloors[i])
            {
                var itemTile = child.GetComponent<ItemTile>();
                listSuggest_ItemTiles.Add(itemTile);
            }
        }

        for (var i = 0; i < listPointTileReturn.Count; i++)
        {
            foreach (Transform child in listPointTileReturn[i])
            {
                var itemTile = child.GetComponent<ItemTileSlot>();
                listSuggest_ItemTiles.Add(itemTile.itemTile);
            }
        }

        for (var i = 0; i < listPointTileReturn2.Count; i++)
        {
            foreach (Transform child in listPointTileReturn2[i])
            {
                var itemTile = child.GetComponent<ItemTileSlot>();
                listSuggest_ItemTiles.Add(itemTile.itemTile);
            }
        }

        for (var i = 0; i < listPointTileReturn3.Count; i++)
        {
            foreach (Transform child in listPointTileReturn3[i])
            {
                var itemTile = child.GetComponent<ItemTileSlot>();
                listSuggest_ItemTiles.Add(itemTile.itemTile);
            }
        }

        for (var i = 0; i < listPointTileReturn4.Count; i++)
        {
            foreach (Transform child in listPointTileReturn4[i])
            {
                var itemTile = child.GetComponent<ItemTileSlot>();
                listSuggest_ItemTiles.Add(itemTile.itemTile);
            }
        }

        for (var i = 0; i < listPointTileReturn5.Count; i++)
        {
            foreach (Transform child in listPointTileReturn5[i])
            {
                var itemTile = child.GetComponent<ItemTileSlot>();
                listSuggest_ItemTiles.Add(itemTile.itemTile);
            }
        }

        Split_ListSuggest_ItemTiles();

        List<ItemTile> listItemTileSuggests = AI_Suggest();

        if (listItemTileSuggests.Count > 0)
        {
            return true;
        }

        return false;
    }

    private IEnumerator SetSuggest_IEnumerator(List<ItemTile> listItemTileSuggests)
    {
        yield return new WaitForSeconds(0f);
        foreach (var tile in listItemTileSuggests)
        {
            if (tile.GetComponent<GlueTile>())
                tile.GetComponent<GlueTile>().SetItemTileSuggest();
            else
                tile.SetItemTileSuggest();
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void Split_ListSuggest_ItemTiles()
    {
        List<ItemTile> listTempSuggest_ItemTiles = new List<ItemTile>(listSuggest_ItemTiles);
        dicItemTiles.Clear();
        _dicItemTilesAll.Clear();
        for (int i = listTempSuggest_ItemTiles.Count - 1; i >= 0; i--)
        {
            if (listTempSuggest_ItemTiles[i].GetTouch_Available())
            {
                if (dicItemTiles.ContainsKey(listTempSuggest_ItemTiles[i].itemData.itemType))
                {
                    dicItemTiles[listTempSuggest_ItemTiles[i].itemData.itemType].Add(listTempSuggest_ItemTiles[i]);
                }
                else
                {
                    dicItemTiles.Add(listTempSuggest_ItemTiles[i].itemData.itemType,
                        new List<ItemTile>() { listTempSuggest_ItemTiles[i] });
                }
            }

            if (_dicItemTilesAll.ContainsKey(listTempSuggest_ItemTiles[i].itemData.itemType))
            {
                _dicItemTilesAll[listTempSuggest_ItemTiles[i].itemData.itemType].Add(listTempSuggest_ItemTiles[i]);
            }
            else
            {
                _dicItemTilesAll.Add(listTempSuggest_ItemTiles[i].itemData.itemType,
                    new List<ItemTile>() { listTempSuggest_ItemTiles[i] });
            }
        }
    }

    private List<ItemTile> listItemTile_ItemTileSlot_Checked = new List<ItemTile>();

    public List<ItemTile> AI_Suggest()
    {
        List<ItemTile> listItemTile_AI_Suggest = new List<ItemTile>();

        if (listItemSlots.Count > 0)
        {
            listItemTile_AI_Suggest.Clear();
            List<ItemData> listItemData_Checked = new List<ItemData>();
            for (int i = 0; i < listItemSlots.Count; i++)
            {
                ItemData itemData = listItemSlots[i].itemTile.itemData;
                if (listItemData_Checked.Contains(itemData))
                {
                    break;
                }

                listItemData_Checked.Add(itemData);

                int countItemData = CountItemTileSlot_Have_ItemData(itemData);

                if (countItemData == 1)
                {
                    if (listItemSlots.Count <= 5)
                    {
                        if (dicItemTiles.ContainsKey(itemData.itemType))
                        {
                            List<ItemTile> listItemTile_Have_ItemData = dicItemTiles[itemData.itemType];
                            if (listItemTile_Have_ItemData.Count >= 2)
                            {
                                listItemTile_AI_Suggest.Add(listItemTile_Have_ItemData[0]);
                                listItemTile_AI_Suggest.Add(listItemTile_Have_ItemData[1]);
                                return listItemTile_AI_Suggest;
                            }
                        }
                    }
                }
                else if (countItemData == 2)
                {
                    if (listItemSlots.Count <= 6)
                    {
                        if (dicItemTiles.ContainsKey(itemData.itemType))
                        {
                            List<ItemTile> listItemTile_Have_ItemData = dicItemTiles[itemData.itemType];
                            if (listItemTile_Have_ItemData.Count >= 1)
                            {
                                listItemTile_AI_Suggest.Add(listItemTile_Have_ItemData[0]);
                                return listItemTile_AI_Suggest;
                            }
                        }
                    }
                }
            }
        }

        //Neu ko tim dc bo nao thich hop voi cac item trong slot
        //->>> Tim 1 bo 3 moi de cho vao slot voi dieu kien so itemslot hien tai nho hon hoac bang 4
        if (listItemSlots.Count <= 4)
        {
            foreach (KeyValuePair<Config.ITEM_TYPE, List<ItemTile>> kvp in dicItemTiles)
            {
                if (kvp.Value.Count >= 3)
                {
                    listItemTile_AI_Suggest.Add(kvp.Value[0]);
                    listItemTile_AI_Suggest.Add(kvp.Value[1]);
                    listItemTile_AI_Suggest.Add(kvp.Value[2]);
                    return listItemTile_AI_Suggest;
                }
            }
        }
        else
        {
            //Neu so itemSlot lon hon 4

            //Neu số itemSlot >= 6
            if (listItemSlots.Count >= 6)
            {
                //Duyet cac bo 2 cua itemslot ->Tim 1 item mo cho xuong

                List<ItemData> listItemData_Checked = new List<ItemData>();
                for (int i = 0; i < listItemSlots.Count; i++)
                {
                    ItemData itemData = listItemSlots[i].itemTile.itemData;
                    if (listItemData_Checked.Contains(itemData))
                    {
                        break;
                    }

                    listItemData_Checked.Add(itemData);

                    int countItemData = CountItemTileSlot_Have_ItemData(itemData);

                    if (countItemData == 2)
                    {
                        //Tim 1 item o mo cho xuong item slot
                        List<ItemTile> listItemTile_Have_ItemData = _dicItemTilesAll[itemData.itemType];
                        if (listItemTile_Have_ItemData.Count >= 1)
                        {
                            listItemTile_AI_Suggest.Add(
                                listItemTile_Have_ItemData[listItemTile_Have_ItemData.Count - 1]);
                            return listItemTile_AI_Suggest;
                        }
                    }
                }

                //Nếu đang có 6 itemSlot mà ko tìm đc bộ 2 nào thì ko suggest đc
                return listItemTile_AI_Suggest;
            }

            //Kiem tra listItemSlot ==5 ko? Neu co tim 1 bo 2 item cả mờ và ko sáng cho xuống
            for (int i = 0; i < listItemSlots.Count; i++)
            {
                //Ko tìm dc bộ 2 nào thì chắc chắn 5 item này là 5 cái khác nhau
                ItemData itemData = listItemSlots[i].itemTile.itemData;
                List<ItemTile> listItemTile_Have_ItemData = _dicItemTilesAll[itemData.itemType];
                if (listItemTile_Have_ItemData.Count >= 2)
                {
                    listItemTile_AI_Suggest.Add(listItemTile_Have_ItemData[listItemTile_Have_ItemData.Count - 1]);
                    listItemTile_AI_Suggest.Add(listItemTile_Have_ItemData[listItemTile_Have_ItemData.Count - 2]);
                    return listItemTile_AI_Suggest;
                }
            }

            return listItemTile_AI_Suggest;
        }


        //Nếu vẫn ko tìm đc thì tìm 3 bộ cả sáng cả mờ cho xuống 
        //Với điều kiện là listItemSlot <=4
        if (listItemSlots.Count <= 4)
        {
            foreach (KeyValuePair<Config.ITEM_TYPE, List<ItemTile>> kvp in _dicItemTilesAll)
            {
                if (kvp.Value.Count >= 3)
                {
                    listItemTile_AI_Suggest.Add(kvp.Value[0]);
                    listItemTile_AI_Suggest.Add(kvp.Value[1]);
                    listItemTile_AI_Suggest.Add(kvp.Value[2]);
                    return listItemTile_AI_Suggest;
                }
            }
        }

        return listItemTile_AI_Suggest;
    }

    public int CountItemTileSlot_Have_ItemData(ItemData itemData)
    {
        int countItemSlot_Have_ItemData = 0;
        for (int i = 0; i < listItemSlots.Count; i++)
        {
            if (listItemSlots[i].itemTile.itemData == itemData)
            {
                countItemSlot_Have_ItemData++;
            }
        }

        return countItemSlot_Have_ItemData;
    }


    private ItemTile Find_1_ItemTile(ItemTile itemTile)
    {
        return Find_ItemTile(itemTile);
    }

    private List<ItemTile> Find_2_ItemTile(ItemTile itemTile)
    {
        List<ItemTile> listTempSuggest_ItemTiles = new List<ItemTile>(listSuggest_ItemTiles);
        List<ItemTile> listResult_ItemTiles = new List<ItemTile>();
        for (int i = listTempSuggest_ItemTiles.Count - 1; i >= 0; i--)
        {
            if (listTempSuggest_ItemTiles[i].GetTouch_Available())
            {
                if (listTempSuggest_ItemTiles[i].itemData.itemType == itemTile.itemData.itemType)
                {
                    listResult_ItemTiles.Add(itemTile);

                    if (listResult_ItemTiles.Count == 2)
                    {
                        return listResult_ItemTiles;
                    }
                }
            }
        }

        return null;
    }


    //Tim 1 item tile co itemData giong itemTile hien tai
    private ItemTile Find_ItemTile(ItemTile itemTile)
    {
        for (int i = listSuggest_ItemTiles.Count - 1; i >= 0; i--)
        {
            if (listSuggest_ItemTiles[i].GetTouch_Available())
            {
                if (listSuggest_ItemTiles[i].itemData.itemType == itemTile.itemData.itemType)
                {
                    return itemTile;
                }
            }
        }

        return null;
    }

    #endregion

    #region AddSlotButton

    public void RecruitSlot()
    {
        if (extraSlotIcon == null)
            return;
        Slot = 8;
        extraSlotFX.Play();
        extraSlotIcon.transform.DOScale(0, 0.5f);
    }

    #endregion

    #region REVIVE

    public void Revive()
    {
        StartCoroutine(Revive_IEnumerator());
    }

    public void AddTime(int time)
    {
    }

    public IEnumerator Revive_IEnumerator()
    {
        yield return new WaitForSeconds(0.1f);
        SetUndoAll();
        yield return new WaitForSeconds(0.1f);
        SetShuffle();
    }

    #endregion

    #region TUT

    public List<ItemTile> listItemTile_Tutorials = new();

    public void ShowTutClickTile_Match3_HandGuild()
    {
        var spriteRenderer = listItemTile_Tutorials[0].bg;
        var sprite = spriteRenderer.sprite;
        var deltaSize = sprite.rect.size;
        TutorialManager.Instance.SetDeltaSizeUnMask(deltaSize * floor.localScale.x, .2f, sprite);

        var posX = listItemTile_Tutorials[0].transform.position.x;
        var posY = listItemTile_Tutorials[0].transform.position.y;
        TutorialManager.Instance.SetPositionHandGuild_AndMask(new Vector3(posX, posY, 0));
    }

    public void ShowTutClickTileGlue_HandGuild()
    {
        SetItemTilesTut();

        var posX = listItemTile_Tutorials[0].transform.position.x;
        var posY = listItemTile_Tutorials[0].transform.position.y;
        TutorialManager.Instance.SetPositionHandGuild(new Vector3(posX, posY, 0));
    }

    public void SetItemTilesTut()
    {
        foreach (var tile in listItemTile_Tutorials)
        {
            tile.SetItemTileTut();
        }
    }

    public void ShowTutClickTileChain_HandGuild()
    {
        SetItemTilesTut();
        var tile = listItemTile_Tutorials.Find(x => x.obstacleType == Config.OBSTACLE_TYPE.NONE);

        var spriteRenderer = tile.bg;
        var sprite = spriteRenderer.sprite;
        var deltaSize = sprite.rect.size;
        TutorialManager.Instance.SetDeltaSizeUnMask(deltaSize * floor.localScale.x, .2f, sprite);

        var posX = tile.transform.position.x;
        var posY = tile.transform.position.y;
        TutorialManager.Instance.SetPositionHandGuild_AndMask(new Vector3(posX, posY, 0));
    }

    public void SetNextTutClickTile(ItemTile itemTile)
    {
        if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.CHAIN))
            listItemTile_Tutorials.Clear();
        else
        {
            listItemTile_Tutorials.Remove(itemTile);
        }


        if (listItemTile_Tutorials.Count > 0)
        {
            if (Config.CheckTutorial_Match3())
                ShowTutClickTile_Match3_HandGuild();
        }
        else
        {
            if (Config.CheckTutorial_Match3())
                TutorialManager.Instance.HideTut_ClickTile();

            if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.GLUE))
                TutorialManager.Instance.HideTut_ClickTileGlue();

            if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.CHAIN))
                TutorialManager.Instance.HideTut_ClickTileChain();

            slotBG.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        }
    }

    #endregion

    [Button]
    public void Count()
    {
        var tileOnMap = 0;
        foreach (var map in listTileMaps)
        {
            for (var y = map.cellBounds.yMin; y < map.cellBounds.yMax; y++)
            {
                for (var x = map.cellBounds.xMin; x < map.cellBounds.xMax; x++)
                {
                    if (map.HasTile(new Vector3Int(x, y, 0)))
                    {
                        tileOnMap++;
                    }
                }
            }
        }

        Debug.LogError("tileOnMap: " + tileOnMap);
    }
}