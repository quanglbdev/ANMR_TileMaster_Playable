using DG.Tweening;
using HellTap.PoolKit;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    [Header("TUTORIAL MASK")] public RectTransform unMask;

    [SerializeField] private GameObject tutorialMask;

    [Header("HAND GUILD")] [SerializeField]
    private GameObject handGuidePrefab;
    
    [Header("ARROW GUILD")] [SerializeField]
    private GameObject arrowPrefab;

    private GameObject _handGuild;
    private GameObject _arrowGuild;
    private Image _maskImage;

    private Image MaskImage
    {
        get
        {
            if (_maskImage == null) _maskImage = tutorialMask.GetComponent<Image>();
            return _maskImage;
        }
    }

    [Header("TUTORIAL")] [SerializeField] private TutorialText
        tutorialMatch3,
        tutorialCombo,
        tutorialBuilding,
        tutorialUndo,
        tutorialShuffle,
        tutorialTileReturn,
        tutorialSuggest,
        tutorialExtraSlot,
        tutorialProfile,
        tutorialLeaderboard,
        tutorialGiftBoxLeaderboard,
        tutorialSpin,
        tutorialGlue,
        tutorialChain,
        tutorialIce,
        tutorialGrass,
        tutorialBoom,
        tutorialBee,
        tutorialClock;

    [Header("Pool")] [SerializeField] private Pool objPool;

    private GameObject HandGuild
    {
        get
        {
            if (_handGuild == null)
                _handGuild = objPool.SpawnGO(handGuidePrefab, Vector3.zero, new Quaternion(), transform);
            return _handGuild;
        }
    }
    
    private GameObject Arrow
    {
        get
        {
            if (_arrowGuild == null)
                _arrowGuild = objPool.SpawnGO(arrowPrefab, Vector3.zero, new Quaternion(), transform);
            return _arrowGuild;
        }
    }

    public Transform OldButton
    {
        get => _oldButton;
        set
        {
            if (value != null)
                _oldPositionButton = value.position;
            _oldButton = value;
        }
    }

    private Transform _parent;
    private Transform _oldButton;
    private Vector3 _oldPositionButton;

    private void Enable_UnMask(Vector3 movePosition)
    {
        tutorialMask.SetActive(true);
        unMask.DOMove(movePosition, 0).OnComplete(() => { unMask.gameObject.SetActive(true); });
    }

    private void Disable_UnMask()
    {
        unMask.gameObject.SetActive(false);
        tutorialMask.SetActive(false);
    }

    private void SetFadeMask(float value = 0.1f)
    {
        var color = MaskImage.color;
        if (color.a.Equals(value)) return;

        color.a = value;
        MaskImage.color = color;
    }

    public void SetDeltaSizeUnMask(Vector3 sizeDelta, float duration, Sprite sprite)
    {
        if (sprite != null)
            unMask.GetComponent<Image>().sprite = sprite;
        unMask.DOSizeDelta(new(sizeDelta.x - 8f, sizeDelta.y - 8f), duration);
    }

    public void SetPositionHandGuild_AndMask(Vector3 position)
    {
        SetPositionHandGuild(position);
        unMask.gameObject.SetActive(false);
        Enable_UnMask(position);
    }

    public void SetPositionHandGuild(Vector3 position)
    {
        unMask.gameObject.SetActive(false);
        if (HandGuild != null)
        {
            HandGuild.SetActive(true);
            HandGuild.transform.position = position;
        }
    }
    
    public void SetPositionArrowGuild(Vector3 position)
    {
        unMask.gameObject.SetActive(false);
        if (Arrow != null)
        {
            Arrow.SetActive(true);
            Arrow.transform.position = position;
        }
    }

    public void SetPositionHandGuild_AI(Vector3 position)
    {
        if (HandGuild.transform.localScale != Vector3.one)
            HandGuild.transform.localScale = Vector3.one;
        HandGuild.SetActive(true);
        HandGuild.transform.position = position;
    }

    public void SetPositionHandGuild_NextLevel(Vector3 position)
    {
        HandGuild.transform.localScale = Vector3.one * 1.4f;
        HandGuild.SetActive(true);
        HandGuild.transform.position = position;
    }

    private void RollbackParent(Transform child)
    {
        child.SetParent(_parent);
        child.position = _oldPositionButton;
        _parent = null;
    }

    public void RollbackOldButton()
    {
        if (OldButton == null) return;
        RollbackParent(OldButton);
        OldButton = null;
    }

    private void SetParent(Transform child, Transform parent)
    {
        _parent = child.parent;
        child.SetParent(parent);
    }

    #region Match3

    public void ShowTut_ClickTile()
    {
        SetFadeMask();
        tutorialMatch3.Enable();
        GameLevelManager.Instance.ShowTutClickTile_Match3_HandGuild();
    }

    public async void HideTut_ClickTile()
    {
        HideHandGuild();
        unMask.gameObject.SetActive(false);
        GameLevelManager.Instance.FinishTutorial();
        await tutorialMatch3.Disable();
        Disable_UnMask();
        Config.SetTut_ClickTile_Finished();
    }

    #endregion

    #region Glue
    public void ShowTut_ClickTileGlue()
    {
        tutorialMask.SetActive(true);
        SetFadeMask();
        tutorialGlue.Enable();
        GameLevelManager.Instance.ShowTutClickTileGlue_HandGuild();
    }

    public async void HideTut_ClickTileGlue()
    {
        HideHandGuild();
        unMask.gameObject.SetActive(false);
        await tutorialGlue.Disable();
        Disable_UnMask();
        Config.SetShowItemUnlockFinished(Config.ITEM_UNLOCK.GLUE);
        GameLevelManager.Instance.FinishTutorial();
    }
    #endregion
    
    #region tutorialChain
    public void ShowTut_ClickTileChain()
    {
        tutorialMask.SetActive(true);
        SetFadeMask();
        tutorialChain.Enable();
        GameLevelManager.Instance.ShowTutClickTileChain_HandGuild();
    }

    public async void HideTut_ClickTileChain()
    {
        HideHandGuild();
        unMask.gameObject.SetActive(false);
        await tutorialChain.Disable();
        GameLevelManager.Instance.FinishTutorial();
        Disable_UnMask();
        Config.SetShowItemUnlockFinished(Config.ITEM_UNLOCK.CHAIN);
    }
    #endregion
    
    #region tutorialIce
    public void ShowTut_ClickTileIce()
    {
        tutorialMask.SetActive(true);
        unMask.gameObject.SetActive(false);
        SetFadeMask();
        tutorialIce.Enable();
        GameLevelManager.Instance.SetItemTilesTut();
    }

    public async void HideTut_ClickTileIce()
    {
        HideHandGuild();
        GameLevelManager.Instance.FinishTutorial();
        await tutorialIce.Disable();
        Disable_UnMask();
        Config.SetShowItemUnlockFinished(Config.ITEM_UNLOCK.ICE);
    }
    #endregion
    
    #region tutorialGrass
    public void ShowTut_ClickTileGrass()
    {
        tutorialMask.SetActive(true);
        unMask.gameObject.SetActive(false);
        SetFadeMask();
        tutorialGrass.Enable();
        GameLevelManager.Instance.SetItemTilesTut();
    }

    public async void HideTut_ClickTileGrass()
    {
        HideHandGuild();
        await tutorialGrass.Disable();
        GameLevelManager.Instance.FinishTutorial();
        Disable_UnMask();
        Config.SetShowItemUnlockFinished(Config.ITEM_UNLOCK.GRASS);
    }
    #endregion 
    
    #region tutorialBoom
    public void ShowTut_ClickTileBoom()
    {
        tutorialMask.SetActive(true);
        unMask.gameObject.SetActive(false);
        SetFadeMask();
        tutorialBoom.Enable();
        GameLevelManager.Instance.SetItemTilesTut();
    }

    public async void HideTut_ClickTileBoom()
    {
        HideHandGuild();
        GameLevelManager.Instance.FinishTutorial();
        await tutorialBoom.Disable();
        Disable_UnMask();
        Config.SetShowItemUnlockFinished(Config.ITEM_UNLOCK.BOMB);
    }
    #endregion
    
    #region tutorialBee
    public void ShowTut_ClickTileBee()
    {
        tutorialMask.SetActive(true);
        unMask.gameObject.SetActive(false);
        SetFadeMask();
        tutorialBee.Enable();
        GameLevelManager.Instance.SetItemTilesTut();
    }

    public async void HideTut_ClickTileBee()
    {
        HideHandGuild();
        await tutorialBee.Disable();
        GameLevelManager.Instance.FinishTutorial();
        Disable_UnMask();
        Config.SetShowItemUnlockFinished(Config.ITEM_UNLOCK.BEE);
    }
    #endregion

    #region Combo

    [Header("Combo")] [SerializeField] private RectTransform combo;

    public void ShowTut_Combo()
    {
        SetFadeMask();
        tutorialCombo.Enable();

        ShowTut_Combo_HandGuild();
    }

    private void ShowTut_Combo_HandGuild()
    {
        tutorialMask.SetActive(true);
        SetParent(combo, tutorialCombo.transform);
        OldButton = combo;
        if (_handGuild != null)
            _handGuild.SetActive(false);
    }

    public async void HideTut_Combo()
    {
        HideHandGuild();
        RollbackParent(combo);
        unMask.gameObject.SetActive(false);
        await tutorialCombo.Disable();
        Disable_UnMask();
        Config.SetTut_Combo_Finished();
        Config.gameState = Config.GAME_STATE.PLAYING;
        GamePlayManager.Instance.SortingLayerCanvasPlaying();
    }

    #endregion

    #region Building

    [Header("Building")] [SerializeField] private RectTransform btnBuilding;


    public void ShowTut_ClickUpgrade_HandGuild(Transform button, bool isLast)
    {
        SetFadeMask(0f);
        RollbackOldButton();
        OldButton = button;
        if (isLast == false)
            SetParent(button.transform, tutorialBuilding.transform);
        SetPositionHandGuild(button.position);
    }

    public void HideBeeBuilding()
    {
        tutorialBuilding.HideCharacterBee();
    }

    private void ShowTut_ClickTown_HandGuild()
    {
        tutorialMask.SetActive(true);
        OldButton = btnBuilding;
        SetPositionHandGuild(btnBuilding.position);
        SetParent(btnBuilding.transform, tutorialBuilding.transform);
    }

    public void ShowTut_ClickTown()
    {
        tutorialBuilding.Enable();
        ShowTut_ClickTown_HandGuild();
    }

    public async void HideTut_ClickTown()
    {
        HideHandGuild();
        RollbackOldButton();
        await tutorialBuilding.Disable();
        Disable_UnMask();

        Config.SetTut_ClickTown_Finished();
    }

    public void ShowContent2Building()
    {
        tutorialBuilding.ShowContent2();
    }

    #endregion

    #region Undo

    [Header("Undo")] [SerializeField] private RectTransform btnUndo;

    private void ShowTut2_Undo_HandGuild()
    {
        SetFadeMask();
        tutorialMask.SetActive(true);
        OldButton = btnUndo;
        SetPositionArrowGuild(btnUndo.position);
        SetParent(btnUndo, tutorialUndo.transform);
    }

    public void ShowTut_Undo()
    {
        Config.IsShowTutUndo = true;
        tutorialUndo.Enable();
        ShowTut2_Undo_HandGuild();
    }


    public async void HideTut_Undo()
    {
        HideArrowGuild();
        RollbackParent(btnUndo);
        await tutorialUndo.Disable();
        Config.IsShowTutUndo = false;
        Disable_UnMask();

        Config.SetTut_2_Undo_Finished();
    }

    #endregion

    #region Suggest

    [Header("Suggest")] [SerializeField] private RectTransform btnSuggest;

    private void ShowTut_Suggest_HandGuild()
    {
        tutorialMask.SetActive(true);
        OldButton = btnSuggest;
        SetPositionArrowGuild(btnSuggest.position);
        SetParent(btnSuggest, tutorialSuggest.transform);
    }

    public void ShowTut_Suggest()
    {
        Config.isShowTut_suggest = true;
        tutorialSuggest.Enable();

        ShowTut_Suggest_HandGuild();
    }

    public async void HideTut_Suggest()
    {
        HideArrowGuild();
        RollbackParent(btnSuggest);
        await tutorialSuggest.Disable();
        Disable_UnMask();
        Config.isShowTut_suggest = false;

        Config.SetTut_Suggest_Finished();
    }

    #endregion

    #region Shuffle

    [Header("Shuffle")] [SerializeField] private RectTransform btnShuffle;

    private void ShowTut_Shuffle_HandGuild()
    {
        tutorialMask.SetActive(true);
        OldButton = btnSuggest;
        SetPositionArrowGuild(btnShuffle.position);
        SetParent(btnShuffle, tutorialShuffle.transform);
    }

    public void ShowTut_Shuffle()
    {
        Config.IsShowTutShuffle = true;
        tutorialShuffle.Enable();
        ShowTut_Shuffle_HandGuild();
    }

    public async void HideTut_Shuffle()
    {
        HideArrowGuild();
        RollbackParent(btnShuffle);
        await tutorialShuffle.Disable();
        Disable_UnMask();
        Config.IsShowTutShuffle = false;

        Config.SetTut_Shuffle_Finished();
    }

    #endregion

    #region ExtraSlot

    [Header("ExtraSlot")] [SerializeField] private RectTransform btnExtraSlot;

    private void ShowTut_AddSlot_HandGuild()
    {
        tutorialMask.SetActive(true);
        OldButton = btnExtraSlot;
        SetPositionArrowGuild(btnExtraSlot.position);
        SetParent(btnExtraSlot, tutorialExtraSlot.transform);
    }


    public void ShowTut_ExtraSlot()
    {
        Config.isShowTut5 = true;
        tutorialExtraSlot.Enable();

        ShowTut_AddSlot_HandGuild();
    }

    public async void HideTut_AddSlot()
    {
        HideArrowGuild();
        RollbackParent(btnExtraSlot);
        await tutorialExtraSlot.Disable();
        Disable_UnMask();
        Config.isShowTut5 = false;
        Config.SetTut_ExtraSlot_Finished();
        Config.gameState = Config.GAME_STATE.PLAYING;
        GamePlayManager.Instance.SortingLayerCanvasPlaying();
    }

    #endregion

    #region TileReturn

    [Header("TileReturn")] [SerializeField]
    private RectTransform btnTileReturn;

    private void ShowTut_TileReturn_HandGuild()
    {
        tutorialMask.SetActive(true);
        OldButton = btnTileReturn;
        SetPositionArrowGuild(btnTileReturn.position);
        SetParent(btnTileReturn, tutorialTileReturn.transform);
    }

    public void ShowTut_TileReturn()
    {
        Config.isShowTut_TileReturn = true;
        tutorialTileReturn.Enable();
        Config.gameState = Config.GAME_STATE.TUTORIAL;

        ShowTut_TileReturn_HandGuild();
    }

    public async void HideTut_TileReturn()
    {
        HideArrowGuild();
        RollbackParent(btnTileReturn);
        await tutorialTileReturn.Disable();
        Disable_UnMask();
        Config.isShowTut_TileReturn = false;
        Config.gameState = Config.GAME_STATE.PLAYING;
        GamePlayManager.Instance.SortingLayerCanvasPlaying();
        Config.SetTut_TileReturn_Finished();
    }

    #endregion

    #region Profile

    [Header("Profile")] [SerializeField] private RectTransform settingBtn;

    public void ShowTut_Profile()
    {
        Config.isShowTut_TileReturn = true;
        tutorialProfile.Enable();

        ShowTut_ClickMenu_HandGuild();
    }

    private void ShowTut_ClickMenu_HandGuild()
    {
        tutorialMask.SetActive(true);
        OldButton = settingBtn;
        SetPositionHandGuild(settingBtn.position);
        SetParent(settingBtn, tutorialProfile.transform);
    }

    public void HideCharacterBee()
    {
        tutorialProfile.HideCharacterBee();
    }

    public void ShowTut_ClickProfile_HandGuild(Transform button)
    {
        RollbackOldButton();
        OldButton = button;
        SetParent(button.transform, tutorialProfile.transform);
        SetPositionHandGuild(button.position);
    }

    public void HideTut_Profile()
    {
        HideHandGuild();
        RollbackOldButton();
        Disable_UnMask();
        Config.SetTut_Profile_Finished();
        tutorialProfile.Disable();
    }

    #endregion

    #region Leaderboard

    [Header("Leaderboard")] [SerializeField]
    private RectTransform leaderboardBtn;

    [SerializeField] private Transform leaderboardOsa;

    public void ShowTut_Leaderboard()
    {
        Config.isShowTut_TileReturn = true;
        tutorialLeaderboard.Enable();

        ShowTut_ClickLeaderboard_HandGuild();
    }

    private void ShowTut_ClickLeaderboard_HandGuild()
    {
        SetDeltaSizeUnMask(leaderboardBtn.GetChild(0).GetComponent<RectTransform>().sizeDelta, .2f,
            leaderboardBtn.GetChild(0).GetComponent<Image>().sprite);
        SetPositionHandGuild_AndMask(leaderboardBtn.GetChild(0).transform.position);
    }

    private Transform GetGiftBoxLeaderboard()
    {
        var content = leaderboardOsa.GetChild(0).GetChild(0);
        foreach (Transform item in content)
        {
            var leaderboard = item.GetComponent<LeaderBoardCard>();
            return leaderboard.giftBox.transform;
        }

        return null;
    }

    public async void ShowTut_ClickGiftBoxLeaderboard_HandGuild()
    {
        await tutorialGiftBoxLeaderboard.Disable();
        var giftBox = GetGiftBoxLeaderboard();
        SetDeltaSizeUnMask(giftBox.GetComponent<RectTransform>().sizeDelta, .2f, giftBox.GetComponent<Image>().sprite);
        SetPositionHandGuild_AndMask(giftBox.position);
    }

    public async void ShowTut_LeaderboardGiftBox()
    {
        HideHandGuild();
        unMask.gameObject.SetActive(false);
        //Disable_UnMask();
        await tutorialLeaderboard.Disable();
        tutorialGiftBoxLeaderboard.Enable();
    }

    public void HideTut_Leaderboard()
    {
        HideHandGuild();
        Disable_UnMask();

        Config.SetTut_Leaderboard_Finished();
    }

    #endregion

    #region Spin

    [Header("Spin")] [SerializeField] private RectTransform spinBtn;

    public void ShowTut_Spin()
    {
        Config.isShowTut_TileReturn = true;
        tutorialSpin.Enable();

        ShowTut_SpinBtn_HandGuild();
    }

    private void ShowTut_SpinBtn_HandGuild()
    {
        tutorialMask.SetActive(true);
        OldButton = spinBtn;
        SetPositionHandGuild(spinBtn.position);
        SetParent(spinBtn, tutorialSpin.transform);
    }

    public void ShowTut_ClickSpin_HandGuild(Transform rect)
    {
        tutorialSpin.HideCharacterBee();
        RollbackOldButton();
        OldButton = rect;
        SetPositionHandGuild(rect.position);
        SetParent(rect, tutorialSpin.transform);
    }

    public void HideTut_Spin()
    {
        HideHandGuild();
        Disable_UnMask();
        RollbackOldButton();
        Config.SetTut_Spin_Finished();
        tutorialSpin.Disable();
    }

    #endregion

    public void HideHandGuild()
    {
        if (_handGuild != null) objPool.Despawn(_handGuild);
    }
    
    public void HideArrowGuild()
    {
        if (_arrowGuild != null) objPool.Despawn(_arrowGuild);
    }
}