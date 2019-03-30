using UnityEngine;
using UnityEditor;

public class ConsoleProLowerPanel : BaseConsoleProPanel
{
    private ConsoleProResizePanel m_consoleProResizePanel;
    private GUIStyle m_textAreaStyle;

    public ConsoleProLowerPanel(EditorWindow editorWindow, LogMessageReceiver logMessageReceiver, ConsoleProResizePanel consoleProResizePanel) : base(editorWindow, logMessageReceiver)
    {
        m_consoleProResizePanel = consoleProResizePanel;

        m_textAreaStyle = new GUIStyle();
        m_textAreaStyle.normal.textColor = Color.black;
        m_textAreaStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/projectbrowsericonareabg.png") as Texture2D;
    }

    private Rect m_lowerPanelRect;
    private Vector2 m_lowerPanelScrollPosition;
    public override void OnGUI()
    {
        m_lowerPanelRect = new Rect(1, (m_editorWindow.position.height * m_consoleProResizePanel.sizeRatio) + ConsoleProResizePanel.PANEL_HEIGHT / 2, m_editorWindow.position.width, (m_editorWindow.position.height * (1 - m_consoleProResizePanel.sizeRatio)) - ConsoleProResizePanel.PANEL_HEIGHT / 2);

        GUILayout.BeginArea(m_lowerPanelRect);
        using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(m_lowerPanelScrollPosition))
        {
            m_lowerPanelScrollPosition = scrollViewScope.scrollPosition;

            if (m_logMessageReceiver.selectedLog != null)
            {
                GUILayout.TextArea(m_logMessageReceiver.selectedLog.lowerText, m_textAreaStyle);
            }
        }
        GUILayout.EndArea();
    }
}
