using UnityEngine;
using System;
using System.Collections.Generic;

public class LogMessegeReceiver
{
    private Action<Log> m_onLogMessageReceived;

    private List<Log> m_allLogs = new List<Log>();
    public int m_allLogCount;
    public int m_allWarningCount;
    public int m_allErrorCount;

    private List<Log> m_collapseLogs = new List<Log>();
    public int m_collapseLogCount;
    public int m_collapseWarningCount;
    public int m_collapseErrorCount;

    private List<Log> m_toggleLogs = new List<Log>();
    public List<Log> m_filterLogs = new List<Log>();
    public Log m_selectedLog;

    private bool m_toggleCollapse;
    private bool m_toggleLog = true;
    private bool m_toggleWarning = true;
    private bool m_toggleError = true;
    private string m_searchFilter;

    public LogMessegeReceiver(Action<Log> onLogMessageReceived)
    {
        m_onLogMessageReceived = onLogMessageReceived;

        Clear();

        Application.logMessageReceived += OnLogMessageReceived;
    }

    public void Destroy()
    {
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    public void Clear()
    {
        m_allLogs.Clear();
        m_allLogCount = 0;
        m_allWarningCount = 0;
        m_allErrorCount = 0;

        m_collapseLogs.Clear();
        m_collapseLogCount = 0;
        m_collapseWarningCount = 0;
        m_collapseErrorCount = 0;

        m_toggleLogs.Clear();
        m_filterLogs.Clear();

        m_selectedLog = null;

        UpdateLogs(null);
    }

    private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        Log log = new Log(condition, stackTrace, type);

        UpdateLogs(log);
    }

    private void UpdateLogs(Log log = null)
    {
        UpdateAllLogs(log);
        UpdateCollapseLogs(log);
        UpdateToggleLogs();
        UpdateFilterLogs();

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
                m_allLogCount++;
                break;
            case LogType.Warning:
                m_allWarningCount++;
                break;
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
                m_allErrorCount++;
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
                    m_collapseLogCount++;
                    break;
                case LogType.Warning:
                    m_collapseWarningCount++;
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    m_collapseErrorCount++;
                    break;
            }
        }
    }

    private void UpdateToggleLogs()
    {
        List<Log> logs = new List<Log>();
        if (m_toggleCollapse)
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
                    if (m_toggleLog)
                    {
                        m_toggleLogs.Add(cacheLog);
                    }
                    break;
                case LogType.Warning:
                    if (m_toggleWarning)
                    {
                        m_toggleLogs.Add(cacheLog);
                    }
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    if (m_toggleError)
                    {
                        m_toggleLogs.Add(cacheLog);
                    }
                    break;
            }
        }
    }

    private void UpdateFilterLogs()
    {
        m_filterLogs.Clear();

        if (string.IsNullOrEmpty(m_searchFilter))
        {
            m_filterLogs.AddRange(m_toggleLogs);
        }
        else
        {
            m_filterLogs = m_toggleLogs.FindAll(templateLog => templateLog.condition.Contains(m_searchFilter));
        }
    }

    public void SetToggleCollapse(bool value)
    {
        m_toggleCollapse = value;
        UpdateLogs(null);
    }

    public void SetToggleLog(bool value)
    {
        m_toggleLog = value;
        UpdateLogs(null);
    }

    public void SetToggleWarning(bool value)
    {
        m_toggleWarning = value;
        UpdateLogs(null);
    }

    public void SetToggleError(bool value)
    {
        m_toggleError = value;
        UpdateLogs(null);
    }

    public void SetSearchFilter(string value)
    {
        m_searchFilter = value;
        UpdateLogs(null);
    }
}
