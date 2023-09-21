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
        canvas.enabled = false;
        GamePlayManager.Instance.SetLoadGame(44);
        //GamePlayManager.Instance.SetLoadGame(Random.Range(13, 31));
    }
}
