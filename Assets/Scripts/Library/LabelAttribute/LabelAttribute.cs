using System;
using UnityEngine;

/// <summary>
/// 代替原变量标签
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class LabelAttribute : PropertyAttribute
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

    public LabelAttribute(bool isConst = false)
    {
        this.Disable = true;
        this.Const = isConst;
    }
    public LabelAttribute(string label, bool isConst = false)
    {
        this.Label = label;
        this.Const = isConst;
    }
}