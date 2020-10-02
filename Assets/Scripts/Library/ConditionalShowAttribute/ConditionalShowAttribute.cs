using System;
using UnityEngine;

/// <summary>
/// 满足特定条件时才显示
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ConditionalShowAttribute : PropertyAttribute
{
    //条件字段，必须是整型
    public string ConditionalIntField = "";
    //预期值
    public int[] ExpectedValues;
    public bool Disabled = false;

    public string Label = "";
    public bool AlwaysShow = false;

    public ConditionalShowAttribute(string conditionalIntField, bool expectedValue)
    {
        this.ConditionalIntField = conditionalIntField;
        this.ExpectedValues = new int[] { expectedValue ? 1 : 0 };
    }
    public ConditionalShowAttribute(string conditionalIntField, object expectedValue)
    {
        this.ConditionalIntField = conditionalIntField;
        this.ExpectedValues = new int[] { (int)expectedValue };
    }
    public ConditionalShowAttribute(string conditionalIntField, params object[] expectedValues)
    {
        this.ConditionalIntField = conditionalIntField;
        this.ExpectedValues = new int[expectedValues.Length];
        for (int i = 0; i < expectedValues.Length; i++) this.ExpectedValues[i] = (int)expectedValues[i];
    }
    public ConditionalShowAttribute()
    {
        Disabled = true;
    }
}