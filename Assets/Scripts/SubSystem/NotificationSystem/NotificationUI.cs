using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI of Notification System
/// </summary>
public class NotificationUI : MonoBehaviour
{
    [MinsHeader("通知UI", SummaryType.TitleGray, -1)]
    [Label]
    public GUIStyle style;
    public StringEvent onShowNotification;

    string text;

    void ShowNotification(string text)
    {
        this.text = text;
        onShowNotification?.Invoke(text);
    }
    private void Awake()
    {
        NotificationSystem.onShowNotification += ShowNotification;
    }
    private void OnDestroy()
    {
        NotificationSystem.onShowNotification -= ShowNotification;
    }

    private void OnGUI()
    {
        GUILayout.Label(text, style);
    }
}
