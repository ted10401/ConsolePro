using UnityEngine;
using UnityEditor;

public class ConsoleProDetailPanel : BaseConsoleProPanel
{
    private ConsoleProResizedPanel m_consoleProResizePanel;
    private GUIStyle m_style;
    private Rect m_rect;
    private Vector2 m_scrollPosition;

    public ConsoleProDetailPanel(EditorWindow editorWindow, LogMessageReceiver logMessageReceiver, ConsoleProResizedPanel consoleProResizePanel) : base(editorWindow, logMessageReceiver)
    {
        m_consoleProResizePanel = consoleProResizePanel;

        m_style = new GUIStyle();
        m_style.normal.textColor = Color.black;
        m_style.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/projectbrowsericonareabg.png") as Texture2D;
    }

    public override void OnGUI()
    {
        m_rect = new Rect(1, (m_editorWindow.position.height * m_consoleProResizePanel.sizeRatio) + ConsoleProResizedPanel.PANEL_HEIGHT / 2, m_editorWindow.position.width, (m_editorWindow.position.height * (1 - m_consoleProResizePanel.sizeRatio)) - ConsoleProResizedPanel.PANEL_HEIGHT / 2);

        GUILayout.BeginArea(m_rect);
        using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(m_scrollPosition))
        {
            m_scrollPosition = scrollViewScope.scrollPosition;

            if (m_logMessageReceiver.selectedLog != null)
            {
                GUILayout.TextArea(m_logMessageReceiver.selectedLog.detailLogString, m_style);
            }
        }
        GUILayout.EndArea();
    }
}
