using UnityEditor;

public abstract class BaseConsoleProPanel
{
    protected EditorWindow m_editorWindow;
    protected LogMessageReceiver m_logMessageReceiver;

    public BaseConsoleProPanel(EditorWindow editorWindow, LogMessageReceiver logMessageReceiver)
    {
        m_editorWindow = editorWindow;
        m_logMessageReceiver = logMessageReceiver;
    }

    public abstract void OnGUI();
}
