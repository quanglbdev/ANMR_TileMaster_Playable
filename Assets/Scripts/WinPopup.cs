using System.Collections.Generic;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine.Events;

public class WinPopup : MonoBehaviour
{
    [Header("Button")] public Button btnReward, btnNext;

    [Header("Popup")] public GameObject popup;
    public GameObject lockGroup;
    public Image bgPopup;

    public TextMeshProUGUI txtRewardAmount;
    public Sprite sprite_coin;
    public GameObject coin;


    [Header("Reward Bar")] public GameObject coinGroup, hammerGroup, starGroup;

    [Header("SkeletonGraphic")] public SkeletonGraphic hammerSkeletonGraphic;
    [Header("Particle")] public GameObject particle;

    public AnimationReferenceAsset intro, loop;
    [Header("Star")] public List<Image> stars;

    public Sprite star_On, star_Off;

    [Header("Chest Process")] public GameObject chestProcess;

    [Header("SkeletonGraphic - Title")] public SkeletonGraphic title;
    public AnimationReferenceAsset introText, outroText, idleText;

    [Header("UIParticle")] [SerializeField]
    private UIParticle processFX;

    private void Start()
    {
        //btnReward.onClick.AddListener(TouchXReward);
        btnNext.onClick.AddListener(() => { TouchNext(); });

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }

    private void HideView()
    {
        title.AnimationState.AddAnimation(0, outroText, false, 0);

        var sequenceHideView = DOTween.Sequence();
        sequenceHideView.InsertCallback(outroText.Animation.Duration + .2f, () =>
        {
            lockGroup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().HideView();
        });
        sequenceHideView.OnComplete(() =>
        {
            GamePlayManager.Instance.HideView();
        });
    }

    private void HidePopup_Finished()
    {
        gameObject.SetActive(false);
        HideFinished();
    }

    private void HideFinished()
    {
        GamePlayManager.Instance.HideViewWhenWinGame();
    }

    private void TouchNext()
    {
        SoundManager.Instance.PlaySound_Click();
        lockGroup.gameObject.SetActive(true);
        HideView();
    }

    private void TouchXReward()
    {
        SoundManager.Instance.PlaySound_Click();
        lockGroup.gameObject.SetActive(true);
    }

    private int _countStar, _coinValue;
    private int _addStar, _thisLevel;
    private int _hammerAdd;

    private UnityAction _unlockItemCallback;

    public void ShowWinPopup(int level, int countStar)
    {
        _countStar = countStar;
        _thisLevel = level;

        SoundManager.Instance.PlaySound_Win();
        for (var i = 0; i < stars.Count; i++)
        {
            var star = stars[i];
            star.sprite = (i + 1) <= countStar ? star_On : star_Off;
        }

        coinGroup.SetActive(true);
        hammerGroup.SetActive(true);
        starGroup.SetActive(true);

        particle.SetActive(false);

        PlayAnimHammer();
        hammerSkeletonGraphic.gameObject.SetActive(false);

        gameObject.SetActive(true);

        _addStar = _countStar - Config.LevelStar(Config.currSelectLevel);
        if (_addStar < 0) _addStar = 0;

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
        btnNext.gameObject.SetActive(false);
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

        chestProcess.SetActive(true);
        InitViews_ShowView();
    }

    private Sequence _sequenceShowView;

    private void InitViews_ShowView()
    {
        _sequenceShowView?.Kill();

        _sequenceShowView = DOTween.Sequence();
        _sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();

            if (_thisLevel != 1)
            {
                btnNext.interactable = true;
            }
        });


        _sequenceShowView.InsertCallback(0.2f, () =>
        {
            if (_countStar >= 1)
            {
                SoundManager.Instance.PlaySound_WinStarPop();
            }

            stars[0].gameObject.SetActive(true);
            stars[0].GetComponent<BBUIView>().ShowView();


            hammerSkeletonGraphic.gameObject.SetActive(true);
        });

        _sequenceShowView.InsertCallback(0.3f, () =>
        {
            if (_countStar >= 2)
            {
                SoundManager.Instance.PlaySound_WinStarPop();
            }

            stars[1].gameObject.SetActive(true);
            stars[1].GetComponent<BBUIView>().ShowView();

            btnNext.gameObject.SetActive(true);

            btnReward.gameObject.SetActive(true);
            btnReward.interactable = true;

            PlayAnimText();

            OnAnimProcess();
        });

        _sequenceShowView.InsertCallback(0.5f, () =>
        {
            if (_countStar >= 3)
            {
                SoundManager.Instance.PlaySound_WinStarPop();
            }

            stars[2].gameObject.SetActive(true);
            stars[2].GetComponent<BBUIView>().ShowView();
        });

        _sequenceShowView.InsertCallback(1f, () =>
        {
            if (_thisLevel == 1)
            {
                btnNext.interactable = true;
            }
        });

        _sequenceShowView.OnComplete(ShowViewFinished);
    }

    private void ShowViewFinished()
    {
        lockGroup.gameObject.SetActive(false);
    }

    #region CHEST

    [Header("Chest")] public TextMeshProUGUI txtChestCountStar;

    public Image imgChestProgress;

    #endregion

    public void SetEnableNativeAd()
    {
        bgPopup.raycastTarget = false;
    }
}