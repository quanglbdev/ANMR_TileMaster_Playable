using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class z : MonoBehaviour
{
    [SerializeField] private Transform top, bottom;

    private Camera _camera;
    private void Start()
    {
        if (_camera == null)
            _camera = Camera.main;
        var aspectRatio = _camera.aspect;
        
        var expectedOrthographicSize = 5f;
        var expectedBoardPositionY = 0f;
        var topPosition = top.position;
        var bottomPosition = bottom.position;
        if (aspectRatio >= 0.55f) // SCREEN_9x16
        {
           
        }
        else if (aspectRatio >= 0.47f)  // SCREEN_1080x2280
        {
            topPosition = new Vector3(topPosition.x, 11.3f, topPosition.z);
            bottomPosition = new Vector3(topPosition.x, -7.6f, topPosition.z);
        }
        else if (aspectRatio >= 0.46f)  // SCREEN_720x1560
        {
            topPosition = new Vector3(topPosition.x, 11.5f, topPosition.z);
            bottomPosition = new Vector3(topPosition.x, -8f, topPosition.z);
        }
        else if (aspectRatio >= 0.44f)  // SCREEN_1080x2400
        {
            
        }
        else  // SCREEN_1080x2400
        {
            
        }
        top.position = topPosition;
        bottom.position = bottomPosition;

    }
}
