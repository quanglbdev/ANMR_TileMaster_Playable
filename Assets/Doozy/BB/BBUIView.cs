using Doozy.Engine.UI;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BBUIView : UIComponentBase<BBUIView>
{
    /// <summary> Behavior when this UIView gets shown (becomes visible on screen) </summary>
    [ToggleGroup("UseShowBehavior")] [SerializeField] private bool UseShowBehavior;
    [ToggleGroup("UseShowBehavior")] public BBUIViewBehavior ShowBehavior = new BBUIViewBehavior();


    /// <summary> Behavior when this UIView gets hidden (goes off screen) </summary>
    [ToggleGroup("UseHideBehavior")] [SerializeField] private bool UseHideBehavior;
    [ToggleGroup("UseHideBehavior")] public BBUIViewBehavior HideBehavior = new BBUIViewBehavior();
    

    /// <summary> Returns TRUE if this UIView is NOT visible (is NOT in view) </summary>
    public bool IsHidden { get { return Visibility == VisibilityState.NotVisible; } }

    /// <summary> Returns TRUE if this UIView is playing the Hide Animation to get out of view </summary>
    public bool IsHiding { get { return Visibility == VisibilityState.Hiding; } }

    /// <summary> Returns TRUE if this UIView is playing the Show Animation to become visible </summary>
    public bool IsShowing { get { return Visibility == VisibilityState.Showing; } }

    /// <summary> Returns TRUE if this UIView is visible (is in view) </summary>
    public bool IsVisible { get { return Visibility == VisibilityState.Visible; } }


    /// <summary> Internal variable that keeps track of this UIView's visibility state (Visible, NotVisible, Hiding or Showing) </summary>
    private VisibilityState m_visibility = VisibilityState.Visible;
    public VisibilityState Visibility
    {
        get { return m_visibility; }
        set
        {
            m_visibility = value;
        }
    }


    public override void Awake()
    {
        base.Awake();
        if (UseShowBehavior)
        {
            ShowBehavior.Init();
        }
        if (UseHideBehavior)
        {
            HideBehavior.Init();
        }
    }

    #region SHOW_VIEW
    /// <summary> Internal variable used to keep track of the coroutine used for when this UIView is shown </summary>
    private Coroutine m_showCoroutine;
    public void ShowView(){
        if (UseShowBehavior)
        {
            gameObject.SetActive(true);
            if (!ShowBehavior.Animation.Enabled)
            {
                return; //no SHOW animations have been enabled -> cannot show this UIView -> stop here
            }


            if (Visibility == VisibilityState.Showing) return;

            m_showCoroutine = StartCoroutine(ShowEnumerator());
        }
        else {
            gameObject.SetActive(true);
        }
    }

    private IEnumerator ShowEnumerator()
    {
        UIAnimator.StopAnimations(RectTransform, ShowBehavior.Animation.AnimationType); //stop any SHOW animations
        
        
        //MOVE
        Vector3 moveFrom = UIAnimator.GetAnimationMoveFrom(RectTransform, ShowBehavior.Animation, StartPosition);
        Vector3 moveTo = UIAnimator.GetAnimationMoveTo(RectTransform, ShowBehavior.Animation, StartPosition);
        if (!ShowBehavior.Animation.Move.Enabled) ResetPosition();
        UIAnimator.Move(RectTransform, ShowBehavior.Animation, moveFrom, moveTo); //initialize and play the SHOW Move tween

        //ROTATE
        Vector3 rotateFrom = UIAnimator.GetAnimationRotateFrom(ShowBehavior.Animation, StartRotation);
        Vector3 rotateTo = UIAnimator.GetAnimationRotateTo(ShowBehavior.Animation, StartRotation);
        if (!ShowBehavior.Animation.Rotate.Enabled) ResetRotation();
        UIAnimator.Rotate(RectTransform, ShowBehavior.Animation, rotateFrom, rotateTo); //initialize and play the SHOW Rotate tween

        //SCALE
        Vector3 scaleFrom = UIAnimator.GetAnimationScaleFrom(ShowBehavior.Animation, StartScale);
        Vector3 scaleTo = UIAnimator.GetAnimationScaleTo(ShowBehavior.Animation, StartScale);
        if (!ShowBehavior.Animation.Scale.Enabled) ResetScale();
        UIAnimator.Scale(RectTransform, ShowBehavior.Animation, scaleFrom, scaleTo); //initialize and play the SHOW Scale tween

        //FADE
        float fadeFrom = UIAnimator.GetAnimationFadeFrom(ShowBehavior.Animation, StartAlpha);
        float fadeTo = UIAnimator.GetAnimationFadeTo(ShowBehavior.Animation, StartAlpha);
        if (!ShowBehavior.Animation.Fade.Enabled) ResetAlpha();
        UIAnimator.Fade(RectTransform, ShowBehavior.Animation, fadeFrom, fadeTo); //initialize and play the SHOW Fade tween

        Visibility = VisibilityState.Showing; //update the visibility state
        

        float startTime = Time.realtimeSinceStartup;
        float totalDuration = ShowBehavior.Animation.TotalDuration;
        float elapsedTime = startTime - Time.realtimeSinceStartup;
        float startDelay = ShowBehavior.Animation.StartDelay;
        bool invokedOnStart = false;
        while (elapsedTime <= totalDuration+0.1f) //wait for seconds realtime (ignore Unity's Time.Timescale)
        {
            elapsedTime = Time.realtimeSinceStartup - startTime;

            if (!invokedOnStart && elapsedTime > startDelay)
            {
                ShowBehavior.onCallback_Start.Invoke() ;
                invokedOnStart = true;
            }
            yield return null;
        }

        ShowBehavior.onCallback_Completed.Invoke();
        Visibility = VisibilityState.Visible; //update the visibility state
        m_showCoroutine = null;     //clear the coroutine reference
    }
    #endregion

    #region HIDE_VIEW
    private Coroutine m_hideCoroutine;
    public void HideView()
    {
        if (UseHideBehavior)
        {
            gameObject.SetActive(true);
            if (!ShowBehavior.Animation.Enabled)
            {
                return; //no SHOW animations have been enabled -> cannot show this UIView -> stop here
            }


            if (Visibility == VisibilityState.Hiding) return;

            m_hideCoroutine = StartCoroutine(HideEnumerator());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator HideEnumerator()
    {
        UIAnimator.StopAnimations(RectTransform, HideBehavior.Animation.AnimationType); //stop any SHOW animations
        Vector3 moveFrom = UIAnimator.GetAnimationMoveFrom(RectTransform, HideBehavior.Animation, StartPosition);
        Vector3 moveTo = UIAnimator.GetAnimationMoveTo(RectTransform, HideBehavior.Animation, StartPosition);
        if (!HideBehavior.Animation.Move.Enabled) ResetPosition();
        UIAnimator.Move(RectTransform, HideBehavior.Animation, moveFrom, moveTo); //initialize and play the HIDE Move tween

        //ROTATE
        Vector3 rotateFrom = UIAnimator.GetAnimationRotateFrom(HideBehavior.Animation, StartRotation);
        Vector3 rotateTo = UIAnimator.GetAnimationRotateTo(HideBehavior.Animation, StartRotation);
        if (!HideBehavior.Animation.Rotate.Enabled) ResetRotation();
        UIAnimator.Rotate(RectTransform, HideBehavior.Animation, rotateFrom, rotateTo); //initialize and play the HIDE Rotate tween

        //SCALE
        Vector3 scaleFrom = UIAnimator.GetAnimationScaleFrom(HideBehavior.Animation, StartScale);
        Vector3 scaleTo = UIAnimator.GetAnimationScaleTo(HideBehavior.Animation, StartScale);
        if (!HideBehavior.Animation.Scale.Enabled) ResetScale();
        UIAnimator.Scale(RectTransform, HideBehavior.Animation, scaleFrom, scaleTo); //initialize and play the HIDE Scale tween

        //FADE
        float fadeFrom = UIAnimator.GetAnimationFadeFrom(HideBehavior.Animation, StartAlpha);
        float fadeTo = UIAnimator.GetAnimationFadeTo(HideBehavior.Animation, StartAlpha);
        if (!HideBehavior.Animation.Fade.Enabled) ResetAlpha();
        UIAnimator.Fade(RectTransform, HideBehavior.Animation, fadeFrom, fadeTo); //initialize and play the HIDE Fade tween

        Visibility = VisibilityState.Hiding;

        float startTime = Time.realtimeSinceStartup;
        float totalDuration = HideBehavior.Animation.TotalDuration;
        float elapsedTime = startTime - Time.realtimeSinceStartup;
        float startDelay = HideBehavior.Animation.StartDelay;
        bool invokedOnStart = false;
        while (elapsedTime <= totalDuration) //wait for seconds realtime (ignore Unity's Time.Timescale)
        {
            elapsedTime = Time.realtimeSinceStartup - startTime;

            if (!invokedOnStart && elapsedTime > startDelay)
            {
                HideBehavior.onCallback_Start.Invoke();
                invokedOnStart = true;
            }
            yield return null;
        }

        HideBehavior.onCallback_Completed.Invoke();
        
        Visibility = VisibilityState.NotVisible; //update the visibility state
        m_hideCoroutine = null;     //clear the coroutine reference
        gameObject.SetActive(false);
    }
    #endregion
}
