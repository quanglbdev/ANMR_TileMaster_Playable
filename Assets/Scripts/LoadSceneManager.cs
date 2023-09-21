using System.Collections;
using System.Collections.Generic;
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
        Config.GetLevelStar();
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