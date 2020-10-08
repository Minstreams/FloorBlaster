using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// 通知系统，用于在游戏中显示通知
    /// </summary>
    public class NotificationSystem : SubSystem<NotificationSystemSetting>
    {
        public static event System.Action<string> onShowNotification;


        //API---------------------------------
        public static void ShowNotification(string text)
        {
            onShowNotification?.Invoke(text);
        }
    }
}
