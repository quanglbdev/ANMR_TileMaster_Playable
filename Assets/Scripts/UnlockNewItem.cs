using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Engine.Utils.ColorModels;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnlockNewItem : MonoBehaviour
{
    [Header("Popup")] public GameObject popup;
    public GameObject lockGroup;

    [SerializeField] private BBUIButton continueBtn;
    [SerializeField] private Image unLockItem;
    [SerializeField] private TextMeshProUGUI text;
    [Header("SkeletonGraphic - New Item")] public SkeletonGraphic titleNewItem;
    public AnimationReferenceAsset newItemIntroText, newItemOutroText, newItemIdleText;

    [SerializeField] private TextMeshProUGUI textButton;

    [Header("SkeletonGraphic - New Booster")]
    public SkeletonGraphic titleBooster;

    public AnimationReferenceAsset boosterIntroText, boosterOutroText, boosterIdleText;

    [Header("Popup")] public Sprite combo, undo, suggest, shuffle, addSlot, tileReturn, building, starChest, profile;
    [Header("UnlockStarChest")] public UnlockStarChest unlockStarChest;

    private void Start()
    {
        continueBtn.OnPointerClickCallBack_Start.AddListener(TouchContinue);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }

    private UnityAction _callBack;
    private Config.UNLOCK_SHOW _unlockShow;
    private Config.ITEM_UNLOCK _itemUnlock;
    private bool _isSpecialTile;

    public void ShowUnLockNewItem(UnlockNewItemData data, UnityAction callBack)
    {
        SoundManager.Instance.PlaySound_Win();
        _callBack = callBack;
        _unlockShow = data.unlockShow;
        _itemUnlock = data.itemUnlock;
        unLockItem.sprite = data.sprite;
        _isSpecialTile = data.isSpecialTile;
        text.text = GetText(data.itemUnlock);

        gameObject.SetActive(true);
        InitViews();
    }

    private string GetText(Config.ITEM_UNLOCK itemUnlock)
    {
        return itemUnlock switch
        {
            Config.ITEM_UNLOCK.COMBO => "You unlocked \n<color=#FFFD34>COMBO</color>!",
            Config.ITEM_UNLOCK.BUILDING => "You unlocked \n<color=#2CFF10>GARDEN</color>!",
            Config.ITEM_UNLOCK.UNDO => "<color=#FFFD34>UNDO</color>",
            Config.ITEM_UNLOCK.SUGGEST => "<color=#FFFD34>MAGIC WAND</color>",
            Config.ITEM_UNLOCK.SHUFFLE => "<color=#FFFD34>SHUFFLE</color>",
            Config.ITEM_UNLOCK.EXTRA_SLOT => "<color=#FFFD34>EXTRA SLOT</color>",
            Config.ITEM_UNLOCK.TILE_RETURN => "<color=#FFFD34>TILE RETURN</color>",
            Config.ITEM_UNLOCK.STAR_CHEST => "You unlocked \n<color=#2CFF10>StarChest</color>!",
            Config.ITEM_UNLOCK.PROFILE => "You unlocked \n<color=#FFFD34>PROFILE</color>!",
            Config.ITEM_UNLOCK.GLUE => "You unlocked \n<color=#2CFF10>HONEY</color>!",
            Config.ITEM_UNLOCK.ICE => "You unlocked \n<color=#2CFF10>ICE</color>!",
            Config.ITEM_UNLOCK.GRASS => "You unlocked \n<color=#2CFF10>VINES</color>!",
            Config.ITEM_UNLOCK.CHAIN => "You unlocked \n<color=#2CFF10>VINE</color>!",
            Config.ITEM_UNLOCK.CLOCK => "You unlocked \n<color=#2CFF10>CLOCK</color>!",
            Config.ITEM_UNLOCK.BEE => "You unlocked \n<color=#2CFF10>BEE</color>!",
            Config.ITEM_UNLOCK.BOMB => "You unlocked \n<color=#2CFF10>BOMB</color>!",
            _ => throw new ArgumentOutOfRangeException(nameof(itemUnlock), itemUnlock, null)
        };
    }

    private Sprite GetSprite(Config.ITEM_UNLOCK itemUnlock)
    {
        return itemUnlock switch
        {
            Config.ITEM_UNLOCK.COMBO => combo,
            Config.ITEM_UNLOCK.UNDO => undo,
            Config.ITEM_UNLOCK.BUILDING => building,
            Config.ITEM_UNLOCK.SUGGEST => suggest,
            Config.ITEM_UNLOCK.SHUFFLE => shuffle,
            Config.ITEM_UNLOCK.EXTRA_SLOT => addSlot,
            Config.ITEM_UNLOCK.TILE_RETURN => tileReturn,
            Config.ITEM_UNLOCK.STAR_CHEST => starChest,
            Config.ITEM_UNLOCK.PROFILE => profile,
            Config.ITEM_UNLOCK.GLUE => profile,
            Config.ITEM_UNLOCK.ICE => profile,
            Config.ITEM_UNLOCK.GRASS => profile,
            Config.ITEM_UNLOCK.CHAIN => profile,
            Config.ITEM_UNLOCK.CLOCK => profile,
            Config.ITEM_UNLOCK.BEE => profile,
            _ => throw new ArgumentOutOfRangeException(nameof(itemUnlock), itemUnlock, null)
        };
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(true);
        popup.gameObject.SetActive(false);
        continueBtn.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        unLockItem.gameObject.SetActive(false);
        titleNewItem.gameObject.SetActive(false);
        titleBooster.gameObject.SetActive(false);
        _shouldExitLoop = false;

        textButton.text = IsShowMeText() ? "Show me" : "Continue";
        InitViews_ShowView();
    }

    private bool IsShowMeText()
    {
        return _itemUnlock is Config.ITEM_UNLOCK.SHUFFLE or Config.ITEM_UNLOCK.SUGGEST or Config.ITEM_UNLOCK.TILE_RETURN
            or Config.ITEM_UNLOCK.EXTRA_SLOT;
    }

    Sequence _sequenceShowView;

    private async UniTask PlayAnimText()
    {
        titleNewItem.AnimationState.SetAnimation(0, newItemIntroText, false);
        titleBooster.AnimationState.SetAnimation(0, boosterIntroText, false);
        var duration = _unlockShow == Config.UNLOCK_SHOW.START
            ? boosterIntroText.Animation.Duration
            : newItemIntroText.Animation.Duration;
        await UniTask.Delay((int)(duration * 1000));
        StartCoroutine(LoopAnimationWithDelay());
    }

    private readonly float _loopDelay = 1.5f;
    private bool _shouldExitLoop;

    private IEnumerator LoopAnimationWithDelay()
    {
        titleNewItem.AnimationState.ClearTrack(0);
        titleBooster.AnimationState.ClearTrack(0);

        var duration = _unlockShow == Config.UNLOCK_SHOW.START
            ? boosterIdleText.Animation.Duration
            : newItemIdleText.Animation.Duration;

        while (true)
        {
            if (!_shouldExitLoop)
            {
                titleNewItem.AnimationState.ClearTrack(0);
                titleBooster.AnimationState.ClearTrack(0);
            }

            if (_shouldExitLoop)
                break;
            if (!_shouldExitLoop)
            {
                titleNewItem.AnimationState.SetAnimation(0, newItemIdleText, true);
                titleBooster.AnimationState.SetAnimation(0, boosterIdleText, true);
            }

            yield return new WaitForSeconds(duration);
            if (!_shouldExitLoop)
            {
                titleNewItem.AnimationState.ClearTrack(0);
                titleBooster.AnimationState.ClearTrack(0);
            }

            yield return new WaitForSeconds(_loopDelay);
        }
    }

    private void StopLoopAndPlayOutro()
    {
        _shouldExitLoop = true;
        titleNewItem.AnimationState.ClearTrack(0);
        titleBooster.AnimationState.ClearTrack(0);

        titleNewItem.AnimationState.SetAnimation(0, newItemOutroText, false);
        titleBooster.AnimationState.SetAnimation(0, boosterOutroText, false);
    }

    private void InitViews_ShowView()
    {
        _sequenceShowView?.Kill();

        _sequenceShowView = DOTween.Sequence();
        _sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();

            if (_isSpecialTile == false)
            {
                titleNewItem.gameObject.SetActive(_unlockShow == Config.UNLOCK_SHOW.WIN);
                titleBooster.gameObject.SetActive(_unlockShow == Config.UNLOCK_SHOW.START);
            }
            else
            {
                titleNewItem.gameObject.SetActive(true);
            }
            
            PlayAnimText();
        });

        _sequenceShowView.InsertCallback(0.2f, () =>
        {
            continueBtn.gameObject.SetActive(true);
            continueBtn.GetComponent<BBUIView>().ShowView();

            text.gameObject.SetActive(true);
        });

        _sequenceShowView.InsertCallback(0.4f, () =>
        {
            unLockItem.gameObject.SetActive(true);
            unLockItem.GetComponent<BBUIView>().ShowView();

            lockGroup.gameObject.SetActive(false);
        });
    }

    private void TouchContinue()
    {
        StopLoopAndPlayOutro();
        var sequenceHideView = DOTween.Sequence();
        sequenceHideView.InsertCallback(newItemOutroText.Animation.Duration - .2f, () =>
        {
            lockGroup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().HideView();
        });
    }

    private void HidePopup_Finished()
    {
        gameObject.SetActive(false);
        if (_unlockShow == Config.UNLOCK_SHOW.WIN && _itemUnlock == Config.ITEM_UNLOCK.BUILDING)
            WinPopup.Instance.HideFinished();

        if (_unlockShow == Config.UNLOCK_SHOW.START && _itemUnlock != Config.ITEM_UNLOCK.EXTRA_SLOT)
        {
            Config.gameState = Config.GAME_STATE.PLAYING;
            GamePlayManager.Instance.SortingLayerCanvasPlaying();
        }

        if (_unlockShow == Config.UNLOCK_SHOW.WIN && _itemUnlock == Config.ITEM_UNLOCK.STAR_CHEST)
            unlockStarChest.Show();

        _callBack?.Invoke();
    }
}