using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelInWinStreak : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Material materialPresetGray, materialPresetYellow;
    
    [Header("Image")]
    [SerializeField] private Image image;
    [SerializeField] private Sprite notPass, passed;
    
    [Header("Avatar position")]
    [SerializeField] private Transform avatarPosition;

    private int _level;
    public Transform AvatarPosition => avatarPosition;
    public int Level
    { get => _level;
        set
        {
            _level = value;
            Setup();
        } 
    }

    private void Setup()
    {
        levelText.text = $"{Level}";

        if (Level > Config.WIN_STREAK_INDEX)
        {
            image.sprite = notPass;
            levelText.fontSharedMaterial = materialPresetGray;
        }
        
        if (Level <= Config.WIN_STREAK_INDEX)
        {
            image.sprite = passed;
            levelText.fontSharedMaterial = materialPresetYellow;
        }
    }
}