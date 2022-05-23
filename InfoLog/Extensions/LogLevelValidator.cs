using System;
using InfoLog.Senders;

namespace InfoLog.Extensions;

/// <summary>
/// 
/// </summary>
public static class LogLevelValidator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public static bool ValidateLogLevel(this ISender sender, ILogger.LogLevel logLevel)
    {
        if (!sender.Config.ContainsKey("minlevel")) return true;
        string minLevel = sender.Config["minlevel"].ToUpper();
        var minLogLevel = Enum.Parse(typeof(ILogger.LogLevel), minLevel);
        return (int)logLevel >= (int)(ILogger.LogLevel) minLogLevel;
    }
}