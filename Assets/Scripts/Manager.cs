using UnityEngine;

public class Manager : MonoBehaviour
{
    public BBUIButton btnPlay;
    public Canvas canvas;

    private void Start()
    {
        btnPlay.OnPointerClickCallBack_Start.AddListener(TouchPlay);
    }

    private void TouchPlay()
    {
        LevelPopup.instance.ShowLevelPopup();
        btnPlay.gameObject.SetActive(false);
    }

    public void DisableCanvas()
    {
        canvas.enabled = false;
    }
    
    public void SelectLevel(int level)
    {
        GamePlayManager.Instance.SetLoadGame(level);
    }
}
