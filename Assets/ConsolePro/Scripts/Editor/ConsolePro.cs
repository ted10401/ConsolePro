using UnityEngine;
using UnityEditor;

public class ConsolePro : EditorWindow
{
    private static ConsolePro m_window;

    [MenuItem("Window/Console Pro")]
    private static void OpenWindow()
    {
        if(m_window == null)
        {
            m_window = GetWindow<ConsolePro>();
            m_window.titleContent = new GUIContent("Console Pro", EditorGUIUtility.Load("icons/d_UnityEditor.ConsoleWindow.png") as Texture2D);
        }

        m_window.Focus();
    }

    private LogMessageReceiver m_logMessageReceiver;
    private ConsoleProTitlePanel m_consoleProTitlePanel;
    private ConsoleProUpperPanel m_consoleProUpperPanel;
    private ConsoleProResizePanel m_consoleProResizePanel;
    private ConsoleProLowerPanel m_consoleProLowerPanel;

    private void OnEnable()
    {
        m_logMessageReceiver = new LogMessageReceiver(OnLogMessageReceived);
        m_consoleProTitlePanel = new ConsoleProTitlePanel(this, m_logMessageReceiver);
        m_consoleProResizePanel = new ConsoleProResizePanel(this, m_logMessageReceiver);
        m_consoleProLowerPanel = new ConsoleProLowerPanel(this, m_logMessageReceiver, m_consoleProResizePanel);
        m_consoleProUpperPanel = new ConsoleProUpperPanel(this, m_logMessageReceiver, m_consoleProResizePanel);

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        if (m_logMessageReceiver != null)
        {
            m_logMessageReceiver.Destroy();
            m_logMessageReceiver = null;
        }

        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnDestroy()
    {
        if (m_logMessageReceiver != null)
        {
            m_logMessageReceiver.Destroy();
            m_logMessageReceiver = null;
        }

        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnLogMessageReceived(Log log)
    {
        if (log != null)
        {
            if(m_consoleProTitlePanel != null && m_consoleProTitlePanel.toggleFocusOnBottom)
            {
                m_consoleProUpperPanel.SetScrollPosition(new Vector2(0, m_logMessageReceiver.filterLogs.Count * 32));
            }

            switch (log.type)
            {
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    if (m_consoleProTitlePanel != null && m_consoleProTitlePanel.toggleErrorPause)
                    {
                        EditorApplication.isPaused = true;
                    }
                    break;
            }
        }

        Repaint();
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        if (obj == PlayModeStateChange.EnteredPlayMode)
        {
            if (m_consoleProTitlePanel.toggleClearOnPlay)
            {
                m_logMessageReceiver.Clear();
            }
        }
    }

    private void OnGUI()
    {
        m_consoleProTitlePanel.OnGUI();
        m_consoleProUpperPanel.OnGUI();
        m_consoleProResizePanel.OnGUI();
        m_consoleProLowerPanel.OnGUI();

        if(GUI.changed)
        {
            Repaint();
        }
    }
}