using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRewardPopup : Singleton<LeaderboardRewardPopup>
{
    [SerializeField] private Transform content;
    [SerializeField] private Button hideBtn;
    [SerializeField] private GameObject rewardItem;

    [SerializeField] private Transform frameReward;

    [SerializeField] private Sprite coin, undo, suggest, shuffle, tileReturn, extraSlot;

    private readonly CustomDictionary<int, List<ConfigItemShopData>> _rewardsByPosition = new();

    private CustomDictionary<int, List<ConfigItemShopData>> RewardsByPosition
    {
        get
        {
            if (_rewardsByPosition.Count == 0)
            {
                _rewardsByPosition.Add(0, new List<ConfigItemShopData>()
                {
                    new(Config.SHOPITEM.COIN, 500),
                    new(Config.SHOPITEM.UNDO, 2),
                    new(Config.SHOPITEM.SUGGEST, 2),
                    new(Config.SHOPITEM.SHUFFLE, 2),
                    new(Config.SHOPITEM.TILE_RETURN, 1),
                    new(Config.SHOPITEM.EXTRA_SLOT, 1)
                });

                _rewardsByPosition.Add(1, new List<ConfigItemShopData>()
                {
                    new(Config.SHOPITEM.COIN, 400),
                    new(Config.SHOPITEM.UNDO, 2),
                    new(Config.SHOPITEM.SHUFFLE, 2),
                    new(Config.SHOPITEM.SUGGEST, 1)
                });
                _rewardsByPosition.Add(2, new List<ConfigItemShopData>()
                {
                    new(Config.SHOPITEM.COIN, 300),
                    new(Config.SHOPITEM.UNDO, 1),
                    new(Config.SHOPITEM.SUGGEST, 1)
                });
            }

            return _rewardsByPosition;
        }
    }

    public void Show(int position)
    {
        if (Config.CheckTutorial_Leaderboard())
        {
            TutorialManager.Instance.HideTut_Leaderboard();
        }

        hideBtn.onClick.RemoveListener(Hide);
        hideBtn.onClick.AddListener(Hide);
        hideBtn.gameObject.SetActive(true);

        RemoveAllChildren(content.gameObject);

        frameReward.position = new Vector3(frameReward.position.x, GetYMousePositionToCanvas());

        var rewards = RewardsByPosition[position];
        foreach (var reward in rewards)
        {
            var rewardSpawn = Instantiate(rewardItem, content);
            rewardSpawn.GetComponent<Image>().sprite = GetSprite(reward.shopItemType);
            rewardSpawn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"x{reward.countItem}";
        }
    }

    void RemoveAllChildren(GameObject parent)
    {
        while (parent.transform.childCount > 0)
        {
            DestroyImmediate(parent.transform.GetChild(0).gameObject);
        }
    }

    private float GetYMousePositionToCanvas()
    {
        Vector2 mousePosition = Input.mousePosition;
        Camera main;
        var worldPosition = ((main = Camera.main)!).ScreenToWorldPoint(mousePosition);
        return worldPosition.y + 1f;
    }

    private void Hide()
    {
        hideBtn.gameObject.SetActive(false);
    }

    private Sprite GetSprite(Config.SHOPITEM type)
    {
        return type switch
        {
            Config.SHOPITEM.COIN => coin,
            Config.SHOPITEM.UNDO => undo,
            Config.SHOPITEM.SUGGEST => suggest,
            Config.SHOPITEM.SHUFFLE => shuffle,
            Config.SHOPITEM.TILE_RETURN => tileReturn,
            Config.SHOPITEM.EXTRA_SLOT => extraSlot
        };
    }
}