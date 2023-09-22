using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private BBUIButton btnPlay;
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        btnPlay.OnPointerClickCallBack_Start.AddListener(TouchPlay);
    }

    private void TouchPlay()
    {
        LevelPopup.instance.ShowLevelPopup();
        //GamePlayManager.Instance.SetLoadGame(1);
        //GamePlayManager.Instance.SetLoadGame(Random.Range(13, 31));
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
