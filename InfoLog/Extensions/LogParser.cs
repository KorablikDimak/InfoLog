using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace InfoLog.Extensions;

public static class LogParser
{
    /// <summary>
    /// Return formatted by layout string.
    /// Target class can be found only if this method before await construction.
    /// </summary>
    /// <param name="message">First : message, Second : calling method, third : calling line</param>
    /// <param name="layout"></param>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public static string CreateLogMessage(string[] message = null, string layout = "", ILogger.LogLevel logLevel = ILogger.LogLevel.TRACE)
    {
        if (message is not { Length: 3 })
        {
            return "";
        }
            
        string targetClass = "";
        if (layout.Contains("{class}"))
        {
            try
            {
                var stack = new StackTrace();
                var declaringType = stack
                    .GetFrames().Single(s => s.GetMethod()?.Name == message[1]).GetMethod()
                    ?.DeclaringType;
                if (declaringType != null) targetClass = declaringType.FullName;
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        return layout
            .Replace("{method}", message[1], true, CultureInfo.CurrentCulture)
            .Replace("{line}", message[2], true, CultureInfo.CurrentCulture)
            .Replace("{longdate}", DateTime.Now.ToLongDateString(), true, CultureInfo.CurrentCulture)
            .Replace("{shortdate}", DateTime.Now.ToShortDateString(), true, CultureInfo.CurrentCulture)
            .Replace("{longtime}", DateTime.Now.ToLongTimeString(), true, CultureInfo.CurrentCulture)
            .Replace("{shorttime}", DateTime.Now.ToShortTimeString(), true, CultureInfo.CurrentCulture)
            .Replace("{level}", logLevel.ToString(), true, CultureInfo.CurrentCulture)
            .Replace("{message}", message[0], true, CultureInfo.CurrentCulture)
            .Replace("{millisecond}", DateTime.Now.Millisecond.ToString(), true, CultureInfo.CurrentCulture)
            .Replace("{class}", targetClass, true, CultureInfo.CurrentCulture)
            .Replace("{basedir}", Directory.GetCurrentDirectory(), true, CultureInfo.CurrentCulture);
    }
}