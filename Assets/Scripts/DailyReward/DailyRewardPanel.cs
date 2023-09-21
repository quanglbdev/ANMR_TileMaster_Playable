using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Config;

public class DailyRewardPanel : MonoBehaviour
{
    [Header("Button")] [SerializeField] private List<Button> collectBtns;
    [SerializeField] private BBUIButton collectBtn, collectX2Btn;

    [Header("Day 7")] [SerializeField] private List<Transform> rewardsDay7;

    [Header("Reward Data")] [SerializeField]
    private List<ConfigItemShopData> prizesDay1;

    [SerializeField] private List<ConfigItemShopData> prizesDay2;
    [SerializeField] private List<ConfigItemShopData> prizesDay3;
    [SerializeField] private List<ConfigItemShopData> prizesDay4;
    [SerializeField] private List<ConfigItemShopData> prizesDay5;
    [SerializeField] private List<ConfigItemShopData> prizesDay6;
    [SerializeField] private List<ConfigItemShopData> prizesDay7;

    [Header("Popup")] public GameObject popup;
    public GameObject lockGroup;

    private enum HidePopupState
    {
        Claim,
        X2
    }

    private HidePopupState _state;

    private void Start()
    {
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);

        for (var i = 0; i < collectBtns.Count; i++)
        {
            collectBtns[i].onClick.AddListener(CollectDailyReward);
            collectBtns[i].interactable = false;

            collectBtns[DAILY_REWARD_INDEX].interactable = true;
            collectBtn.OnPointerClickCallBack_Start.AddListener(CollectDailyReward);
            collectX2Btn.OnPointerClickCallBack_Start.AddListener(CollectX2DailyReward);
            Refresh();

            var dayTxt = collectBtns[i].transform.Find("DayNo").GetComponent<TextMeshProUGUI>();
            dayTxt.text = $"Day {i + 1}";
            if (i == 6)
            {
                var prizeDay7 = GetRewardData(7);
                for (var j = 0; j < rewardsDay7.Count; j++)
                {
                    var reward = rewardsDay7[j];
                    var amount = reward.Find("Amount").GetComponent<TextMeshProUGUI>();

                    switch (prizeDay7[j].shopItemType)
                    {
                        case SHOPITEM.COIN:
                            amount.text = $"{prizeDay7[j].countItem}";
                            break;
                        case SHOPITEM.UNDO:
                        case SHOPITEM.SUGGEST:
                        case SHOPITEM.SHUFFLE:
                        case SHOPITEM.TILE_RETURN:
                        case SHOPITEM.EXTRA_SLOT:
                            amount.text = $"x{prizeDay7[j].countItem}";
                            break;
                    }
                }

                return;
            }

            var amountTxt = collectBtns[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>();
            var prize = GetRewardData(i + 1)[0];
            switch (prize.shopItemType)
            {
                case SHOPITEM.COIN:
                    amountTxt.text = $"{prize.countItem}";
                    break;
                case SHOPITEM.UNDO:
                case SHOPITEM.SUGGEST:
                case SHOPITEM.SHUFFLE:
                case SHOPITEM.TILE_RETURN:
                case SHOPITEM.EXTRA_SLOT:
                case SHOPITEM.HEART:
                    amountTxt.text = $"x{prize.countItem}";
                    break;
                case SHOPITEM.FREE_HEART:
                    amountTxt.text = $"+{prize.countItem}m";
                    break;
            }
        }
    }

    private void Refresh()
    {
        for (int i = 0; i < DAILY_REWARD_INDEX; i++)
        {
            collectBtns[i].transform.Find("Claimed").gameObject.SetActive(true);
        }

        for (var i = 0; i < collectBtns.Count; i++)
        {
            var currentDay = collectBtns[i].transform.Find("CurrentDay").gameObject;
            currentDay.SetActive(i == DAILY_REWARD_INDEX);
        }
    }

    private void CollectDailyReward()
    {
        collectBtn.Interactable = false;
        collectX2Btn.Interactable = false;
        Refresh();
        _state = HidePopupState.Claim;
        popup.GetComponent<BBUIView>().HideView();
    }

    private void CollectX2DailyReward()
    {
        AdsManager.Instance.ShowRewardAd_CallBack(FirebaseManager.RewardFor.DailyReward, CollectX2DailyRewardCallback);
    }

    private void CollectX2DailyRewardCallback(AdsManager.ADS_CALLBACK_STATE state)
    {
        if (state == AdsManager.ADS_CALLBACK_STATE.SUCCESS)
        {
            collectBtn.Interactable = false;
            collectX2Btn.Interactable = false;
            Refresh();
            _state = HidePopupState.X2;
            popup.GetComponent<BBUIView>().HideView();
        }
    }

    private List<ConfigItemShopData> GetRewardData(int dayNo)
    {
        return dayNo switch
        {
            1 => prizesDay1,
            2 => prizesDay2,
            3 => prizesDay3,
            4 => prizesDay4,
            5 => prizesDay5,
            6 => prizesDay6,
            7 => prizesDay7,
            _ => throw new ArgumentOutOfRangeException(nameof(dayNo), dayNo, "Data is NULL")
        };
    }

    public void ShowDailyReward()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(true);
        popup.gameObject.SetActive(false);

        collectBtn.gameObject.SetActive(false);
        collectX2Btn.gameObject.SetActive(false);

        InitViews_ShowView();
    }

    private void InitViews_ShowView()
    {
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.2f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            collectX2Btn.gameObject.SetActive(true);
            collectX2Btn.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.6f, () =>
        {
            collectBtn.gameObject.SetActive(true);
            collectBtn.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.OnComplete(() => { lockGroup.gameObject.SetActive(false); });
    }

    private void HidePopup_Finished()
    {
        gameObject.SetActive(false);

        var prizes = GetRewardData(DAILY_REWARD_INDEX + 1);
        if (_state == HidePopupState.X2)
        {
            var rewards = prizes.Select(prize
                => new ConfigItemShopData(prize.shopItemType, prize.countItem * 2)).ToList();

            GameDisplay.Instance.OpenRewardPopup(rewards, false);
            foreach (var reward in rewards)
            {
                if(reward.shopItemType == SHOPITEM.COIN)
                    FirebaseManager.Instance.LogEarnVirtualCoin(reward.countItem, "daily_reward_x2");
            }
            
        }
        else
        {
            GameDisplay.Instance.OpenRewardPopup(prizes, false);

            foreach (var reward in prizes)
            {
                if(reward.shopItemType == SHOPITEM.COIN)
                    FirebaseManager.Instance.LogEarnVirtualCoin(reward.countItem, "daily_rewar");
            }
        }


        DAILY_REWARD_INDEX++;
        DAILY_REWARD_TIME = GetDateTimeNow();
    }
}