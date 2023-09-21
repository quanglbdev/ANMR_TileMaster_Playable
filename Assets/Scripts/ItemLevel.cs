using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemLevel : MonoBehaviour
{
    public TextMeshProUGUI txtLevel, txtCurrLevel, txtLevel_lock;
    public Image iconLock;
    public GameObject currentLevel;
    public List<Image> listStars;
    public Sprite spriteStar_On;
    public Sprite spriteStar_Off;
    public BBUIButton btnLevel;

    private InfoLevel infoLevel;

    void Start()
    {
        btnLevel.OnPointerClickCallBack_Start.AddListener(TouchLevel);
    }

    private void TouchLevel()
    {
        if (!Config.FREE_HEART)
        {
            if (Config.currHeart <= 0)
            {
                GameDisplay.Instance.OpenMoreHeartPopup();

                return;
            }
        }

        LevelPopup.instance.SelectLevel(infoLevel);
    }

    public void SetInitItemLevel(InfoLevel _infoLevel)
    {
        infoLevel = _infoLevel;

        ShowInfoLevel();
    }

    private void ShowInfoLevel()
    {
        var size = infoLevel.level == 1000 ? 36 : infoLevel.level < 100 ? 52 : infoLevel.level < 10 ? 56 : 44;
        if (infoLevel.level < Config.currLevel)
        {
            //Da mo khoa
            btnLevel.Interactable = true;
            currentLevel.SetActive(false);
            iconLock.gameObject.SetActive(false);
            txtLevel.gameObject.SetActive(true);
            txtLevel.text = $"<size={size}>{infoLevel.level}";
            for (var i = 0; i < listStars.Count; i++)
            {
                listStars[i].gameObject.SetActive(false);
                //listStars[i].sprite = spriteStar_Off;
            }

            if (infoLevel.star >= 3) infoLevel.star = 3;
            for (var i = 0; i < infoLevel.star; i++)
            {
                listStars[i].gameObject.SetActive(true);
                listStars[i].sprite = spriteStar_On;
            }
        }
        else if (infoLevel.level == Config.currLevel)
        {
            //CurrLevel
            currentLevel.SetActive(true);
            btnLevel.Interactable = true;
            iconLock.gameObject.SetActive(false);
            txtLevel.gameObject.SetActive(false);
            txtCurrLevel.text = $"<size={size}>{infoLevel.level}";
            for (var i = 0; i < listStars.Count; i++)
            {
                listStars[i].gameObject.SetActive(false);
                //listStars[i].sprite = spriteStar_Off;
            }
        }
        else
        {
            if (Config.isHackMode)
            {
                btnLevel.Interactable = true;
                iconLock.gameObject.SetActive(false);
                txtLevel.gameObject.SetActive(true);
                currentLevel.SetActive(false);
                txtLevel.text = $"<size={size}>{infoLevel.level}";
                for (int i = 0; i < listStars.Count; i++)
                {
                    listStars[i].gameObject.SetActive(true);
                    listStars[i].sprite = spriteStar_Off;
                }

                if (infoLevel.star >= 3) infoLevel.star = 3;
                for (int i = 0; i < infoLevel.star; i++)
                {
                    listStars[i].gameObject.SetActive(true);
                    listStars[i].sprite = spriteStar_On;
                }
            }
            else
            {
                btnLevel.Interactable = false;
                iconLock.gameObject.SetActive(true);
                txtLevel.gameObject.SetActive(false);
                txtLevel.text = $"<size={size}>{infoLevel.level}";
                txtLevel_lock.text = $"<size={size}>{infoLevel.level}";
                foreach (var star in listStars)
                {
                    star.gameObject.SetActive(false);
                }
            }
        }
    }
}