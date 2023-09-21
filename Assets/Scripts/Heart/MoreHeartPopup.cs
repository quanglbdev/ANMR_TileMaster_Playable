using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoreHeartPopup : MonoBehaviour
{
    [Header("Popup")] public BBUIButton btnClose;
    public BBUIButton btnAds;
    public BBUIButton btnRefill;
    public GameObject popup;
    public GameObject lockGroup;

    public GameObject heart;
    public TextMeshProUGUI text;
    public TextMeshProUGUI heartCountText;

    private void Start()
    {
        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        btnAds.OnPointerClickCallBack_Start.AddListener(TouchAds);
        btnRefill.OnPointerClickCallBack_Start.AddListener(TouchRefill);
        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    private void TouchRefill()
    {
        if (Config.currCoin < 100)
        {
            GameDisplay.Instance.OpenShop();
            return;
        }

        Config.SetCoin(Config.currCoin - 100);
        FirebaseManager.Instance.LogSpendVirtualCoin(100, "refill_heart");
        OnAnimHeart(5);
        TouchClose();
    }

    private void TouchAds()
    {
        lockGroup.SetActive(true);

        AdsManager.Instance.ShowRewardAd_CallBack(FirebaseManager.RewardFor.AdsHeart, _ =>
        {
            lockGroup.SetActive(false);
            SoundManager.Instance.PlaySound_Cash();
            OnAnimHeart(1);
            TouchClose();
        }, () => { lockGroup.SetActive(false); }, () => { lockGroup.SetActive(false); });
    }

    [Header("Prefab Heart")] [SerializeField]
    private Transform prefabHeart;

    [SerializeField] private Transform spawnHeartPoint;
    [SerializeField] private Transform endHeartPoint;

    private void OnAnimHeart(int amount)
    {
        var position = spawnHeartPoint.position;
        for (var i = 0; i < amount; i++)
        {
            DOVirtual.DelayedCall(i * 0.1f, () =>
            {
                if (Config.currHeart == 5) return;
                var heartPrefab = Instantiate(prefabHeart, new Vector3(Random.Range(position.x - 1f, position.x + 1f),
                    Random.Range(position.y - 1f, position.y + 1f)), Quaternion.identity);
                heartPrefab.DOMove(endHeartPoint.position, 0.5f).SetEase(Ease.InCubic)
                    .OnComplete(() =>
                    {
                        Destroy(heartPrefab.gameObject);
                        //endHeartPoint.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 2f).SetEase(Ease.InOutBack);
                        if (Config.currHeart == 5)
                            return;
                        Config.SetHeart(Config.currHeart + 1);
                    });
            });
        }
    }

    public void OpenMoreHeartPopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(false);

        popup.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        heart.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        btnAds.gameObject.SetActive(false);
        btnRefill.gameObject.SetActive(false);
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
            heart.SetActive(true);
            heart.GetComponent<BBUIView>().ShowView();

            text.gameObject.SetActive(true);
            text.GetComponent<BBUIView>().ShowView();

            btnAds.gameObject.SetActive(true);
            btnAds.GetComponent<BBUIView>().ShowView();

            btnRefill.gameObject.SetActive(true);
            btnRefill.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.4f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();
        });
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
}