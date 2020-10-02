using System;
using UnityEngine;

/// <summary>
/// 代替原变量标签
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class LabelRangeAttribute : PropertyAttribute
{
    /// <summary>
    /// 标签内容
    /// </summary>
    public string Label = "";
    /// <summary>
    /// 常量在游戏中不允许改变
    /// </summary>
    public bool Const = false;
    public bool Disable = false;

    public float Left;
    public float Right;

    public LabelRangeAttribute(float left, float right)
    {
        this.Disable = true;
        this.Left = left;
        this.Right = Math.Max(left, right);
    }
    public LabelRangeAttribute(string label, float left, float right)
    {
        this.Label = label;
        this.Left = left;
        this.Right = Math.Max(left, right);
    }
    public LabelRangeAttribute(string label, float left, float right, bool isConst)
    {
        this.Label = label;
        this.Left = left;
        this.Right = Math.Max(left, right);
        this.Const = isConst;
    }
}