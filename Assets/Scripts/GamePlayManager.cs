using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using HellTap.PoolKit;
using TMPro;
using UnityEngine.Serialization;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager Instance;
    public Canvas canvas;
    public Canvas canvasStart;
    [Header("Select Level")] public int level = 1;

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
        EnableCanvas();
    }

    private void StartGame(int levelSet = -100)
    {
        levelGame.gameObject.SetActive(true);
        levelGame.RemoveTileInFloor();
        canvas.enabled = true;
        _isRevive = false;

        btnUndo.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnTileReturn.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnExtraSlot.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnSuggest.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnShuffle.OnPointerClickCallBack_Start.RemoveAllListeners();
        btnHideUI.OnPointerClickCallBack_Start.RemoveAllListeners();

        btnHideUI.OnPointerClickCallBack_Start.AddListener(TouchHideUI);
        btnUndo.OnPointerClickCallBack_Start.AddListener(TouchUndo);
        btnSuggest.OnPointerClickCallBack_Start.AddListener(TouchSuggest);
        btnShuffle.OnPointerClickCallBack_Start.AddListener(TouchShuffle);
        btnTileReturn.OnPointerClickCallBack_Start.AddListener(TouchTileReturn);
        btnExtraSlot.OnPointerClickCallBack_Start.AddListener(TouchExtraSlot);

        level = levelSet;
        
        txtLevel.text = $"Level: {level}";
        Config.currSelectLevel = level;
        Config.gameState = Config.GAME_STATE.START;
        InitViews();
        LoadLevelGame();
        SetUpdate_CountItem();
        InitViews_ShowView();
        _timePlayed = 0;
        StartCoroutine(YieldUpdateTime());
    }

    private IEnumerator YieldUpdateTime()
    {
        while (true)
        {
            _timePlayed++;
            if (Config.gameState == Config.GAME_STATE.WIN) break;
            yield return new WaitForSecondsRealtime(1f);
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

    public void HideElementUI()
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

    public void EnableCanvas()
    {
        canvasStart.enabled = true;
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

        btnOutGame.Interactable = true;
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

            comboProcess.SetActive(true);
            comboProcess.GetComponent<BBUIView>().ShowView();
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
        sequenceShowView.OnComplete(() =>
        {
            HideView_Finished(hideCanvas);
            Config.gameState = Config.GAME_STATE.END;
            SortingLayerCanvasUI();
        });
    }

    private void HideView_Finished(bool hideCanvas)
    {
        HideElementUI();
        canvas.enabled = hideCanvas;
    }

    public void HideViewWhenWinGame()
    {
        EnableCanvas();
    }

    private void InitViews_ShowView_Finished()
    {
    }

    public void SetStartPlayingGame()
    {
        if (levelGame.secondsRequired > 0)
        {
            OpenStartWithClockPopup();
            return;
        }

        DisableClock();
        Config.gameState = Config.GAME_STATE.PLAYING;
        SortingLayerCanvasPlaying();
    }

    #region PAUSE

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
        }
        else if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.UNDO) == 0)
        {
            OpenBuyItemPopup(Config.ITEMHELP_TYPE.UNDO);
        }
    }

    public void SetStatusUndoButton(bool isLock)
    {
        UndoButtonManager.SetSpriteIcon(isLock);
    }

    #endregion

    #region SUGGEST

    private void TouchSuggest()
    {
        if (Config.gameState != Config.GAME_STATE.PLAYING && Config.gameState != Config.GAME_STATE.TUTORIAL)
            return;

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SUGGEST) > 0)
        {
            GameLevelManager.Instance.SetSuggest();
        }
        else
        {
            OpenBuyItemPopup(Config.ITEMHELP_TYPE.SUGGEST);
        }
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

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SHUFFLE) > 0)
        {
            GameLevelManager.Instance.SetShuffle();
            Config.UseItemHelp(Config.ITEMHELP_TYPE.SHUFFLE);
            SetUpdate_CountItem();
            objShuffleBG.gameObject.SetActive(true);
        }
        else
        {
            OpenBuyItemPopup(Config.ITEMHELP_TYPE.SHUFFLE);
        }
    }

    #endregion

    #region TILE_RETURN

    private void TouchTileReturn()
    {
        if (Config.gameState != Config.GAME_STATE.PLAYING && Config.gameState != Config.GAME_STATE.TUTORIAL)
            return;

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
        }
        else
        {
            OpenBuyItemPopup(Config.ITEMHELP_TYPE.TILE_RETURN);
        }
    }

    public void UpdateTileReturnStatus(bool isLock)
    {
        TileReturnButtonManager.SetSpriteIcon(isLock);
    }

    #endregion

    #region EXTRA SLOT

    private void TouchExtraSlot()
    {
        if (Config.gameState != Config.GAME_STATE.PLAYING && Config.gameState != Config.GAME_STATE.TUTORIAL)
            return;

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.EXTRA_SLOT) > 0)
        {
            if (GameLevelManager.Instance.Slot == 8) return;

            GameLevelManager.Instance.RecruitSlot();
            Config.UseItemHelp(Config.ITEMHELP_TYPE.EXTRA_SLOT);
            SetUpdate_CountItem();
            objExtraSlotBG.gameObject.SetActive(true);
        }
        else
        {
            OpenBuyItemPopup(Config.ITEMHELP_TYPE.EXTRA_SLOT);
        }
    }

    #endregion

    #region WIN

    public WinPopup winPopup;

    public void SetGameWin()
    {
        if (Config.gameState != Config.GAME_STATE.WIN)
        {
            SetFinishedGame();
            SortingLayerCanvasUI();
            Config.gameState = Config.GAME_STATE.WIN;

            var star = levelGame.secondsRequired == 0 ? starGroup.GetCurrStar() : 3;

            winPopup.ShowWinPopup(level, star);
        }
    }

    public void SetLoadGame(int levelSet = -100)
    {
        StartGame(levelSet);
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

        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.UNDO) == 0)
        {
            txtUndoAdd.gameObject.SetActive(true);
            txtUndoCount.gameObject.SetActive(false);
            txtUndoCount.text = "+";
        }
        else
        {
            UndoButtonManager.ShowButtonItem_Lock(false);
            UndoButtonManager.SetSpriteIcon(true);
        }

        txtSuggestCount.text = $"{Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SUGGEST)}";
        txtSuggestCount.gameObject.SetActive(true);
        txtSuggestAdd.gameObject.SetActive(false);


        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SUGGEST) == 0)
        {
            txtSuggestAdd.gameObject.SetActive(true);
            txtSuggestCount.gameObject.SetActive(false);
            txtSuggestCount.text = "+";
        }
        else
        {
            SuggestButtonManager.ShowButtonItem_Lock(false);
        }

        txtShuffleCount.text = $"{Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SHUFFLE)}";
        txtShuffleCount.gameObject.SetActive(true);
        txtShuffleAdd.gameObject.SetActive(false);


        if (Config.GetCount_ItemHelp(Config.ITEMHELP_TYPE.SHUFFLE) == 0)
        {
            txtShuffleAdd.gameObject.SetActive(true);
            txtShuffleCount.gameObject.SetActive(false);
            txtShuffleCount.text = "+";
        }
        else
        {
            ShuffleButtonManager.ShowButtonItem_Lock(false);
        }


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

            TileReturnButtonManager.ShowButtonItemLockWithoutSprite(false);
            TileReturnButtonManager.SetSpriteIcon(true);
        }


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

            ExtraSlotButtonManager.ShowButtonItem_Lock(false);
        }
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

    #endregion

    #region TRY AGAIN

    [Header("TRY AGAIN")] public TryAgainPopup tryAgainPopup;

    public void OpenTryAgainPopup()
    {
        tryAgainPopup.ShowTryAgainPopup(Config.currSelectLevel);
    }

    #endregion

    #region CLOCK POPUP

    [Header("CLOCK POPUP")] public StartWithClockPopup startWithClockPopup;
    public ClockGroup clockGroup;

    private void OpenStartWithClockPopup()
    {
        SortingLayerCanvasUI();

        Config.gameState = Config.GAME_STATE.START;
        startWithClockPopup.Show(levelGame.secondsRequired);
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

    public void SortingLayerCanvasUI()
    {
        canvas.sortingLayerName = "UI";
    }

    public void SortingLayerCanvasPlaying()
    {
        canvas.sortingLayerName = "Default";
    }
}