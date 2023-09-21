using Cysharp.Threading.Tasks;
using DG.Tweening;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private string content, content2;
    [SerializeField] private TextMeshProUGUI tutorialText;

    // [SerializeField] private GameObject characterBee;
    [SerializeField] private SkeletonGraphic skeletonCharacterBee;
    [SerializeField] private AnimationReferenceAsset idle, hide;

    [Header("Hide Point")] [SerializeField]
    private Transform hidePoint;

    [Header("Chat Frame")] [SerializeField]
    private Transform chatFrame;

    private Vector3 _showPoint;

    private void Start()
    {
        SetComponent();
        if (skeletonCharacterBee != null)
        {
            _showPoint = skeletonCharacterBee.transform.position;
        }

        chatFrame.GetComponent<BBUIView>().HideBehavior.onCallback_Completed.AddListener(HideChatFrame_Finished);
    }

    private void SetComponent()
    {
        if (skeletonCharacterBee == null)
            skeletonCharacterBee = transform.GetChild(0).gameObject.GetComponent<SkeletonGraphic>();

        if (chatFrame == null)
            chatFrame = transform.GetChild(1);

        if (hidePoint == null)
            hidePoint = transform.GetChild(2);
    }

    public void Enable()
    {
        SetComponent();
        chatFrame.gameObject.SetActive(true);

        if (skeletonCharacterBee != null)
        {
            skeletonCharacterBee.gameObject.SetActive(true);
            var state = skeletonCharacterBee.AnimationState;
            if (state == null)
            {
                Debug.LogError("AnimationState is Null");
            }
            else
                skeletonCharacterBee.AnimationState.SetAnimation(0, idle, true);
        }

        gameObject.SetActive(true);
    }

    public async UniTask Disable()
    {
        if (gameObject.activeSelf == false) return;
        chatFrame.GetComponent<BBUIView>().HideView();
        if (skeletonCharacterBee != null)
        {
            if (!skeletonCharacterBee.gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                return;
            }
            skeletonCharacterBee.AnimationState.SetAnimation(0, hide, true);
            await UniTask.Delay((int)((hide.Animation.Duration - 0.05f) * 1000));
            gameObject.SetActive(false);
            skeletonCharacterBee.gameObject.SetActive(false);
        }

        await UniTask.DelayFrame(1);
    }

    public async UniTask HideCharacterBee()
    {
        chatFrame.GetComponent<BBUIView>().HideView();
        if (skeletonCharacterBee != null && skeletonCharacterBee.gameObject.activeSelf)
        {
            await UniTask.DelayFrame(1);
            skeletonCharacterBee.AnimationState.SetAnimation(0, hide, true);
            await UniTask.Delay((int)((hide.Animation.Duration - 0.05f) * 1000));
            skeletonCharacterBee.gameObject.SetActive(false);
        }
    }

    private async void Action()
    {
        await UniTask.DelayFrame(1);
        gameObject.SetActive(false);
        if (skeletonCharacterBee != null)
            skeletonCharacterBee.transform.position = _showPoint;
    }

    public void OnEnable()
    {
        tutorialText.text = string.Empty;
        var str = string.Empty;
        DOTween.To(() => str, x => str = x, content, 1.2f).OnUpdate(
            () => { tutorialText.text = str; }
        );
    }

    public void ShowContent2()
    {
        tutorialText.text = string.Empty;
        var str = string.Empty;
        DOTween.To(() => str, x => str = x, content2, 1.2f).OnUpdate(
            () => { tutorialText.text = str; }
        );
    }

    private void HideChatFrame_Finished()
    {
        chatFrame.gameObject.SetActive(false);
    }
}