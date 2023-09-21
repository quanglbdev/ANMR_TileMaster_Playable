using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using TMPro;
using UnityEngine.EventSystems;

namespace Your.Namespace.Here.UniqueStringHereToAvoidNamespaceConflicts.Lists
{
    public class LeaderBoardOSA : OSA<MyParams, MyListItemViewsHolder>
    {
        private SimpleDataHelper<Model> Data { get; set; }

        #region OSA implementation

        protected override void Start()
        {
            var temp = _Params.LeaderboardType == Config.LEADERBOARD_TYPE.weekly
                ? AssetManager.Instance.LeaderboardDefinitions_WEEK
                : AssetManager.Instance.LeaderboardDefinitions_TOP;

            var orderByDescending = temp.OrderByDescending(x => x.score);

            var selfData = orderByDescending.ToList().Find(x => x.id == 0);
            var selfIndex = orderByDescending.ToList().IndexOf(selfData);

            _Params.userRanks = orderByDescending.ToList();
            _Params.selfCard.gameObject.SetActive(false);

            var count = selfIndex == orderByDescending.Count() - 1
                ? _Params.userRanks.Count
                : _Params.userRanks.Count - 1;


            for (var i = 0; i < count; i++)
            {
                if (_Params.userRanks[i].id != 0) continue;
                _Params.InitCard(i + 1, this, _Params.userRanks[i]);
                break;
            }

            Data = new SimpleDataHelper<Model>(this);

            base.Start();

            RetrieveDataAndUpdate(count);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Start();
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
        }

        private Model CreateNewModel(int index)
        {
            var data = _Params.userRanks[index];
            var model = new Model()
            {
                ID = data.id,
                AvatarId = data.avatarId,
                Name = data.playerName,
                Score = $"{data.score}",
                Order = index >= 100 ? $"100+" : $"{index + 1}",
                Index = index,
            };

            return model;
        }

        protected override MyListItemViewsHolder CreateViewsHolder(int itemIndex)
        {
            var instance = new MyListItemViewsHolder();
            instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
            return instance;
        }

        protected override void UpdateViewsHolder(MyListItemViewsHolder newOrRecycled)
        {
            if (newOrRecycled.ItemIndex >= Data.Count)
                return;

            var model = Data[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateViews(model);
            foreach (var item in VisibleItems)
            {
                if (item.root.GetComponent<LeaderBoardCard>().isSelfCard)
                {
                    _Params.ShowOrHideSelfCard(false);
                    return;
                }

                _Params.ShowOrHideSelfCard(true);
            }
        }

        #endregion


        #region data manipulation

        public void AddItemsAt(int index, IList<Model> items)
        {
            Data.InsertItems(index, items);
        }

        public void RemoveItemsFrom(int index, int count)
        {
            Data.RemoveItems(index, count);
        }

        public void RemoveItemsFromStart(int count)
        {
            Data.RemoveItemsFromStart(count);
        }

        public void SetItems(IList<Model> items)
        {
            Data.ResetItems(items);
        }

        #endregion


        void RetrieveDataAndUpdate(int count)
        {
            StartCoroutine(FetchMoreItemsFromDataSourceAndUpdate(count));
        }

        IEnumerator FetchMoreItemsFromDataSourceAndUpdate(int count)
        {
            yield return new WaitForEndOfFrame();

            var newItems = new Model[count];
            for (var i = 0; i < count; ++i)
            {
                var model = CreateNewModel(i);
                if (model.ID == 0)
                {
                    count = 100;
                }

                newItems[i] = model;
            }

            OnDataRetrieved(newItems);
        }

        void OnDataRetrieved(Model[] newItems)
        {
            SetItems(newItems.ToList());
        }
    }

    public class Model
    {
        public int ID;
        public int AvatarId;
        public int Index;
        public string Name, Score, Order;
    }

    public class MyListItemViewsHolder : BaseItemViewsHolder
    {
        [Header("Sprite")] private Sprite _trophy1, _trophy2, _trophy3;
        private Sprite _gift1, _gift2, _gift3;

        public Image avatar;
        public TextMeshProUGUI textName, textScore, textOrder;
        public Image trophy, giftBox;
        public Transform selfFrame;
        public LeaderBoardCard leaderBoardCard;

        private LeaderBoardPopup _leaderBoardPopup;
        private RectTransform _rectTransform;
        private Camera _camera;


        public override void CollectViews()
        {
            base.CollectViews();

            var mainPanel = root.GetChild(0);
            mainPanel.GetComponentAtPath("AvatarFrame/Mask/Avartar", out avatar);
            mainPanel.GetComponentAtPath("Name", out textName);
            mainPanel.GetComponentAtPath("ScoreText", out textScore);
            mainPanel.GetComponentAtPath("Index", out textOrder);
            mainPanel.GetComponentAtPath("Trophy", out trophy);
            mainPanel.GetComponentAtPath("GiftBox", out giftBox);
            mainPanel.GetComponentAtPath("SelfFlame", out selfFrame);
            leaderBoardCard = root.GetComponent<LeaderBoardCard>();
        }

        public void UpdateViews(Model model)
        {
            if (model == null) return;
            avatar.sprite = AssetManager.Instance.GetAvatarDefinition(model.AvatarId)?.avatarSprite;
            textName.text = model.Name;
            textScore.text = model.Score;
            textOrder.text = model.Order;
            if (model.ID == 0)
            {
                selfFrame.gameObject.SetActive(true);
                leaderBoardCard.isSelfCard = true;
            }
            else
            {
                selfFrame.gameObject.SetActive(false);
                leaderBoardCard.isSelfCard = false;
            }

            var giftBoxSprite = AssetManager.Instance.GetSpriteGiftBox(model.Index);
            if (giftBoxSprite != null)
            {
                giftBox.gameObject.SetActive(true);
                giftBox.sprite = giftBoxSprite;

                giftBox.GetComponent<Button>().onClick.RemoveAllListeners();
                giftBox.GetComponent<Button>().onClick.AddListener(() =>
                {
                    LeaderboardRewardPopup.Instance.Show(model.Index);
                });
            }
            else
                giftBox.gameObject.SetActive(false);

            var trophySprite = AssetManager.Instance.GetSpriteTrophy(model.Index);
            if (trophySprite != null)
            {
                trophy.sprite = trophySprite;
                trophy.gameObject.SetActive(true);
            }
            else
                trophy.gameObject.SetActive(false);
        }
    }


    [Serializable]
    public class MyParams : BaseParamsWithPrefab
    {
        public List<UserRankSO> userRanks = new();
        public Config.LEADERBOARD_TYPE LeaderboardType;
        public LeaderBoardCard selfCard;

        public void InitCard(int order, LeaderBoardOSA leaderBoard, UserRankSO data)
        {
            selfCard.gameObject.SetActive(true);
            selfCard.Init(
                AssetManager.Instance.GetSpriteTrophy(order),
                AssetManager.Instance.GetSpriteGiftBox(order),
                order,
                leaderBoard,
                data);

            selfCard.isSelfCard = false;
        }

        public void ShowOrHideSelfCard(bool isShow)
        {
            selfCard.gameObject.SetActive(isShow);
        }
    }
}