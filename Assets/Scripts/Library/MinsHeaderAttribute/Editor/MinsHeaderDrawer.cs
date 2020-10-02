using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinsHeaderAttribute))]
public class MinsHeaderDrawer : DecoratorDrawer
{
    private MinsHeaderAttribute sa { get { return attribute as MinsHeaderAttribute; } }
    private GUIStyle style = null;
    private float width = 512;
    private GUIStyle Style
    {
        get
        {
            if (style == null)
            {
                style = new GUIStyle(sa.Style);
                if (sa.Style.StartsWith("flow"))
                {
                    style.fontSize = 14;
                    style.fontStyle = FontStyle.Bold;
                    style.padding = new RectOffset(0, 0, 8, 8);
                    style.contentOffset = Vector2.zero;
                }
                if (sa.Style == "MeTimeLabel")
                {
                    style.wordWrap = true;
                }
                
            }
            return style;
        }
    }
    public override float GetHeight()
    {
        float height = Style.CalcHeight(new GUIContent(sa.Summary), width) + (sa.Style.StartsWith("Channel") ? 2 : 8);
        return height;
    }
    public override void OnGUI(Rect position)
    {
        if (Event.current.type == EventType.Repaint && width != position.width) width = position.width;
        GUIContent summary = new GUIContent(sa.Summary);
        var h = Style.CalcHeight(summary, width);
        Rect headerRect = new Rect(position.x, position.y + 8, position.width, h);
        EditorGUI.LabelField(headerRect, summary, Style);
    }
}
