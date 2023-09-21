using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Your.Namespace.Here.UniqueStringHereToAvoidNamespaceConflicts.Grids;

public class ThemeGridItem : MonoBehaviour
{
    [SerializeField] private GameObject _lock, _selected, _selectedPanel;
    [SerializeField] private TextMeshProUGUI _txtAreaIndex, _txtAreaName;
    [SerializeField] private Image _imgArea;
    [SerializeField] private Button _selectBtn;

    public void Init(int index, string areaName, Sprite sprite, ThemePageController controller)
    {
        _selectBtn.onClick.RemoveAllListeners();
        _selectBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound_Click();
            if (Config.MAP_SELECTED == index)
                return;
            GameDisplay.Instance.OpenLoadingPopup();
            Selected(index);
            MenuManager.instance.UpdateBackground();
            controller.Restart();
            DOVirtual.DelayedCall(1f, () => { GameDisplay.Instance.CloseLoadingPopup(); });
        });
        _imgArea.sprite = sprite;
        var isLock = Config.MAP_INDEX < index;
        var isSelected = Config.MAP_SELECTED == index;
        _selected.SetActive(isSelected);
        _selectedPanel.SetActive(isSelected);
        _lock.SetActive(isLock);
        _imgArea.gameObject.SetActive(!isLock);
        _txtAreaIndex.text = $"Area {index}";
        _txtAreaName.text = areaName;
    }

    private void Selected(int index)
    {
        Config.MAP_SELECTED = index;
    }
}