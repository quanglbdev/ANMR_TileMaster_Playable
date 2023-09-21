using System.Collections;
using UnityEngine;
using Doozy.Engine.UI.Animation;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Doozy.Engine.UI.Base;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(CanvasGroup))]
public class BBUIButton : UIComponentBase<BBUIButton>, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    public bool AllowMultipleClicks = false;

    public bool BlockAllButtonClicks = true;

    [SerializeField] private bool UseOnPointerClick;

    public BBUIButtonBehavior OnPointerClick_BBUIButtonBehavior;

    public UnityEvent OnPointerClickCallBack_Start;

    [SerializeField] private bool UseOnPointerDown;

    public UnityEvent OnPointerDownCallBack;

    [SerializeField] private bool UseOnPointerUp;

    public UnityEvent OnPointerUpCallBack;


    public override void Start()
    {
        base.Start();
        SettingBBUIButton.isClickAvaiable = true;
        OnPointerClickCallBack_Start.AddListener(() => { SoundManager.Instance.PlaySound_Click(); });
    }


    public void OnDeselect(BaseEventData eventData)
    {
        //throw new NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClick();
    }


    public void OnPointerClick()
    {
        if (UseOnPointerClick)
        {
            if (SettingBBUIButton.isClickAvaiable)
            {
                if (Interactable)
                {
                    if (!AllowMultipleClicks) DisableButton(OnPointerClick_BBUIButtonBehavior.GetTotalDuration());
                    if (BlockAllButtonClicks)
                        ButtonManager.Instance.DisableAllBBUIButton(
                            OnPointerClick_BBUIButtonBehavior.GetTotalDuration());
                    if (OnPointerClickCallBack_Start != null)
                    {
                        OnPointerClickCallBack_Start.Invoke();
                    }

                    OnPointerClick_BBUIButtonBehavior.PlayAnimation(this);
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (UseOnPointerDown)
        {
            OnPointerDownCallBack.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (UseOnPointerUp)
        {
            OnPointerUpCallBack.Invoke();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
    }

    #region MULTI_CLICK

    private Coroutine m_disableButtonCoroutine;
    private Coroutine m_disableAllBBUIButtonCoroutine;
    private Button m_button;

    /// <summary> Reference to the Button component </summary>
    public Button Button
    {
        get
        {
            if (m_button != null) return m_button;
            m_button = GetComponent<Button>();
            return m_button;
        }
    }

    /// <summary> Returns TRUE if the Button component is interactable. This also toggles the interactability of this UIButton </summary>
    public bool Interactable
    {
        get { return Button.interactable; }
        set { Button.interactable = value; }
    }

    /// <summary> Sets Interactable property to FALSE </summary>
    public void DisableButton()
    {
        Interactable = false;
    }

    /// <summary> Disable the button for a set time duration </summary>
    /// <param name="duration"> How long will the button get disabled for </param>
    private void DisableButton(float duration)
    {
        if (!Interactable) return;
        DisableButton();

        ButtonManager.Instance.DisableButton(duration, DisableButton,
            EnableButton);
    }

    public void DisableAllBBUIButton(float duration)
    {
        SettingBBUIButton.isClickAvaiable = false;
        StartCoroutine(DisableAllBBUIButtonEnumerator(duration));
    }

    /// <summary> Sets Interactable to TRUE </summary>
    public void EnableButton()
    {
        Interactable = true;
    }

    private IEnumerator DisableButtonEnumerator(float duration)
    {
        DisableButton();
        if (Settings.IgnoreUnityTimescale) //check if the UI ignores Unity's Time.Timescale or not
            yield return
                new WaitForSecondsRealtime(duration); //wait for seconds realtime (ignore Unity's Time.Timescale)
        else
            yield return new WaitForSeconds(duration); //wait for seconds (respect Unity's Time.Timescale)
        EnableButton();
        m_disableButtonCoroutine = null;
    }

    private IEnumerator DisableAllBBUIButtonEnumerator(float duration)
    {
        if (Settings.IgnoreUnityTimescale) //check if the UI ignores Unity's Time.Timescale or not
            yield return
                new WaitForSecondsRealtime(duration); //wait for seconds realtime (ignore Unity's Time.Timescale)
        else
            yield return new WaitForSeconds(duration); //wait for seconds (respect Unity's Time.Timescale)

        SettingBBUIButton.isClickAvaiable = true;
        m_disableAllBBUIButtonCoroutine = null;
    }


    private void CallBack_OnPointerClick_Completed(float duration)
    {
        StartCoroutine(CallBack_OnPointerClick_Completed_IEnumerator(duration));
    }

    private IEnumerator CallBack_OnPointerClick_Completed_IEnumerator(float duration)
    {
        if (Settings.IgnoreUnityTimescale) //check if the UI ignores Unity's Time.Timescale or not
            yield return
                new WaitForSecondsRealtime(duration); //wait for seconds realtime (ignore Unity's Time.Timescale)
        else
            yield return new WaitForSeconds(duration); //wait for seconds (respect Unity's Time.Timescale)


        StartCoroutine(DisableAllBBUIButtonEnumerator(0f));
    }

    #endregion

    public override void OnDisable()
    {
        base.OnDisable();

        UIAnimator.StopAnimations(RectTransform, AnimationType.Punch);
        UIAnimator.StopAnimations(RectTransform, AnimationType.State);

        ResetToStartValues();


        if (m_disableButtonCoroutine == null) return;
        StopCoroutine(m_disableButtonCoroutine);
        m_disableButtonCoroutine = null;
        EnableButton();
    }
}