using UnityEngine;
using UnityEngine.Serialization;

public class BackgroundFitAll : MonoBehaviour
{
    [FormerlySerializedAs("cameraBG")] public Camera cameraBg;
    private void Start()
    {
        Config.GetCurrLevel();
        var x = Config.currLevel >= 3 ? 1.4f : 1.35f;
        gameObject.transform.localScale = Vector3.one * x;
         if (Config.TARGET_ASPECT < cameraBg.aspect)
         {
            gameObject.transform.localScale = Vector3.one * x * (cameraBg.aspect / Config.TARGET_ASPECT + 0.05f);
        }
    }
}
