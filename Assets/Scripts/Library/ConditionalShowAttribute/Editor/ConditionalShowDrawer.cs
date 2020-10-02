using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System;

[CustomPropertyDrawer(typeof(ConditionalShowAttribute))]
public class ConditionalShowDrawer : PropertyDrawer
{
    private UnityEditorInternal.UnityEventDrawer ueDrawer;
    private UnityEditorInternal.UnityEventDrawer UEDrawer
    {
        get
        {
            if (ueDrawer == null)
            {
                ueDrawer = new UnityEditorInternal.UnityEventDrawer();
            }
            return ueDrawer;
        }
    }

    ConditionalShowAttribute condSAtt { get { return (ConditionalShowAttribute)attribute; } }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (IsConditionMet(property))
        {
            //条件满足，开始绘制
            if (property.type.EndsWith("Event")) UEDrawer.OnGUI(position, property, label);
            else
            {
                LabelDrawer.DrawLabel(position, property, condSAtt.Label);
            }
        }
        else if (condSAtt.AlwaysShow && Event.current.type == EventType.Repaint)
        {
            var tc = GUI.color;
            GUI.color = Color.gray;
            if (property.type.EndsWith("Event")) UEDrawer.OnGUI(position, property, label);
            else
            {
                LabelDrawer.DrawLabel(position, property, condSAtt.Label);
            }
            GUI.color = tc;
        }
    }
    private bool IsConditionMet(SerializedProperty property)
    {
        if (condSAtt.Disabled) return false;
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(condSAtt.ConditionalIntField);
        if (sourcePropertyValue == null)
        {
            Debug.LogWarning("ConditionalShowAttribute 指向了一个不存在的条件字段: " + condSAtt.ConditionalIntField);
            return false;
        }
        int intVal = sourcePropertyValue.propertyType == SerializedPropertyType.Boolean ? (sourcePropertyValue.boolValue ? 1 : 0) : sourcePropertyValue.intValue;
        for (int i = 0; i < condSAtt.ExpectedValues.Length; i++)
        {
            if (condSAtt.ExpectedValues[i] == intVal) return true;
        }
        return false;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (IsConditionMet(property) || condSAtt.AlwaysShow)
        {
            if (property.type.EndsWith("Event")) return UEDrawer.GetPropertyHeight(property, label);
            return LabelDrawer.GetHeight(property, label);
        }
        return -EditorGUIUtility.standardVerticalSpacing;
    }
}
