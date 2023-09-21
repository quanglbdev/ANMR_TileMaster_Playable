using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu_HammerGroup : MonoBehaviour
{
    public Button btnAddHammer;
    public TextMeshProUGUI txtHammer;
    private void Start()
    {
        if(btnAddHammer != null) 
            btnAddHammer.onClick.AddListener(TouchHammer);
        Config.OnChangeHammer += OnChangeHammer;
        ShowHammer();
    }

    private void OnDestroy()
    {
        Config.OnChangeHammer -= OnChangeHammer;
    }

    private void OnChangeHammer(int hammerValue) {
        ShowHammer();
    }

    private void ShowHammer() {
        DOTween.Kill(txtHammer.transform);
        
        txtHammer.text = $"{Config.currHammer}";
        txtHammer.transform.localScale = Vector3.one;
        txtHammer.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 10, 2f).SetEase(Ease.InOutBack).SetRelative(true).SetLoops(3,LoopType.Restart);
    }

    private void TouchHammer() 
    {
        SoundManager.Instance.PlaySound_Click();        
    }
}
