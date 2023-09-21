using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        Application.targetFrameRate = 60;
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init();
            Debug.Log("FBInit is called with appID:" + FB.AppId);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void Start()
    {
        Config.GetCurrLevel();
        Config.SetCurrLevel(100);

        Config.GetSound();
        Config.GetMusic();
        Config.currCoin = Config.GetCoin();
        Config.currHeart = Config.GetHeart();
        Config.currHammer = Config.GetHammer();
        Config.currStar = Config.GetStar();
        Config.currBuildingStar = Config.GetBuildingStar();
        Config.currPiggyBankCoin = Config.GetPiggyBank();
        Config.GetLevelStar();
        Config.GetFailCount();
        Config.GetCurrentEvent();
        Config.GetRewardsClaimedWinStreak();
        Config.SetCurrentEvent(new List<Config.EVENT> { Config.EVENT.WIN_STREAK });
        if (Config.isMusic)
        {
            MusicManager.Instance.PlayMusicBackground();
        }

        StartCoroutine(LoadMenuScene_IEnumerator());
    }

    private IEnumerator LoadMenuScene_IEnumerator()
    {
        yield return new WaitForSeconds(1f);
        LoadMenuScene();
    }

    public void LoadMenuScene()
    {
        Debug.Log("LoadSceneMenu");
        SceneManager.LoadSceneAsync("Menu");
    }
}