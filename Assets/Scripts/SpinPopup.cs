using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using bonbon;
using Random = UnityEngine.Random;

public class SpinPopup : MonoBehaviour
{
    public BBUIButton btnClose, btnWatchAds, btnSpinFree;
    public GameObject btnLock;
    public GameObject popup;
    public GameObject lockGroup;
    public GameObject spinObj;

    [Header("ConfigDatas")] public List<ConfigItemShopData> listDatas = new List<ConfigItemShopData>();
    [Header("Percent Random")] public List<float> listPercents = new List<float>();
    public List<ConfigItemShopData> listSpinDatas = new List<ConfigItemShopData>();


    [Header("LIST ITEM PREFAB")] public SpinItem spinItem_Coin_Prefab;
    public SpinItem spinItem_Undo_Prefab;
    public SpinItem spinItem_Suggest_Prefab;
    public SpinItem spinItem_Shuffle_Prefab;
    public SpinItem spinItem_TileReturn_Prefab;
    public SpinItem spinItem_FreeHeart_Prefab;
    public SpinItem spinItem_Heart_Prefab;
    public SpinItem spinItem_Combo_Prefab;

    [Header("LIST POSITION ITEMS")] public List<Transform> listItemsPos = new List<Transform>();

    [Header("AnimationCurve")] [SerializeField]
    AnimationCurve spinCurve;

    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        btnWatchAds.OnPointerClickCallBack_Start.AddListener(TouchWatchAds);
        btnSpinFree.OnPointerClickCallBack_Start.AddListener(TouchSpinFree);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HidePopup_Finished);
    }


    int rotateSoundIndex = 0;

    private void Update()
    {
        if (_isSpin)
        {
            int spinIndex = Mathf.FloorToInt(spinObj.GetComponent<RectTransform>().eulerAngles.z / 45f);
            if (rotateSoundIndex != spinIndex)
            {
                rotateSoundIndex = spinIndex;
                SoundManager.Instance.PlaySound_Spin();
            }
        }
    }

    public void OpenSpinPopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void TouchClose()
    {
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void UpdateButtonSpin()
    {
        btnSpinFree.gameObject.SetActive(Config.SPIN_FREE_COUNT > 0);
        btnWatchAds.gameObject.SetActive(Config.SPIN_FREE_COUNT == 0);
        var canSpin = DateTime.Parse(Config.SPIN_LAST) - Config.GetDateTimeNow() <= _timeSpanCheck;
        //btnWatchAds.gameObject.SetActive(canSpin);
        btnLock.gameObject.SetActive(!canSpin);
        btnWatchAds.Interactable = canSpin;
    }

    private void TouchSpinCoin()
    {
        if (Config.currCoin >= Config.SPIN_PRICE)
        {
            Config.SetCoin(Config.currCoin - Config.SPIN_PRICE);
            FirebaseManager.Instance.LogSpendVirtualCoin(Config.SPIN_PRICE, "spin");
            WatchAds_Finished();
        }
        else
        {
            NotificationPopup.instance.AddNotification("Not enough Coin!");
            GamePlayManager.Instance.OpenShopPopup();
        }
    }

    private void TouchSpinFree()
    {
        if (Config.CheckTutorial_Spin())
        {
            TutorialManager.Instance.HideTut_Spin();
        }

        btnSpinFree.Interactable = false;
        Config.SPIN_FREE_COUNT -= 1;
        lockGroup.gameObject.SetActive(true);
        WatchAds_Finished(true);
    }

    private void TouchWatchAds()
    {
        btnWatchAds.Interactable = false;
        lockGroup.gameObject.SetActive(true);
        AdsManager.Instance.ShowRewardAd_CallBack(FirebaseManager.RewardFor.Spin, callbackState =>
            {
                lockGroup.gameObject.SetActive(false);
                //WatchAds
                WatchAds_Finished();
            },
            () =>
            {
                lockGroup.gameObject.SetActive(false);
                btnWatchAds.Interactable = true;
            },
            () =>
            {
                lockGroup.gameObject.SetActive(false);
                btnWatchAds.Interactable = true;
            }
        );
    }

    private int _indexResult;
    private bool _isSpin = false;

    private void WatchAds_Finished(bool free = false)
    {
        //Spin
        _indexResult = RandomExtension.RandomPercentage(listPercents.ToArray());
        //indexResult = 1;
        if (_indexResult == 8) _indexResult = 0;
        lockGroup.gameObject.SetActive(true);
        _isSpin = true;
        spinObj.transform
            .DORotate(new Vector3(0f, 0f, -4 * 360 - (8 - _indexResult) * 45 + Random.Range(-10f, 10f)), 5f)
            .SetEase(spinCurve).OnComplete(() =>
            {
                _isSpin = false;
                lockGroup.gameObject.SetActive(false);
                if (free == false)
                    Config.SetSpin_LastTime();
                UpdateButtonSpin();
                var reward = listDatas[_indexResult];
                GameDisplay.Instance.OpenRewardPopup(new List<ConfigItemShopData> { reward },
                    true);
                if(reward.shopItemType == Config.SHOPITEM.COIN)
                    FirebaseManager.Instance.LogEarnVirtualCoin(reward.countItem, "lucky_spin");
            });
    }

    private void HidePopup_Finished()
    {
        GamePlayManager.Instance.CloseShopSuccess();
        gameObject.SetActive(false);
    }

    private readonly TimeSpan _timeSpanCheck = new(0, 0, 0);

    private void InitViews()
    {
        SoundManager.Instance.PlaySound_Popup();
        lockGroup.gameObject.SetActive(false);
        popup.gameObject.SetActive(false);
        btnWatchAds.gameObject.SetActive(false);
        btnSpinFree.gameObject.SetActive(false);
        btnLock.gameObject.SetActive(false);
        //btnCoin.gameObject.SetActive(false);

        var canSpin = DateTime.Parse(Config.SPIN_LAST) - Config.GetDateTimeNow() <= _timeSpanCheck;
        btnLock.gameObject.SetActive(!canSpin);
        btnWatchAds.Interactable = canSpin;

        btnClose.gameObject.SetActive(false);
        foreach (var position in listItemsPos)
        {
            position.gameObject.SetActive(false);
        }

        InitListItems();

        InitView_ShowView();
    }


    private void InitListItems()
    {
        for (int i = 0; i < listItemsPos.Count; i++)
        {
            foreach (Transform child in listItemsPos[i])
            {
                Destroy(child.gameObject);
            }
        }

        if (Config.CheckTutorial_Spin())
        {
            TutorialManager.Instance.RollbackOldButton();
            TutorialManager.Instance.HideHandGuild();
        }

        List<ConfigItemShopData> listSpinDatas_Temp = new List<ConfigItemShopData>(listDatas);
        listSpinDatas.Clear();
        for (int i = 0; i < listItemsPos.Count; i++)
        {
            SpinItem spinItem;
            switch (listDatas[i].shopItemType)
            {
                case Config.SHOPITEM.COIN:
                    spinItem = Instantiate(spinItem_Coin_Prefab, listItemsPos[i]);
                    break;
                case Config.SHOPITEM.UNDO:
                    spinItem = Instantiate(spinItem_Undo_Prefab, listItemsPos[i]);
                    break;
                case Config.SHOPITEM.SUGGEST:
                    spinItem = Instantiate(spinItem_Suggest_Prefab, listItemsPos[i]);
                    break;
                case Config.SHOPITEM.SHUFFLE:
                    spinItem = Instantiate(spinItem_Shuffle_Prefab, listItemsPos[i]);
                    break;
                case Config.SHOPITEM.TILE_RETURN:
                    spinItem = Instantiate(spinItem_TileReturn_Prefab, listItemsPos[i]);
                    break;
                case Config.SHOPITEM.FREE_HEART:
                    spinItem = Instantiate(spinItem_FreeHeart_Prefab, listItemsPos[i]);
                    break;
                case Config.SHOPITEM.HEART:
                    spinItem = Instantiate(spinItem_Heart_Prefab, listItemsPos[i]);
                    break;
                case Config.SHOPITEM.EXTRA_SLOT:
                    spinItem = Instantiate(spinItem_Combo_Prefab, listItemsPos[i]);
                    break;
                default:
                    spinItem = null;
                    break;
            }

            spinItem.InitSpinItem(listDatas[i]);
        }
    }

    private void InitView_ShowView()
    {
        Sequence sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.1f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.3f, () =>
        {
            SoundManager.Instance.PlaySound_WinStarPop();
            listItemsPos[0].gameObject.SetActive(true);
            listItemsPos[0].GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.35f, () =>
        {
            SoundManager.Instance.PlaySound_WinStarPop();
            listItemsPos[1].gameObject.SetActive(true);
            listItemsPos[1].GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            SoundManager.Instance.PlaySound_WinStarPop();
            listItemsPos[2].gameObject.SetActive(true);
            listItemsPos[2].GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.45f, () =>
        {
            SoundManager.Instance.PlaySound_WinStarPop();
            listItemsPos[3].gameObject.SetActive(true);
            listItemsPos[3].GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.5f, () =>
        {
            SoundManager.Instance.PlaySound_WinStarPop();
            listItemsPos[4].gameObject.SetActive(true);
            listItemsPos[4].GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.55f, () =>
        {
            SoundManager.Instance.PlaySound_WinStarPop();
            listItemsPos[5].gameObject.SetActive(true);
            listItemsPos[5].GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.6f, () =>
        {
            SoundManager.Instance.PlaySound_WinStarPop();
            listItemsPos[6].gameObject.SetActive(true);
            listItemsPos[6].GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.65f, () =>
        {
            SoundManager.Instance.PlaySound_WinStarPop();
            listItemsPos[7].gameObject.SetActive(true);
            listItemsPos[7].GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.2f, () =>
        {
            if (Config.SPIN_FREE_COUNT == 0)
            {
                btnWatchAds.gameObject.SetActive(true);
                btnWatchAds.gameObject.GetComponent<BBUIView>().ShowView();
            }
            else
            {
                btnSpinFree.gameObject.SetActive(true);
                btnSpinFree.gameObject.GetComponent<BBUIView>().ShowView();
            }
        });

        sequenceShowView.InsertCallback(0.8f, () =>
        {
            btnClose.gameObject.SetActive(false);
            btnClose.gameObject.GetComponent<BBUIView>().ShowView();

            if (Config.CheckTutorial_Spin())
            {
                TutorialManager.Instance.ShowTut_ClickSpin_HandGuild(btnSpinFree.transform);
            }
        });
    }
}