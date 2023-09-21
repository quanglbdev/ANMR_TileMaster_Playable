using UnityEngine;

public class CameraFitWidth : MonoBehaviour
{
    private const float InitialSize = 13.56f;

    private void Awake()
    {
        var main = Camera.main;
        if (main != null)
        {
            main.orthographicSize = InitialSize * (Config.TARGET_ASPECT / main.aspect);
        }
    }
}
