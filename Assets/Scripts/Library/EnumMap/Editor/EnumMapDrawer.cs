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

    SerializedProperty GetList(SerializedProperty property)
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
        Rect windowRect = new Rect(position.x, position.y + 2, position.width, position.height - 4);
        Rect propRect = new Rect(position.x + 14, position.y + 13, position.width - 26, position.height - 22);
        GUI.Box(windowRect, GUIContent.none, "window");

        var list = GetList(property);
        int count = list.arraySize;
        if (count == 0)
        {
            EditorGUI.LabelField(propRect, "Empty...", "window");
        }
        else
        {
            var enums = System.Enum.GetNames(typeof(ET));
            propRect.height = EditorGUIUtility.singleLineHeight;
            Rect outerRect = new Rect(propRect.x - 6, propRect.y - 4, propRect.width + 12, propRect.height + 8);
            Rect preRect = new Rect(outerRect.x - 6, outerRect.y + 4, 12, 12);
            Rect startRect = new Rect(outerRect.x, outerRect.y, outerRect.width + 16, outerRect.height);
            GUI.Label(startRect, label, "AnimationEventTooltip");
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < enums.Length; ++i)
            {
                propRect.y += EditorGUIUtility.singleLineHeight + 14;
                outerRect.y += EditorGUIUtility.singleLineHeight + 14;
                preRect.y += EditorGUIUtility.singleLineHeight + 14;
                GUI.Button(preRect, GUIContent.none, "Radio");
                GUI.Box(outerRect, GUIContent.none, "window");
                EditorGUI.PropertyField(propRect, list.GetArrayElementAtIndex(i), new GUIContent(enums[i]));
            }
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("Save Assets!");
                AssetDatabase.SaveAssets();
            }
        }

        EditorGUI.EndProperty();
    }
}
