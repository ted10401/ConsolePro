﻿using UnityEngine;
using UnityEditor;

public class ConsolePro : EditorWindow
{
    [MenuItem("Window/Console Pro")]
    private static void OpenWindow()
    {
        ConsolePro window = GetWindow<ConsolePro>();
        window.titleContent = new GUIContent("Console Pro", EditorGUIUtility.Load("icons/d_UnityEditor.ConsoleWindow.png") as Texture2D);
    }

    private LogMessegeReceiver m_logMessageReceiver;

    private void OnEnable()
    {
        OnTitleCached();
        OnUpperPanelCached();
        OnResizerCached();
        OnLowerPanelCached();

        m_logMessageReceiver = new LogMessegeReceiver(OnLogMessageReceived);
        EditorApplication.playModeStateChanged += EditorApplication_PlayModeStateChanged;
    }

    private Texture2D m_infoIconSmall;
    private Texture2D m_warningIconSmall;
    private Texture2D m_errorIconSmall;
    private void OnTitleCached()
    {
        m_infoIconSmall = EditorGUIUtility.Load("icons/console.infoicon.sml.png") as Texture2D;
        m_warningIconSmall = EditorGUIUtility.Load("icons/console.warnicon.sml.png") as Texture2D;
        m_errorIconSmall = EditorGUIUtility.Load("icons/console.erroricon.sml.png") as Texture2D;
    }

    private Texture2D m_errorIcon;
    private Texture2D m_warningIcon;
    private Texture2D m_infoIcon;
    private GUIStyle m_boxStyle;
    private GUIStyle m_collapseStyle;
    private Texture2D m_boxBgOdd;
    private Texture2D m_boxBgEven;
    private Texture2D m_boxBgSelected;
    private void OnUpperPanelCached()
    {
        m_infoIcon = EditorGUIUtility.Load("icons/console.infoicon.png") as Texture2D;
        m_warningIcon = EditorGUIUtility.Load("icons/console.warnicon.png") as Texture2D;
        m_errorIcon = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D;

        m_boxStyle = new GUIStyle();
        m_boxStyle.normal.textColor = Color.black;

        m_collapseStyle = new GUIStyle();
        m_collapseStyle.normal.textColor = Color.black;
        m_collapseStyle.alignment = TextAnchor.MiddleRight;

        m_boxBgOdd = EditorGUIUtility.Load("builtin skins/lightskin/images/cn entrybackodd.png") as Texture2D;
        m_boxBgEven = EditorGUIUtility.Load("builtin skins/lightskin/images/cnentrybackeven.png") as Texture2D;
        m_boxBgSelected = EditorGUIUtility.Load("builtin skins/lightskin/images/menuitemhover.png") as Texture2D;
    }

    private GUIStyle m_resizerStyle;
    private void OnResizerCached()
    {
        m_resizerStyle = new GUIStyle();
        m_resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
    }

    private GUIStyle m_textAreaStyle;
    private void OnLowerPanelCached()
    {
        m_textAreaStyle = new GUIStyle();
        m_textAreaStyle.normal.textColor = Color.black;
        m_textAreaStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/projectbrowsericonareabg.png") as Texture2D;
    }

    private void OnDisable()
    {
        if (m_logMessageReceiver != null)
        {
            m_logMessageReceiver.Destroy();
        }

        EditorApplication.playModeStateChanged -= EditorApplication_PlayModeStateChanged;
    }

    private void OnDestroy()
    {
        if(m_logMessageReceiver != null)
        {
            m_logMessageReceiver.Destroy();
        }

        EditorApplication.playModeStateChanged -= EditorApplication_PlayModeStateChanged;
    }

