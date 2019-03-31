using UnityEngine;

public class Log
{
    private const string TIME_FORMAT = "[{0}] ";

    public string logString;
    public string stackTrace;
    public LogType type;
    public string upperText;
    public string lowerText;
    public bool isSelected;
    public int collapseCount;

    public Log(string logString, string stackTrace, LogType type)
    {
        this.logString = logString;
        this.stackTrace = stackTrace;
        this.type = type;

        upperText = GetTime() + logString + "\n" + stackTrace.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries)[0];
        lowerText = logString + "\n" + stackTrace;
        isSelected = false;
        collapseCount = 0;
    }

    private string GetTime()
    {
        return string.Format(TIME_FORMAT, System.DateTime.Now.ToString("HH:mm:ss"));
    }
}