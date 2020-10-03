using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return GetHeight(property, label);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var la = (LabelAttribute)attribute;
        DrawLabel(position, property, la.Label, la.Const);
    }

    // static API
    public static void DrawLabel(Rect position, SerializedProperty property, string label, bool isConst = false)
    {
        Rect windowRect = new Rect(position.x, position.y + 2, position.width, position.height - 4);
        Rect propRect = new Rect(position.x + 4, position.y + 6, position.width - 8, position.height - 12);

        if (!isConst || !EditorApplication.isPlaying)
        {
            GUI.Box(windowRect, GUIContent.none, property.propertyType == SerializedPropertyType.Generic ? "FrameBox" : "button");
            EditorGUI.PropertyField(propRect, property, new GUIContent(string.IsNullOrEmpty(label) ? property.displayName : (property.displayName.StartsWith("Element") ? label + property.displayName.Substring(7) : label)), true);
        }
        else if (Event.current.type == EventType.Repaint)
        {
            // 播放状态下禁止编辑
            var tc = GUI.color;
            GUI.color = Color.gray;
            GUI.Box(windowRect, GUIContent.none, property.propertyType == SerializedPropertyType.Generic ? "FrameBox" : "button");
            EditorGUI.PropertyField(propRect, property, new GUIContent(string.IsNullOrEmpty(label) ? property.displayName : (property.displayName.StartsWith("Element") ? label + property.displayName.Substring(7) : label)), true);
            GUI.color = tc;
        }
    }

    public static float GetHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label) + 12;
    }
}
