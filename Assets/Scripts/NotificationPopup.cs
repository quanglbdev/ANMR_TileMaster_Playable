using UnityEngine;

public class NotificationPopup : MonoBehaviour
{
    public static NotificationPopup instance;

    public ItemNotification itemNotificationPrefab;

    private void Awake()
    {
        instance = this;
    }

    public void AddNotification(string content)
    {
        SoundManager.Instance.PlaySound_Notification();
        ItemNotification itemNotification = Instantiate(itemNotificationPrefab, gameObject.transform);
        itemNotification.ShowNotification(content);
    }

    public void AddNotification(string content, Vector3 position)
    {
        SoundManager.Instance.PlaySound_Notification();
        ItemNotification itemNotification = Instantiate(itemNotificationPrefab, gameObject.transform);
        itemNotification.transform.position = position;
        itemNotification.ShowNotification(content);
    }
}