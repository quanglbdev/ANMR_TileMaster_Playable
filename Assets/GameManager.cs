using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private GameObject winPopUp;
    public bool isTest = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        if(!isTest) LoadLevel();
    }

    void Start()
    {
        //AudioManager.instance.Play("Bmg");
        if (PlayerPrefs.HasKey("CURREN_LEVEL"))
            currentLevel = PlayerPrefs.GetInt("CURREN_LEVEL");
    }

    public int currentLevel = 1;
    public int maxLevel = 20;
    GameObject level;
    public void LoadLevel()
    {
        List<GameObject> ls = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            ls.Add(transform.GetChild(i).gameObject);
            ls[i].transform.parent = transform.parent;
        }
        Debug.Log("Current Level " + currentLevel);
        var levelPrb = Resources.Load("Level/Level" + currentLevel) as GameObject;
       
        level = Instantiate(levelPrb, Vector3.zero, Quaternion.identity);
        level.transform.parent = transform;
        level.transform.localPosition = Vector3.zero;
        level.transform.localScale = new Vector3(level.transform.localScale.x*transform.localScale.x, level.transform.localScale.y * transform.localScale.y, level.transform.localScale.z * transform.localScale.z);
        //swap Level to Top
        for (int i = 0; i < ls.Count; i++)
        {
            ls[i].transform.parent = transform;
            ls[i].transform.localPosition = Vector3.zero;
        }
    }

    public void WinPhase()
    {
        PlayerPrefs.SetInt("CURREN_LEVEL", currentLevel + 1);
        StartCoroutine(ShowWin(1f));
    }

    public IEnumerator ShowWin(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        winPopUp.SetActive(true);
        Destroy(level);
    }

    public void NextLevel()
    {
        currentLevel++;
        if (currentLevel > maxLevel)
        {
            int k = UnityEngine.Random.Range(1, maxLevel);
            currentLevel = k;
            PlayerPrefs.SetInt("CURREN_LEVEL", currentLevel);
        }
        LoadLevel();
        winPopUp.SetActive(false);
    }
}
#if UNITY_EDITOR
public class MenuItems
{
    [MenuItem("Tools/PlayPrefs/Clear PlayerPrefs")]
    private static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}

#endif