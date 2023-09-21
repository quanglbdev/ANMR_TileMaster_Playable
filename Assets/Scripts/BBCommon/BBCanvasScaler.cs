using UnityEngine;
using UnityEngine.UI;
public class BBCanvasScaler : MonoBehaviour
{
    private CanvasScaler canvasScaler;
    private void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }
    void Start()
    {
        if (Screen.width * 1f / Screen.height >  946f/ 2045f)
        {
            canvasScaler.matchWidthOrHeight = 1f;
        } else{
            canvasScaler.matchWidthOrHeight = 0f;
        }
    }
}
