using UnityEngine;
using log4net;
public static class UnityLogger
{
    public static void Init()
    {
        Application.logMessageReceived += OnLogMessageReceived;
        Common.Log.Init("Unity");
    }

    static ILog log = LogManager.GetLogger("unity");
    static void OnLogMessageReceived(string condition, string stacktrace, LogType type)
    {
        switch(type)
        {
            case LogType.Error:
                log.ErrorFormat("{0}\r\n{1}", condition, stacktrace.Replace("\n", "\r\n"));
                break;
            case LogType.Assert:
                log.DebugFormat("{0}\r\n{1}", condition, stacktrace.Replace("\n", "\r\n"));
                break;
            case LogType.Exception:
                log.FatalFormat("{0\r\n{1}", condition, stacktrace.Replace("\n", "\r\n"));
                break;
            case LogType.Warning:
                log.WarnFormat("{0}\r\n{1}", condition, stacktrace.Replace("\n", "\r\n"));
                break;
            default:
                log.Info(condition);
                break;
        }
    }
}

