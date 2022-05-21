using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfoLog.Extensions;

namespace InfoLog.Senders;

public class ConsoleSender : ISender
{
    public Dictionary<string, string> Config { get; set; }

    public Task SendLog(string[] message, ILogger.LogLevel logLevel)
    {
        if (!this.ValidateLogLevel(logLevel)) return Task.CompletedTask;
            
        if (!Config.ContainsKey("layout")) return Task.CompletedTask;
        string logMessage = LogParser.CreateLogMessage(message, Config["layout"], logLevel);
            
        ConsoleColorWrite(logMessage);
        return Task.CompletedTask;
    }

    private void ConsoleColorWrite(string logMessage)
    {
        foreach (var part in logMessage.Split("|"))
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            string sourceString = part;
            while (true)
            {
                if (!sourceString.Contains("{"))
                {
                    Console.Write(sourceString);
                    break;
                }
                    
                string pair = sourceString.Substring(
                    sourceString.IndexOf("{"), 
                    sourceString.IndexOf("}") - sourceString.IndexOf("{") + 1);
                
                int position = pair.IndexOf("=");
                if (position < 0)
                    break;

                if (pair.Substring(1, position - 1).ToLower().Contains("foregroundcolor"))
                {
                    ConsoleColor.TryParse(pair
                        .Substring(position + 1, pair.Length - position - 2)
                        .Replace(" ", ""), true, out ConsoleColor color);
                    Console.ForegroundColor = color;
                }
                
                if (pair.Substring(1, position - 1).ToLower().Contains("backgroundcolor"))
                {
                    ConsoleColor.TryParse(pair
                        .Substring(position + 1, pair.Length - position - 2)
                        .Replace(" ", ""), true, out ConsoleColor color);
                    Console.BackgroundColor = color;
                }
                    
                sourceString = sourceString.Remove(sourceString.IndexOf("{"),
                    sourceString.IndexOf("}") - sourceString.IndexOf("{") + 1);
            }
        }
            
        Console.WriteLine();
    }
}