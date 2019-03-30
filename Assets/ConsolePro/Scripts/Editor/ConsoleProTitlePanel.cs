using UnityEditor;
using UnityEngine;

public class ConsoleProTitlePanel : BaseConsoleProPanel
{
    public const float PANEL_HEIGHT = 20f;
    private readonly Texture2D m_logIcon;
    private readonly Texture2D m_warningIcon;
    private readonly Texture2D m_errorIcon;

    public bool toggleClearOnPlay = true;
    public bool toggleErrorPause;
    public bool toggleLockOnBottom = true;

    private bool m_toggleCollapse;
    private string m_searchFilter;
    private bool m_toggleLog = true;
    private bool m_toggleWarning = true;
    private bool m_toggleError = true;

    public ConsoleProTitlePanel(EditorWindow editorWindow, LogMessageReceiver logMessageReceiver) : base(editorWindow, logMessageReceiver)
    {
        m_logIcon = EditorGUIUtility.Load("icons/console.infoicon.sml.png") as Texture2D;
        m_warningIcon = EditorGUIUtility.Load("icons/console.warnicon.sml.png") as Texture2D;
        m_errorIcon = EditorGUIUtility.Load("icons/console.erroricon.sml.png") as Texture2D;
    }

    private const float TEXT_FIELD_SEARCH_FILTER_WIDTH = 100f;
    private Rect m_rect;
    public override void OnGUI()
    {
        m_rect = new Rect(0, 0, m_editorWindow.position.width, PANEL_HEIGHT);

        GUILayout.BeginArea(m_rect, EditorStyles.toolbar);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(new GUIContent("Clear"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
        {
            m_logMessageReceiver.Clear();
        }

        GUILayout.Space(5);

        m_toggleCollapse = GUILayout.Toggle(m_toggleCollapse, new GUIContent("Collapse"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        toggleClearOnPlay = GUILayout.Toggle(toggleClearOnPlay, new GUIContent("Clear on Play"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        toggleErrorPause = GUILayout.Toggle(toggleErrorPause, new GUIContent("Error Pause"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        toggleLockOnBottom = GUILayout.Toggle(toggleLockOnBottom, new GUIContent("Lock on Bottom"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

        GUILayout.Space(1);

        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        m_searchFilter = GUILayout.TextField(m_searchFilter, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.Width(TEXT_FIELD_SEARCH_FILTER_WIDTH));
        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        {
            m_searchFilter = string.Empty;
            GUI.FocusControl(null);
        }
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        if (m_toggleCollapse)
        {
            m_toggleLog = GUILayout.Toggle(m_toggleLog, new GUIContent(m_logMessageReceiver.collapseLogCount.ToString(), m_logIcon), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleWarning = GUILayout.Toggle(m_toggleWarning, new GUIContent(m_logMessageReceiver.collapseWarningCount.ToString(), m_warningIcon), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleError = GUILayout.Toggle(m_toggleError, new GUIContent(m_logMessageReceiver.collapseErrorCount.ToString(), m_errorIcon), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        }
        else
        {
            m_toggleLog = GUILayout.Toggle(m_toggleLog, new GUIContent(m_logMessageReceiver.logCount.ToString(), m_logIcon), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleWarning = GUILayout.Toggle(m_toggleWarning, new GUIContent(m_logMessageReceiver.warningCount.ToString(), m_warningIcon), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleError = GUILayout.Toggle(m_toggleError, new GUIContent(m_logMessageReceiver.errorCount.ToString(), m_errorIcon), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        }

        m_logMessageReceiver.toggleCollapse = m_toggleCollapse;
        m_logMessageReceiver.searchFilter = m_searchFilter;
        m_logMessageReceiver.toggleLog = m_toggleLog;
        m_logMessageReceiver.toggleWarning = m_toggleWarning;
        m_logMessageReceiver.toggleError = m_toggleError;

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
