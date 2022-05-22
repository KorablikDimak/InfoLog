using System.Collections.Generic;
using System.Threading.Tasks;
using InfoLog.Senders;

namespace InfoLog;

public interface ILogger
{
    public enum LogLevel
    {
        TRACE,
        DEBUG,
        INFO,
        WARNING,
        ERROR,
        CRITICAL
    }

    /// <summary>
    /// First param is the log message, other params are system info and do not need to be filled.
    /// </summary>
    /// <param name="message">log message</param>
    /// <param name="memberName">system info</param>
    /// <param name="sourceLineNumber">system info</param>
    public Task Trace(string message,
        [System.Runtime.CompilerServices.CallerMemberName]
        string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber]
        int sourceLineNumber = 0);

    /// <summary>
    /// First param is the log message, other params are system info and do not need to be filled.
    /// </summary>
    /// <param name="message">log message</param>
    /// <param name="memberName">system info</param>
    /// <param name="sourceLineNumber">system info</param>
    public Task Debug(string message,
        [System.Runtime.CompilerServices.CallerMemberName]
        string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber]
        int sourceLineNumber = 0);

    /// <summary>
    /// First param is the log message, other params are system info and do not need to be filled.
    /// </summary>
    /// <param name="message">log message</param>
    /// <param name="memberName">system info</param>
    /// <param name="sourceLineNumber">system info</param>
    public Task Info(string message,
        [System.Runtime.CompilerServices.CallerMemberName]
        string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber]
        int sourceLineNumber = 0);

    /// <summary>
    /// First param is the log message, other params are system info and do not need to be filled.
    /// </summary>
    /// <param name="message">log message</param>
    /// <param name="memberName">system info</param>
    /// <param name="sourceLineNumber">system info</param>
    public Task Warning(string message,
        [System.Runtime.CompilerServices.CallerMemberName]
        string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber]
        int sourceLineNumber = 0);

    /// <summary>
    /// First param is the log message, other params are system info and do not need to be filled.
    /// </summary>
    /// <param name="message">log message</param>
    /// <param name="memberName">system info</param>
    /// <param name="sourceLineNumber">system info</param>
    public Task Error(string message,
        [System.Runtime.CompilerServices.CallerMemberName]
        string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber]
        int sourceLineNumber = 0);

    /// <summary>
    /// First param is the log message, other params are system info and do not need to be filled.
    /// </summary>
    /// <param name="message">log message</param>
    /// <param name="memberName">system info</param>
    /// <param name="sourceLineNumber">system info</param>
    public Task Critical(string message,
        [System.Runtime.CompilerServices.CallerMemberName]
        string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber]
        int sourceLineNumber = 0);

    /// <summary>
    /// Add sender to logger. Senders are called in the same order they are added.
    /// </summary>
    /// <param name="config"> Must have a key-value pair "logsender"="{name of your class sender}".
    /// For example: "logsender"="File" or "FileSender" and class sender name is "FileSender".
    /// </param>
    public void AddSender(Dictionary<string, string> config);
    /// <summary>
    /// Creates a sender with the initial configuration.
    /// </summary>
    /// <param name="sender"></param>
    public void AddSender(ISender sender);
    /// <summary>
    /// Remove a sender with the initial configuration.
    /// </summary>
    /// <param name="sender"></param>
    public void RemoveSender(ISender sender);
    /// <summary>
    /// Remove a sender.
    /// </summary>
    /// <param name="senderName"> Name of your class sender </param>
    public void RemoveSender(string senderName);
}