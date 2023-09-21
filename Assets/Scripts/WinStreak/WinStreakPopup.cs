using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WinStreakPopup : MonoBehaviour, IPointerDownHandler
{
    public static WinStreakPopup Instance;
    [SerializeField] private WinStreakCard winStreakCard;
    [SerializeField] private RectTransform content;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Popup")] [SerializeField] private GameObject popup;
    [SerializeField] private GameObject lockGroup;

    [Header("Button close")] [SerializeField]
    private BBUIButton btnClose;

    //[SerializeField] private TextMeshProUGUI currentWinStreakText, processWinStreakText;

    [SerializeField] private Image process;

    [Header("Footer - Header")] [SerializeField]
    private Transform footer, header;

    [Header("Avatar group")] [SerializeField]
    private Transform avatarGroup;

    [Header("Avatar group")] [SerializeField]
    private LevelInWinStreak streak1;

    [SerializeField] private Transform defaultAvatarPosition;

    private bool _isInitial = false;
    private List<WinStreakCard> _winStreakCards = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    private void TouchClose()
    {
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void PopupHideView_Finished()
    {
        gameObject.SetActive(false);
    }

    [Button]
    public void OpenWinStreakPopup()
    {
        if (_isInitial == false)
        {
            _isInitial = true;
            var listData = AssetManager.Instance.GetWinStreakDefinitions();
            for (var i = listData.Count - 1; i >= 0; i--)
            {
                var data = listData[i];
                var spawn = Instantiate(winStreakCard, content, true);
                spawn.Init(data.winStreak, data.rewards);
                spawn.gameObject.SetActive(true);
                spawn.transform.localScale = Vector3.one;
                _winStreakCards.Add(spawn);
            }

            footer.SetSiblingIndex(content.childCount - 1);
            header.SetSiblingIndex(0);
            streak1.Level = 1;
        }


        foreach (var card in _winStreakCards)
        {
            card.Open();
        }

        gameObject.SetActive(true);
        InitViews();

        avatarGroup.position = defaultAvatarPosition.position;
        avatarGroup.GetChild(0).GetComponent<Image>().sprite
            = AssetManager.Instance.GetAvatarDefinition(Config.AVATAR_ID).avatarSprite;

        GetAvatarPosition();
    }


    [Button]
    public void SetWinStreak(int index)
    {
        Config.WIN_STREAK_INDEX = index;
    }

    public void GetAvatarPosition()
    {
        var streak = Config.WIN_STREAK_OLD_INDEX;
        if (streak == 0)
        {
            SetParentAvatar(defaultAvatarPosition);
            return;
        }

        if (streak == 1)
        {
            SetParentAvatar(streak1.AvatarPosition);
            return;
        }

        var currentWinStreak = _winStreakCards.Find(x => x.Milestone == streak
                                                         || x.Milestone + 1 == streak
                                                         || x.Milestone + 2 == streak);
        SetParentAvatar(currentWinStreak.AvatarPosition);
    }

    private void SetNewPositionAvatar()
    {
        var streak = Config.WIN_STREAK_INDEX;
        if (streak == 0)
        {
            SetParentAvatar(defaultAvatarPosition);
            lockGroup.gameObject.SetActive(false);
        }
        if (streak == 1)
        {
            UpdateParentAvatar(streak1.AvatarPosition);
        }
        else
        {
            var currentWinStreak = _winStreakCards.Find(x => x.Milestone == streak
                                                             || x.Milestone + 1 == streak
                                                             || x.Milestone + 2 == streak);
            UpdateParentAvatar(currentWinStreak.AvatarPosition);
        }
    }

    private void UpdateParentAvatar(Transform parent)
    {
        avatarGroup.parent = parent;
        avatarGroup.localScale = Vector3.one;
        avatarGroup.DOLocalMove(Vector3.zero, 1f).OnComplete(() =>
        {
            Config.WIN_STREAK_OLD_INDEX = Config.WIN_STREAK_INDEX;
            lockGroup.gameObject.SetActive(false);
            if (_currentCard != null)
            {
                var streak = AssetManager.Instance.GetWinStreakDefinition(Config.WIN_STREAK_INDEX);
                if (_currentCard.Milestone >= streak.winStreak && !Config.HasClaimRewardClaimedWinStreak(streak.winStreak))
                {
                    _currentCard.ClaimReward();
                }
            }
        });
    }

    private void SetParentAvatar(Transform parent)
    {
        avatarGroup.parent = parent;
        avatarGroup.localScale = Vector3.one;
        avatarGroup.localPosition = Vector3.zero;
    }

    private void InitViews()
    {
        SoundManager.Instance.PlaySound_Popup();
        lockGroup.gameObject.SetActive(true);

        popup.gameObject.SetActive(false);

        btnClose.gameObject.SetActive(true);
        InitViews_ShowView();
    }

    private void InitViews_ShowView()
    {
        var sequenceShowView = DOTween.Sequence();
        sequenceShowView.InsertCallback(0.01f, () =>
        {
            popup.gameObject.SetActive(true);
            popup.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.2f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();

            UpdateScroll();
            SetNewPositionAvatar();
            if (Config.WIN_STREAK_INDEX > 11)
                OnSnapTo();
        });
    }

    private void UpdateScroll()
    {
        _currentCard = null;
        var winStreakOld = Config.WIN_STREAK_OLD_INDEX == 0 ? 1 : Config.WIN_STREAK_OLD_INDEX;
        if (winStreakOld > 11)
        {
            foreach (var card in _winStreakCards)
            {
                if (card.Milestone == winStreakOld || card.Milestone + 1 == winStreakOld ||
                    card.Milestone + 2 == winStreakOld)
                    ScrollToCenter(card.GetComponent<RectTransform>());
            }
        }
        else
        {
            scrollRect.verticalNormalizedPosition = -7.7f;
        }

        var currentWinStreak = Config.WIN_STREAK_INDEX;

        foreach (var card in _winStreakCards)
        {
            if (card.Milestone == currentWinStreak || card.Milestone + 1 == currentWinStreak ||
                card.Milestone + 2 == currentWinStreak)
                GetTargetPosition(card.GetComponent<RectTransform>());

            if (card.Milestone == currentWinStreak)
            {
                _currentCard = card;
            }
        }
    }

    private Vector3 _targetPosition;
    private float _targetVerticalNormalizedPosition;
    private WinStreakCard _currentCard;

    private void GetTargetPosition(RectTransform target)
    {
        _targetVerticalNormalizedPosition = scrollRect.GetNormalizeScroll(target);
    }

    private void ScrollToCenter(RectTransform target)
    {
        scrollRect.ScrollToCenter(target);
    }


    private void OnSnapTo()
    {
        scrollRect.DOVerticalNormalizedPos(_targetVerticalNormalizedPosition, 1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        HideReward();
    }

    public void HideReward()
    {
        foreach (var reward in _winStreakCards)
        {
            reward.HideReward();
        }
    }
}