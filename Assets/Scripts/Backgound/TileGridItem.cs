using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TileGridItem : MonoBehaviour
{
     [SerializeField] private GameObject _lock, _selected, _selectedPanel;
      [SerializeField] private Image _imgArea;
      [SerializeField] private Button _selectBtn;
      
      public void Init(int index, Sprite sprite, Config.THEME_TYPE themeType , TilePageController controller)
      {
         _selectBtn.onClick.RemoveAllListeners();
         _selectBtn.onClick.AddListener(()=>
         {
            SoundManager.Instance.PlaySound_Click();
            if(Config.currTheme == themeType)return;
            GameDisplay.Instance.OpenLoadingPopup();
            Selected(themeType);
            controller.Restart();
            DOVirtual.DelayedCall(1f, () =>
            {
               GameDisplay.Instance.CloseLoadingPopup();
            });
            
         });
         _imgArea.sprite = sprite;
         //var isLock =  Config.MAP_INDEX < index;
         var isLock =  false;
         var isSelected =  Config.currTheme == themeType;
         _selected.SetActive(isSelected);
         _selectedPanel.SetActive(isSelected);
         _lock.SetActive(isLock);
      }
   
      private void Selected(Config.THEME_TYPE themeType)
      {
         Config.currTheme = themeType;
      }
}
