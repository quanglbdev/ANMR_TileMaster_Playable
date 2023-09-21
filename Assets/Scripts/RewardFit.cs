using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardFit : MonoBehaviour
{
    public Camera cameraBg;
    
    private void Start()
    {
        if (cameraBg == null)
        {
            cameraBg = Camera.main;
        }
        gameObject.transform.localScale = Vector3.one;
        if (cameraBg != null && Config.TARGET_ASPECT < cameraBg.aspect)
        {
            gameObject.transform.localScale = Vector3.one * ( Config.TARGET_ASPECT/cameraBg.aspect);
        }
    }
}
