using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using HellTap.PoolKit;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager Instance;
    public Canvas canvas;
    [Header("Select Level")] public int level = 1;

    public bool isTest = false;

    [Header("Button PAUSE")] public BBUIButton btnPause;
    [Header("Button HIDE UI")] public BBUIButton btnHideUI;
    [Header("Button HIDE UI")] public GameObject topGroup;
    public BBUIButton btnOutGame;
    [Header("Button UNDO")] public BBUIButton btnUndo;
    public TextMeshProUGUI txtUndoCount;
    public TextMeshProUGUI txtUndoAdd;
    public GameObject objUndoBG;

    [Header("Button SUGGEST")] public BBUIButton btnSuggest;
    public TextMeshProUGUI txtSuggestCount;
    public TextMeshProUGUI txtSuggestAdd;
    public GameObject objSuggestBG;

    [Header("Button SHUFFLE")] public BBUIButton btnShuffle;
    public TextMeshProUGUI txtShuffleCount;
    public TextMeshProUGUI txtShuffleAdd;
    public GameObject objShuffleBG;

    [FormerlySerializedAs("btnUndoAll")] [Header("Button TILE RETURN")]
    public BBUIButton btnTileReturn;

    public TextMeshProUGUI txtUndoAllCount;
    public TextMeshProUGUI txtUndoAllAdd;
    public GameObject objUndoAllBG;

    [Header("Button EXTRA SLOT")] public BBUIButton btnExtraSlot;
    public TextMeshProUGUI txtExtraSlotCount;
    public TextMeshProUGUI txtExtraSlotAdd;
    public GameObject objExtraSlotBG;

    [Header("Text Level")] public TextMeshProUGUI txtLevel;

    [Header("BG Footer")] public GameObject bgFooter;

    [Header("Booster Group")] public GameObject boosterGroup;

    [Header("Star Group")] public StarGroup starGroup;

    [Header("ComboProcess")] public GameObject comboProcess;

    [Header("Pool")] [SerializeField] private Pool poolObj;
    [Header("HandGuide")] [SerializeField] private GameObject handGuide, handGuide2;

    private int _timePlayed;

    #region ButtonItemLockManager

    private ButtonItemLockManager _buttonUndoManager;
    private ButtonItemLockManager _buttonShuffleManager;
    private ButtonItemLockManager _buttonSuggestManager;
    private ButtonItemLockManager _buttonTileReturnManager;
    private ButtonItemLockManager _buttonExtraSlotManager;

    public ButtonItemLockManager UndoButtonManager
    {
        get
        {
            if (_buttonUndoManager == null) _buttonUndoManager = btnUndo.GetComponent<ButtonItemLockManager>();
            return _buttonUndoManager;
        }
    }

    public ButtonItemLockManager ShuffleButtonManager
    {
        get
        {
            if (_buttonShuffleManager == null) _buttonShuffleManager = btnShuffle.GetComponent<ButtonItemLockManager>();
            return _buttonShuffleManager;
        }
    }

    public ButtonItemLockManager SuggestButtonManager
    {
        get
        {
            if (_buttonSuggestManager == null) _buttonSuggestManager = btnSuggest.GetComponent<ButtonItemLockManager>();
            return _buttonSuggestManager;
        }
    }

    public ButtonItemLockManager TileReturnButtonManager
    {
        get
        {
            if (_buttonTileReturnManager == null)
                _buttonTileReturnManager = btnTileReturn.GetComponent<ButtonItemLockManager>();
            return _buttonTileReturnManager;
        }
    }

    public ButtonItemLockManager ExtraSlotButtonManager
    {
        get
        {
            if (_buttonExtraSlotManager == null)
                _buttonExtraSlotManager = btnExtraSlot.GetComponent<ButtonItemLockManager>();
            return _buttonExtraSlotManager;
        }
    }

    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        HideElementUI();
    }

    public void StartGame()
    {
        levelGame.gameObject.SetActive(true);
        levelGame.RemoveTileInFloor();
        canvas.enabled = true;
        _isRevive = false;

        btnPause.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnUndo.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnTileReturn.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnExtraSlot.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnSuggest.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnShuffle.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnOutGame.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnHideUI.OnPointerClickCallBack_Start.RemoveAllListeners();

        btnPause.OnPointerClickCallBack_Start.AddListener(TouchPause);
        btnHideUI.OnPointerClickCallBack_Start.AddListener(TouchHideUI);
        btnOutGame.OnPointerClickCallBack_Start.AddListener(OpenOutGamePopup);

        btnUndo.OnPointerClickCallBack_Start.AddListener(TouchUndo);
        btnSuggest.OnPointerClickCallBack_Start.AddListener(TouchSuggest);
        btnShuffle.OnPointerClickCallBack_Start.AddListener(TouchShuffle);
        btnTileReturn.OnPointerClickCallBack_Start.AddListener(TouchTileReturn);
        btnExtraSlot.OnPointerClickCallBack_Start.AddListener(TouchExtraSlot);

        if (!isTest)
        {
            if (Config.isSelectLevel)
            {
                Config.isSelectLevel = false;
                level = Config.currSelectLevel;
            }
            else
            {
                level = PlayerPrefs.GetInt(Config.CURR_LEVEL);
            }
        }

        if (level > Config.MAX_LEVEL)
        {
            level = Config.MAX_LEVEL;
        }

        if (level == 0) level = 1;
        if (level == 2)
        {
            Config.IsShowTutUndo = false;
        }

        txtLevel.text = $"Level: {level}";
        Config.currSelectLevel = level;
        Config.gameState = Config.GAME_STATE.START;
        InitViews();
        LoadLevelGame();
        SetUpdate_CountItem();
        InitViews_ShowView();
        FirebaseManager.Instance.LogLevelStart(Config.currLevel);
        _timePlayed = 0;
        StartCoroutine(YieldUpdateTime());
    }
    
    private IEnumerator YieldUpdateTime()
    {
        while (true)
        {
            _timePlayed++;
            if(Config.gameState == Config.GAME_STATE.WIN) break;
            yield return new WaitForSecondsRealtime(1f);
        }
    }
    private void Update()
    {
        if (Config.gameState != Config.GAME_STATE.PLAYING || !canvas.enabled) return;
        if (level <= Config.LEVEL_CAN_AI_SUGGEST || _isShowAutoSuggestShuffle) return;
        if (level <= Config.LEVEL_UNLOCK_GLUE || _isShowAutoSuggestShuffle) return;
        if (level <= Config.LEVEL_UNLOCK_CHAIN || _isShowAutoSuggestShuffle) return;
        if (level <= Config.LEVEL_UNLOCK_ICE || _isShowAutoSuggestShuffle) return;
        if (level <= Config.LEVEL_UNLOCK_GLASS || _isShowAutoSuggestShuffle) return;
        if (level <= Config.LEVEL_UNLOCK_CLOCK || _isShowAutoSuggestShuffle) return;
        if (level <= Config.LEVEL_UNLOCK_BOMB || _isShowAutoSuggestShuffle) return;
        if (level <= Config.LEVEL_UNLOCK_BEE || _isShowAutoSuggestShuffle) return;

        _timeAutoSuggestShuffle += Time.deltaTime;

        if (_timeAutoSuggestShuffle >= Config.AUTO_TIME_TO_SUGGEST_SHUFFLE)
        {
            ShowAutoSuggest_Shuffle();
        }
    }

    public GameLevelManager levelGame;

    private void LoadLevelGame()
    {
        levelGame.InitSlotPosition(slotBGTranform.position);

        levelGame.StartGame();
        StartCoroutine(LoadLevelGame_IEnumerator());
    }

    private IEnumerator LoadLevelGame_IEnumerator()
    {
        yield return new WaitForSeconds(0.02f);
        levelGame.InitSlotPosition(slotBGTranform.position);
    }

    private void HideElementUI()
    {
        Config.gameState = Config.GAME_STATE.PAUSE;
        canvas.enabled = false;
        btnPause.gameObject.SetActive(false);
        boosterGroup.gameObject.SetActive(false);
        starGroup.gameObject.SetActive(false);
        txtLevel.gameObject.SetActive(false);
        bgFooter.gameObject.SetActive(false);
        btnOutGame.gameObject.SetActive(false);

        levelGame.score.gameObject.SetActive(false);
        comboProcess.SetActive(false);
    }

    private void InitViews()
    {
        SetStart_Success();
        objUndoBG.gameObject.SetActive(true);
        objSuggestBG.gameObject.SetActive(true);
        objShuffleBG.gameObject.SetActive(true);
        objUndoAllBG.gameObject.SetActive(true);
        objExtraSlotBG.gameObject.SetActive(true);
        starGroup.gameObject.SetActive(false);
        clockGroup.gameObject.SetActive(false);

        btnOutGame.Interactable = Config.currLevel > Config.LEVEL_UNLOCK_BUILDING;
        btnPause.Interactable = true;
    }

    private void InitViews_ShowView()
    {
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.2f, () =>
        {
            SoundManager.Instance.PlaySound_ShowView();

            bgFooter.gameObject.SetActive(true);
            bgFooter.GetComponent<BBUIView>().ShowView();

            boosterGroup.gameObject.SetActive(true);
            boosterGroup.GetComponent<BBUIView>().ShowView();

            if (Config.currLevel >= Config.LEVEL_UNLOCK_COMBO)
            {
                comboProcess.SetActive(true);
                comboProcess.GetComponent<BBUIView>().ShowView();
            }
        });

        sequenceShowView.InsertCallback(0.2f, () =>
        {
            txtLevel.gameObject.SetActive(true);
            txtLevel.GetComponent<BBUIView>().ShowView();

            if (levelGame.secondsRequired == 0)
            {
                starGroup.gameObject.SetActive(true);
                starGroup.GetComponent<BBUIView>().ShowView();
            }
            else
            {
                clockGroup.gameObject.SetActive(true);
                clockGroup.GetComponent<BBUIView>().ShowView();
            }
        });

        sequenceShowView.InsertCallback(0.3f, () =>
        {
            btnPause.gameObject.SetActive(true);
            btnPause.GetComponent<BBUIView>().ShowView();

            btnOutGame.gameObject.SetActive(true);
            btnOutGame.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.5f, () => { InitViews_ShowView_Finished(); });
    }

    public void HideView(bool hideCanvas = false)
    {
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.2f, () =>
        {
            levelGame.RemoveTileInFloor();

            SoundManager.Instance.PlaySound_ShowView();

            bgFooter.GetComponent<BBUIView>().HideView();

            btnPause.GetComponent<BBUIView>().HideView();

            btnOutGame.GetComponent<BBUIView>().HideView();

            //levelGame.score.GetComponent<BBUIView>().HideView();

            comboProcess.GetComponent<BBUIView>().HideView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            txtLevel.GetComponent<BBUIView>().HideView();
            if (levelGame.secondsRequired == 0)
                starGroup.GetComponent<BBUIView>().HideView();
            else
            {
                clockGroup.GetComponent<BBUIView>().HideView();
            }
        });

        sequenceShowView.InsertCallback(0.5f, () => { boosterGroup.GetComponent<BBUIView>().HideView(); });
        sequenceShowView.InsertCallback(0.6f, () =>
        {
            HideView_Finished(hideCanvas);
            Config.gameState = Config.GAME_STATE.END;
            SortingLayerCanvasUI();
            if (!hideCanvas)
                MenuManager.instance.StartMenuManager();
        });
    }

    private void HideView_Finished(bool hideCanvas)
    {
        HideElementUI();
        canvas.enabled = hideCanvas;
    }

    public async void HideViewWhenWinGame()
    {
        await MenuManager.instance.TransactionLoadBackground();
        canvas.enabled = false;
        MenuManager.instance.StartMenuManager();
    }

    private void InitViews_ShowView_Finished()
    {
    }

    public void SetStartPlayingGame()
    {
        if (Config.CheckTutorial_Match3())
        {
            TutorialManager.Instance.ShowTut_ClickTile();

            Config.gameState = Config.GAME_STATE.PLAYING;
            SortingLayerCanvasPlaying();
            return;
        }

        if (Config.CheckTutorial_Combo())
        {
            ShowTut_Combo();
            return;
        }

        if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.GLUE))
        {
            ShowUnlockItem(Config.ITEM_UNLOCK.GLUE, () => { TutorialManager.Instance.ShowTut_ClickTileGlue(); });
            return;
        }

        if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.CHAIN))
        {
            ShowUnlockItem(Config.ITEM_UNLOCK.CHAIN, () => { TutorialManager.Instance.ShowTut_ClickTileChain(); });
            return;
        }

        if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.ICE))
        {
            ShowUnlockItem(Config.ITEM_UNLOCK.ICE, () => { TutorialManager.Instance.ShowTut_ClickTileIce(); });
            return;
        }

        if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.GRASS))
        {
            ShowUnlockItem(Config.ITEM_UNLOCK.GRASS, () => { TutorialManager.Instance.ShowTut_ClickTileGrass(); });
            return;
        }

        if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.CLOCK))
        {
            ShowUnlockItem(Config.ITEM_UNLOCK.CLOCK, OpenStartWithClockPopup);
            return;
        }

        if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.BOMB))
        {
            ShowUnlockItem(Config.ITEM_UNLOCK.BOMB, () => { TutorialManager.Instance.ShowTut_ClickTileBoom(); });
            return;
        }

        if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.BEE))
        {
            ShowUnlockItem(Config.ITEM_UNLOCK.BEE, () => { TutorialManager.Instance.ShowTut_ClickTileBee(); });
            return;
        }

        if (Config.CheckTutorial_Undo())
        {
            var nextLevelDefinition = AssetManager.Instance.GetItemUnlockDefinition(Config.ITEM_UNLOCK.UNDO);
            OpenUnLockNewItem(nextLevelDefinition, null);
            return;
        }

        if (Config.CheckTutorial_Suggest())
        {
            var nextLevelDefinition = AssetManager.Instance.GetItemUnlockDefinition(Config.ITEM_UNLOCK.SUGGEST);
            OpenUnLockNewItem(nextLevelDefinition, () => { TutorialManager.Instance.ShowTut_Suggest(); }
            );
            return;
        }

        if (Config.CheckTutorial_Shuffle())
        {
            var nextLevelDefinition = AssetManager.Instance.GetItemUnlockDefinition(Config.ITEM_UNLOCK.SHUFFLE);
            OpenUnLockNewItem(nextLevelDefinition,
                () => { TutorialManager.Instance.ShowTut_Shuffle(); });

            return;
        }

        if (Config.CheckTutorial_TileReturn())
        {
            var nextLevelDefinition = AssetManager.Instance.GetItemUnlockDefinition(Config.ITEM_UNLOCK.TILE_RETURN);
            OpenUnLockNewItem(nextLevelDefinition,
                () => { TutorialManager.Instance.ShowTut_TileReturn(); });
            return;
        }

        if (Config.CheckTutorial_ExtraSlot())
        {
            var nextLevelDefinition = AssetManager.Instance.GetItemUnlockDefinition(Config.ITEM_UNLOCK.EXTRA_SLOT);
            OpenUnLockNewItem(nextLevelDefinition,
                () =>
                {
                    Config.gameState = Config.GAME_STATE.TUTORIAL;
                    TutorialManager.Instance.ShowTut_ExtraSlot();
                    SortingLayerCanvasPlaying();
                });
            return;
        }

        if (levelGame.secondsRequired > 0)
        {
            OpenStartWithClockPopup();
            return;
        }

        DisableClock();
        Config.gameState = Config.GAME_STATE.PLAYING;
        SortingLayerCanvasPlaying();
    }

    private void ShowUnlockItem(Config.ITEM_UNLOCK itemUnlock, UnityAction callback = null)
    {
        var nextLevelDefinition = AssetManager.Instance.GetItemUnlockDefinition(itemUnlock);
        OpenUnLockNewItem(nextLevelDefinition, callback);
        Config.gameState = Config.GAME_STATE.PLAYING;
    }

    #region PAUSE

    private void TouchPause()
    {
        if (Config.gameState == Config.GAME_STATE.PLAYING)
        {
            Config.gameState = Config.GAME_STATE.PAUSE;
            SortingLayerCanvasUI();

            GameDisplay.Instance.OpenSettingPopup();

            HideTut_HandGuide();
        }
    }
    
    private void TouchHideUI()
    {
        topGroup.SetActive(!topGroup.activeSelf);
    }


    public void SetUnPause()
    {
        Config.gameState = Config.GAME_STATE.PLAYING;
        SortingLayerCanvasPlaying();
    }

    #endregion

    #region UNDO

    private void TouchUndo()
    {
        if (Config.gameState != Config.GAME_STATE.PLAYING && Config.gameState != Config.GAME_STATE.TUTORIAL)
            return;

        if (!GameLevelManager.Instance.CheckUndoAvailable())
        {
            NotificationPopup.instance.AddNotification($"Unlock at level {Config.LEVEL_UNLOCK_UNDO}",
                slotBGTranform.position);
            return;
        }

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.UNDO) > 0)
        {
            var undo = GameLevelManager.Instance.SetUndo();
            if (!undo)
            {
                NotificationPopup.instance.AddNotification("NO MOVE TO UNDO", slotBGTranform.position);
                return;
            }

            Config.UseItemHelp(Config.ITEMHELP_TYPE.UNDO);
            SetUpdate_CountItem();
            objUndoBG.gameObject.SetActive(true);

            if (Config.CheckTutorial_Undo() && Config.IsShowTutUndo)
                TutorialManager.Instance.HideTut_Undo();

            FirebaseManager.Instance.LogUsePowerUp(FirebaseManager.POWERUP_UNDO, level);
        }
        else if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.UNDO) == 0)
        {
            OpenBuyItemPopup(Config.ITEMHELP_TYPE.UNDO);
        }

        HideTut_HandGuide();
    }

    public void SetStatusUndoButton(bool isLock)
    {
        if (Config.currLevel < Config.LEVEL_UNLOCK_UNDO) return;
        UndoButtonManager.SetSpriteIcon(isLock);
    }

    #endregion

    #region SUGGEST

    private void TouchSuggest()
    {
        if (Config.gameState != Config.GAME_STATE.PLAYING && Config.gameState != Config.GAME_STATE.TUTORIAL)
            return;

        if (Config.currLevel < Config.LEVEL_UNLOCK_SUGGEST)
        {
            NotificationPopup.instance.AddNotification($"Unlock at level {Config.LEVEL_UNLOCK_SUGGEST}",
                slotBGTranform.position);
            return;
        }

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SUGGEST) > 0)
        {
            GameLevelManager.Instance.SetSuggest();

            if (Config.CheckTutorial_Suggest() && Config.isShowTut_suggest)
            {
                TutorialManager.Instance.HideTut_Suggest();
            }

            FirebaseManager.Instance.LogUsePowerUp(FirebaseManager.POWERUP_HINT, level);
        }
        else
        {
            OpenBuyItemPopup(Config.ITEMHELP_TYPE.SUGGEST);
        }

        HideTut_HandGuide();
    }

    public void SetSuggestSuccess()
    {
        Config.UseItemHelp(Config.ITEMHELP_TYPE.SUGGEST);
        SetUpdate_CountItem();
        objSuggestBG.gameObject.SetActive(true);
    }

    #endregion

    #region SHUFFLE

    private void TouchShuffle()
    {
        if (Config.gameState != Config.GAME_STATE.PLAYING && Config.gameState != Config.GAME_STATE.TUTORIAL)
            return;

        if (Config.currLevel < Config.LEVEL_UNLOCK_SHUFFLE)
        {
            NotificationPopup.instance.AddNotification($"Unlock at level {Config.LEVEL_UNLOCK_SHUFFLE}",
                slotBGTranform.position);
            return;
        }

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SHUFFLE) > 0)
        {
            GameLevelManager.Instance.SetShuffle();
            Config.UseItemHelp(Config.ITEMHELP_TYPE.SHUFFLE);
            SetUpdate_CountItem();
            objShuffleBG.gameObject.SetActive(true);

            if (Config.CheckTutorial_Shuffle() && Config.IsShowTutShuffle)
            {
                TutorialManager.Instance.HideTut_Shuffle();
            }

            FirebaseManager.Instance.LogUsePowerUp(FirebaseManager.POWERUP_SHUFFLE, level);
        }
        else
        {
            OpenBuyItemPopup(Config.ITEMHELP_TYPE.SHUFFLE);
        }

        HideTut_HandGuide();
    }

    #endregion

    #region TILE_RETURN

    private void TouchTileReturn()
    {
        if (Config.gameState != Config.GAME_STATE.PLAYING && Config.gameState != Config.GAME_STATE.TUTORIAL)
            return;

        if (Config.currLevel < Config.LEVEL_UNLOCK_TILE_RETURN)
        {
            NotificationPopup.instance.AddNotification($"Unlock at level {Config.LEVEL_UNLOCK_TILE_RETURN}",
                slotBGTranform.position);
            return;
        }

        if (!GameLevelManager.Instance.CheckTileReturnAvailable())
        {
            NotificationPopup.instance.AddNotification("NO MOVE TO TILE RETURN", slotBGTranform.position);
            return;
        }

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.TILE_RETURN) > 0)
        {
            GameLevelManager.Instance.SetTileReturn();
            Config.UseItemHelp(Config.ITEMHELP_TYPE.TILE_RETURN);
            SetUpdate_CountItem();
            objShuffleBG.gameObject.SetActive(true);

            FirebaseManager.Instance.LogUsePowerUp(FirebaseManager.POWERUP_TILE_RETURN, level);
        }
        else
        {
            OpenBuyItemPopup(Config.ITEMHELP_TYPE.TILE_RETURN);
        }

        HideTut_HandGuide();
    }

    public void UpdateTileReturnStatus(bool isLock)
    {
        if (Config.currLevel < Config.LEVEL_UNLOCK_TILE_RETURN || Config.CheckTutorial_TileReturn()) return;
        TileReturnButtonManager.SetSpriteIcon(isLock);
    }

    #endregion

    #region EXTRA SLOT

    private void TouchExtraSlot()
    {
        if (Config.gameState != Config.GAME_STATE.PLAYING && Config.gameState != Config.GAME_STATE.TUTORIAL)
            return;

        if (Config.currLevel < Config.LEVEL_UNLOCK_EXTRA_SLOT)
        {
            NotificationPopup.instance.AddNotification($"Unlock at level {Config.LEVEL_UNLOCK_EXTRA_SLOT}",
                slotBGTranform.position);
            return;
        }

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.EXTRA_SLOT) > 0)
        {
            if (GameLevelManager.Instance.Slot == 8) return;

            GameLevelManager.Instance.RecruitSlot();
            Config.UseItemHelp(Config.ITEMHELP_TYPE.EXTRA_SLOT);
            SetUpdate_CountItem();
            objExtraSlotBG.gameObject.SetActive(true);

            if (Config.CheckTutorial_ExtraSlot() && Config.isShowTut5)
            {
                TutorialManager.Instance.HideTut_AddSlot();
            }

            FirebaseManager.Instance.LogUsePowerUp(FirebaseManager.POWERUP_EXTRA_SLOT, level);
        }
        else
        {
            OpenBuyItemPopup(Config.ITEMHELP_TYPE.EXTRA_SLOT);
        }

        HideTut_HandGuide();
    }

    #endregion

    #region WIN

    public WinPopup winPopup;

    public void SetGameWin()
    {
        if (Config.gameState != Config.GAME_STATE.WIN)
        {
            FirebaseManager.Instance.LogLevelWin(level, _timePlayed);
            SetFinishedGame();
            SortingLayerCanvasUI();
            Config.gameState = Config.GAME_STATE.WIN;

            var star = levelGame.secondsRequired == 0 ? starGroup.GetCurrStar() : 3;

            winPopup.ShowWinPopup(level, star,
                GameLevelManager.Instance.configLevelGame.listRewards_CoinValue[star]);

            Config.SetCurrLevel(level + 1);
            Config.currSelectLevel = Config.currLevel;
        }
    }

    public void SetLoadGame()
    {
        StartGame();
    }

    public void SetFinishedGame()
    {
        btnPause.Interactable = false;
    }

    #endregion

    #region LOSE

    public LosePopup losePopup;

    public void SetGameLose()
    {
        if (Config.gameState != Config.GAME_STATE.LOSE)
        {
            SortingLayerCanvasUI();
            SetFinishedGame();
            Config.gameState = Config.GAME_STATE.LOSE;
            losePopup.ShowLosePopup(level, _isRevive);
        }
    }

    public void SetReplayGame()
    {
        SetLoadGame();
    }

    #endregion

    #region TIME_UP

    public TimeUpPopup timeUpPopup;

    public void SetGameTimeUp()
    {
        timeUpPopup.ShowTimeUpPopup(level);
    }

    #endregion

    #region FREE_ITEM

    public void SetBuyItem_Success()
    {
        SetUpdate_CountItem();
    }

    #endregion

    #region BUY_ITEM

    public BuyItemPopup buyItemPopup;

    private void OpenBuyItemPopup(Config.ITEMHELP_TYPE itemHelpType)
    {
        Config.gameState = Config.GAME_STATE.SHOP;
        SortingLayerCanvasUI();
        buyItemPopup.OpenBuyItemPopup(itemHelpType);
    }

    #endregion

    #region OTHER

    public void SetUpdate_CountItem()
    {
        txtUndoCount.text = $"{Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.UNDO)}";
        txtUndoCount.gameObject.SetActive(true);
        txtUndoAdd.gameObject.SetActive(false);
        if (Config.GetCount_ItemHelpFree(Config.ITEMHELP_TYPE.UNDO) > 0 && level == Config.LEVEL_UNLOCK_UNDO)
        {
            UndoButtonManager.ShowButtonItem_Lock(Config.currLevel < Config.LEVEL_UNLOCK_UNDO);
            UndoButtonManager.SetSpriteIcon(true);
            UndoButtonManager.SetFree();
        }
        else
        {
            if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.UNDO) == 0)
            {
                txtUndoAdd.gameObject.SetActive(true);
                txtUndoCount.gameObject.SetActive(false);
                txtUndoCount.text = "+";
            }
            else
            {
                UndoButtonManager.ShowButtonItem_Lock(Config.currLevel < Config.LEVEL_UNLOCK_UNDO);
                UndoButtonManager.SetSpriteIcon(true);
            }
        }


        txtSuggestCount.text = $"{Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SUGGEST)}";
        txtSuggestCount.gameObject.SetActive(true);
        txtSuggestAdd.gameObject.SetActive(false);

        if (Config.GetCount_ItemHelpFree(Config.ITEMHELP_TYPE.SUGGEST) > 0 &&
            level == Config.LEVEL_UNLOCK_SUGGEST)
        {
            SuggestButtonManager.ShowButtonItem_Lock(Config.currLevel < Config.LEVEL_UNLOCK_SUGGEST);
            SuggestButtonManager.SetFree();
        }
        else
        {
            if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SUGGEST) == 0)
            {
                txtSuggestAdd.gameObject.SetActive(true);
                txtSuggestCount.gameObject.SetActive(false);
                txtSuggestCount.text = $"+";
            }
            else
            {
                SuggestButtonManager.ShowButtonItem_Lock(Config.currLevel < Config.LEVEL_UNLOCK_SUGGEST);
            }
        }

        txtShuffleCount.text = $"{Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SHUFFLE)}";
        txtShuffleCount.gameObject.SetActive(true);
        txtShuffleAdd.gameObject.SetActive(false);

        if (Config.GetCount_ItemHelpFree(Config.ITEMHELP_TYPE.SHUFFLE) > 0 &&
            level == Config.LEVEL_UNLOCK_SHUFFLE)
        {
            ShuffleButtonManager.ShowButtonItem_Lock(Config.currLevel < Config.LEVEL_UNLOCK_SHUFFLE);
            ShuffleButtonManager.SetFree();
        }
        else
        {
            if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SHUFFLE) == 0)
            {
                txtShuffleAdd.gameObject.SetActive(true);
                txtShuffleCount.gameObject.SetActive(false);
                txtShuffleCount.text = "+";
            }
            else
            {
                ShuffleButtonManager.ShowButtonItem_Lock(Config.currLevel < Config.LEVEL_UNLOCK_SHUFFLE);
            }
        }

        if (Config.GetCount_ItemHelpFree(Config.ITEMHELP_TYPE.TILE_RETURN) > 0 &&
            level == Config.LEVEL_UNLOCK_TILE_RETURN)
        {
            TileReturnButtonManager.ShowButtonItemLockWithoutSprite(Config.currLevel < Config.LEVEL_UNLOCK_TILE_RETURN);
            TileReturnButtonManager.SetSpriteIcon(!Config.CheckTutorial_TileReturn());
            TileReturnButtonManager.SetFree();
        }
        else
        {
            if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.TILE_RETURN) == 0)
            {
                txtUndoAllCount.gameObject.SetActive(false);
                txtUndoAllAdd.text = "+";
                txtUndoAllAdd.gameObject.SetActive(true);
            }
            else
            {
                txtUndoAllCount.text = $"{Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.TILE_RETURN)}";
                txtUndoAllCount.gameObject.SetActive(true);
                txtUndoAllAdd.gameObject.SetActive(false);

                TileReturnButtonManager.ShowButtonItemLockWithoutSprite(Config.currLevel <
                                                                        Config.LEVEL_UNLOCK_TILE_RETURN);
                TileReturnButtonManager.SetSpriteIcon(!Config.CheckTutorial_TileReturn());
            }
        }

        if (Config.GetCount_ItemHelpFree(Config.ITEMHELP_TYPE.EXTRA_SLOT) > 0 &&
            level == Config.LEVEL_UNLOCK_EXTRA_SLOT)
        {
            ExtraSlotButtonManager.ShowButtonItem_Lock(Config.currLevel < Config.LEVEL_UNLOCK_EXTRA_SLOT);
            //ExtraSlotButtonManager.SetSpriteIcon(true);
            ExtraSlotButtonManager.SetFree();
        }
        else
        {
            if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.EXTRA_SLOT) == 0)
            {
                txtExtraSlotAdd.gameObject.SetActive(true);
                txtExtraSlotCount.gameObject.SetActive(false);
                txtExtraSlotAdd.text = "+";
            }
            else
            {
                txtExtraSlotCount.text = $"{Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.EXTRA_SLOT)}";
                txtExtraSlotCount.gameObject.SetActive(true);
                txtExtraSlotAdd.gameObject.SetActive(false);

                ExtraSlotButtonManager.ShowButtonItem_Lock(Config.currLevel < Config.LEVEL_UNLOCK_EXTRA_SLOT);
                //ExtraSlotButtonManager.SetSpriteIcon(true);
            }
        }
    }

    public void CloseShopSuccess()
    {
        Config.gameState = Config.GAME_STATE.PLAYING;
        SortingLayerCanvasPlaying();
    }

    #endregion

    #region CHESTPOPUP

    public void CloseChestPopup()
    {
        if (WinPopup.Instance.isActiveAndEnabled)
        {
            WinPopup.Instance.SetEnableNativeAd();
        }
    }

    #endregion

    #region REWARD POPUP

    public void OpenRewardPopup(List<ConfigItemShopData> listData, bool isShowCollectx2 = true)
    {
        Config.gameState = Config.GAME_STATE.WIN;
        SortingLayerCanvasUI();
        GameDisplay.Instance.OpenRewardPopup(listData, isShowCollectx2);
    }

    #endregion

    #region REVIVE

    private bool _isRevive;

    public void SetRevive_Success()
    {
        _isRevive = true;
        starGroup.Revive_InitStarGroup();

        StartCoroutine(YieldSetPlaying());
        btnPause.Interactable = true;
    }

    private IEnumerator YieldSetPlaying()
    {
        yield return new WaitForSeconds(0.2f);
        SortingLayerCanvasPlaying();
        Config.gameState = Config.GAME_STATE.PLAYING;
    }

    private void SetStart_Success()
    {
        _isRevive = true;
        starGroup.Revive_InitStarGroup();
    }

    #endregion

    #region TUTORIAL

    private void AI_Suggest_Magic()
    {
        TutorialManager.Instance.SetPositionHandGuild_AI(btnSuggest.transform.position);
    }

    private void AI_Suggest_Shuffle()
    {
        TutorialManager.Instance.SetPositionHandGuild_AI(btnShuffle.transform.position);
    }

    private void AI_Suggest_ExtraSlot()
    {
        TutorialManager.Instance.SetPositionHandGuild_AI(btnExtraSlot.transform.position);
    }

    private void AI_Suggest_TileReturn()
    {
        TutorialManager.Instance.SetPositionHandGuild_AI(btnExtraSlot.transform.position);
    }

    private void ShowTut_Combo()
    {
        TutorialManager.Instance.ShowTut_Combo();
    }

    public void HideTut_HandGuide()
    {
        ResetTimeAutoSuggest();
        TutorialManager.Instance.HideHandGuild();
    }

    public void ShowTut_NextLevel(Vector3 pos)
    {
        if (!Config.GetTut_Finished(Config.TUT.TUT_NEXTLEVEL_LEVEL1))
        {
            Config.SetTut_Finished(Config.TUT.TUT_NEXTLEVEL_LEVEL1);
            TutorialManager.Instance.SetPositionHandGuild_NextLevel(pos);
        }
    }

    public void HideTut_NextLevel()
    {
        HideTut_HandGuide();
    }

    #endregion

    #region Particle_Match3

    private GameObject _particleMatch3;

    public async UniTask SpawnParticle(Vector3 spawnPosition)
    {
        if (_particleMatch3 == null)
            _particleMatch3 = Resources.Load<GameObject>("Particle_Match3");

        var particle = Instantiate(_particleMatch3, gameObject.transform);
        particle.transform.position = spawnPosition;

        await UniTask.Delay(500);
        Destroy(particle.gameObject);
    }

    #endregion

    #region RATEPOPUP

    public void OpenRatePopup()
    {
        StartCoroutine(OpenRatePopup_IEnumerator());
    }

    private IEnumerator OpenRatePopup_IEnumerator()
    {
        yield return new WaitForSeconds(0.5f);
        GameDisplay.Instance.OpenRatePopup();
    }

    #endregion

    #region SLOT BG

    public Transform slotBGTranform;

    #endregion

    #region AUTO SHUFFLE SUGGEST

    private float _timeAutoSuggestShuffle = 0;
    private bool _isShowAutoSuggestShuffle = false;


    private void ResetTimeAutoSuggest()
    {
        _timeAutoSuggestShuffle = 0;
        _isShowAutoSuggestShuffle = false;
    }

    private void ShowAutoSuggest_Shuffle()
    {
        _isShowAutoSuggestShuffle = true;
        _timeAutoSuggestShuffle = 0;

        if (GameLevelManager.Instance.CheckSuggestAvailable() &&
            Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SUGGEST) > 0)
        {
            AI_Suggest_Magic();
            return;
        }

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SHUFFLE) > 0)
        {
            AI_Suggest_Shuffle();
            return;
        }

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.TILE_RETURN) > 0
            && GameLevelManager.Instance.CheckTileReturnAvailable())
        {
            AI_Suggest_TileReturn();
            return;
        }

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.EXTRA_SLOT) > 0)
        {
            AI_Suggest_ExtraSlot();
            return;
        }

        AI_Suggest_Magic();
    }

    #endregion

    #region SHOP

    public void OpenShopPopup()
    {
        GameDisplay.Instance.OpenShop();
    }

    #endregion

    #region TRY AGAIN

    [Header("TRY AGAIN")] public TryAgainPopup tryAgainPopup;

    public void OpenTryAgainPopup()
    {
        tryAgainPopup.ShowTryAgainPopup(Config.currSelectLevel);
    }

    #endregion

    #region OUT GAME

    [Header("OUT GAME")] public OutGamePopup outGamePopup;

    private void OpenOutGamePopup()
    {
        if (Config.gameState == Config.GAME_STATE.PLAYING)
        {
            SortingLayerCanvasUI();
            Config.gameState = Config.GAME_STATE.PAUSE;
            outGamePopup.ShowOutGamePopup();

            HideTut_HandGuide();
        }
    }

    #endregion

    #region CLOCK POPUP

    [Header("CLOCK POPUP")] public StartWithClockPopup startWithClockPopup;
    public ClockGroup clockGroup;

    private void OpenStartWithClockPopup()
    {
        SortingLayerCanvasUI();
        if (Config.CheckShowItemUnlockFinished(Config.ITEM_UNLOCK.CLOCK))
        {
            Config.SetShowItemUnlockFinished(Config.ITEM_UNLOCK.CLOCK);
        }

        Config.gameState = Config.GAME_STATE.START;
        startWithClockPopup.Show(levelGame.secondsRequired);

        HideTut_HandGuide();
    }

    public void EnableClock(Vector3 starPosition, int seconds)
    {
        clockGroup.Enable(starPosition, seconds);
    }

    private void DisableClock()
    {
        clockGroup.Disable();
    }

    #endregion

    #region UNLOCK NEW ITEM

    [FormerlySerializedAs("unLockNewItem")] [Header("OUT GAME")]
    public UnlockNewItem unlockNewItem;

    public void OpenUnLockNewItem(UnlockNewItemData data, UnityAction action)
    {
        unlockNewItem.ShowUnLockNewItem(data, action);
    }

    #endregion

    public void SortingLayerCanvasUI()
    {
        canvas.sortingLayerName = "UI";
    }

    public void SortingLayerCanvasPlaying()
    {
        canvas.sortingLayerName = "Default";
    }
}