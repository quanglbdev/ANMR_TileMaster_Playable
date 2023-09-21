using UnityEngine;
using UnityEngine.Serialization;

public class LevelFit : MonoBehaviour
{
    [FormerlySerializedAs("cameraBG")] public Camera cameraBg;
    private void Start()
    {
        if (cameraBg == null)
        {
            cameraBg = Camera.main;
        }
        gameObject.transform.localScale = Vector3.one * 1.2f;
        if (cameraBg != null && Config.TARGET_ASPECT < cameraBg.aspect)
        {
            gameObject.transform.localScale = Vector3.one * 1.2f * ( Config.TARGET_ASPECT/cameraBg.aspect);
        }
    }
}
