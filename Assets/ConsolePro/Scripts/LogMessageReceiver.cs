using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class LogMessageReceiver
{
    public bool toggleCollapse { get { return m_toggleCollapse; } set { m_toggleCollapse = value; UpdateLogs(); } }
    private bool m_toggleCollapse;

    public bool toggleLog { get { return m_toggleLog; } set { m_toggleLog = value; UpdateLogs(); } }
    private bool m_toggleLog;

    public bool toggleWarning { get { return m_toggleWarning; } set { m_toggleWarning = value; UpdateLogs(); } }
    private bool m_toggleWarning;

    public bool toggleError { get { return m_toggleError; } set { m_toggleError = value; UpdateLogs(); } }
    private bool m_toggleError;

    public string searchFilter { get { return m_searchFilter; } set { m_searchFilter = value; UpdateLogs(); } }
    private string m_searchFilter;

    public int logCount;
    public int warningCount;
    public int errorCount;
    public int collapseLogCount;
    public int collapseWarningCount;
    public int collapseErrorCount;
    public List<Log> filterLogs
    {
        get
        {
            if (string.IsNullOrEmpty(searchFilter))
            {
                return m_toggleLogs;
            }
            else
            {
                return m_toggleLogs.FindAll(templateLog => templateLog.condition.Contains(searchFilter));
            }
        }
    }
    public Log selectedLog;

    private int m_mainThreadID;
    private Action<Log> m_onLogMessageReceived;

    private List<Log> m_allLogs = new List<Log>();
    private List<Log> m_collapseLogs = new List<Log>();
    private List<Log> m_toggleLogs = new List<Log>();

    public LogMessageReceiver(Action<Log> onLogMessageReceived)
    {
        m_onLogMessageReceived = onLogMessageReceived;
        m_mainThreadID = Thread.CurrentThread.ManagedThreadId;

        Application.logMessageReceived += OnLogMessageReceived;
        Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
    }

    public void Destroy()
    {
        Application.logMessageReceived -= OnLogMessageReceived;
        Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
    }

    public void Clear()
    {
        m_allLogs.Clear();
        logCount = 0;
        warningCount = 0;
        errorCount = 0;

        m_collapseLogs.Clear();
        collapseLogCount = 0;
        collapseWarningCount = 0;
        collapseErrorCount = 0;

        m_toggleLogs.Clear();
        filterLogs.Clear();

        selectedLog = null;

        UpdateLogs();
    }

    private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (m_mainThreadID != Thread.CurrentThread.ManagedThreadId)
        {
            return;
        }

        Log log = new Log(condition, stackTrace, type);
        UpdateLogs(log);
    }

    public void OnLogMessageReceivedThreaded(string condition, string stackTrace, LogType type)
    {
        if (m_mainThreadID == Thread.CurrentThread.ManagedThreadId)
        {
            return;
        }

        Log log = new Log(condition, stackTrace, type);
        UpdateLogs(log);
    }

    private void UpdateLogs(Log log = null)
    {
        UpdateAllLogs(log);
        UpdateCollapseLogs(log);
        UpdateToggleLogs();

        m_onLogMessageReceived?.Invoke(log);
    }

    private void UpdateAllLogs(Log log)
    {
        if (log == null)
        {
            return;
        }

        m_allLogs.Add(log);

        switch (log.type)
        {
            case LogType.Log:
                logCount++;
                break;
            case LogType.Warning:
                warningCount++;
                break;
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
                errorCount++;
                break;
        }
    }

    private void UpdateCollapseLogs(Log log)
    {
        if (log == null)
        {
            return;
        }

        Log cacheLog = m_collapseLogs.Find((Log obj) => obj.condition == log.condition && obj.stackTrace == log.stackTrace && obj.type == log.type);
        if (cacheLog != null)
        {
            cacheLog.collapseCount++;
        }
        else
        {
            log.collapseCount = 1;
            m_collapseLogs.Add(log);

            switch (log.type)
            {
                case LogType.Log:
                    collapseLogCount++;
                    break;
                case LogType.Warning:
                    collapseWarningCount++;
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    collapseErrorCount++;
                    break;
            }
        }
    }

    private void UpdateToggleLogs()
    {
        List<Log> logs = new List<Log>();
        if (toggleCollapse)
        {
            logs.AddRange(m_collapseLogs);
        }
        else
        {
            logs.AddRange(m_allLogs);
        }

        m_toggleLogs.Clear();

        Log cacheLog = null;
        for (int i = 0; i < logs.Count; i++)
        {
            cacheLog = logs[i];
            switch (cacheLog.type)
            {
                case LogType.Log:
                    if (toggleLog)
                    {
                        m_toggleLogs.Add(cacheLog);
                    }
                    break;
                case LogType.Warning:
                    if (toggleWarning)
                    {
                        m_toggleLogs.Add(cacheLog);
                    }
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    if (toggleError)
                    {
                        m_toggleLogs.Add(cacheLog);
                    }
                    break;
            }
        }
    }
}
