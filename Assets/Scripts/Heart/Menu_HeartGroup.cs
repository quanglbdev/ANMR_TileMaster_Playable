using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Menu_HeartGroup : MonoBehaviour
{
    public BBUIButton btnAddHeart;

    [Header("HEART IN FRAME")] public TextMeshProUGUI txtHeart;

    [Header("COUNTDOWN HEART")] public TextMeshProUGUI txtCountdownHeart;
    public TextMeshProUGUI txtHeart2;

    [Header("FREE HEART")] public TextMeshProUGUI txtCountdownFreeHeart;
    public GameObject infinityGameObject;
    
    public GameObject addBtn;


    private Action<object> _updateHeartCountDown;
    private Action<object> _updateFreeHeartCountDown;

    private void Start()
    {
        if(btnAddHeart != null)
            btnAddHeart.OnPointerClickCallBack_Start.AddListener(TouchAddHeart);
        Config.OnChangeHeart += OnChangeHeart;
        ShowHeart();
        _updateHeartCountDown = (param) => UpdateHeartCountdown((TimeSpan)param);
        _updateFreeHeartCountDown = (param) => UpdateFreeHeartCountdown((TimeSpan)param);
        EventDispatcher.Instance.RegisterListener(EventID.HeartCountdown, _updateHeartCountDown);
        EventDispatcher.Instance.RegisterListener(EventID.FreeHeartCountdown, _updateFreeHeartCountDown);
    }

    private void OnDestroy()
    {
        Config.OnChangeHeart -= OnChangeHeart;
        EventDispatcher.Instance.RemoveListener(EventID.HeartCountdown, _updateHeartCountDown);
        EventDispatcher.Instance.RemoveListener(EventID.FreeHeartCountdown, _updateFreeHeartCountDown);
    }

    private void OnChangeHeart(int heartValue)
    {
        ShowHeart();
    }

    private void ShowHeart()
    {
        DOTween.Kill(txtHeart.transform);

        infinityGameObject.SetActive(false);
        txtCountdownFreeHeart.gameObject.SetActive(false);
        txtCountdownHeart.gameObject.SetActive(false);

        txtHeart.gameObject.SetActive(true);
        txtHeart2.text = $"{Config.currHeart}";
        MenuManager.instance.SetCountHeartTextMoreHeartPopup($"{Config.currHeart}");
        
        addBtn.SetActive(Config.currHeart < Config.MAX_HEART);
        if (Config.currHeart >= Config.MAX_HEART)
            txtHeart.text = "Full";
    }

    private void TouchAddHeart()
    {
        if (Config.FREE_HEART)
        {
            MenuManager.instance.OpenShowFreeHeartPopup();
            return;
        }
        
        if(Config.currHeart == 5)
            MenuManager.instance.OpenShowFullHeartPopup();
        else
            MenuManager.instance.OpenMoreHeartPopup();
    }

    private readonly TimeSpan _maxEnergyRefillTime = new(0, Config.TIME_TO_RELOAD_HEART, 0);

    private void UpdateHeartCountdown(TimeSpan _countdown)
    {
        if (_countdown.TotalSeconds != 0)
        {
            infinityGameObject.SetActive(false);
            txtCountdownFreeHeart.gameObject.SetActive(false);
            txtHeart.gameObject.SetActive(false);

            txtCountdownHeart.gameObject.SetActive(true);
            txtHeart2.gameObject.SetActive(true);
            txtHeart2.text = $"{Config.currHeart}";
            _countdown = _maxEnergyRefillTime - _countdown;
            txtCountdownHeart.text = $"{_countdown.Minutes:00}:{_countdown.Seconds:00}";
            MenuManager.instance.SetNextLiveTextMoreHeartPopup($"Next lives: <color=#FFDD1E>{_countdown.Minutes:00}:{_countdown.Seconds:00}");;
        }
        else
            ShowHeart();
    }

    private void UpdateFreeHeartCountdown(TimeSpan _countdown)
    {
        if (_countdown.TotalSeconds != 0)
        {
            txtCountdownHeart.gameObject.SetActive(false);
            txtHeart2.gameObject.SetActive(false);
            txtHeart.gameObject.SetActive(false);
            TimeSpan countDownFreeHeartTime = new(0, Config.FREE_HEART_TIME, 0);
            _countdown = countDownFreeHeartTime - _countdown;

            infinityGameObject.SetActive(true);
            txtCountdownFreeHeart.gameObject.SetActive(true);

            txtCountdownFreeHeart.text = _countdown.Hours > 0
                ? $"{_countdown.Hours:0}:{_countdown.Minutes:00}:{_countdown.Seconds:00}"
                : $"{_countdown.Minutes:00}:{_countdown.Seconds:00}";
        }
        else
            ShowHeart();
    }

#if UNITY_EDITOR
    [Button]
    public void TestUsingHeart(int heart)
    {
        Config.SetHeart(Config.currHeart - heart);
    }

    [Button]
    public void TestAddFreeHeart(int heart)
    {
        Config.FREE_HEART_TIME = +heart;
        Config.FREE_HEART_DATE_ADD = DateTime.Now;
    }
#endif
}