using System.Collections.Generic;
using System.Threading.Tasks;
using InfoLog.Extensions;

namespace InfoLog.Senders;

public class FileSender : ISender
{
    public Dictionary<string, string> Config { get; set; }

    public async Task SendLog(string[] message, ILogger.LogLevel logLevel)
    {
        if (!this.ValidateLogLevel(logLevel)) return;
            
        if (!Config.ContainsKey("layout")) return;
        string logMessage = LogParser.CreateLogMessage(message, Config["layout"], logLevel);

        if (!Config.ContainsKey("filepath")) return;
        string filepath = LogParser.CreateLogMessage(message, Config["filepath"], logLevel);

        await FileSaver.SaveFileAsync(logMessage, filepath);
    }
}