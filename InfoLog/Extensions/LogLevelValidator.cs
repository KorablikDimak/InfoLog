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
    public static bool ValidateLogLevel(this ISender sender, LogLevel logLevel)
    {
        if (!sender.Config.ContainsKey("minlevel")) return true;
        string configLevel = sender.Config["minlevel"].ToUpper();
        object minLogLevel = Enum.Parse(typeof(LogLevel), configLevel);
        return (int) logLevel >= (int) (LogLevel) minLogLevel;
    }
}