using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label) + 12;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var la = (LabelAttribute)attribute;
        Rect windowRect = new Rect(position.x, position.y + 2, position.width, position.height - 4);
        Rect propRect = new Rect(position.x + 4, position.y + 6, position.width - 8, position.height - 12);
        GUI.Box(windowRect, GUIContent.none, property.propertyType == SerializedPropertyType.Generic ? "FrameBox" : "button");
        if (!la.Const || !EditorApplication.isPlaying)
        {
            EditorGUI.PropertyField(propRect, property, new GUIContent(la.Disable ? property.displayName : (property.displayName.StartsWith("Element") ? la.Label + property.displayName.Substring(7) : la.Label)), true);
        }
        else if (Event.current.type == EventType.Repaint)
        {
            // 播放状态下禁止编辑
            var tc = GUI.color;
            GUI.color = Color.gray;
            EditorGUI.PropertyField(propRect, property, new GUIContent(la.Disable ? property.displayName : (property.displayName.StartsWith("Element") ? la.Label + property.displayName.Substring(7) : la.Label)), true);
            GUI.color = tc;
        }
    }
}
