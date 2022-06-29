
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class UIDisplayMessage : MonoBehaviour
{
    public UITable DisplayMessageRect;
    public UINotificationToast MessagePrefab;

    List<(float timestamp, float delay, string message, UINotificationToast notification)> pendingMessages;

    void Awake()
    {
        EventUtil.AddListener<DisplayMessageEvent>(OnDisplayMessageEvent);
        pendingMessages = new List<(float, float, string, UINotificationToast)>();
    }

    void OnDisplayMessageEvent(DisplayMessageEvent evt)
    {
        UINotificationToast notification = Instantiate(MessagePrefab, DisplayMessageRect.transform).GetComponent<UINotificationToast>();
        pendingMessages.Add((Time.time, evt.DelayBeforeDisplay, evt.Message, notification));
    }

    void Update()
    {
        foreach (var message in pendingMessages)
        {
            if (Time.time - message.timestamp > message.delay)
            {
                message.Item4.Initialize(message.message);
                DisplayMessage(message.notification);
            }
        }

        // Clear deprecated messages
        pendingMessages.RemoveAll(x => x.notification.Initialized);
    }

    void DisplayMessage(UINotificationToast notification)
    {
        DisplayMessageRect.UpdateTable(notification.gameObject);
        //StartCoroutine(MessagePrefab.ReturnWithDelay(notification.gameObject, notification.TotalRunTime));
    }

    void OnDestroy()
    {
        EventUtil.RemoveListener<DisplayMessageEvent>(OnDisplayMessageEvent);
    }
}

