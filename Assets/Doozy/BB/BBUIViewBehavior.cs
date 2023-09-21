using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BBUIViewBehavior
{
    public UIAnimationData uIAnimationData;
    [HideInInspector]
    public UIAnimation Animation;
    public UnityEvent onCallback_Start;
    public UnityEvent onCallback_Completed;
    #region Constructors

    /// <summary> Initializes a new instance of the class </summary>
    /// <param name="animationType"> AnimationType for the UIAnimation Animation </param>
    public BBUIViewBehavior() { }
    
    public void Init() {
        Animation = uIAnimationData.Animation.Copy();
    }
    #endregion

}
