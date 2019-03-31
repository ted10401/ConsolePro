using UnityEngine;
using UnityEditor;

public class ConsoleProResizedPanel : BaseConsoleProPanel
{
    public const float PANEL_HEIGHT = 1f;
    public float sizeRatio = 0.5f;

    private Rect m_rect;
    private GUIStyle m_gUIStyle;
    private bool m_isResizing;

    public ConsoleProResizedPanel(EditorWindow editorWindow, LogMessageReceiver logMessageReceiver) : base(editorWindow, logMessageReceiver)
    {
        m_gUIStyle = new GUIStyle();
        m_gUIStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
    }

    public override void OnGUI()
    {
        m_rect = new Rect(0, (m_editorWindow.position.height * sizeRatio) - PANEL_HEIGHT / 2, m_editorWindow.position.width, PANEL_HEIGHT);

        GUILayout.BeginArea(new Rect(m_rect.position, new Vector2(m_editorWindow.position.width, PANEL_HEIGHT)), m_gUIStyle);
        GUILayout.EndArea();

        EditorGUIUtility.AddCursorRect(m_rect, MouseCursor.ResizeVertical);

        OnProcessEvents(Event.current);
    }

    private void OnProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
            case EventType.MouseDrag:
                if (e.button == 0 && m_rect.Contains(e.mousePosition))
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
                sizeRatio = 50 / m_editorWindow.position.height;
            }
            else if (m_editorWindow.position.height - e.mousePosition.y < 30)
            {
                sizeRatio = (m_editorWindow.position.height - 30) / m_editorWindow.position.height;
            }
            else
            {
                sizeRatio = e.mousePosition.y / m_editorWindow.position.height;
            }

            m_editorWindow.Repaint();
        }
    }
}
