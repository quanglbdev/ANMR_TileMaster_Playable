using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using TMPro;
using UnityEngine.Events;

public class WinPopup : MonoBehaviour
{
    public static WinPopup Instance;
    [Header("Button")] public Button btnReward, btnNext, btnNextLevel;
    public TextMeshProUGUI txtNextLevel;

    [Header("Popup")] public GameObject popup, popupStarChest;
    public TextMeshProUGUI txtRewardCoin;
    public GameObject lockGroup;
    public Image bgPopup;

    [Header("Reward")] public Image reward;
    public TextMeshProUGUI txtRewardAmount;
    public Sprite sprite_coin;
    public GameObject coin;

    [Header("Win Streak")] public GameObject trophy;
    public TextMeshProUGUI txtWinStreak;

    [Header("Reward Bar")] public GameObject coinGroup, hammerGroup, starGroup;

    [Header("SkeletonGraphic")] public SkeletonGraphic hammerSkeletonGraphic;
    [Header("Particle")] public GameObject particle;

    public AnimationReferenceAsset intro, loop;
    [Header("Star")] public List<Image> stars;

    public Sprite star_On, star_Off;

    [Header("nextItem")] public Image nextItem;
    [Header("nextItem")] public GameObject nextItemGroup;
    [Header("Chest Process")] public GameObject chestProcess;

    [Header("SkeletonGraphic - Title")] public SkeletonGraphic title;
    public AnimationReferenceAsset introText, outroText, idleText;

    [Header("UIParticle")] [SerializeField]
    private UIParticle processFX;

    [Header("SkeletonGraphic - StarChest")] [SerializeField]
    private SkeletonGraphic starChestSkeleton;

    [SerializeField] private Transform chestPosition;

    [SerializeField] private Win_StarChestPopup winStarChestPopup;
    public Transform ChestPosition => chestPosition;

