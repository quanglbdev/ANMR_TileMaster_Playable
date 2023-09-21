using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
    private void Start()
    {
        comboBtn.onClick.RemoveAllListeners();
        comboBtn.onClick.AddListener(ContinueCombo);
        
        leaderboardBtn.onClick.RemoveAllListeners();
        leaderboardBtn.onClick.AddListener(ContinueLeaderboard);
        
        iceBtn.onClick.RemoveAllListeners();
        iceBtn.onClick.AddListener(ContinueTutorialIce);
        
        grassBtn.onClick.RemoveAllListeners();
        grassBtn.onClick.AddListener(ContinueTutorialGrass);
        
        boomBtn.onClick.RemoveAllListeners();
        boomBtn.onClick.AddListener(ContinueTutorialBoom);
        
        beeBtn.onClick.RemoveAllListeners();
        beeBtn.onClick.AddListener(ContinueTutorialBee);
        
    }
    [Header("COMBO")]
    public Button comboBtn;
    private void ContinueCombo()
    {
        SoundManager.Instance.PlaySound_Click();        
        TutorialManager.Instance.HideTut_Combo();
    }
    
    [Header("LEADERBOARD")]
    public Button leaderboardBtn;
    private void ContinueLeaderboard()
    {
        SoundManager.Instance.PlaySound_Click();        
        if (Config.CheckTutorial_Leaderboard())
        {
            TutorialManager.Instance.ShowTut_ClickGiftBoxLeaderboard_HandGuild();
        }
    }
    
    [Header("ICE")]
    public Button iceBtn;
    private void ContinueTutorialIce()
    {
        SoundManager.Instance.PlaySound_Click();        
        TutorialManager.Instance.HideTut_ClickTileIce();
    }
    
    [Header("GRASS")]
    public Button grassBtn;
    private void ContinueTutorialGrass()
    {
        SoundManager.Instance.PlaySound_Click();
        TutorialManager.Instance.HideTut_ClickTileGrass();
    }
    
    [Header("BOOM")]
    public Button boomBtn;
    private void ContinueTutorialBoom()
    {
        SoundManager.Instance.PlaySound_Click();        
        TutorialManager.Instance.HideTut_ClickTileBoom();
    }
    
    [Header("BEE")]
    public Button beeBtn;
    private void ContinueTutorialBee()
    {
        SoundManager.Instance.PlaySound_Click();        
        TutorialManager.Instance.HideTut_ClickTileBee();
    }
}
