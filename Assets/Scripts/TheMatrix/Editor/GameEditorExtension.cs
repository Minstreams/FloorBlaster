using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using GameSystem;
using System.IO;
using UnityEngine.WSA;

public class GameEditorExtension : EditorWindow
{
    //这里是一些编辑器方法
    [MenuItem("MatrixTool/Open Tool Window 打开工具箱 #F1")]
    public static void OpenToolWindow()
    {
        var comfirmWindow = EditorWindow.GetWindow<GameEditorExtension>("Minstreams工具箱");
    }
    /// <summary>
    /// 导航到系统配置文件
    /// </summary>
    [MenuItem("MatrixTool/System Config 系统配置 _F2")]
    public static void NavToSystemConfig()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/Resources/System/TheMatrixSetting.asset");
    }
    /// <summary>
    /// 测试当前场景
    /// </summary>
    [MenuItem("MatrixTool/Debug Current Scene 测试当前场景 #F5")]
    public static void DebugCurrent()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            return;
        }
        EditorSceneManager.SaveOpenScenes();
        var sysScene = EditorSceneManager.OpenScene("Assets/Scenes/System.unity", OpenSceneMode.Additive);
        sysScene.GetRootGameObjects()[0].GetComponent<TheMatrix>().testAll = false;
        EditorApplication.isPlaying = true;
    }
    /// <summary>
    /// 测试全部场景
    /// </summary>
    [MenuItem("MatrixTool/Debug All Scenea 测试全部场景 _F5")]
    public static void DebugAll()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            return;
        }
        EditorSceneManager.SaveOpenScenes();
        var sysScene = EditorSceneManager.OpenScene("Assets/Scenes/System.unity", OpenSceneMode.Single);
        sysScene.GetRootGameObjects()[0].GetComponent<TheMatrix>().testAll = true;
        foreach (string sceneName in TheMatrix.Setting.gameSceneMap.list)
        {
            var a = TheMatrix.Setting;
            Debug.Log(a.gameSceneMap.list.Count);
            EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + ".unity", OpenSceneMode.AdditiveWithoutLoading);
        }
        EditorApplication.isPlaying = true;
    }
    /// <summary>
    /// 添加子系统
    /// </summary>
    public static void AddSubSystem(string name, string comment)
    {
        if (!wordReg.IsMatch(name))
        {
            EditorUtility.DisplayDialog("Minstreams工具箱", "Invalid Name", "~");
            return;
        }
        if (string.IsNullOrWhiteSpace(comment))
        {
            EditorUtility.DisplayDialog("Minstreams工具箱", "Please write some comment!", "Ok~");
            return;
        }
        if (!name.EndsWith("System")) name += "System";
        if (AssetDatabase.IsValidFolder("Assets/Scripts/SubSystem/" + name))
        {
            EditorUtility.DisplayDialog("Minstreams工具箱", name + " already Exists!", "Oh~");
            return;
        }
        AssetDatabase.CreateFolder("Assets/Scripts/SubSystem", name);
        //Setting-------------------------------
        var fSetting = File.CreateText("Assets/Scripts/SubSystem/" + name + "/" + name + "Setting.cs");
        fSetting.Write(
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Setting
    {
        [CreateAssetMenu(fileName = " + "\"" + name + "Setting\", menuName = \"系统配置文件/" + name + "Setting\"" + @")]
        public class " + name + @"Setting : ScriptableObject
        {
            //[MinsHeader(" + "\"" + name + " Setting\", SummaryType.Title, -2)]" + @"
            //[MinsHeader(" + "\"" + comment + "\", SummaryType.CommentCenter, -1)]" + @"

            //[MinsHeader(" + "\"Data\", SummaryType.Header), Space(16)]" + @"

        }
    }
}");
        fSetting.Close();

        var fSystem = File.CreateText("Assets/Scripts/SubSystem/" + name + "/" + name + ".cs");
        fSystem.Write(
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    public class " + name + @" : SubSystem<" + name + @"Setting>
    {
        //Your code here


        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            //用于控制Action初始化
            TheMatrix.onGameAwake += OnGameAwake;
            TheMatrix.onGameStart += OnGameStart;
        }
        private static void OnGameAwake()
        {
            //在进入游戏第一个场景时调用
        }
        private static void OnGameStart()
        {
            //在主场景游戏开始时和游戏重新开始时调用
        }


        //API---------------------------------
        //public static void SomeFunction(){}
    }
}
");
        fSystem.Close();

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/Scripts/SubSystems/" + name);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Minstreams工具箱", name + " created!", "Cool~");
    }
    public void CreateSettingAsset()
    {
        if (string.IsNullOrWhiteSpace(subSystemName)) return;
        if (!subSystemName.EndsWith("System")) subSystemName += "System";
        if (!AssetDatabase.IsValidFolder("Assets/Scripts/SubSystem/" + subSystemName))
        {
            EditorUtility.DisplayDialog("Minstreams工具箱", subSystemName + " doesn't exist!", "Oh");
            return;
        }
        if (File.Exists("Assets/Resources/System/" + subSystemName + "Setting.asset"))
        {
            EditorUtility.DisplayDialog("Minstreams工具箱", subSystemName + " Setting Asset already exist!", "Oh");
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/Resources/System/" + subSystemName + "Setting.asset");
            return;
        }
        Selection.selectionChanged += _CreateSettingAsset;
        NavToSystemConfig();
    }
    private void _CreateSettingAsset()
    {
        Selection.selectionChanged -= _CreateSettingAsset;
        EditorApplication.ExecuteMenuItem("Assets/Create/系统配置文件/" + subSystemName + "Setting");
        EditorUtility.DisplayDialog("Minstreams工具箱", subSystemName + "  Setting Asset created!", "Cool~");
    }
    private void ScanSubSystem()
    {
        subSystemSettings = AssetDatabase.FindAssets("SystemSetting t:ScriptableObject");
        for (int i = 0; i < subSystemSettings.Length; ++i)
        {
            subSystemSettings[i] = AssetDatabase.GUIDToAssetPath(subSystemSettings[i]);
        }
    }
    /// <summary>
    /// 添加Linker
    /// </summary>
    public static void AddLinker(string name, string title, string comment, string subSystemName = "")
    {
        if (!wordReg.IsMatch(name))
        {
            EditorUtility.DisplayDialog("Minstreams工具箱", "Invalid Name", "~");
            return;
        }
        bool isOfSubSys = !string.IsNullOrWhiteSpace(subSystemName);
        if (isOfSubSys)
        {
            if (!subSystemName.EndsWith("System")) subSystemName += "System";
            if (!AssetDatabase.IsValidFolder("Assets/Scripts/SubSystem/" + subSystemName))
            {
                EditorUtility.DisplayDialog("Minstreams工具箱", subSystemName + " doesn't exist!", "Oh");
                return;
            }
            if (!AssetDatabase.IsValidFolder("Assets/Scripts/SubSystem/" + subSystemName + "/Linker")) AssetDatabase.CreateFolder("Assets/Scripts/SubSystem/" + subSystemName, "Linker");
        }
        var f = File.CreateText("Assets/Scripts/" + (isOfSubSys ? ("SubSystem/" + subSystemName + "/") : "") + "Linker/" + name + ".cs");
        f.Write(
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Linker
    {
        " + (string.IsNullOrEmpty(comment) ? "" : (
        @"/// <summary>
        /// " + comment + @"
        /// </summary>")) + @"
        [AddComponentMenu(" + "\"" + (isOfSubSys ? ("[" + subSystemName + "]/") : "|") + "Linker/" + name + "\"" + @")]
        public class " + name + @" : MonoBehaviour
        {
#if UNITY_EDITOR" + (isOfSubSys ? @"
            [MinsHeader(" + "\"Linker of " + subSystemName + "\", SummaryType.PreTitleLinker, -1)]" : "") + @"
            [MinsHeader(" + "\"" + title + "\"" + @", SummaryType.Title" + (isOfSubSys ? "Blue" : "Cyan") + @", 0)]
            [MinsHeader(" + "\"" + comment + "\"" + @", SummaryType.CommentCenter, 1)]
            [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif

            //Data
            [MinsHeader(" + "\"Data\"" + @", SummaryType.Header, 2)]

            //Inner code here

            //Output
            [MinsHeader(" + "\"Output\"" + @", SummaryType.Header, 3)]
            public SimpleEvent output;

            //Input
            [ContextMenu(" + "\"Invoke\"" + @")]
            public void Invoke()
            {
                output?.Invoke();
            }
        }
    }
}");
        f.Close();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Minstreams工具箱", (string.IsNullOrWhiteSpace(subSystemName) ? "" : (subSystemName + "/")) + name + " created!", "Cool~");
    }
    /// <summary>
    /// 添加Operator
    /// </summary>
    public static void AddOperator(string name, string title, string comment, string subSystemName = "")
    {
        if (!wordReg.IsMatch(name))
        {
            EditorUtility.DisplayDialog("Minstreams工具箱", "Invalid Name", "~");
            return;
        }
        bool isOfSubSys = !string.IsNullOrWhiteSpace(subSystemName);
        if (isOfSubSys)
        {
            if (!subSystemName.EndsWith("System")) subSystemName += "System";
            if (!AssetDatabase.IsValidFolder("Assets/Scripts/SubSystem/" + subSystemName))
            {
                EditorUtility.DisplayDialog("Minstreams工具箱", subSystemName + " doesn't exist!", "Oh");
                return;
            }
            if (!AssetDatabase.IsValidFolder("Assets/Scripts/SubSystem/" + subSystemName + "/Operator")) AssetDatabase.CreateFolder("Assets/Scripts/SubSystem/" + subSystemName, "Operator");
        }
        var f = File.CreateText("Assets/Scripts/" + (isOfSubSys ? ("SubSystem/" + subSystemName + "/") : "") + "Operator/" + name + ".cs");
        f.Write(
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Operator
    {
        " + (string.IsNullOrEmpty(comment) ? "" : (
        @"/// <summary>
        /// " + comment + @"
        /// </summary>")) + @"
        [AddComponentMenu(" + "\"" + (isOfSubSys ? ("[" + subSystemName + "]/") : "|") + "Operator/" + name + "\"" + @")]
        public class " + name + @" : MonoBehaviour
        {
#if UNITY_EDITOR" + (isOfSubSys ? @"
            [MinsHeader(" + "\"Operator of " + subSystemName + "\", SummaryType.PreTitleOperator, -1)]" : "") + @"
            [MinsHeader(" + "\"" + title + "\"" + @", SummaryType.Title" + (isOfSubSys ? "Orange" : "Yellow") + @", 0)]
            [MinsHeader(" + "\"" + comment + "\"" + @", SummaryType.CommentCenter, 1)]
            [ConditionalShow, SerializeField] private bool useless; //在没有数据的时候让标题正常显示
#endif

            //Data
            //[MinsHeader(" + "\"Data\"" + @", SummaryType.Header, 2)]

            //Inner code here

            //Input
            //[ContextMenu(" + "\"SomeFuntion\"" + @")]
            //public void SomeFuntion(){}
        }
    }
}");
        f.Close();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Minstreams工具箱", (string.IsNullOrWhiteSpace(subSystemName) ? "" : (subSystemName + "/")) + name + " created!", "Cool~");
    }
    /// <summary>
    /// 添加Savable
    /// </summary>
    public static void AddSavable(string name, string title, string comment, string subSystemName = "")
    {
        if (!wordReg.IsMatch(name))
        {
            EditorUtility.DisplayDialog("Minstreams工具箱", "Invalid Name", "~");
            return;
        }
        bool isOfSubSys = !string.IsNullOrWhiteSpace(subSystemName);
        if (isOfSubSys)
        {
            if (!subSystemName.EndsWith("System")) subSystemName += "System";
            if (!AssetDatabase.IsValidFolder("Assets/Scripts/SubSystem/" + subSystemName))
            {
                EditorUtility.DisplayDialog("Minstreams工具箱", subSystemName + " doesn't exist!", "Oh");
                return;
            }
            if (!AssetDatabase.IsValidFolder("Assets/Scripts/SubSystem/" + subSystemName + "/Savable")) AssetDatabase.CreateFolder("Assets/Scripts/SubSystem/" + subSystemName, "Savable");
        }
        var f = File.CreateText("Assets/Scripts/" + (isOfSubSys ? ("SubSystem/" + subSystemName + "/") : "") + "Savable/" + name + ".cs");
        f.Write(
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Savable
    {
        " + (string.IsNullOrEmpty(comment) ? "" : (
        @"/// <summary>
        /// " + comment + @"
        /// </summary>")) + @"
        [CreateAssetMenu(fileName = " + "\"" + name + "\", menuName = \"Savable/" + name + "\"" + @")]
        public class " + name + @" : SavableObject
        {" + (isOfSubSys ? @"
            [MinsHeader(" + "\"SavableObject of " + subSystemName + "\", SummaryType.PreTitleSavable, -1)]" : "") + @"
            [MinsHeader(" + "\"" + title + "\"" + @", SummaryType.TitleGreen, 0)]
            [MinsHeader(" + "\"" + comment + "\"" + @", SummaryType.CommentCenter, 1)]

            //Your Data here
            [Label]
            public float data1;

            //APPLY the data to game
            public override void ApplyData()
            {
                //apply(data1);
            }

            //Collect and UPDATE data
            public override void UpdateData()
            {
                //data1 = ...
            }
        }
    }
}");
        f.Close();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Minstreams工具箱", (string.IsNullOrWhiteSpace(subSystemName) ? "" : (subSystemName + "/")) + name + " created!", "Cool~");

    }

    private static System.Text.RegularExpressions.Regex wordReg = new System.Text.RegularExpressions.Regex("^\\w+$");
    private string[] subSystemSettings = null;
    public enum EditorMode
    {
        SubSystem,
        Linker,
        Operator,
        Savable
    }
    private EditorMode editorMode;
    private string subSystemName = "";
    private string subSystemComment = "";
    private string linkerName = "";
    private string linkerTitle = "";
    private string linkerComment = "";
    private string operatorName = "";
    private string operatorTitle = "";
    private string operatorComment = "";
    private string savableName = "";
    private string savableTitle = "";
    private string savableComment = "";
    private GUIStyle headerStyle;
    private GUIStyle HeaderStyle
    {
        get
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle("ProfilerBadge");
                headerStyle.alignment = TextAnchor.MiddleCenter;
                headerStyle.fontSize = 18;
                headerStyle.fixedHeight = 32;
                headerStyle.margin = new RectOffset(2, 2, 2, 2);
            }
            return headerStyle;
        }
    }
    private GUIStyle btnStyle;
    private GUIStyle BtnStyle
    {
        get
        {
            if (btnStyle == null)
            {
                btnStyle = new GUIStyle("toolbarbutton");
                headerStyle.alignment = TextAnchor.MiddleCenter;
                btnStyle.fixedHeight = 20;
                btnStyle.margin = new RectOffset(4, 4, 4, 4);
            }
            return btnStyle;
        }
    }
    private GUIStyle labelStyle;
    private GUIStyle LabelStyle
    {
        get
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle("ColorPickerBackground");
                labelStyle.padding = new RectOffset(8, 8, 4, 4);
                labelStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
            }
            return labelStyle;
        }
    }
    private GUIStyle tabBGStyle;
    private GUIStyle TabBGStyle
    {
        get
        {
            if (tabBGStyle == null)
            {
                tabBGStyle = new GUIStyle("IN ThumbnailShadow");
                tabBGStyle.fixedHeight = 0;
            }
            return tabBGStyle;
        }
    }
    private GUIStyle tabStyle;
    private GUIStyle TabStyle
    {
        get
        {
            if (tabStyle == null)
            {
                tabStyle = new GUIStyle("PreLabel");
                tabStyle.padding = TabLabelStyle.padding;
                tabStyle.stretchHeight = false;
                tabStyle.alignment = TextAnchor.MiddleCenter;
                tabStyle.contentOffset = Vector2.zero;
                tabStyle.margin = new RectOffset(2, 2, 2, 2);
                tabStyle.fontStyle = FontStyle.Normal;
                tabStyle.normal.textColor = Color.black;
            }
            return tabStyle;
        }
    }
    private GUIStyle tabLabelStyle;
    private GUIStyle TabLabelStyle
    {
        get
        {
            if (tabLabelStyle == null)
            {
                tabLabelStyle = new GUIStyle("ShurikenEffectBg");
                tabLabelStyle.stretchHeight = false;
                tabLabelStyle.alignment = TextAnchor.MiddleCenter;
                tabLabelStyle.contentOffset = Vector2.zero;
                tabLabelStyle.margin = new RectOffset(2, 2, 2, 2);
                tabLabelStyle.fontStyle = FontStyle.Bold;
                tabLabelStyle.normal.textColor = Color.white;
            }
            return tabLabelStyle;
        }
    }
    /// <summary>
    /// 分隔符
    /// </summary>
    private void Separator()
    {
        GUILayout.Label("", "RL DragHandle", GUILayout.ExpandWidth(true));
    }
    private void SectionHeader(string title)
    {
        Separator();
        GUILayout.Label(title, HeaderStyle, GUILayout.ExpandWidth(true));
    }
    private string TextArea(string name, string target, int maxLength = 24)
    {
        string result;
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(name, "ProfilerSelectedLabel", GUILayout.ExpandWidth(false), GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.2f));
            result = GUILayout.TextField(target, maxLength, "SearchTextField");
        }
        GUILayout.EndHorizontal();
        return result;
    }
    private void Label(string text)
    {
        GUILayout.Label(text, LabelStyle);
    }



    private void OnEnable()
    {
        ScanSubSystem();
        Input.imeCompositionMode = IMECompositionMode.On;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical("GameViewBackground", GUILayout.ExpandHeight(true));
        SectionHeader("数据导航");
        if (GUILayout.Button("系统配置", BtnStyle)) NavToSystemConfig();
        if (subSystemSettings != null)
        {
            for (int i = 0; i < subSystemSettings.Length; ++i)
            {
                int start = Mathf.Max(subSystemSettings[i].LastIndexOf('/') + 1, 0);
                string subName = subSystemSettings[i].Substring(start, subSystemSettings[i].Length - start - 19);
                if (GUILayout.Button(subName, BtnStyle))
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(subSystemSettings[i]);
                    subSystemName = subName;
                    subSystemComment = "";
                }
            }
        }
        if (GUILayout.Button("刷新", BtnStyle)) ScanSubSystem();

        SectionHeader("自动测试");
        if (GUILayout.Button("测试全部场景", BtnStyle)) DebugAll();
        if (GUILayout.Button("测试当前场景", BtnStyle)) DebugCurrent();

        SectionHeader("自动化代码生成");
        GUILayout.BeginHorizontal(TabBGStyle);
        {
            if (GUILayout.Button("SubSystem", editorMode == EditorMode.SubSystem ? TabLabelStyle : TabStyle)) editorMode = EditorMode.SubSystem;
            if (GUILayout.Button("Linker", editorMode == EditorMode.Linker ? TabLabelStyle : TabStyle)) editorMode = EditorMode.Linker;
            if (GUILayout.Button("Operator", editorMode == EditorMode.Operator ? TabLabelStyle : TabStyle)) editorMode = EditorMode.Operator;
            if (GUILayout.Button("Savable", editorMode == EditorMode.Savable ? TabLabelStyle : TabStyle)) editorMode = EditorMode.Savable;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(4);
        switch (editorMode)
        {
            case EditorMode.SubSystem:
                subSystemName = TextArea("Sub System Name", subSystemName);
                subSystemComment = TextArea("Comment", subSystemComment, 64);
                Label("自动生成一个子系统 Sub System");
                Label("由于实在无法把生成代码与生成配置文件功能做到一起，生成新系统时，请依次点这两个按钮。");
                if (GUILayout.Button("Add", BtnStyle)) AddSubSystem(subSystemName, subSystemComment);
                if (GUILayout.Button("Create Setting Asset", BtnStyle)) CreateSettingAsset();
                break;
            case EditorMode.Linker:
                EditorGUI.BeginChangeCheck();
                linkerName = TextArea("Linker Name", linkerName);
                if (EditorGUI.EndChangeCheck()) linkerTitle = linkerName;
                linkerTitle = TextArea("Linker Title", linkerTitle);
                linkerComment = TextArea("Comment", linkerComment, 64);
                Label("自动生成一个连接节点 Linker，用于连接和处理数据。");
                if (GUILayout.Button("Add", BtnStyle)) AddLinker(linkerName, linkerTitle, linkerComment);
                if (GUILayout.Button("Add To Current SubSystem", BtnStyle))
                {
                    if (string.IsNullOrWhiteSpace(subSystemName)) EditorUtility.DisplayDialog("Minstreams工具箱", "请先在‘数字导航’一栏中选择子系统", "好的");
                    else AddLinker(linkerName, linkerTitle, linkerComment, subSystemName);
                }
                break;
            case EditorMode.Operator:
                EditorGUI.BeginChangeCheck();
                operatorName = TextArea("Operator Name", operatorName);
                if (EditorGUI.EndChangeCheck()) operatorTitle = operatorName;
                operatorTitle = TextArea("Operator Title", operatorTitle);
                operatorComment = TextArea("Comment", operatorComment, 64);
                Label("自动生成一个操作节点 Operator，用于执行具体动作。");
                if (GUILayout.Button("Add", BtnStyle)) AddOperator(operatorName, operatorTitle, operatorComment);
                if (GUILayout.Button("Add To Current SubSystem", BtnStyle))
                {
                    if (string.IsNullOrWhiteSpace(subSystemName)) EditorUtility.DisplayDialog("Minstreams工具箱", "请先在‘数字导航’一栏中选择子系统", "好的");
                    else AddOperator(operatorName, operatorTitle, operatorComment, subSystemName);
                }
                break;
            case EditorMode.Savable:
                EditorGUI.BeginChangeCheck();
                savableName = TextArea("Savable Name", savableName);
                if (EditorGUI.EndChangeCheck()) savableTitle = savableName;
                savableTitle = TextArea("Savable Title", savableTitle);
                savableComment = TextArea("Comment", savableComment, 64);
                Label("自动生成一个可存储对象SavableObject，用于持久化存储数据。");
                if (GUILayout.Button("Add", BtnStyle)) AddSavable(savableName, savableTitle, savableComment);
                if (GUILayout.Button("Add To Current SubSystem", BtnStyle))
                {
                    if (string.IsNullOrWhiteSpace(subSystemName)) EditorUtility.DisplayDialog("Minstreams工具箱", "请先在‘数字导航’一栏中选择子系统", "好的");
                    else AddSavable(savableName, savableTitle, savableComment, subSystemName);
                }
                break;
        }
        Separator();
        GUILayout.EndVertical();
    }
}

#region EnumMap Drawer Definition
[CustomPropertyDrawer(typeof(GameSceneMap), true)]
public class GameSceneMapDrawer : EnumMapDrawer<TheMatrix.GameScene> { }
[CustomPropertyDrawer(typeof(SoundClipMap), true)]
public class AudioClipMapDrawer : EnumMapDrawer<AudioSystem.Sound> { }
[CustomPropertyDrawer(typeof(InputKeyMap), true)]
public class InputKeyMapDrawer : EnumMapDrawer<InputSystem.InputKey> { }
#endregion