    public bool isAddWinStreak;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        btnReward.onClick.AddListener(TouchXReward);
        btnNext.onClick.AddListener(() => { TouchNext(); });
        btnNextLevel.onClick.AddListener(() => { TouchNext(); });

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }

    private void HideView()
    {
        GamePlayManager.Instance.HideTut_NextLevel();
        title.AnimationState.AddAnimation(0, outroText, false, 0);

        var sequenceHideView = DOTween.Sequence();
        sequenceHideView.InsertCallback(outroText.Animation.Duration + .2f, () =>
        {
            lockGroup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().HideView();
            reward.transform.DOScale(Vector3.one * 0.3f, 0.2f);
        });
    }

    private void HidePopup_Finished()
    {
        gameObject.SetActive(false);
        if (!_isUnlockItem)
            HideFinished();
        else
            _unlockItemCallback?.Invoke();
    }

    public void HideFinished()
    {
        GamePlayManager.Instance.HideViewWhenWinGame();
    }

    public void BuyRemoveAd(ConfigPackData configPackData)
    {
        lockGroup.gameObject.SetActive(true);

        PurchaserManager.instance.BuyConsumable(configPackData.idPack,
            (string _iapID, PurchaserManager.IAP_CALLBACK_STATE _state) =>
            {
                if (_state == PurchaserManager.IAP_CALLBACK_STATE.SUCCESS)
                {
                    lockGroup.gameObject.SetActive(false);
                    if (_iapID.Equals(Config.IAP_ID.removead.ToString()))
                    {
                        Config.SetRemoveAd();
                        NotificationPopup.instance.AddNotification("RemoveAd Success!");
                    }
                }
                else
                {
                    lockGroup.gameObject.SetActive(false);
                    NotificationPopup.instance.AddNotification("Buy Fail!");
                }
            });
    }


    private async UniTask TouchNext()
    {
        SoundManager.Instance.PlaySound_Click();
        lockGroup.gameObject.SetActive(true);
        HideView();

        if (_isLockHome)
        {
            await UniTask.Delay(500);
            if (!_hasPassLevel)
            {
                MenuManager.instance.AddHammerAnim(hammerGroup.transform.GetChild(1));
            }

            MenuManager.instance.AddCoinAnim(_coinValue, coinGroup.transform.GetChild(1));
        }
        else
        {
            Config.IsShowRewardWhenHowHome = true;
            Config.HammerAdd = !_hasPassLevel ? 1 : 0;
            Config.CoinAdd = _coinValue;
        }
    }

    private void TouchXReward()
    {
        SoundManager.Instance.PlaySound_Click();
        lockGroup.gameObject.SetActive(true);
        AdsManager.Instance.ShowRewardAd_CallBack(FirebaseManager.RewardFor.X3RewardWin, callbackState =>
            {
                lockGroup.gameObject.SetActive(false);
                btnReward.interactable = false;
                xReward_Finished();
            },
            () => { lockGroup.gameObject.SetActive(false); },
            () => { lockGroup.gameObject.SetActive(false); });
    }

    private void xReward_Finished()
    {
        txtRewardCoin.text = $"{3 * _coinValue}";
        PlayerPrefs.SetInt(Config.COIN, Config.currCoin + _coinValue * 2);
        FirebaseManager.Instance.LogEarnVirtualCoin(_coinValue, "x3_reward_win");
        txtRewardCoin.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 2f, 5, 1).SetEase(Ease.InQuart);
        _coinValue *= 3;
    }

    private int _countStar, _coinValue;
    private int _addStar, _thisLevel;
    private int _hammerAdd;
    private bool _hasPassLevel;
    private bool _isLockHome;
    private bool _isUnlockItem;
    private bool _isNextLevel;

    private UnityAction _unlockItemCallback;

    public void ShowWinPopup(int level, int countStar, int coinValue)
    {
        SoundManager.Instance.PlaySound_Win();
        _isLockHome = level <= 3;
        for (var i = 0; i < stars.Count; i++)
        {
            var star = stars[i];
            star.sprite = (i + 1) <= countStar ? star_On : star_Off;
        }

        _thisLevel = level;
        _isNextLevel = Config.currLevel <= 2;
        txtNextLevel.text = _isNextLevel ? $"<size=48>Play Level {level + 1}" : "Continue";

        coinGroup.SetActive(true);
        hammerGroup.SetActive(true);
        starGroup.SetActive(true);
        Config.interstitialAd_countWin++;
        _hasPassLevel = Config.currSelectLevel != Config.currLevel;
        _coinValue = _hasPassLevel ? 1 : coinValue;

        _countStar = countStar;
        particle.SetActive(false);

        if (!_hasPassLevel)
            PlayAnimHammer();
        _hammerAdd = _hasPassLevel ? 0 : 1;
        hammerSkeletonGraphic.gameObject.SetActive(false);

        reward.gameObject.SetActive(_hasPassLevel);
        reward.transform.localScale = Vector3.one;
        reward.sprite = sprite_coin;

        coin.SetActive(!_hasPassLevel);
        trophy.SetActive(!_hasPassLevel && WinStreakManager.Instance.Active);
        txtRewardCoin.gameObject.SetActive(!_hasPassLevel);

        if (!_hasPassLevel && WinStreakManager.Instance.Active && Config.currLevel >= 5)
        {
            Config.WIN_STREAK_INDEX++;
            if (Config.WIN_STREAK_INDEX == Config.WIN_STREAK_NEXT_INDEX)
            {
                Config.WIN_STREAK_NEXT_INDEX += 3;
            }

            isAddWinStreak = true;
        }

        txtRewardCoin.text = $"{_coinValue}";
        txtRewardAmount.text = $"{_coinValue}";
        txtWinStreak.text = $"{Config.WIN_STREAK_INDEX}";
        gameObject.SetActive(true);

        PlayerPrefs.SetInt(Config.COIN, Config.currCoin + coinValue);
        FirebaseManager.Instance.LogEarnVirtualCoin(_coinValue, "reward_win");
        PlayerPrefs.SetInt(Config.HAMMER_KEY, Config.currHammer + _hammerAdd);
        PlayerPrefs.Save();

        Config.SetHeart(Config.currHeart + Config.HEART_REQUIRE_TO_PLAY);

        _addStar = _countStar - Config.LevelStar(Config.currSelectLevel);
        if (_addStar < 0) _addStar = 0;

        Config.SetLevelStar(level, countStar);
        GamePlayManager.Instance.HideView(true);
        InitViews();
    }

    private void OnAnimProcess()
    {
        processFX.gameObject.SetActive(true);
        processFX.Play();
    }

    private async UniTask PlayAnimHammer()
    {
        if (hammerSkeletonGraphic.AnimationState != null)
            hammerSkeletonGraphic.AnimationState.SetAnimation(0, intro, false);
        await UniTask.Delay((int)((introText.Animation.Duration - 0.3f) * 1000));
        particle.SetActive(true);
        await UniTask.Delay((int)(0.3f * 1000));
        if (hammerSkeletonGraphic.AnimationState != null)
            hammerSkeletonGraphic.AnimationState.SetAnimation(0, loop, true);
    }

    private async UniTask PlayAnimText()
    {
        title.AnimationState.SetAnimation(0, introText, false);
        await UniTask.Delay((int)(introText.Animation.Duration * 1000));
        title.AnimationState.SetAnimation(0, idleText, true);
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(true);
        popup.gameObject.SetActive(false);
        popupStarChest.gameObject.SetActive(false);
        btnNext.gameObject.SetActive(false);
        btnNextLevel.gameObject.SetActive(false);
        btnReward.gameObject.SetActive(false);
        processFX.Clear();
        DOTween.Kill(btnReward.transform);

        if (_thisLevel == 1)
            btnNext.interactable = false;

        foreach (var star in stars)
        {
            star.gameObject.SetActive(false);
            DOTween.Kill(star.transform);
        }

        chestProcess.SetActive(Config.currLevel >= Config.LEVEL_UNLOCK_STAR_CHEST + 1);
        InitViews_ShowView();

        InitChestStar();
    }

    Sequence sequenceShowView;

    private void InitViews_ShowView()
    {
        sequenceShowView?.Kill();

        sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();

            if (_thisLevel != 1)
            {
                btnNext.interactable = true;
            }
        });


        sequenceShowView.InsertCallback(0.2f, () =>
        {
            if (_countStar >= 1)
            {
                SoundManager.Instance.PlaySound_WinStarPop();
            }

            stars[0].gameObject.SetActive(true);
            stars[0].GetComponent<BBUIView>().ShowView();

            if (!_hasPassLevel)
            {
                hammerSkeletonGraphic.gameObject.SetActive(true);
            }
            else
            {
                reward.gameObject.SetActive(true);
                reward.GetComponent<BBUIView>().ShowView();
            }
        });

        sequenceShowView.InsertCallback(0.3f, () =>
        {
            if (_countStar >= 2)
            {
                SoundManager.Instance.PlaySound_WinStarPop();
            }

            stars[1].gameObject.SetActive(true);
            stars[1].GetComponent<BBUIView>().ShowView();

            if (_isNextLevel)
                btnNextLevel.gameObject.SetActive(true);
            else
                btnNext.gameObject.SetActive(true);

            if (_thisLevel >= Config.LEVEL_UNLOCK_STAR_CHEST + 1)
            {
                btnReward.gameObject.SetActive(true);
                btnReward.interactable = true;
            }

            PlayAnimText();

            if (!_hasPassLevel)
            {
                OnAnimProcess();
            }
        });

        sequenceShowView.InsertCallback(0.5f, () =>
        {
            if (_countStar >= 3)
            {
                SoundManager.Instance.PlaySound_WinStarPop();
            }

            stars[2].gameObject.SetActive(true);
            stars[2].GetComponent<BBUIView>().ShowView();
        });


        sequenceShowView.InsertCallback(1f, () =>
        {
            if (_thisLevel == 1)
            {
                btnNext.interactable = true;
                GamePlayManager.Instance.ShowTut_NextLevel(btnNextLevel.transform.position);
            }
        });

        sequenceShowView.OnComplete(ShowViewFinished);
    }

    private void ShowViewFinished()
    {
    }

    #region CHEST

    [Header("Chest")] public TextMeshProUGUI txtChestCountStar;

    public Image imgChestProgress;
    public TextMeshProUGUI txtNewItemCount;
    public Image imgNewItemProgress;

    private void InitChestStar()
    {
        if (Config.currLevel > 3)
        {
            nextItemGroup.SetActive(false);
        }
        else
        {
            var nextLevelDefinition = AssetManager.Instance.GetItemUnlockDefinition(3);
            imgNewItemProgress.fillAmount = (Config.currLevel - 1) * 1f / nextLevelDefinition.unlockLevel;
            txtNewItemCount.text = $"{Config.currLevel - 1}/{nextLevelDefinition.unlockLevel}";

            nextItem.sprite = nextLevelDefinition.sprite;
            nextItemGroup.SetActive(true);
        }

        if (starChestSkeleton.AnimationState != null)
            starChestSkeleton.AnimationState.SetEmptyAnimation(0, 0);
        starChestSkeleton.transform.localPosition = Vector3.zero;

        var starCount = Config.GetChestCountStar();
        if (starCount > Config.STEP_STAR_CHEST)
            starCount = Config.STEP_STAR_CHEST;

        txtChestCountStar.text = $"{starCount}/{Config.STEP_STAR_CHEST}";
        imgChestProgress.fillAmount = starCount * 1f / (Config.STEP_STAR_CHEST);

        StartCoroutine(AddChestStar_Finished());
    }

    private IEnumerator AddChestStar_Finished()
    {
        yield return new WaitForSeconds(1f);
        var currLevel = Config.currLevel;
        _isUnlockItem = false;

        if (currLevel > Config.LEVEL_UNLOCK_STAR_CHEST)
        {
            var stepStarGetReward = Config.STEP_STAR_CHEST;
            Config.SetChestCountStar(Config.GetChestCountStar() + _addStar);
            var starCount = Config.GetChestCountStar();

            txtChestCountStar.text = $"{starCount}/{stepStarGetReward}";
            imgChestProgress.DOFillAmount(starCount * 1f / stepStarGetReward, 0.5f).OnComplete(() =>
            {
                if (stepStarGetReward <= starCount)
                {
                    winStarChestPopup.Show(starChestSkeleton);
                    starChestSkeleton.AnimationState.SetAnimation(0, winStarChestPopup.PrepareChest,true);
                }
            });
            
        }

        //Check unlock new item
        if (currLevel - 1 <= AssetManager.Instance.GetLastItemUnlock())
        {
            var nextLevelDefinition = AssetManager.Instance.GetItemUnlockDefinition(Config.GetUnLockLevel());
            var unlockLevel = nextLevelDefinition.unlockLevel;
            var unlockShow = nextLevelDefinition.unlockShow;

            imgNewItemProgress.DOFillAmount((currLevel - 1) * 1f / unlockLevel, 0.5f);
            txtNewItemCount.text = $"{currLevel - 1}/{unlockLevel}";

            if ((currLevel - 1) == unlockLevel)
            {
                if (unlockShow == Config.UNLOCK_SHOW.WIN)
                {
                    _isUnlockItem = true;
                    _unlockItemCallback = () =>
                    {
                        GamePlayManager.Instance.OpenUnLockNewItem(nextLevelDefinition, null);
                    };
                    var nextLevel = AssetManager.Instance.GetCurrentItemUnlock();
                    if (nextLevel != null)
                        Config.SetUnLockLevel(10);
                }
            }
        }

        lockGroup.gameObject.SetActive(false);
    }

    #endregion


    public void ResetChestProcess()
    {
        imgChestProgress.fillAmount = 0f;
        txtChestCountStar.text = "0/18";
    }
    public void SetEnableNativeAd()
    {
        bgPopup.raycastTarget = false;
    }

    [Button]
    public void OpenChest()
    {
        winStarChestPopup.Show(starChestSkeleton);
        starChestSkeleton.AnimationState.SetAnimation(0, winStarChestPopup.PrepareChest,true);
    }
}