    private void OnLogMessageReceived(Log log = null)
    {
        if (log != null)
        {
            if (m_toggleFocusOnBottom)
            {
                m_upperPanelScrollPosition = new Vector2(0, m_logMessageReceiver.m_filterLogs.Count * 32);
            }

            switch (log.type)
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

    private void EditorApplication_PlayModeStateChanged(PlayModeStateChange obj)
    {
        if (obj == PlayModeStateChange.EnteredPlayMode)
        {
            if (m_toggleClearOnPlay)
            {
                m_logMessageReceiver.Clear();
            }
        }
    }

    private void OnGUI()
    {
        OnDrawTitle();
        OnDrawUpperPanel();
        OnDrawResizer();
        OnProcessEvents(Event.current);
        OnDrawLowerPanel();

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private const float TITLE_HEIGHT = 20f;
    private const float TEXT_FIELD_SEARCH_FILTER_WIDTH = 100f;
    private Rect m_titleRect;
    private bool m_toggleCollapse;
    private bool m_toggleClearOnPlay = true;
    private bool m_toggleErrorPause;
    private bool m_toggleFocusOnBottom;
    private string m_searchFilter;
    private bool m_toggleLog = true;
    private bool m_toggleWarning = true;
    private bool m_toggleError = true;
    private void OnDrawTitle()
    {
        m_titleRect = new Rect(0, 0, position.width, TITLE_HEIGHT);

        GUILayout.BeginArea(m_titleRect, EditorStyles.toolbar);

        GUILayout.BeginHorizontal();

        if(GUILayout.Button(new GUIContent("Clear"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
        {
            m_logMessageReceiver.Clear();
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
            m_toggleLog = GUILayout.Toggle(m_toggleLog, new GUIContent(m_logMessageReceiver.m_collapseLogCount.ToString(), m_infoIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleWarning = GUILayout.Toggle(m_toggleWarning, new GUIContent(m_logMessageReceiver.m_collapseWarningCount.ToString(), m_warningIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleError = GUILayout.Toggle(m_toggleError, new GUIContent(m_logMessageReceiver.m_collapseErrorCount.ToString(), m_errorIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        }
        else
        {
            m_toggleLog = GUILayout.Toggle(m_toggleLog, new GUIContent(m_logMessageReceiver.m_allLogCount.ToString(), m_infoIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleWarning = GUILayout.Toggle(m_toggleWarning, new GUIContent(m_logMessageReceiver.m_allWarningCount.ToString(), m_warningIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            m_toggleError = GUILayout.Toggle(m_toggleError, new GUIContent(m_logMessageReceiver.m_allErrorCount.ToString(), m_errorIconSmall), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        }

        if(EditorGUI.EndChangeCheck())
        {
            m_logMessageReceiver.SetToggleCollapse(m_toggleCollapse);
            m_logMessageReceiver.SetSearchFilter(m_searchFilter);
            m_logMessageReceiver.SetToggleLog(m_toggleLog);
            m_logMessageReceiver.SetToggleWarning(m_toggleWarning);
            m_logMessageReceiver.SetToggleError(m_toggleError);
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private Rect m_upperPanelRect;
    private Vector2 m_upperPanelScrollPosition;
    private void OnDrawUpperPanel()
    {
        m_upperPanelRect = new Rect(0, TITLE_HEIGHT - 1.5f, position.width, position.height * m_sizeRatio - TITLE_HEIGHT - RESIZER_HEIGHT / 2);

        GUILayout.BeginArea(m_upperPanelRect);
        using(GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(m_upperPanelScrollPosition))
        {
            m_upperPanelScrollPosition = scrollViewScope.scrollPosition;

            if (m_logMessageReceiver.m_filterLogs != null && m_logMessageReceiver.m_filterLogs.Count > 0)
            {
                int count = -1;
                for (int i = 0; i < m_logMessageReceiver.m_filterLogs.Count; i++)
                {
                    switch (m_logMessageReceiver.m_filterLogs[i].type)
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

                    if (OnDrawLog(m_logMessageReceiver.m_filterLogs[i], count % 2 == 0))
                    {
                        if (m_logMessageReceiver.m_selectedLog != null)
                        {
                            m_logMessageReceiver.m_selectedLog.isSelected = false;
                        }

                        m_logMessageReceiver.m_filterLogs[i].isSelected = true;
                        m_logMessageReceiver.m_selectedLog = m_logMessageReceiver.m_filterLogs[i];
                        GUI.changed = true;
                    }
                }
            }
        }
        GUILayout.EndArea();
    }

    private Texture2D m_cacheIcon;
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
                m_cacheIcon = m_infoIcon;
                break;
            case LogType.Warning:
                m_cacheIcon = m_warningIcon;
                break;
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
                m_cacheIcon = m_errorIcon;
                break;
        }

        GUILayout.BeginHorizontal();

        bool selected = false;

        if(m_toggleCollapse)
        {
            selected = GUILayout.Button(new GUIContent(log.upperText, m_cacheIcon), m_boxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(32));
            GUILayout.Label(new GUIContent(log.collapseCount.ToString() + "  "), m_collapseStyle, GUILayout.Width(32), GUILayout.Height(32));
        }
        else
        {
            selected = GUILayout.Button(new GUIContent(log.upperText, m_cacheIcon), m_boxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(32));
        }

        GUILayout.EndHorizontal();

        return selected;
    }

    private const float RESIZER_HEIGHT = 1f;
    private Rect m_resizerRect;
    private void OnDrawResizer()
    {
        m_resizerRect = new Rect(0, (position.height * m_sizeRatio) - RESIZER_HEIGHT / 2, position.width, RESIZER_HEIGHT);

        GUILayout.BeginArea(new Rect(m_resizerRect.position, new Vector2(position.width, RESIZER_HEIGHT)), m_resizerStyle);
        GUILayout.EndArea();

        EditorGUIUtility.AddCursorRect(m_resizerRect, MouseCursor.ResizeVertical);
    }

    private bool m_isResizing;
    private float m_sizeRatio;
    private void OnProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
            case EventType.MouseDrag:
                if (e.button == 0 && m_resizerRect.Contains(e.mousePosition))
                {
                    m_isResizing = true;
                }
                break;
            case EventType.MouseUp:
                m_isResizing = false;
                break;
        }

        if (m_isResizing)
        {
            if (e.mousePosition.y < 50)
            {
                m_sizeRatio = 50 / position.height;
            }
            else if (position.height - e.mousePosition.y < 30)
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

    private Rect m_lowerPanelRect;
    private Vector2 m_lowerPanelScrollPosition;
    private void OnDrawLowerPanel()
    {
        m_lowerPanelRect = new Rect(0, (position.height * m_sizeRatio) + RESIZER_HEIGHT / 2, position.width, (position.height * (1 - m_sizeRatio)) - RESIZER_HEIGHT / 2);

        GUILayout.BeginArea(m_lowerPanelRect);
        using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(m_upperPanelScrollPosition))
        {
            m_lowerPanelScrollPosition = scrollViewScope.scrollPosition;

            if (m_logMessageReceiver.m_selectedLog != null)
            {
                GUILayout.TextArea(m_logMessageReceiver.m_selectedLog.lowerText, m_textAreaStyle);
            }
        }
        GUILayout.EndArea();
    }
}