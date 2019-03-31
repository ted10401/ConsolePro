using UnityEngine;
using UnityEditor;

public class ConsoleProEditorWindow : EditorWindow
{
    private static ConsoleProEditorWindow m_window;

    [MenuItem("Window/Console Pro")]
    private static void OpenWindow()
    {
        if(m_window == null)
        {
            m_window = GetWindow<ConsoleProEditorWindow>();
            m_window.titleContent = new GUIContent("Console Pro", EditorGUIUtility.Load("icons/d_UnityEditor.ConsoleWindow.png") as Texture2D);
        }

        m_window.Focus();
    }

    private LogMessageReceiver m_logMessageReceiver;
    private ConsoleProTitlePanel m_titlePanel;
    private ConsoleProResizedPanel m_resizedPanel;
    private ConsoleProSimplePanel m_simplePanel;
    private ConsoleProDetailPanel m_detailPanel;

    private void OnEnable()
    {
        m_logMessageReceiver = new LogMessageReceiver(OnLogMessageReceived);
        m_titlePanel = new ConsoleProTitlePanel(this, m_logMessageReceiver);
        m_resizedPanel = new ConsoleProResizedPanel(this, m_logMessageReceiver);
        m_simplePanel = new ConsoleProSimplePanel(this, m_logMessageReceiver, m_resizedPanel);
        m_detailPanel = new ConsoleProDetailPanel(this, m_logMessageReceiver, m_resizedPanel);

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
            if(m_titlePanel != null && m_titlePanel.toggleLockOnBottom)
            {
                m_simplePanel.SetScrollPosition(new Vector2(0, m_logMessageReceiver.filterLogs.Count * 32));
            }

            switch (log.type)
            {
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    if (m_titlePanel != null && m_titlePanel.toggleErrorPause)
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
            if (m_titlePanel.toggleClearOnPlay)
            {
                m_logMessageReceiver.Clear();
            }
        }
    }

    private void OnGUI()
    {
        m_titlePanel.OnGUI();
        m_simplePanel.OnGUI();
        m_resizedPanel.OnGUI();
        m_detailPanel.OnGUI();

        if(GUI.changed)
        {
            Repaint();
        }
    }
}