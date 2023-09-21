using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private GameObject backgroundDefault;
    [SerializeField] private Image backgroundMap;

    [SerializeField] private Image blur;
    private Material _material;

    private int _currentMapId = -999;

    private void Start()
    {
        _material = blur.material;
    }

    public void UpdateBackground()
    {
        UnBlurMaterial();
        backgroundDefault.SetActive(false);
        SetBackgroundIndex();
    }

    public void SetBackgroundSelected()
    {
        if (_currentMapId == Config.MAP_SELECTED) return;
        _currentMapId = Config.MAP_SELECTED;
        SetBackground();
    }

    public void SetBackgroundIndex()
    {
        if (_currentMapId == Config.MAP_INDEX) return;
        _currentMapId = Config.MAP_INDEX;
        SetBackground();
    }

    private void SetBackground()
    {
        var map = AssetManager.Instance.GetMapDefinition(_currentMapId);
        backgroundMap.sprite = map.background;
    }

    public void BlurMaterial()
    {
        if (_material.HasProperty("_Size"))
        {
            var newSize = _material.GetFloat("_Size");
            DOTween.To(() => newSize, x => newSize = x, 2f, 0.5f).OnUpdate(
                () => { _material.SetFloat("_Size", newSize); }
            );
        }

        var map = AssetManager.Instance.GetMapDefinition(Config.MAP_SELECTED);
        backgroundMap.sprite = map.background;
    }

    private void UnBlurMaterial()
    {
        if (_material.HasProperty("_Size"))
        {
            var newSize = _material.GetFloat("_Size");
            DOTween.To(() => newSize, x => newSize = x, 0f, 0.5f).OnUpdate(
                () => { _material.SetFloat("_Size", newSize); }
            );
        }
    }
}