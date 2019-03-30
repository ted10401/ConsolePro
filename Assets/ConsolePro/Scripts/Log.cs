using UnityEngine;

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