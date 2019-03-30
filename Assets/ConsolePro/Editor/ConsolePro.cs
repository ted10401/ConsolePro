using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ConsolePro : EditorWindow
{
    private const float RESIZER_HEIGHT = 1f;
    private float m_menuBarHeight = 20f;
    private const float TEXT_FIELD_SEARCH_FILTER_WIDTH = 100f;

    private Rect m_upperPanel;
    private Rect m_lowerPanel;
    private Rect m_resizer;
    private Rect m_menuBar;

    private float m_sizeRatio = 0.5f;
    private bool m_isResizing;

    private bool m_toggleCollapse;
    private bool m_toggleClearOnPlay = true;
    private bool m_toggleErrorPause;
    private bool m_toggleFocusOnBottom;
    private string m_searchFilter;

    private bool m_toggleLog = true;
    private bool m_toggleWarning = true;
    private bool m_toggleError = true;

    private Vector2 m_upperPanelScroll;
    private Vector2 m_lowerPanelScroll;

    private GUIStyle m_toolbarSearchTextField;
    private GUIStyle m_resizerStyle;
    private GUIStyle m_boxStyle;
    private GUIStyle m_collapseStyle;
    private GUIStyle m_textAreaStyle;

    private Texture2D m_icon;
    private Texture2D m_errorIcon;
    private Texture2D m_errorIconSmall;
    private Texture2D m_warningIcon;
    private Texture2D m_warningIconSmall;
    private Texture2D m_infoIcon;
    private Texture2D m_infoIconSmall;

    private Texture2D m_boxBgOdd;
    private Texture2D m_boxBgEven;
    private Texture2D m_boxBgSelected;

    private List<Log> m_allLogs = new List<Log>();
    private int m_allLogCount;
    private int m_allWarningCount;
    private int m_allErrorCount;

    private List<Log> m_collapseLogs = new List<Log>();
    private int m_collapseLogCount;
    private int m_collapseWarningCount;
    private int m_collapseErrorCount;

    private List<Log> m_toggleLogs = new List<Log>();
    private List<Log> m_filterLogs = new List<Log>();
    private Log m_selectedLog;

    [MenuItem("Window/Console Pro")]
    private static void OpenWindow()
    {
        ConsolePro window = GetWindow<ConsolePro>();
        window.titleContent = new GUIContent("Console Pro", EditorGUIUtility.Load("icons/d_UnityEditor.ConsoleWindow.png") as Texture2D);
    }

    private void OnEnable()
    {
        m_errorIcon = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D;
        m_warningIcon = EditorGUIUtility.Load("icons/console.warnicon.png") as Texture2D;
        m_infoIcon = EditorGUIUtility.Load("icons/console.infoicon.png") as Texture2D;

        m_errorIconSmall = EditorGUIUtility.Load("icons/console.erroricon.sml.png") as Texture2D;
        m_warningIconSmall = EditorGUIUtility.Load("icons/console.warnicon.sml.png") as Texture2D;
        m_infoIconSmall = EditorGUIUtility.Load("icons/console.infoicon.sml.png") as Texture2D;

        m_resizerStyle = new GUIStyle();
        m_resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

        m_boxStyle = new GUIStyle();
        m_boxStyle.normal.textColor = Color.black;

        m_collapseStyle = new GUIStyle();
        m_collapseStyle.normal.textColor = Color.black;
        m_collapseStyle.alignment = TextAnchor.MiddleRight;

        m_boxBgOdd = EditorGUIUtility.Load("builtin skins/lightskin/images/cn entrybackodd.png") as Texture2D;
        m_boxBgEven = EditorGUIUtility.Load("builtin skins/lightskin/images/cnentrybackeven.png") as Texture2D;
        m_boxBgSelected = EditorGUIUtility.Load("builtin skins/lightskin/images/menuitemhover.png") as Texture2D;

        m_textAreaStyle = new GUIStyle();
        m_textAreaStyle.normal.textColor = Color.black;
        m_textAreaStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/projectbrowsericonareabg.png") as Texture2D;

        ClearLogs();

        Application.logMessageReceived += LogMessageReceived;
        EditorApplication.playModeStateChanged += EditorApplication_PlayModeStateChanged;
    }

    void EditorApplication_PlayModeStateChanged(PlayModeStateChange obj)
    {
        if(obj == PlayModeStateChange.EnteredPlayMode)
        {
            if(m_toggleClearOnPlay)
            {
                ClearLogs();
            }
        }
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= LogMessageReceived;
        EditorApplication.playModeStateChanged -= EditorApplication_PlayModeStateChanged;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= LogMessageReceived;
        EditorApplication.playModeStateChanged -= EditorApplication_PlayModeStateChanged;
    }

    private void OnGUI()
    {
        OnDrawTitle();
        OnDrawUpperPanel();
        OnDrawLowerPanel();
        OnDrawResizer();

        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void OnDrawTitle()
    {
        m_menuBar = new Rect(0, 0, position.width, m_menuBarHeight);

        GUILayout.BeginArea(m_menuBar, EditorStyles.toolbar);

        GUILayout.BeginHorizontal();

        if(GUILayout.Button(new GUIContent("Clear"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
        {
            ClearLogs();
        }

        GUILayout.Space(5);

        EditorGUI.BeginChangeCheck();

        m_toggleCollapse = GUILayout.Toggle(m_toggleCollapse, new GUIContent("Collapse"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        m_toggleClearOnPlay = GUILayout.Toggle(m_toggleClearOnPlay, new GUIContent("Clear On Play"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        m_toggleErrorPause = GUILayout.Toggle(m_toggleErrorPause, new GUIContent("Error Pause"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        m_toggleFocusOnBottom = GUILayout.Toggle(m_toggleFocusOnBottom, new GUIContent("Focus On Bottom"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

        GUILayout.Space(1);

        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        m_searchFilter = GUILayout.TextField(m_searchFilter, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.Width(TEXT_FIELD_SEARCH_FILTER_WIDTH));
        if(GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        {
            m_searchFilter = string.Empty;
            GUI.FocusControl(null);
        }
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        if(m_toggleCollapse)
        {
            m_toggleLog = GUILayout.Toggle(m_toggleLog, new GUIContent(m_collapseLogCount.ToString(), m_infoIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleWarning = GUILayout.Toggle(m_toggleWarning, new GUIContent(m_collapseWarningCount.ToString(), m_warningIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleError = GUILayout.Toggle(m_toggleError, new GUIContent(m_collapseErrorCount.ToString(), m_errorIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        }
        else
        {
            m_toggleLog = GUILayout.Toggle(m_toggleLog, new GUIContent(m_allLogCount.ToString(), m_infoIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleWarning = GUILayout.Toggle(m_toggleWarning, new GUIContent(m_allWarningCount.ToString(), m_warningIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleError = GUILayout.Toggle(m_toggleError, new GUIContent(m_allErrorCount.ToString(), m_errorIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        }

        if(EditorGUI.EndChangeCheck())
        {
            UpdateLogs(null);
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void OnDrawUpperPanel()
    {
        m_upperPanel = new Rect(0, m_menuBarHeight - 1.5f, position.width, position.height * m_sizeRatio - m_menuBarHeight - RESIZER_HEIGHT / 2);

        GUILayout.BeginArea(m_upperPanel);

        using(GUILayout.ScrollViewScope scrollScope = new GUILayout.ScrollViewScope(m_upperPanelScroll))
        {
            m_upperPanelScroll = scrollScope.scrollPosition;

            if (m_filterLogs != null && m_filterLogs.Count > 0)
            {
                int count = -1;
                for (int i = 0; i < m_filterLogs.Count; i++)
                {
                    switch (m_filterLogs[i].type)
                    {
                        case LogType.Log:
                            if (!m_toggleLog)
                            {
                                continue;
                            }
                            break;
                        case LogType.Warning:
                            if (!m_toggleWarning)
                            {
                                continue;
                            }
                            break;
                        case LogType.Error:
                        case LogType.Assert:
                        case LogType.Exception:
                            if (!m_toggleError)
                            {
                                continue;
                            }
                            break;
                    }

                    count++;

                    if (OnDrawLog(m_filterLogs[i], count % 2 == 0))
                    {
                        if (m_selectedLog != null)
                        {
                            m_selectedLog.isSelected = false;
                        }

                        m_filterLogs[i].isSelected = true;
                        m_selectedLog = m_filterLogs[i];
                        GUI.changed = true;
                    }
                }
            }
        }
        GUILayout.EndArea();
    }

    private bool OnDrawLog(Log log, bool isOdd)
    {
        if (log.isSelected)
        {
            m_boxStyle.normal.background = m_boxBgSelected;
            m_collapseStyle.normal.background = m_boxBgSelected;
        }
        else
        {
            if (isOdd)
            {
                m_boxStyle.normal.background = m_boxBgOdd;
                m_collapseStyle.normal.background = m_boxBgOdd;
            }
            else
            {
                m_boxStyle.normal.background = m_boxBgEven;
                m_collapseStyle.normal.background = m_boxBgEven;
            }
        }

        switch (log.type)
        {
            case LogType.Log:
                m_icon = m_infoIcon;
                break;
            case LogType.Warning:
                m_icon = m_warningIcon;
                break;
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
                m_icon = m_errorIcon;
                break;
        }

        GUILayout.BeginHorizontal();

        bool selected = false;

        if(m_toggleCollapse)
        {
            selected = GUILayout.Button(new GUIContent(log.upperText, m_icon), m_boxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(32));
            GUILayout.Label(new GUIContent(log.collapseCount.ToString() + "  "), m_collapseStyle, GUILayout.Width(32), GUILayout.Height(32));
        }
        else
        {
            selected = GUILayout.Button(new GUIContent(log.upperText, m_icon), m_boxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(32));
        }

        GUILayout.EndHorizontal();

        return selected;
    }

    private void OnDrawResizer()
    {
        m_resizer = new Rect(0, (position.height * m_sizeRatio) - RESIZER_HEIGHT / 2, position.width, RESIZER_HEIGHT);

        GUILayout.BeginArea(new Rect(m_resizer.position, new Vector2(position.width, RESIZER_HEIGHT)), m_resizerStyle);
        GUILayout.EndArea();

        EditorGUIUtility.AddCursorRect(m_resizer, MouseCursor.ResizeVertical);
    }

    private void OnDrawLowerPanel()
    {
        m_lowerPanel = new Rect(0, (position.height * m_sizeRatio) + RESIZER_HEIGHT / 2, position.width, (position.height * (1 - m_sizeRatio)) - RESIZER_HEIGHT / 2);

        GUILayout.BeginArea(m_lowerPanel);
        m_lowerPanelScroll = GUILayout.BeginScrollView(m_lowerPanelScroll);

        if (m_selectedLog != null)
        {
            GUILayout.TextArea(m_selectedLog.lowerText, m_textAreaStyle);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
            case EventType.MouseDrag:
                if (e.button == 0 && m_resizer.Contains(e.mousePosition))
                {
                    m_isResizing = true;
                }
                else if(!m_upperPanel.Contains(e.mousePosition) &&
                        !m_lowerPanel.Contains(e.mousePosition) &&
                        !m_resizer.Contains(e.mousePosition))
                {
                    m_isResizing = false;
                }
                break;

            case EventType.MouseUp:
            case EventType.MouseLeaveWindow:
                m_isResizing = false;
                break;
        }

        Resize(e);
    }

    private void Resize(Event e)
    {
        if (m_isResizing)
        {
            if(e.mousePosition.y < 50)
            {
                m_sizeRatio = 50 / position.height;
            }
            else if(position.height - e.mousePosition.y < 30)
            {
                m_sizeRatio = (position.height - 30) / position.height;
            }
            else
            {
                m_sizeRatio = e.mousePosition.y / position.height;
            }

            Repaint();
        }
    }

    private void LogMessageReceived(string condition, string stackTrace, LogType type)
    {
        Log log = new Log(condition, stackTrace, type);

        UpdateLogs(log);
    }

    private void ClearLogs()
    {
        m_allLogs.Clear();
        m_allLogCount = 0;
        m_allWarningCount = 0;
        m_allErrorCount = 0;

        m_collapseLogs.Clear();
        m_collapseLogCount = 0;
        m_collapseWarningCount = 0;
        m_collapseErrorCount = 0;

        m_toggleLogs.Clear();
        m_filterLogs.Clear();

        m_selectedLog = null;

        UpdateLogs(null);
    }

    private void UpdateLogs(Log log = null)
    {
        UpdateAllLogs(log);
        UpdateCollapseLogs(log);
        UpdateToggleLogs();
        UpdateFilterLogs();

        if(log != null)
        {
            if(m_toggleFocusOnBottom)
            {
                m_upperPanelScroll = new Vector2(0, m_filterLogs.Count * 32);
            }

            switch(log.type)
            {
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    if (m_toggleErrorPause)
                    {
                        EditorApplication.isPaused = true;
                    }
                    break;
            }
        }

        Repaint();
    }

    private void UpdateAllLogs(Log log)
    {
        if(log == null)
        {
            return;
        }

        m_allLogs.Add(log);

        switch (log.type)
        {
            case LogType.Log:
                m_allLogCount++;
                break;
            case LogType.Warning:
                m_allWarningCount++;
                break;
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
                m_allErrorCount++;
                break;
        }
    }

    private void UpdateCollapseLogs(Log log)
    {
        if(log == null)
        {
            return;
        }

        Log cacheLog = m_collapseLogs.Find((Log obj) => obj.condition == log.condition && obj.stackTrace == log.stackTrace && obj.type == log.type);
        if (cacheLog != null)
        {
            cacheLog.collapseCount++;
        }
        else
        {
            log.collapseCount = 1;
            m_collapseLogs.Add(log);

            switch (log.type)
            {
                case LogType.Log:
                    m_collapseLogCount++;
                    break;
                case LogType.Warning:
                    m_collapseWarningCount++;
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    m_collapseErrorCount++;
                    break;
            }
        }
    }

    private void UpdateToggleLogs()
    {
        List<Log> logs = new List<Log>();
        if (m_toggleCollapse)
        {
            logs.AddRange(m_collapseLogs);
        }
        else
        {
            logs.AddRange(m_allLogs);
        }

        m_toggleLogs.Clear();

        Log cacheLog = null;
        for (int i = 0; i < logs.Count; i++)
        {
            cacheLog = logs[i];
            switch (cacheLog.type)
            {
                case LogType.Log:
                    if (m_toggleLog)
                    {
                        m_toggleLogs.Add(cacheLog);
                    }
                    break;
                case LogType.Warning:
                    if (m_toggleWarning)
                    {
                        m_toggleLogs.Add(cacheLog);
                    }
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    if (m_toggleError)
                    {
                        m_toggleLogs.Add(cacheLog);
                    }
                    break;
            }
        }
    }

    private void UpdateFilterLogs()
    {
        m_filterLogs.Clear();

        if (string.IsNullOrEmpty(m_searchFilter))
        {
            m_filterLogs.AddRange(m_toggleLogs);
        }
        else
        {
            m_filterLogs = m_toggleLogs.FindAll(templateLog => templateLog.condition.Contains(m_searchFilter));
        }
    }
}

public class Log
{
    private const string TIME_FORMAT = "[{0}] ";

    public string condition;
    public string stackTrace;
    public LogType type;
    public string upperText;
    public string lowerText;
    public bool isSelected;
    public int collapseCount;

    public Log(string condition, string stackTrace, LogType type)
    {
        this.condition = condition;
        this.stackTrace = stackTrace;
        this.type = type;

        upperText = GetTime() + condition + "\n" + stackTrace.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries)[0];
        lowerText = condition + "\n" + stackTrace;
        isSelected = false;
        collapseCount = 0;
    }

    private string GetTime()
    {
        return string.Format(TIME_FORMAT, System.DateTime.Now.ToString("HH:mm:ss"));
    }
}