using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ProfilePopup : MonoBehaviour
{
    public static ProfilePopup Instance;
    [Header("Button")] public BBUIButton btnClose;
    [FormerlySerializedAs("btnComfirm")] public BBUIButton btnConfirm;
    public TMP_InputField inputField;
    [Header("Popup")] public GameObject popup;
    public GameObject lockGroup;

    [Header("NameCard")] public RectTransform nameCard;

    [Header("AvatarController")] [SerializeField]
    private AvatarController _avatarController;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        _avatarController.Init();

        btnClose.OnPointerClickCallBack_Start.AddListener(TouchClose);
        btnConfirm.OnPointerClickCallBack_Start.AddListener(TouchConfirm);

        inputField.onEndEdit.AddListener(OnInputEndEdit);

        popup.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(PopupHideView_Finished);
    }

    public void OpenProfilePopup()
    {
        gameObject.SetActive(true);
        InitViews();
    }

    private string _currentName;

    private void InitViews()
    {
        lockGroup.gameObject.SetActive(true);

        popup.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        btnConfirm.gameObject.SetActive(false);

        inputField.text = Config.PROFILE_NAME;
        _currentName = inputField.text;

        if (Config.CheckTutorial_Profile())
        {
            TutorialManager.Instance.HideCharacterBee();
            TutorialManager.Instance.HideHandGuild();
        }

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
            btnConfirm.gameObject.SetActive(true);
            btnConfirm.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.3f, () =>
        {
            btnClose.gameObject.SetActive(true);
            btnClose.GetComponent<BBUIView>().ShowView();
        });

        sequenceShowView.InsertCallback(0.6f, () =>
        {
            lockGroup.gameObject.SetActive(false);
            if (Config.CheckTutorial_Profile())
            {
                TutorialManager.Instance.ShowTut_ClickProfile_HandGuild(_avatarController.avatarCards[2].transform);
            }
        });
    }

    private void OnInputEndEdit(string inputText)
    {
        if (Config.CheckTutorial_Profile())
        {
            TutorialManager.Instance.ShowTut_ClickProfile_HandGuild(btnConfirm.transform);
        }
    }

    private void TouchConfirm()
    {
        var success = IsValidName(inputField.text) && _currentName != inputField.text;

        if (success)
        {
            NotificationPopup.instance.AddNotification("Change Name Success!");
            Config.PROFILE_NAME = inputField.text == "" ? _currentName : inputField.text;
        }

        if (Config.CheckTutorial_Profile())
        {
            TutorialManager.Instance.HideTut_Profile();
        }
    }

    static bool IsValidName(string name)
    {
        // Check for null or empty input
        if (string.IsNullOrWhiteSpace(name))
        {
            NotificationPopup.instance.AddNotification("Your name is null or white space");
            return false;
        }

        // Check for minimum and maximum length (adjust as needed)
        if (name.Length < 4 || name.Length > 12)
        {
            NotificationPopup.instance.AddNotification("Your name must be 4-12 characters");
            return false;
        }

        // Check for invalid characters using regex
        if (Regex.IsMatch(name, @"^[!@#$%^&*()_+={}\[\]|\\;:'""<>,.?/`~-]+$"))
        {
            NotificationPopup.instance.AddNotification("Your name can't have special characters");
            return false;
        }

        // If all conditions pass, the name is considered valid
        return true;
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