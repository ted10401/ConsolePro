using UnityEngine;
using UnityEditor;

public class ConsoleProUpperPanel : BaseConsoleProPanel
{
    private readonly Texture2D m_logIcon;
    private readonly Texture2D m_warningIcon;
    private readonly Texture2D m_errorIcon;
    private readonly Texture2D m_logBackgroundOdd;
    private readonly Texture2D m_logBackgroundEven;
    private readonly Texture2D m_logBackgroundSelected;
    private GUIStyle m_logStyle;
    private GUIStyle m_collapseStyle;

    private ConsoleProResizePanel m_consoleProResizePanel;

    private Rect m_rect;
    private Vector2 m_scrollPosition;

    public ConsoleProUpperPanel(EditorWindow editorWindow, LogMessageReceiver logMessageReceiver, ConsoleProResizePanel consoleProResizePanel) : base(editorWindow, logMessageReceiver)
    {
        m_consoleProResizePanel = consoleProResizePanel;

        m_logIcon = EditorGUIUtility.Load("icons/console.infoicon.png") as Texture2D;
        m_warningIcon = EditorGUIUtility.Load("icons/console.warnicon.png") as Texture2D;
        m_errorIcon = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D;
        m_logBackgroundOdd = EditorGUIUtility.Load("builtin skins/lightskin/images/cn entrybackodd.png") as Texture2D;
        m_logBackgroundEven = EditorGUIUtility.Load("builtin skins/lightskin/images/cnentrybackeven.png") as Texture2D;
        m_logBackgroundSelected = EditorGUIUtility.Load("builtin skins/lightskin/images/menuitemhover.png") as Texture2D;

        m_logStyle = new GUIStyle();
        m_logStyle.normal.textColor = Color.black;

        m_collapseStyle = new GUIStyle();
        m_collapseStyle.normal.textColor = Color.black;
        m_collapseStyle.alignment = TextAnchor.MiddleRight;
    }

    public void SetScrollPosition(Vector2 value)
    {
        m_scrollPosition = value;
    }

    private int m_lastCount;
    public override void OnGUI()
    {
        m_rect = new Rect(0, ConsoleProTitlePanel.PANEL_HEIGHT - 1.5f, m_editorWindow.position.width, m_editorWindow.position.height * m_consoleProResizePanel.sizeRatio - ConsoleProTitlePanel.PANEL_HEIGHT);

        GUILayout.BeginArea(m_rect);
        using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(m_scrollPosition))
        {
            m_scrollPosition = scrollViewScope.scrollPosition;

            if (m_logMessageReceiver.filterLogs != null && m_logMessageReceiver.filterLogs.Count > 0)
            {
                int count = -1;
                for (int i = 0; i < m_logMessageReceiver.filterLogs.Count; i++)
                {
                    switch (m_logMessageReceiver.filterLogs[i].type)
                    {
                        case LogType.Log:
                            if (!m_logMessageReceiver.toggleLog)
                            {
                                continue;
                            }
                            break;
                        case LogType.Warning:
                            if (!m_logMessageReceiver.toggleWarning)
                            {
                                continue;
                            }
                            break;
                        case LogType.Error:
                        case LogType.Assert:
                        case LogType.Exception:
                            if (!m_logMessageReceiver.toggleError)
                            {
                                continue;
                            }
                            break;
                    }

                    count++;

                    if (OnDrawLog(m_logMessageReceiver.filterLogs[i], count % 2 == 0))
                    {
                        if (m_logMessageReceiver.selectedLog != null)
                        {
                            m_logMessageReceiver.selectedLog.isSelected = false;
                        }

                        m_logMessageReceiver.filterLogs[i].isSelected = true;
                        m_logMessageReceiver.selectedLog = m_logMessageReceiver.filterLogs[i];
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
            m_logStyle.normal.background = m_logBackgroundSelected;
            m_collapseStyle.normal.background = m_logBackgroundSelected;
        }
        else
        {
            if (isOdd)
            {
                m_logStyle.normal.background = m_logBackgroundOdd;
                m_collapseStyle.normal.background = m_logBackgroundOdd;
            }
            else
            {
                m_logStyle.normal.background = m_logBackgroundEven;
                m_collapseStyle.normal.background = m_logBackgroundEven;
            }
        }

        switch (log.type)
        {
            case LogType.Log:
                m_cacheIcon = m_logIcon;
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

        if (m_logMessageReceiver.toggleCollapse)
        {
            selected = GUILayout.Button(new GUIContent(log.upperText, m_cacheIcon), m_logStyle, GUILayout.ExpandWidth(true), GUILayout.Height(32));
            GUILayout.Label(new GUIContent(log.collapseCount.ToString() + "  "), m_collapseStyle, GUILayout.Width(32), GUILayout.Height(32));
        }
        else
        {
            selected = GUILayout.Button(new GUIContent(log.upperText, m_cacheIcon), m_logStyle, GUILayout.ExpandWidth(true), GUILayout.Height(32));
        }

        GUILayout.EndHorizontal();

        return selected;
    }
}
