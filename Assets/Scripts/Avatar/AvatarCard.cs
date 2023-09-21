using System;
using UnityEngine;
using UnityEngine.UI;

public class AvatarCard : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selected, _frameSelected;
    [SerializeField] private Image _image;

    private AvatarDefinition _avatarDefinition;
    private AvatarController _controller;

    private void Start()
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(Select);
    }

    private Sprite _sprite;

    private int Id { get; set; }

    public Sprite Sprite
    {
        set
        {
            _sprite = value;
            _image.sprite = _sprite;
        }
    }


    private void Select()
    {
        SoundManager.Instance.PlaySound_Click();
        Config.AVATAR_ID = Id;
        _controller.Load();
        NotificationPopup.instance.AddNotification("Change Avatar!");
        
        if (Config.CheckTutorial_Profile())
        {
            TutorialManager.Instance.ShowTut_ClickProfile_HandGuild(ProfilePopup.Instance.nameCard.transform);
        }
    }

    public void Load()
    {
        _selected.SetActive(Id == Config.AVATAR_ID);
        _frameSelected.SetActive(Id == Config.AVATAR_ID);
    }

    public void Init(AvatarDefinition avatarDefinition, AvatarController controller)
    {
        Id = avatarDefinition.avatarId;
        Sprite = avatarDefinition.avatarSprite;
        _controller = controller;
        Load();
    }
}