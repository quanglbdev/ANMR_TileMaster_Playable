using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WinStreakCard : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private BBUIButton showRewardBtn;

    [Header("Sprite")] [SerializeField] private LevelInWinStreak winStreak1, winStreak2, winStreak3;

    [Header("Sprite")] [SerializeField] private Sprite chestSprite1, chestSprite2, chestSprite3, chestOpenedSprite;
    [Header("Reward")] [SerializeField] private Transform rewardTransform;
    [Header("Reward")] [SerializeField] private Transform rewardParent;

    [Header("Sprite")] [SerializeField] private Sprite infinityHeart, star, gold, undo, shuffle, magicWand;

    public int Milestone { get; private set; }

    public Transform AvatarPosition
    {
        get
        {
            if (Config.WIN_STREAK_INDEX == winStreak1.Level)
                return winStreak1.AvatarPosition;

            if (Config.WIN_STREAK_INDEX == winStreak2.Level)
                return winStreak2.AvatarPosition;

            return winStreak3.AvatarPosition;
        }
    }

    private bool _canClaim;

    private List<ConfigItemShopData> _rewards = new();

    public void Init(int milestone, List<Reward> rewards)
    {
        Milestone = milestone;
        foreach (var reward in rewards)
        {
            _rewards.Add(new ConfigItemShopData(reward));
            var rewardSpawn = Instantiate(rewardTransform, rewardParent);
            rewardSpawn.localScale = new Vector3(-1, 1, 1);
            rewardSpawn.GetComponent<Image>().sprite = GetSprite(reward.shopItemType);
            rewardSpawn.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                GetText(reward.shopItemType, reward.countItem);
        }

        showRewardBtn.OnPointerClickCallBack_Start.AddListener(ShowReward);
    }

    public void Open()
    {
        winStreak1.Level = Milestone;

        if (Milestone < 50)
        {
            winStreak2.Level = Milestone + 1;
            winStreak3.Level = Milestone + 2;
        }
        else
        {
            winStreak2.gameObject.SetActive(false);
            winStreak3.gameObject.SetActive(false);
        }

        showRewardBtn.GetComponent<Image>().sprite =
            Config.HasClaimRewardClaimedWinStreak(Milestone) ? chestOpenedSprite : GetChtSprite();

        rewardParent.gameObject.SetActive(false);
    }

    private Sprite GetSprite(Config.SHOPITEM shopItem)
    {
        return shopItem switch
        {
            Config.SHOPITEM.COIN => gold,
            Config.SHOPITEM.UNDO => undo,
            Config.SHOPITEM.SUGGEST => magicWand,
            Config.SHOPITEM.SHUFFLE => shuffle,
            Config.SHOPITEM.FREE_HEART => infinityHeart,
            Config.SHOPITEM.STAR => star
        };
    }
    
    private Sprite GetChtSprite()
    {
        var rewardCount = _rewards.Count;
        return rewardCount switch
        {
            2 => chestSprite1,
            3 => chestSprite1,
            4 => chestSprite2,
            _ => chestSprite3
        };
    }


    private string GetText(Config.SHOPITEM shopItem, int amount)
    {
        return shopItem switch
        {
            Config.SHOPITEM.COIN => $"{amount}",
            Config.SHOPITEM.UNDO => $"x{amount}",
            Config.SHOPITEM.SUGGEST => $"x{amount}",
            Config.SHOPITEM.SHUFFLE => $"x{amount}",
            Config.SHOPITEM.FREE_HEART => $"{amount}m",
            Config.SHOPITEM.STAR => $"{amount}",
        };
    }

    public void ClaimReward()
    {
        Config.AddRewardClaimedWinStreak(Milestone);
        GameDisplay.Instance.OpenRewardPopup(_rewards, false);
        foreach (var reward in _rewards)
        {
         if(reward.shopItemType == Config.SHOPITEM.COIN)
             FirebaseManager.Instance.LogEarnVirtualCoin(reward.countItem, "win_streak");
        }
        WinStreakPopup.Instance.GetAvatarPosition();
        Open();
    }

    public void HideReward()
    {
        if (rewardParent.gameObject.activeSelf)
            rewardParent.GetComponent<BBUIView>().HideView();
    }

    private void ShowReward()
    {
        if (!rewardParent.gameObject.activeSelf)
        {
            WinStreakPopup.Instance.HideReward();
            rewardParent.GetComponent<BBUIView>().ShowView();
        }
        else
            rewardParent.GetComponent<BBUIView>().HideView();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        WinStreakPopup.Instance.HideReward();
    }
}