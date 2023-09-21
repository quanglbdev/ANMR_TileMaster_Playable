using Doozy.Engine;
using Doozy.Engine.Settings;
using Doozy.Engine.UI.Animation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BBUIButtonBehavior
{
    public ButtonAnimationType ButtonAnimationType;
    public bool isType_State_Reset;
    public UIAnimationData uiAnimationData;
    


    private UIAnimation PunchAnimation;
    private UIAnimation StateAnimation;
    private BBUIButton bBButton;


    public void PlayAnimation(BBUIButton _bBButton, UnityEvent onStartCallback = null, UnityEvent onCompleteCallback = null)
    {
        bBButton = _bBButton;
        switch (ButtonAnimationType)
        {
            case ButtonAnimationType.Punch:
                PunchAnimation = uiAnimationData.Animation.Copy();
                if (PunchAnimation == null) return;
                UIAnimator.StopAnimations(bBButton.RectTransform, AnimationType.Punch);
                if (PunchAnimation.Move.Enabled) bBButton.ResetPosition();
                if (PunchAnimation.Rotate.Enabled) bBButton.ResetRotation();
                if (PunchAnimation.Scale.Enabled) bBButton.ResetScale();
                UIAnimator.MovePunch(bBButton.RectTransform, PunchAnimation, bBButton.StartPosition);   
                UIAnimator.RotatePunch(bBButton.RectTransform, PunchAnimation, bBButton.StartRotation); 
                UIAnimator.ScalePunch(bBButton.RectTransform, PunchAnimation, bBButton.StartScale);     
                Coroutiner.Start(InvokeCallbacks(PunchAnimation, onStartCallback, onCompleteCallback));
                break;
            case ButtonAnimationType.State:
                StateAnimation = uiAnimationData.Animation.Copy();
                if (StateAnimation == null) return;
                UIAnimator.StopAnimations(bBButton.RectTransform, AnimationType.State);
                UIAnimator.MoveState(bBButton.RectTransform, StateAnimation, bBButton.StartPosition);
                UIAnimator.RotateState(bBButton.RectTransform, StateAnimation, bBButton.StartRotation);
                UIAnimator.ScaleState(bBButton.RectTransform, StateAnimation, bBButton.StartScale);
                UIAnimator.FadeState(bBButton.RectTransform, StateAnimation, bBButton.StartAlpha);
                Coroutiner.Start(InvokeCallbacks(StateAnimation, onStartCallback, onCompleteCallback));
                break;
        }
        
    }

    public void PlayAnimation(BBUIButton _bBButton)
    {
        bBButton = _bBButton;
        switch (ButtonAnimationType)
        {
            case ButtonAnimationType.Punch:
                PunchAnimation = uiAnimationData.Animation.Copy();
                if (PunchAnimation == null) return;
                UIAnimator.StopAnimations(bBButton.RectTransform, AnimationType.Punch);
                if (PunchAnimation.Move.Enabled) bBButton.ResetPosition();
                if (PunchAnimation.Rotate.Enabled) bBButton.ResetRotation();
                if (PunchAnimation.Scale.Enabled) bBButton.ResetScale();
                UIAnimator.MovePunch(bBButton.RectTransform, PunchAnimation, bBButton.StartPosition);
                UIAnimator.RotatePunch(bBButton.RectTransform, PunchAnimation, bBButton.StartRotation);
                UIAnimator.ScalePunch(bBButton.RectTransform, PunchAnimation, bBButton.StartScale);
                break;
            case ButtonAnimationType.State:
                StateAnimation = uiAnimationData.Animation.Copy();
                if (StateAnimation == null) return;
                UIAnimator.StopAnimations(bBButton.RectTransform, AnimationType.State);
                UIAnimator.MoveState(bBButton.RectTransform, StateAnimation, bBButton.StartPosition);
                UIAnimator.RotateState(bBButton.RectTransform, StateAnimation, bBButton.StartRotation);
                UIAnimator.ScaleState(bBButton.RectTransform, StateAnimation, bBButton.StartScale);
                UIAnimator.FadeState(bBButton.RectTransform, StateAnimation, bBButton.StartAlpha);
                break;
        }

    }

    public float GetTotalDuration()
    {
        if (ButtonAnimationType == ButtonAnimationType.Punch)
        {
            PunchAnimation = uiAnimationData.Animation.Copy();
            if (PunchAnimation == null) return 0;
            return PunchAnimation.TotalDuration;
        }
        else if(ButtonAnimationType == ButtonAnimationType.State)
        {
            StateAnimation = uiAnimationData.Animation.Copy();
            if (StateAnimation == null) return 0;
            return StateAnimation.TotalDuration;
        }

        return 0;
    }

    #region IEnumerators

    private IEnumerator InvokeCallbacks(UIAnimation animation, UnityEvent onStartCallback, UnityEvent onCompleteCallback)
    {
        //if (Settings.IgnoreUnityTimescale)                     //check if the UI ignores Unity's Time.Timescale or not
        //    yield return new WaitForSecondsRealtime(duration); //wait for seconds realtime (ignore Unity's Time.Timescale)
        //else
        //    yield return new WaitForSeconds(duration); //wait for seconds (respect Unity's Time.Timescale)
        if (animation == null || !animation.Enabled) yield break;
        if (DoozySettings.Instance.IgnoreUnityTimescale)
        {
            yield return new WaitForSecondsRealtime(animation.StartDelay);
        }
        else {
            yield return new WaitForSeconds(animation.StartDelay);
        }
        if (onStartCallback != null) onStartCallback.Invoke();
        if (DoozySettings.Instance.IgnoreUnityTimescale)
        {
            yield return new WaitForSecondsRealtime(animation.TotalDuration - animation.StartDelay + 0.1f);
        }
        else {
            yield return new WaitForSeconds(animation.TotalDuration - animation.StartDelay + 0.1f);
        }
        Debug.Log("InvokeCallbacks:"+ (animation.TotalDuration - animation.StartDelay + 0.1f));
        if (ButtonAnimationType == ButtonAnimationType.State && isType_State_Reset)
        {
            bBButton.ResetPosition();
            bBButton.ResetRotation();
            bBButton.ResetScale();
            bBButton.ResetAlpha();
        }
        if (onCompleteCallback != null) onCompleteCallback.Invoke();
    }

    #endregion
}
