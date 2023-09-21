using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RewardPopup : MonoBehaviour
{
    [Header("Popup")] public GameObject popup;
    public GameObject lockGroup;
    public List<ConfigItemShopData> listDatas = new();

    [Header("Button")] public Button claimBtn, watchAdsBtn;

    [Header("LIST ITEM PREFAB")] public RewardItem rewardItem_Coin_Prefab;
    public RewardItem rewardItem_Undo_Prefab;
    public RewardItem rewardItem_Suggest_Prefab;
    public RewardItem rewardItem_Shuffle_Prefab;
    public RewardItem rewardItem_TileReturn_Prefab;
    public RewardItem rewardItem_AddToSlot_Prefab;
    public RewardItem rewardItem_FreeHeart_Prefab;
    public RewardItem rewardItem_Heart_Prefab;
    public RewardItem rewardItem_Combo_Prefab;
    public RewardItem rewardItem_Star_Prefab;

    public Transform contentReward, contentReward_2;
    private Config.REWARD_STATE _rewardState;

    private void Start()
    {
        claimBtn.onClick.AddListener(TouchClose);
        watchAdsBtn.onClick.AddListener(TouchWatchAds);
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }


    private bool _isShowCollectX2 = true;

    public void OpenRewardPopup(List<ConfigItemShopData> listData, bool isShowCollectX2 = true,
        Config.REWARD_STATE rewardState = Config.REWARD_STATE.BUY)
    {
        _rewardState = rewardState;
        listDatas = listData;
        gameObject.SetActive(true);
        _isShowCollectX2 = isShowCollectX2;
        InitViews();
    }

    private void TouchClose()
    {
        SoundManager.Instance.PlaySound_Cash();

        lockGroup.gameObject.SetActive(true);
        if (GamePlayManager.Instance != null)
            GamePlayManager.Instance.SetUpdate_CountItem();

        StartCoroutine(TouchClose_IEnumerator());
    }


    private IEnumerator TouchClose_IEnumerator()
    {
        yield return new WaitForSeconds(.1f);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void TouchWatchAds()
    {
        watchAdsBtn.interactable = false;
        lockGroup.gameObject.SetActive(true);

        lockGroup.gameObject.SetActive(false);
        //WatchAds
        WatchAds_Finished();
    }


    private void WatchAds_Finished()
    {
        SoundManager.Instance.PlaySound_Cash();
        foreach (var item in listDatas)
        {
            item.countItem *= 2;
        }

        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
        if (GamePlayManager.Instance != null)
            GamePlayManager.Instance.SetUpdate_CountItem();
    }


    private void InitViews()
    {
        SoundManager.Instance.PlaySound_Popup();
        SoundManager.Instance.PlaySound_Reward();
        lockGroup.gameObject.SetActive(false);
        popup.gameObject.SetActive(false);
        if (_isShowCollectX2)
        {
            watchAdsBtn.gameObject.SetActive(true);
            watchAdsBtn.interactable = true;
        }
        else
        {
            watchAdsBtn.gameObject.SetActive(false);
        }

        claimBtn.gameObject.SetActive(true);
        foreach (Transform child in contentReward)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in contentReward_2)
        {
            Destroy(child.gameObject);
        }

        InitListRewards();

        InitView_ShowView();
    }

    private void InitListRewards()
    {
        contentReward_2.gameObject.SetActive(listDatas.Count > 3);
        for (var i = 0; i < listDatas.Count; i++)
        {
            var item = listDatas[i];
            var parent = i < 3 ? contentReward : contentReward_2;

            var rewardItem = item.shopItemType switch
            {
                Config.SHOPITEM.COIN => Instantiate(rewardItem_Coin_Prefab, parent),
                Config.SHOPITEM.UNDO => Instantiate(rewardItem_Undo_Prefab, parent),
                Config.SHOPITEM.SUGGEST => Instantiate(rewardItem_Suggest_Prefab, parent),
                Config.SHOPITEM.SHUFFLE => Instantiate(rewardItem_Shuffle_Prefab, parent),
                Config.SHOPITEM.TILE_RETURN => Instantiate(rewardItem_TileReturn_Prefab, parent),
                Config.SHOPITEM.EXTRA_SLOT => Instantiate(rewardItem_AddToSlot_Prefab, parent),
                Config.SHOPITEM.FREE_HEART => Instantiate(rewardItem_FreeHeart_Prefab, parent),
                Config.SHOPITEM.HEART => Instantiate(rewardItem_Heart_Prefab, parent),
                Config.SHOPITEM.COMBO => Instantiate(rewardItem_Combo_Prefab, parent),
                Config.SHOPITEM.STAR => Instantiate(rewardItem_Star_Prefab, parent),
                _ => null
            };

            if (rewardItem != null) rewardItem.InitSpinItem(item);
        }
    }

    private void InitView_ShowView()
    {
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.1f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
        });
        sequenceShowView.InsertCallback(0.4f, () =>
        {
            foreach (Transform child in contentReward)
            {
                SoundManager.Instance.PlaySound_WinStarPop();
                child.GetComponent<RewardItem>().ShowView();
            }

            foreach (Transform child in contentReward_2)
            {
                SoundManager.Instance.PlaySound_WinStarPop();
                child.GetComponent<RewardItem>().ShowView();
            }
        });
    }

    private void HidePopup_Finished()
    {
        gameObject.SetActive(false);
    }
}