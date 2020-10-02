using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class EnumMapDrawer<ET> : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var list = GetList(property);
        int count = list.arraySize;
        return Mathf.Max(2, count + 1) * (EditorGUIUtility.singleLineHeight + 14) + 12;
    }

    private SerializedProperty GetList(SerializedProperty property)
    {
        var count = System.Enum.GetNames(typeof(ET)).Length;
        var list = property.FindPropertyRelative("list");
        while (list.arraySize < count)
        {
            list.InsertArrayElementAtIndex(list.arraySize);
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
        while (list.arraySize > count)
        {
            list.DeleteArrayElementAtIndex(list.arraySize - 1);
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
        return list;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect windowRect = new Rect(position.x, position.y + 4, position.width, position.height - 8);
        Rect propRect = new Rect(position.x + 12, position.y + 13, position.width - 24, position.height - 22);
        GUI.Box(windowRect, GUIContent.none, "GroupBox");

        var list = GetList(property);
        int count = list.arraySize;
        if (count == 0)
        {
            EditorGUI.LabelField(propRect, "Empty...", "FrameBox");
        }
        else
        {
            var enums = System.Enum.GetNames(typeof(ET));
            propRect.height = EditorGUIUtility.singleLineHeight;
            Rect outerRect = new Rect(propRect.x - 6, propRect.y - 4, propRect.width + 12, propRect.height + 8);
            Rect preRect = new Rect(outerRect.x - 2, outerRect.y + 7, 14, 14);
            Rect startRect = new Rect(outerRect.x + 2, outerRect.y, outerRect.width + 14, outerRect.height);
            GUI.Label(startRect, label, "AnimationEventTooltip");
            for (int i = 0; i < enums.Length; ++i)
            {
                propRect.y += EditorGUIUtility.singleLineHeight + 14;
                outerRect.y += EditorGUIUtility.singleLineHeight + 14;
                preRect.y += EditorGUIUtility.singleLineHeight + 14;
                GUI.Button(preRect, GUIContent.none, "verticalSliderThumb");
                GUI.Box(outerRect, GUIContent.none, "window");
                EditorGUI.PropertyField(propRect, list.GetArrayElementAtIndex(i), new GUIContent(enums[i]));
            }
        }

        EditorGUI.EndProperty();
    }
}
