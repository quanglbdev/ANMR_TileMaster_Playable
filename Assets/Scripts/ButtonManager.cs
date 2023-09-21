using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ButtonManager : Singleton<ButtonManager>
{
    public void DisableAllBBUIButton(float duration)
    {
        SettingBBUIButton.isClickAvaiable = false;
        StartCoroutine(DisableAllBBUIButtonEnumerator(duration));
    }

    private IEnumerator DisableAllBBUIButtonEnumerator(float duration)
    {
        yield return new WaitForSeconds(duration);

        SettingBBUIButton.isClickAvaiable = true;
    }

    public void DisableButton(float duration, UnityAction callback1, UnityAction callback2)
    {
        StartCoroutine(DisableButtonEnumerator(duration, callback1, callback2));
    }

    public IEnumerator DisableButtonEnumerator(float duration, UnityAction callback1, UnityAction callback2)
    {
        callback1.Invoke();
        yield return new WaitForSeconds(duration);
        callback2.Invoke();
    }
}