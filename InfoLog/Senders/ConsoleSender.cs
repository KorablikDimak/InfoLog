using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfoLog.Extensions;

namespace InfoLog.Senders;

/// <summary>
/// 
/// </summary>
public class ConsoleSender : ISender
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
        if (!this.ValidateLogLevel(logLevel)) return Task.CompletedTask;
            
        if (!Config.ContainsKey("layout")) return Task.CompletedTask;
        string logMessage = Parser.ParseLayout(message, Config["layout"], logLevel);
            
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
                    sourceString.IndexOf("{", StringComparison.Ordinal), 
                    sourceString.IndexOf("}", StringComparison.Ordinal) - 
                    sourceString.IndexOf("{", StringComparison.Ordinal) + 1);
                
                int position = pair.IndexOf("=", StringComparison.Ordinal);
                if (position < 0)
                    break;

                if (pair.Substring(1, position - 1).ToLower().Contains("foregroundcolor"))
                {
                    Enum.TryParse(pair
                        .Substring(position + 1, pair.Length - position - 2)
                        .Replace(" ", ""), true, out ConsoleColor color);
                    Console.ForegroundColor = color;
                }
                
                if (pair.Substring(1, position - 1).ToLower().Contains("backgroundcolor"))
                {
                    Enum.TryParse(pair
                        .Substring(position + 1, pair.Length - position - 2)
                        .Replace(" ", ""), true, out ConsoleColor color);
                    Console.BackgroundColor = color;
                }
                    
                sourceString = sourceString.Remove(sourceString.IndexOf("{", StringComparison.Ordinal),
                    sourceString.IndexOf("}", StringComparison.Ordinal) - 
                    sourceString.IndexOf("{", StringComparison.Ordinal) + 1);
            }
        }
            
        Console.WriteLine();
    }
}