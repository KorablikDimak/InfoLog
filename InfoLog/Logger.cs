using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using InfoLog.Config;
using InfoLog.Senders;

namespace InfoLog;

/// <summary>
/// 
/// </summary>
public class Logger : ILogger
{
    private List<ISender> Senders { get; }

    /// <summary>
    /// Crate empty Logger without senders. Add them separately or better use a LoggerFactory.
    /// </summary>
    public Logger()
    {
        Senders = new List<ISender>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration">The configuration is set when the factory is created and cannot be changed</param>
    public Logger(List<Dictionary<string, string>> configuration)
    {
        Senders = new List<ISender>();
        foreach (var config in configuration)
        {
            AddSender(config);
        }
    }

    /// <summary>
    /// Creates a logger based on a .xml config file.
    /// </summary>
    /// <param name="xmlPath">Absolute or relative path to .xml file</param>
    public Logger(string xmlPath)
    {
        Senders = new List<ISender>();
        foreach (var config in new Configuration(xmlPath).Configs)
        {
            AddSender(config);
        }
    }

    /// <summary>
    /// Creates a logger based on a .xml config file.
    /// </summary>
    /// <param name="configuration">Get from Configuration constructor</param>
    public Logger(Configuration configuration)
    {
        Senders = new List<ISender>();
        foreach (var config in configuration.Configs)
        {
            AddSender(config);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="memberName"></param>
    /// <param name="sourceLineNumber"></param>
    public async Task Trace(string message, 
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        await Log(new []
        {
            message,
            memberName,
            sourceLineNumber.ToString()
        }, LogLevel.TRACE);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="memberName"></param>
    /// <param name="sourceLineNumber"></param>
    public async Task Debug(string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        await Log(new []
        {
            message,
            memberName,
            sourceLineNumber.ToString()
        }, LogLevel.DEBUG);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="memberName"></param>
    /// <param name="sourceLineNumber"></param>
    public async Task Info(string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        await Log(new []
        {
            message,
            memberName,
            sourceLineNumber.ToString()
        }, LogLevel.INFO);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="memberName"></param>
    /// <param name="sourceLineNumber"></param>
    public async Task Warning(string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        await Log(new []
        {
            message,
            memberName,
            sourceLineNumber.ToString()
        }, LogLevel.WARNING);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="memberName"></param>
    /// <param name="sourceLineNumber"></param>
    public async Task Error(string message, 
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        await Log(new []
        {
            message,
            memberName,
            sourceLineNumber.ToString()
        }, LogLevel.ERROR);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="memberName"></param>
    /// <param name="sourceLineNumber"></param>
    public async Task Critical(string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        await Log(new []
        {
            message,
            memberName,
            sourceLineNumber.ToString()
        }, LogLevel.CRITICAL);
    }

    private async Task Log(string[] message, LogLevel logLevel)
    {
        if (Senders == null) return;
        foreach (var sender in Senders)
        {
            await sender.SendLog(message, logLevel);
        }
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    public void AddSender(Dictionary<string, string> config)
    {
        string senderName = config["logsender"];
        if (!senderName.Contains("Sender"))
        {
            senderName += "Sender";
        }

        ISender sender = null;
        var senderType = Type.GetType("InfoLog.Senders." + senderName, false, false);
        if (senderType != null)
        {
            sender = senderType.GetConstructor(Array.Empty<Type>())?
                .Invoke(Array.Empty<object>()) as ISender;
        }

        if (sender == null)
        {
            var types = Assembly.GetEntryAssembly()?.GetTypes();
            if (types != null)
                foreach (var type in types)
                {
                    if (type.FullName != null && !type.FullName.Contains(senderName)) continue;
                    sender = type.GetConstructor(Array.Empty<Type>())?
                        .Invoke(Array.Empty<object>()) as ISender;
                    break;
                }
        }

        if (sender == null) return;
        sender.Config = config;
        Senders.Add(sender);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    public void AddSender(ISender sender)
    {
        Senders.Add(sender);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    public void RemoveSender(ISender sender)
    {
        Senders.Remove(sender);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="senderName"></param>
    public void RemoveSender(string senderName)
    {
        if (!senderName.Contains("Sender"))
        {
            senderName += "Sender";
        }
        foreach (var sender in Senders.Where(sender => sender.GetType().ToString() == senderName))
        {
            Senders.Remove(sender);
            break;
        }
    }
}