using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfoLog.Senders;

/// <summary>
/// 
/// </summary>
public class ApiSender : ISender
{
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, string> Config { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public Task SendLog(string[] message, LogLevel logLevel)
    {
        throw new System.NotImplementedException();
    }
}