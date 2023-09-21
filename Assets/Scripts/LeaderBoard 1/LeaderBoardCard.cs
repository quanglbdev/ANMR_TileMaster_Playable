using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Your.Namespace.Here.UniqueStringHereToAvoidNamespaceConflicts.Lists;

public class LeaderBoardCard : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private TextMeshProUGUI textName, textScore, textOrder;
    [SerializeField] public GameObject trophy, giftBox;
    [SerializeField] private GameObject selfFrame;

    //public UserRankSO data;
    public bool isSelfCard;
    public int index = -1;
    public void Init(Sprite trophySprite, Sprite giftBoxSprite, int order, LeaderBoardOSA leaderBoardPopup,
        UserRankSO data)
    {
        if (order > 100 && data.id != 0)
        {
            gameObject.SetActive(false);
            return;
        }
        index = order;
        avatar.sprite = AssetManager.Instance.GetAvatarDefinition(data.avatarId).avatarSprite;
        textName.text = data.playerName;
        textScore.text = $"{data.score}";
        if (order > 100 && data.id == 0)
        {
            textOrder.text = $"100+";
        }
        else
        {
            textOrder.text = $"{order}";
        }

        selfFrame.SetActive(data.id == 0);
        if (trophySprite == null)
        {
            trophy.SetActive(false);
            textOrder.gameObject.SetActive(true);
        }
        else
        {
            textOrder.gameObject.SetActive(false);
            trophy.SetActive(true);
            trophy.GetComponent<Image>().sprite = trophySprite;
        }

        if (giftBoxSprite == null)
        {
            giftBox.SetActive(false);
        }
        else
        {
            giftBox.SetActive(true);
            giftBox.GetComponent<Image>().sprite = giftBoxSprite;
        }
    }
}