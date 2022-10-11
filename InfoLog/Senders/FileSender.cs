using System.Collections.Generic;
using System.Threading.Tasks;
using InfoLog.Extensions;

namespace InfoLog.Senders;

/// <summary>
/// 
/// </summary>
public class FileSender : ISender
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
    public async Task SendLog(string[] message, LogLevel logLevel)
    {
        if (!this.ValidateLogLevel(logLevel)) return;
            
        if (!Config.ContainsKey("layout")) return;
        string logMessage = Parser.ParseLayout(message, Config["layout"], logLevel);

        if (!Config.ContainsKey("filepath")) return;
        string filepath = Parser.ParseLayout(message, Config["filepath"], logLevel);

        await FileSaver.SaveFileAsync(logMessage, filepath);
    }
}