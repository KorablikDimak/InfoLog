using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfoLog.Senders;

public interface ISender
{
    Dictionary<string, string> Config { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message">First member is the log message, other members are system info</param>
    /// <param name="logLevel"></param>
    Task SendLog(string[] message, ILogger.LogLevel logLevel);
}