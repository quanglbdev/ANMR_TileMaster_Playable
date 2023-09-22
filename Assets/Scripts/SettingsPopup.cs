using UnityEngine;
using DG.Tweening;
using Image = UnityEngine.UI.Image;

public class SettingsPopup : MonoBehaviour
{
    [Header("Button top")] public BBUIButton btnSound;
    public BBUIButton btnMusic;
    public BBUIButton btnVibration;
    [Header("Button bottom")] public BBUIButton btnContinue;
    public BBUIButton btnRateUs;
    public BBUIButton btnSupport;
    public BBUIButton btnAbout;

    [Header("Sound")] public Image icon_Sound;
    public Sprite sprite_SoundOn;
    public Sprite sprite_SoundOff;
    [Header("Music")] public Image icon_Music;
    public Sprite sprite_MusicOn;
    public Sprite sprite_MusicOff;

    [Header("Vibration")] public Image icon_Vibration;
    public Sprite sprite_VibrationOn;
    public Sprite sprite_VibrationOff;

    [Header("Popup")] public GameObject popup;

    [Header("LockGroup")] public GameObject lockGroup;

    enum STATE_CLOSEPOPUP
    {
        CONTINNUE,
        RESTART,
        HOME,
        LEVELSELECT,
        RATE,
        ABOUT,
        FEEDBACK
    }

    STATE_CLOSEPOPUP stateClosePopup = STATE_CLOSEPOPUP.CONTINNUE;

    void Start()
    {
        btnContinue.OnPointerClickCallBack_Start.AddListener(TouchContinue);
        btnRateUs.OnPointerClickCallBack_Start.AddListener(TouchRate);
        btnSupport.OnPointerClickCallBack_Start.AddListener(TouchFeedback);
        btnAbout.OnPointerClickCallBack_Start.AddListener(TouchAbout);

        btnSound.OnPointerClickCallBack_Start.AddListener(TouchSound);
        btnMusic.OnPointerClickCallBack_Start.AddListener(TouchMusic);
        btnVibration.OnPointerClickCallBack_Start.AddListener(TouchVibration);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    private void TouchVibration()
    {
        Config.SetVibration(!Config.isVibration);
        ShowButtonVibration();
    }

    private void TouchMusic()
    {
        Config.SetMusic(!Config.isMusic);
        ShowButtonMusic();
    }

    private void TouchSound()
    {
        Config.SetSound(!Config.isSound);
        ShowButtonSound();
    }

    private void TouchRate()
    {
        stateClosePopup = STATE_CLOSEPOPUP.RATE;
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    public void OpenSettingInGamePopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private void ShowButtonMusic()
    {
        icon_Music.sprite = Config.isMusic ? sprite_MusicOn : sprite_MusicOff;
        icon_Music.SetNativeSize();
    }

    private void ShowButtonSound()
    {
        icon_Sound.sprite = Config.isSound ? sprite_SoundOn : sprite_SoundOff;
        icon_Sound.SetNativeSize();
    }

    private void ShowButtonVibration()
    {
        icon_Vibration.sprite = Config.isVibration ? sprite_VibrationOn : sprite_VibrationOff;
        icon_Vibration.SetNativeSize();
    }


    private void InitViews()
    {
        ShowButtonVibration();
        ShowButtonSound();
        ShowButtonMusic();
        SoundManager.Instance.PlaySound_Popup();
        lockGroup.gameObject.SetActive(false);

        popup.gameObject.SetActive(false);

        btnContinue.gameObject.SetActive(true);
        btnRateUs.gameObject.SetActive(true);
        btnSupport.gameObject.SetActive(true);

        btnMusic.gameObject.SetActive(true);
        btnSound.gameObject.SetActive(true);
        btnVibration.gameObject.SetActive(true);
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
    }

    private void TouchContinue()
    {
        stateClosePopup = STATE_CLOSEPOPUP.CONTINNUE;
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void TouchAbout()
    {
        stateClosePopup = STATE_CLOSEPOPUP.ABOUT;
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void TouchFeedback()
    {
        stateClosePopup = STATE_CLOSEPOPUP.FEEDBACK;
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void TouchLevelSelect()
    {
        stateClosePopup = STATE_CLOSEPOPUP.CONTINNUE;
        lockGroup.gameObject.SetActive(true);
        popup.GetComponent<BBUIView>().HideView();
    }

    private void TouchHome()
    {
        stateClosePopup = STATE_CLOSEPOPUP.HOME;
        lockGroup.gameObject.SetActive(true);

        popup.GetComponent<BBUIView>().HideView();
    }

    private void PopupHideView_Finished()
    {
        switch (stateClosePopup)
        {
            case STATE_CLOSEPOPUP.CONTINNUE:
                GamePlayManager.Instance.SetUnPause();
                break;
            case STATE_CLOSEPOPUP.RESTART:
                GamePlayManager.Instance.SetLoadGame(GamePlayManager.Instance.level);
                break;
            case STATE_CLOSEPOPUP.HOME:
                GamePlayManager.Instance.HideView();
                break;
            case STATE_CLOSEPOPUP.ABOUT:
                break;
        }

        gameObject.SetActive(false);
    }
}