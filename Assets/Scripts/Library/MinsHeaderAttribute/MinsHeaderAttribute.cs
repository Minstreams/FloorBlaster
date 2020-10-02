using System;
using UnityEngine;

/// <summary>
/// 标题/注释/小节等，并可以指定Style
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class MinsHeaderAttribute : PropertyAttribute
{
    //条件字段，必须是整型
    public string Summary = "";
    public string Style = "label";

    public MinsHeaderAttribute(string summary)
    {
        this.Summary = summary;
    }
    public MinsHeaderAttribute(string summary, string style)
    {
        this.Summary = summary;
        this.Style = style;
    }
    public MinsHeaderAttribute(string summary, string style, int order)
    {
        this.Summary = summary;
        this.Style = style;
        this.order = order;
    }
    public MinsHeaderAttribute(string summary, SummaryType summaryType)
    {
        this.Summary = summary;
        this.Style = getStyle(summaryType);
    }
    public MinsHeaderAttribute(string summary, SummaryType summaryType, int order)
    {
        this.Summary = summary;
        this.Style = getStyle(summaryType);
        this.order = order;
    }
    private string getStyle(SummaryType summaryType)
    {
        switch (summaryType)
        {
            case SummaryType.Title: return "WarningOverlay";
            case SummaryType.TitleGray: return "flow node 0";
            case SummaryType.TitleBlue: return "flow node 1";
            case SummaryType.TitleCyan: return "flow node 2";
            case SummaryType.TitleGreen: return "flow node 3";
            case SummaryType.TitleYellow: return "flow node 4";
            case SummaryType.TitleOrange: return "flow node 5";
            case SummaryType.TitleRed: return "flow node 6";
            case SummaryType.SubTitle: return "AC BoldHeader";
            case SummaryType.Header: return "LODRendererRemove";
            case SummaryType.Comment: return "HelpBox";
            case SummaryType.CommentRight: return "flow varPin out";
            case SummaryType.CommentCenter: return "MeTimeLabel";
            case SummaryType.Notification: return "NotificationBackground";
            case SummaryType.PreTitleLinker: return "ChannelStripSendReturnBar";
            case SummaryType.PreTitleOperator: return "ChannelStripAttenuationBar";
            case SummaryType.PreTitleSavable: return "ChannelStripEffectBar";

        }
        return "label";
    }
}
/// <summary>
/// 注释类型
/// </summary>
public enum SummaryType
{
    /// <summary>
    /// 标题
    /// </summary>
    Title,
    TitleGray,
    TitleBlue,
    TitleCyan,
    TitleGreen,
    TitleYellow,
    TitleOrange,
    TitleRed,
    /// <summary>
    /// 次标题
    /// </summary>
    SubTitle,
    /// <summary>
    /// 小节标题
    /// </summary>
    Header,
    /// <summary>
    /// 注释
    /// </summary>
    Comment,
    /// <summary>
    /// 注释,右对齐
    /// </summary>
    CommentRight,
    /// <summary>
    /// 注释，居中
    /// </summary>
    CommentCenter,
    /// <summary>
    /// 大公告
    /// </summary>
    Notification,
    /// <summary>
    /// 标记子系统的Linker标题前缀
    /// </summary>
    PreTitleLinker,
    /// <summary>
    /// 标记子系统的Operator标题前缀
    /// </summary>
    PreTitleOperator,
    /// <summary>
    /// 标记子系统的Savable标题前缀
    /// </summary>
    PreTitleSavable,
}