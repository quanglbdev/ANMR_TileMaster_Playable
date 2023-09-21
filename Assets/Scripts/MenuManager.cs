using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using HellTap.PoolKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using Spine.Unity;
using TMPro;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [Header("Canvas")] public Canvas canvas;

    //public SpriteRenderer background;
    [Space(10)] public BBUIButton btnSetting;
    public BBUIButton btnPlay, btnBuilding, btnHideBuilding;
    public BBUIButton btnLeaderboard, btnSpin, btnEvent, btnTheme, btnRemoveAds, btnFirstPack;
    public BBUIButton btnLevel;
    public TextMeshProUGUI currentWinStreakTxt;
    [Header("Menu")] public Menu_CoinGroup coinGroup;
    public Menu_HeartGroup heartGroup;
    public Menu_HammerGroup hammerGroup;
    public Menu_StarGroup starGroup;
    public GameObject lockGroup;
    public TextMeshProUGUI levelOfBtnPlay;
    [Header("Building")] public MapController mapController;
    public Transform buildingPosition;
    [Header("Position")] public Transform leftPos, rightPos, topPos, botPos;
    [Header("Panel")] public Transform leftPanel, rightPanel, topPanel, botPanel;
    [Header("HandGuide")] [SerializeField] private GameObject handGuide, handGuide2;
    [Header("Skeleton")] [SerializeField] private SkeletonGraphic leaderboardSkeletonGraphic;
    [SerializeField] private BackgroundController backgroundController;
    private bool _isShowNoAdsPack;

    private void Awake()
    {
        instance = this;
    }

    private async void Start()
    {
        _isShowNoAdsPack = false;
        btnSetting.OnPointerClickCallBack_Start.AddListener(TouchOpenGetMenuPopup);
        btnPlay.OnPointerClickCallBack_Start.AddListener(TouchPlay);
        btnBuilding.OnPointerClickCallBack_Start.AddListener(TouchBuilding);
        btnLevel.OnPointerClickCallBack_Start.AddListener(TouchLevel);
        btnHideBuilding.OnPointerClickCallBack_Start.AddListener(TouchHideBuilding);
        btnLeaderboard.OnPointerClickCallBack_Start.AddListener(TouchLoaderBoard);
        btnSpin.OnPointerClickCallBack_Start.AddListener(TouchSpin);
        btnEvent.OnPointerClickCallBack_Start.AddListener(TouchEvent);
        btnTheme.OnPointerClickCallBack_Start.AddListener(TouchTheme);
        btnRemoveAds.OnPointerClickCallBack_Start.AddListener(OpenRemoveAdsPopup);
        btnFirstPack.OnPointerClickCallBack_Start.AddListener(OpenFirstTimePackPopup);
        Config.gameState = Config.GAME_STATE.START;

        AdsManager.Instance.HideBanner();
        await InitExpensivePopup();

        if (Config.IS_SHOW_FIRST_TIME_PACK)
            EventDispatcher.Instance.RegisterListener(EventID.ExpiredFirstTimePack, (_) => ExpiredFirstTimePack());
        if (!Config.GetRemoveAd())
            EventDispatcher.Instance.RegisterListener(EventID.BuyNoAdsPack, (_) => BuyNoAds());
    }

    private void ActiveEvent()
    {
        foreach (var evt in Config.Events)
        {
            if (evt == Config.EVENT.WIN_STREAK &&
                Config.WIN_STREAK_EVENT_DATE - Config.GetDateTimeNow() > TimeSpan.Zero)
            {
                WinStreakManager.Instance.Active = true;
            }
            else
            {
                WinStreakManager.Instance.Active = false;
            }
        }
    }

    private void ExpiredFirstTimePack()
    {
        Config.IS_SHOW_FIRST_TIME_PACK = false;
        btnFirstPack.gameObject.SetActive(false);
    }

    private void BuyNoAds()
    {
        btnRemoveAds.gameObject.SetActive(false);
    }

    public void StartMenuManager()
    {
        Config.gameState = Config.GAME_STATE.START;
        if (Config.currLevel <= Config.LEVEL_UNLOCK_BUILDING)
        {
            Hide();
            return;
        }

        if (Config.IsEvent)
        {
            if (Config.currLevel >= Config.LEVEL_UNLOCK_EVENT
                && Config.WIN_STREAK_EVENT_DATE == new DateTime(2000, 1, 1))
            {
                Config.WIN_STREAK_EVENT_DATE = Config.GetDateTimeNow().AddDays(2);
                Config.WIN_STREAK_EVENT_DATE_NEXT = Config.GetDateTimeNow().AddDays(3);
            }

            if (Config.currLevel >= Config.LEVEL_UNLOCK_EVENT
                && Config.WIN_STREAK_EVENT_DATE != new DateTime(2000, 1, 1)
                && Config.WIN_STREAK_EVENT_DATE_NEXT - Config.GetDateTimeNow() < TimeSpan.Zero)

            {
                Config.WIN_STREAK_EVENT_DATE = Config.GetDateTimeNow().AddDays(2);
                Config.WIN_STREAK_EVENT_DATE_NEXT = Config.GetDateTimeNow().AddDays(3);
            }
            
            ActiveEvent();
        }
        
        UpdateBackground();
        canvas.enabled = true;
        ShowPanelButton();

        leaderboardSkeletonGraphic.freeze = true;
        spinSkeletonGraphic.freeze = true;
        levelOfBtnPlay.text = $"{Config.currLevel}";
        LoadMap();

        InitViews();

        btnHideBuilding.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(BtnHideBuilding_Hide);
    }

    private void BtnHideBuilding_Hide()
    {
        btnHideBuilding.gameObject.SetActive(false);
    }

    public async UniTask TransactionLoadBackground()
    {
        if (Config.currLevel == Config.LEVEL_SHOW_TUT_BUILDING)
        {
            GameDisplay.Instance.OpenLoadingPopup();
            await UniTask.Delay(1000);
            backgroundController.UpdateBackground();
            await UniTask.Delay(500);
            GameDisplay.Instance.CloseLoadingPopup();
        }
        else
        {
            backgroundController.UpdateBackground();
        }
    }

    private void TouchEvent()
    {
        GameDisplay.Instance.OpenWinStreakPopup();
    }

    private void TouchTheme()
    {
        OpenThemePopup();
    }

    #region AddHammer

    [Header("fxPool")] [SerializeField] private Pool fxPool;
    [Header("AddHammer")] [SerializeField] private Transform hammerPrefab;
    [SerializeField] private Transform hammerPosition;

    public void AddHammerAnim(Transform target = null)
    {
        if(Config.HammerAdd == 0) return;
        if (target == null)
            target = btnBuilding.transform;
        var hammer = fxPool.Spawn(hammerPrefab, hammerPosition.position, Quaternion.identity);
        Config.HammerAdd = 0;
        hammer.DOMove(target.position, 0.5f).SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                Config.SetHammer(Config.currHammer + 1);
                fxPool.Despawn(hammer);
            });
    }

    #endregion

    #region AddCoin

    [Header("AddCoin")] [SerializeField] private Transform coinPrefab;
    [SerializeField] private Transform coinPosition;
    [SerializeField] private Transform coinIcon;

    public void AddCoinAnim(int coinAmount, Transform target = null)
    {
        var delayTime = 1f / 60f;
        if (target == null)
            target = coinIcon.transform;

        var coinAdditional = coinAmount > 20 ? 20 : coinAmount;
        var spawnPosition = Vector3.zero;
        UpdateCoin(Config.currCoin + coinAmount);
        for (var i = 0; i < coinAdditional; i++)
        {
            DOVirtual.DelayedCall(i * delayTime, () =>
            {
                var coin = fxPool.Spawn(coinPrefab, new Vector3(
                    Random.Range(spawnPosition.x - 3f, spawnPosition.x + 3f),
                    Random.Range(spawnPosition.y - 3f, spawnPosition.y + 3f)), Quaternion.identity);
                coin.DOMove(target.position, 0.5f).SetEase(Ease.InCubic)
                    .OnComplete(() =>
                    {
                        Config.SetCoin(Config.currCoin + 1);
                        fxPool.Despawn(coin.gameObject);
                    });
            });
        }
    }

    private void UpdateCoin(int newCoinValue)
    {
        SoundManager.Instance.PlaySound_GetCoin();
        var currentCoinValue = Config.currCoin;
        DOTween.To(() => currentCoinValue, x => currentCoinValue = x, newCoinValue, 1f)
            .OnUpdate(() => { Config.SetCoin(currentCoinValue); })
            .OnComplete(() => { Config.SetCoin(newCoinValue); });
    }

    #endregion

    #region AddStar

    [SerializeField] private Transform starPrefab;
    [SerializeField] private Transform starIcon;

    public void AddStarAnim(int starAmount, Vector3 spawnPosition)
    {
        var delayTime = 1f / starAmount / 2f;
        var position = spawnPosition;
        SoundManager.Instance.PlaySound_GetStar();
        for (var i = 0; i < starAmount; i++)
        {
            DOVirtual.DelayedCall(i * delayTime, () =>
            {
                var star = fxPool.Spawn(starPrefab, new Vector3(Random.Range(position.x - 1f, position.x + 1f),
                    Random.Range(position.y - 1f, position.y + 1f)), Quaternion.identity);

                star.DOMove(starIcon.transform.position, 0.5f).SetEase(Ease.InCubic)
                    .OnComplete(() =>
                    {
                        DOTween.Kill(starIcon.gameObject);
                        starIcon.transform.DOPunchScale(Vector3.one * 0.05f, 0.2f, 10, 2f)
                            .SetEase(Ease.InOutBack)
                            .OnComplete(() =>
                            {
                                starIcon.transform.DOScale(Vector3.one, 0.1f);
                                Config.SetStar(Config.currStar + 1);
                            });
                        fxPool.Despawn(star.gameObject);
                    });
            });
        }
    }

    #endregion

    #region Spin

    [Header("SPIN")] [SerializeField] private TextMeshProUGUI _countDownSpin;
    [Header("SPIN")] [SerializeField] private TextMeshProUGUI _countDownSpinPopup;
    [Header("SkeletonGraphic - SPIN")] public SkeletonGraphic spinSkeletonGraphic;
    public AnimationReferenceAsset idleSpin, canSpin;

    private void TouchSpin()
    {
        GameDisplay.Instance.OpenSpinPopup();
    }

    private float _countDownLeaderboard;

    private void Update()
    {
        _countDownLeaderboard -= Time.deltaTime;
        UpdateSpin();
        if (Config.currLevel > Config.LEVEL_SHOW_TUT_LEADERBOARD)
        {
            if (_countDownLeaderboard < 0f)
            {
                leaderboardSkeletonGraphic.freeze = true;
                _countDownLeaderboard = 5f;
            }

            if (_countDownLeaderboard < leaderboardSkeletonGraphic.AnimationState.GetCurrent(0).Animation.Duration)
            {
                leaderboardSkeletonGraphic.freeze = false;
            }
        }

        if (Config.currLevel >= Config.LEVEL_SHOW_TUT_SPIN)
        {
            if (_countDownLeaderboard < 0f)
            {
                spinSkeletonGraphic.freeze = true;
                _countDownLeaderboard = 5f;
            }

            if (_countDownLeaderboard < spinSkeletonGraphic.AnimationState.GetCurrent(0).Animation.Duration)
            {
                spinSkeletonGraphic.freeze = false;
            }
        }
    }

    private readonly TimeSpan _timeSpanCheck = new(0, 0, 0);
    private Config.SPIN_STATE _spinState = Config.SPIN_STATE.IDLE;

    private void UpdateSpin()
    {
        var countDownTimeSpan = DateTime.Parse(Config.SPIN_LAST) - Config.GetDateTimeNow();
        if (countDownTimeSpan <= _timeSpanCheck)
        {
            if (_spinState == Config.SPIN_STATE.READY_SPIN) return;
            spinSkeletonGraphic.AnimationState.SetAnimation(0, canSpin, true);
            _countDownSpin.text = "SPIN";
            if (_countDownSpinPopup != null)
                _countDownSpinPopup.gameObject.SetActive(false);
            _spinState = Config.SPIN_STATE.READY_SPIN;
            return;
        }

        if (_spinState != Config.SPIN_STATE.IDLE)
            spinSkeletonGraphic.AnimationState.SetAnimation(0, idleSpin, true);

        _countDownSpin.text = countDownTimeSpan.Hours > 0
            ? $"{countDownTimeSpan.Hours:00}:{countDownTimeSpan.Minutes:00}:{countDownTimeSpan.Seconds:00}"
            : $"{countDownTimeSpan.Minutes:00}:{countDownTimeSpan.Seconds:00}";

        _spinState = Config.SPIN_STATE.IDLE;
        if (_countDownSpinPopup != null)
        {
            _countDownSpinPopup.gameObject.SetActive(true);

            _countDownSpinPopup.text = _countDownSpin.text;
        }
    }

    #endregion

    private void TouchLoaderBoard()
    {
        if (Config.CheckTutorial_Leaderboard())
        {
            TutorialManager.Instance.ShowTut_LeaderboardGiftBox();
        }

        OpenLeaderBoardPopup();
    }

    private void LoadMap()
    {
        if (mapController != null)
            Destroy(mapController.gameObject);
        var temp = Resources.Load<MapController>($"Building/map_{Config.MAP_INDEX}");
        mapController = Instantiate(temp, buildingPosition);
        Transform mapControllerTransform;
        (mapControllerTransform = mapController.transform).SetPositionAndRotation(buildingPosition.position,
            Quaternion.identity);
        //mapControllerTransform.localScale = Vector3.zero;
        mapControllerTransform.DOScale(1, 0f);
    }

    public async UniTask LoadNewMap()
    {
        LoadMap();
        backgroundController.UpdateBackground();
        await mapController.Init();
        mapController.ShowWhenOpen();
        mapController.HideButton();
        btnHideBuilding.GetComponent<BBUIView>().HideView();
        await UniTask.Delay(1000);
        GameDisplay.Instance.CloseLoadingPopup();
        await UniTask.Delay(400);
        ShowPanelButton();
    }

    private void TouchHideBuilding()
    {
        mapController.HideButton();
        btnHideBuilding.GetComponent<BBUIView>().HideView();
        ShowPanelButton();
    }

    private void TouchBuilding()
    {
        btnHideBuilding.GetComponent<BBUIView>().ShowView();
        mapController.ShowWhenOpen();
        backgroundController.SetBackgroundIndex();
        HidePanelButton();

        if (Config.CheckTutorial_Building())
        {
            TutorialManager.Instance.ShowContent2Building();
        }
    }

    public void SetBuyStarterPackSuccess()
    {
    }

    public void OpenShopCoin()
    {
        GameDisplay.Instance.OpenShop();
    }

    public void TouchPlay()
    {
        if (!Config.FREE_HEART)
        {
            if (Config.currHeart <= 0)
            {
                OpenMoreHeartPopup();

                return;
            }
        }

        HideView();
    }

    private void TouchLevel()
    {
        OpenPopupLevel();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(true);

        btnSetting.gameObject.SetActive(false);
        coinGroup.gameObject.SetActive(false);
        heartGroup.gameObject.SetActive(false);
        hammerGroup.gameObject.SetActive(false);
        starGroup.gameObject.SetActive(false);

        btnPlay.gameObject.SetActive(false);
        btnBuilding.gameObject.SetActive(false);
        btnHideBuilding.gameObject.SetActive(false);
        btnLevel.gameObject.SetActive(false);
        btnLeaderboard.gameObject.SetActive(false);
        btnSpin.gameObject.SetActive(false);
        btnEvent.gameObject.SetActive(false);
        btnTheme.gameObject.SetActive(false);
        btnRemoveAds.gameObject.SetActive(false);
        btnFirstPack.gameObject.SetActive(false);

        hammerGroup.transform.localScale = Vector3.zero;
        hammerGroup.gameObject.SetActive(true);
        mapController.Init();

        if (Config.currLevel <= Config.LEVEL_UNLOCK_BUILDING)
        {
            btnBuilding.Interactable = false;
            btnBuilding.transform.GetChild(0).gameObject.SetActive(false);
            btnBuilding.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            btnBuilding.Interactable = true;
            btnBuilding.transform.GetChild(0).gameObject.SetActive(true);
            btnBuilding.transform.GetChild(1).gameObject.SetActive(false);
        }

        InitViews_ShowView();
    }

    private bool _showEvent;

    private void InitViews_ShowView()
    {
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            SoundManager.Instance.PlaySound_ShowView();
            btnSetting.gameObject.SetActive(true);
            btnSetting.GetComponent<BBUIView>().ShowView();

            coinGroup.gameObject.SetActive(true);
            coinGroup.GetComponent<BBUIView>().ShowView();

            heartGroup.gameObject.SetActive(true);
            heartGroup.GetComponent<BBUIView>().ShowView();

            starGroup.gameObject.SetActive(true);
            starGroup.GetComponent<BBUIView>().ShowView();

            heartGroup.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.1f, () =>
        {
            _showEvent = false;
            if (Config.currLevel >= Config.LEVEL_UNLOCK_EVENT)
            {
                if (WinStreakManager.Instance.Active)
                {
                    currentWinStreakTxt.text = $"{Config.WIN_STREAK_INDEX}";
                    var nextStep = Config.WIN_STREAK_INDEX <= 2 ? 2 : 3;
                    if (WinPopup.Instance != null)
                    {
                        if (WinPopup.Instance.isAddWinStreak)
                        {
                            btnEvent.GetComponent<BBUIViewButton>().ShowViewAndAddWinStreak();
                            WinPopup.Instance.isAddWinStreak = false;
                            if (Config.WIN_STREAK_INDEX >= Config.WIN_STREAK_OLD_INDEX + nextStep)
                            {
                                _showEvent = true;
                            }
                        }
                        else
                        {
                            if (Config.GetShowWinStreak() == false)
                            {
                                btnEvent.GetComponent<BBUIViewButton>().ShowViewAndOpenPopup();
                                Config.SetShowWinStreak(true);
                            }
                        }
                    }
                    else if (Config.WIN_STREAK_INDEX >= Config.WIN_STREAK_OLD_INDEX + nextStep)
                    {
                        _showEvent = true;
                        btnEvent.GetComponent<BBUIViewButton>().ShowViewAndAddWinStreak();
                        WinPopup.Instance.isAddWinStreak = false;
                    }
                    else
                    {
                        btnEvent.GetComponent<BBUIViewButton>().ShowView();
                    }
                }
                // else
                // {
                //     btnEvent.gameObject.SetActive(true);
                //     btnEvent.GetComponent<BBUIViewButton>().ShowView();
                // }
            }
        });


        sequenceShowView.InsertCallback(0.1f, () =>
        {
            if (Config.currLevel >= Config.LEVEL_SHOW_TUT_SPIN)
            {
                btnSpin.gameObject.SetActive(true);
                btnSpin.GetComponent<BBUIView>().ShowView();
            }
        });

        sequenceShowView.InsertCallback(0.1f, () =>
        {
            if (!Config.GetRemoveAd())
            {
                btnRemoveAds.gameObject.SetActive(true);
                btnRemoveAds.GetComponent<BBUIView>().ShowView();
            }
        });
        sequenceShowView.InsertCallback(0.1f, () =>
        {
            if (!Config.GetBuyIAP(Config.IAP_ID.first_time_pack) && Config.IS_SHOW_FIRST_TIME_PACK)
            {
                btnFirstPack.gameObject.SetActive(true);
                btnFirstPack.GetComponent<BBUIView>().ShowView();
            }
        });


        sequenceShowView.InsertCallback(0.1f, () =>
        {
            btnLevel.gameObject.SetActive(true);
            btnLevel.GetComponent<BBUIView>().ShowView();

            // btnTheme.gameObject.SetActive(true);
            // btnTheme.GetComponent<BBUIView>().ShowView();

            if (Config.currLevel >= Config.LEVEL_SHOW_TUT_LEADERBOARD)
            {
                btnLeaderboard.gameObject.SetActive(true);
                btnLeaderboard.GetComponent<BBUIView>().ShowView();
            }

            btnPlay.gameObject.SetActive(true);
            btnPlay.GetComponent<BBUIView>().ShowView();

            btnBuilding.gameObject.SetActive(true);
            btnBuilding.GetComponent<BBUIView>().ShowView();

            btnHideBuilding.gameObject.SetActive(true);
            btnHideBuilding.transform.localScale = Vector3.zero;
        });

        sequenceShowView.InsertCallback(0.3f, () => { mapController.Show(); });

        sequenceShowView.InsertCallback(0.7f, () =>
        {
            if (Config.currLevel > 3
                && Config.currLevel != Config.LEVEL_SHOW_TUT_BUILDING
                && Config.currLevel != Config.LEVEL_SHOW_TUT_SPIN
                && Config.currLevel != Config.LEVEL_SHOW_TUT_PROFILE
                && Config.currLevel != Config.LEVEL_SHOW_TUT_LEADERBOARD
                && _showEvent == false)
            {
                if (Config.GetDateTimeNow().Date > Config.DAILY_REWARD_TIME.Date && Config.currLevel >= 4)
                {
                    OpenDailyRewardPanel();
                    return;
                }

                if (!Config.GetBuyIAP(Config.IAP_ID.first_time_pack) && Config.currLevel > 15)
                {
                    if (Config.IS_SHOW_FIRST_TIME_PACK && Config.IS_SHOW_FIRST_TIME)
                    {
                        Config.IS_SHOW_FIRST_TIME = false;
                        OpenFirstTimePackPopup();
                        return;
                    }
                }

                if (!Config.GetRemoveAd() && Config.currLevel > 15 && _isShowNoAdsPack == false)
                {
                    _isShowNoAdsPack = true;
                    OpenRemoveAdsPopup();
                }
            }
        });

        sequenceShowView.OnComplete(() =>
        {
            lockGroup.gameObject.SetActive(false);

            if (Config.CheckTutorial_Building())
            {
                TutorialManager.Instance.ShowTut_ClickTown();
            }

            if (Config.CheckTutorial_Profile())
            {
                TutorialManager.Instance.ShowTut_Profile();
            }

            if (Config.CheckTutorial_Leaderboard())
            {
                TutorialManager.Instance.ShowTut_Leaderboard();
            }

            if (Config.CheckTutorial_Spin())
            {
                TutorialManager.Instance.ShowTut_Spin();
            }

            if (Config.IsShowRewardWhenHowHome)
            {
                Config.IsShowRewardWhenHowHome = false;
                AddHammerAnim();
                AddCoinAnim(Config.CoinAdd);
            }
        });
    }

    public void SetActiveLockGroup(bool isActive)
    {
        lockGroup.gameObject.SetActive(isActive);
    }

    private void Hide()
    {
        SoundManager.Instance.PlaySound_HideView();
        lockGroup.gameObject.SetActive(true);
        canvas.enabled = false;
        GamePlayManager.Instance.StartGame();
    }

    private void HideView()
    {
        SoundManager.Instance.PlaySound_HideView();
        lockGroup.gameObject.SetActive(true);
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            btnSetting.GetComponent<BBUIView>().HideView();
            coinGroup.GetComponent<BBUIView>().HideView();
            btnPlay.GetComponent<BBUIView>().HideView();
            btnLevel.GetComponent<BBUIView>().HideView();
            btnLeaderboard.GetComponent<BBUIView>().HideView();
            btnEvent.GetComponent<BBUIView>().HideView();
            btnSpin.GetComponent<BBUIView>().HideView();
            HidePanelButton();
            HideTopPanel();
            mapController.Hide();
        });

        DOVirtual.DelayedCall(0.3f, () =>
        {
            canvas.enabled = false;
            if (!Config.FREE_HEART)
            {
                Config.SetHeart(Config.currHeart - Config.HEART_REQUIRE_TO_PLAY);
            }

            backgroundController.SetBackgroundSelected();
            backgroundController.BlurMaterial();
            GamePlayManager.Instance.StartGame();
        });
    }


    #region LEVELS

    [Header("LEVELS")] public LevelPopup levelPopup;

    private void OpenPopupLevel()
    {
        levelPopup.ShowLevelPopup();
    }

    private async UniTask InitExpensivePopup()
    {
        GameDisplay.Instance.ShowLoadingPopup();
        OpenLeaderBoardPopup();
        levelPopup.ShowLevelPopup();
        await UniTask.Delay(1000);
        levelPopup.Hide();
        leaderBoardPopup.Hide();
        UpdateBackground();
        await UniTask.Delay(500);
        GameDisplay.Instance.CloseLoadingPopup();
        await UniTask.DelayFrame(1);
        StartMenuManager();
    }

    public void UpdateBackground()
    {
        backgroundController.UpdateBackground();
    }

    #endregion

    #region MENU_POPUP

    [Header("MENU_POPUP")] public MenuGamePopup menuGamePopup;

    private void TouchOpenGetMenuPopup()
    {
        menuGamePopup.OpenGetMenuPopup();
    }

    #endregion

    [Header("LeaderBoardInfoPopup")] public LeaderBoardInfoPopup leaderBoardInfoPopup;

    public void OpenLeaderBoardInfoPopup()
    {
        leaderBoardInfoPopup.OpenLeaderBoardInfoPopup();
    }

    [Header("LeaderBoardPopup")] public LeaderBoardPopup leaderBoardPopup;

    public void OpenLeaderBoardPopup()
    {
        leaderBoardPopup.OpenLeaderBoardPopup();
    }

    [Header("GetHammerPopup")] public GetHammerPopup getHammerPopup;

    public void OpenGetHammerPopup()
    {
        getHammerPopup.OpenGetHammerPopup();
    }

    [Header("ProfilePopup")] public ProfilePopup profilePopup;

    public void OpenProfilePopup()
    {
        profilePopup.OpenProfilePopup();
    }

    [FormerlySerializedAs("backgroundPopup")] [Header("ThemePopup")]
    public ThemePopup themePopup;

    private void OpenThemePopup()
    {
        themePopup.OpenThemePopup();
    }

    [Header("ShowFullHeartPopup")] public ShowFullHeartPopup showFullHeartPopup;

    public void OpenShowFullHeartPopup()
    {
        showFullHeartPopup.OpenShowFullHeartPopup();
    }

    [Header("RemoveAdsPopup")] public RemoveAdsPopup removeAdsPopup;

    private void OpenRemoveAdsPopup()
    {
        removeAdsPopup.ShowRemoveAdsPopup();
    }

    [Header("FirstTimePackPopup")] public FirstTimePackPopup firstTimePackPopup;

    private void OpenFirstTimePackPopup()
    {
        firstTimePackPopup.ShowFirstTimePack();
    }

    [Header("MoreHeartPopup")] public MoreHeartPopup moreHeartPopup;

    public void OpenMoreHeartPopup()
    {
        GameDisplay.Instance.OpenMoreHeartPopup();
    }

    [Header("ShowFreeHeartPopup")] public ShowFreeHeartPopup _showFreeHeartPopup;

    public void OpenShowFreeHeartPopup()
    {
        _showFreeHeartPopup.OpenShowFreeHeartPopup();
    }

    [Header("DailyRewardPanel")] public DailyRewardPanel dailyRewardPanel;

    public void OpenDailyRewardPanel()
    {
        dailyRewardPanel.ShowDailyReward();
    }

    public void SetNextLiveTextMoreHeartPopup(string text)
    {
        moreHeartPopup.text.text = text;
    }

    public void SetCountHeartTextMoreHeartPopup(string text)
    {
        moreHeartPopup.heartCountText.text = text;
    }

    [Button("Reset SCENE")]
    public void TouchResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [Button]
    public void CheatHammer(int hammer = 30)
    {
        Config.SetHammer(hammer);
    }

    private void HidePanelButton()
    {
        var leftPosition = leftPanel.position;
        leftPanel.DOMove(new(leftPosition.x - 5f, leftPosition.y, leftPosition.z), 0.3f);

        var rightPosition = rightPanel.position;
        rightPanel.DOMove(new(rightPosition.x + 5f, rightPosition.y, rightPosition.z), 0.3f);

        var botPosition = botPanel.position;
        botPanel.DOMove(new(botPosition.x, botPosition.y - 15f, botPosition.z), 0.3f);

        hammerGroup.transform.DOScale(1f, 0.3f);
        heartGroup.transform.DOScale(0f, 0.3f);
    }

    private void HideTopPanel()
    {
        var topPosition = topPanel.position;
        topPanel.DOMove(new(topPosition.x, topPosition.y + 10, topPosition.z), 0.3f);
    }

    private void ShowPanelButton()
    {
        leftPanel.DOMove(leftPos.position, 0.3f).SetEase(Ease.InOutSine);

        rightPanel.DOMove(rightPos.position, 0.3f).SetEase(Ease.InOutSine);

        botPanel.DOMove(botPos.position, 0.2f).SetEase(Ease.InOutSine);

        topPanel.DOMove(topPos.position, 0.3f).SetEase(Ease.InOutSine);

        hammerGroup.transform.DOScale(0, 0.3f);
        heartGroup.transform.DOScale(1, 0.3f);
    }
}