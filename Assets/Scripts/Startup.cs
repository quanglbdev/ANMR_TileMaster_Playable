using UnityEngine;

public class Startup : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Config.SetCurrLevel(Config.MAX_LEVEL);
        Config.GetSound();
        Config.GetMusic();
        Config.currCoin = Config.GetCoin();
        Config.currHeart = Config.GetHeart();
        Config.currHammer = Config.GetHammer();
        Config.currStar = Config.GetStar();
        Config.currBuildingStar = Config.GetBuildingStar();
        Config.GetLevelStar();
        if (Config.isMusic)
        {
            MusicManager.Instance.PlayMusicBackground();
        }
    }
}