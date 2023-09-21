using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private List<Building> _buildings;
    [SerializeField] private TextMeshProUGUI _mapName;
    [SerializeField] private Transform _mapFrame;

    [HideInInspector] public List<ConfigItemShopData> rewards;

    public List<Building> Buildings => _buildings;

    public int NextBuildingTutorial
    {
        get
        {
            _nextBuildingTutorial++;
            return _nextBuildingTutorial;
        }
    }

    private int _nextBuildingTutorial;

    public async UniTask Init()
    {
        //var map = AssetManager.Instance.GetMapDefinition(Config.MAP_SELECTED);
        var map = AssetManager.Instance.GetMapDefinition(Config.MAP_INDEX);

        rewards = map.rewards;
        _mapName.text = $"{map.mapId}. {map.mapName}";
        _mapFrame.localScale = Vector3.zero;

        foreach (var building in Buildings)
        {
            building.transform.localScale = Vector3.zero;
        }

        foreach (var building in Buildings)
        {
            building.Init(this);
            await UniTask.DelayFrame(1);
        }
    }

    public void ShowWhenOpen()
    {
        foreach (var building in Buildings)
        {
            building.ShowWhenOpen();
        }

        _mapFrame.DOScale(1f, 0.2f).OnComplete(() =>
        {
            if (Config.CheckTutorial_Building())
            {
                TutorialManager.Instance.ShowTut_ClickUpgrade_HandGuild(Buildings[0].button.transform, false);
            }
        });
    }

    public void Show()
    {
        foreach (var building in Buildings)
        {
            building.Show();
        }
    }

    public void Hide()
    {
        foreach (var building in Buildings)
        {
            building.Hide();
        }
    }

    public void HideButton()
    {
        _mapFrame.DOScale(0, 0.3f);
        foreach (var building in Buildings)
        {
            building.HideButton();
        }
    }

    public bool IsFinishMap()
    {
        return Buildings.All(building => building.BuildingLevel >= 3);
    }
}