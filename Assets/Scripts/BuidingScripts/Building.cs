using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [Header("BUILDING_TYPE")] [SerializeField]
    private Config.BUILDING_TYPE _buildingType;

    [Header("UI")] [SerializeField] private Image _buidingImage;
    [SerializeField] private List<GameObject> _levels;
    [SerializeField] private TextMeshProUGUI _priceText;

    [FormerlySerializedAs("_button")] [SerializeField]
    public BBUIButton button;

    private string _buildingName;
    private int _buildingLevel;
    private int _price;
    private int _elementId;
    private MapController _controller;

    public int BuildingLevel => _buildingLevel;

    public void Init(MapController controller)
    {
        _controller = controller;
        _elementId = (int)_buildingType;
        _buildingName = _buildingType.ToString();
        _buildingLevel = Config.GetCurrentElement(_elementId);
        if (_price < 3)
        {
            _price = AssetManager.Instance.GetPriceElement(_buildingType, Config.MAP_INDEX, BuildingLevel);
        }

        ReloadBuildingImage();
        if (button != null)
        {
            button.OnPointerClickCallBack_Start.RemoveAllListeners();
            button.OnPointerClickCallBack_Start.AddListener(Upgrade);
            button.transform.localScale = Vector3.zero;
        }
    }

    public void ShowWhenOpen()
    {
        if (transform.localScale != Vector3.one)
        {
            transform.DOScale(1f, 0.3f);
        }

        LoadLevel();
        _priceText.text = $"{_price}";
        ShowOrHideButton(true);
    }

    private void Upgrade()
    {
        if (_buildingLevel == 3)
            return;


        if (Config.currHammer < _price)
        {
            if (Config.CheckTutorial_Building())
            {
                TutorialManager.Instance.HideTut_ClickTown();
            }

            MenuManager.instance.OpenGetHammerPopup();
            return;
        }

        if (Config.CheckTutorial_Building())
        {
            TutorialManager.Instance.HideBeeBuilding();
            TutorialManager.Instance.ShowTut_ClickUpgrade_HandGuild(
                _controller.Buildings[_controller.NextBuildingTutorial].button.transform, Config.currHammer == 0);
        }

        Config.SetHammer(Config.currHammer - _price);
        _buildingLevel++;
        Config.SetCurrentElement(_elementId, _buildingLevel);
        button.Interactable = false;
        StartCoroutine(YieldUpgrade());
    }

    private IEnumerator YieldUpgrade()
    {
        SoundManager.Instance.PlaySound_Building();
        button.transform.DOScale(0f, 0.3f);
        _buidingImage.transform.DOScale(0, 0.3f);
        if (_price < 3)
        {
            _price = AssetManager.Instance.GetPriceElement(_buildingType, Config.MAP_INDEX, BuildingLevel);
            _priceText.text = $"{_price}";
        }

        yield return new WaitForSeconds(0.3f);
        SoundManager.Instance.Stop();
        SoundManager.Instance.PlaySound_BuildingCompleted();
        if (_buildingLevel < 3)
            button.transform.DOScale(1f, 0.3f);
        _buidingImage.transform.DOScale(1, 0.3f).OnComplete(() => { button.Interactable = true; });
        ReloadBuildingImage();
        LoadLevel();
        MenuManager.instance.AddStarAnim(1, _buidingImage.transform.position);
        if (_controller.IsFinishMap())
        {
            yield return new WaitForSeconds(0.3f);
            SoundManager.Instance.Stop();
            Config.MAP_INDEX++;
            Config.MAP_SELECTED = Config.MAP_INDEX;
            SoundManager.Instance.PlaySound_MapCompleted();
            yield return new WaitForSeconds(0.3f);
            SoundManager.Instance.Stop();
            EventDispatcher.Instance.PostEvent(EventID.UpdateHammer);
            GameDisplay.Instance.OpenRewardPopup(_controller.rewards, false, Config.REWARD_STATE.MAP_COMPLETED);
            foreach (var reward in _controller.rewards)
            {
                if(reward.shopItemType == Config.SHOPITEM.COIN)
                    FirebaseManager.Instance.LogEarnVirtualCoin(reward.countItem, "map_completed");
            }
        }
    }

    private void ReloadBuildingImage()
    {
        if (_buidingImage == null) return;
        if (_buildingLevel == 0)
        {
            _buidingImage.enabled = false;
            return;
        }

        _buidingImage.enabled = true;
        _buidingImage.sprite =
            Resources.Load<Sprite>($"Sprite/building/map_{Config.MAP_INDEX}/{_buildingName}/{_buildingLevel}");
        _buidingImage.SetNativeSize();
    }

    private void LoadLevel()
    {
        foreach (var level in _levels)
        {
            level.SetActive(false);
        }

        for (var i = 0; i < _buildingLevel; i++)
        {
            if (_levels[i] != null)
                _levels[i].SetActive(true);
        }
        if(_buildingLevel >= 3)
            button.transform.DOScale(0f, 0.3f);
    }

    public void HideButton()
    {
        ShowOrHideButton(false);
    }

    public void Hide()
    {
        transform.DOScale(0f, 0.3f);
    }

    public void Show()
    {
        transform.DOScale(1f, 0.3f);
    }

    private void ShowOrHideButton(bool isShow)
    {
        if (_buildingLevel == 3)
        {
            button.transform.localScale = Vector3.zero;
            return;
        }

        var to = isShow ? Vector3.one : Vector3.zero;

        button.transform.DOScale(to, 0.3f);
    }
}