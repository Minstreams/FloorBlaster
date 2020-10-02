using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LabelRangeAttribute))]
public class LabelRangeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label) + 12;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var lra = (LabelRangeAttribute)attribute;
        Rect windowRect = new Rect(position.x, position.y + 2, position.width, position.height - 4);
        Rect propRect = new Rect(position.x + 4, position.y + 6, position.width - 8, position.height - 12);
        GUI.Box(windowRect, GUIContent.none, "button");
        if (!lra.Const || !EditorApplication.isPlaying)
        {
            EditorGUI.Slider(propRect, property, lra.Left, lra.Right, new GUIContent(lra.Disable ? property.displayName : lra.Label));
        }
        else if (Event.current.type == EventType.Repaint)
        {
            // 播放状态下禁止编辑
            var tc = GUI.color;
            GUI.color = Color.gray;
            EditorGUI.Slider(propRect, property, lra.Left, lra.Right, new GUIContent(lra.Label));
            GUI.color = tc;
        }
    }
}
