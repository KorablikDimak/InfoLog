using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using InfoLog.Senders;

namespace InfoLog
{
    public class Logger : ILogger
    {
        private List<ISender> Senders { get; }

        public Logger()
        {
            Senders = new List<ISender>();
        }

        public async Task Trace(string message, 
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            await Log(new []
            {
                message,
                memberName,
                sourceLineNumber.ToString()
            }, ILogger.LogLevel.TRACE);
        }
        
        public async Task Debug(string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            await Log(new []
            {
                message,
                memberName,
                sourceLineNumber.ToString()
            }, ILogger.LogLevel.DEBUG);
        }
        
        public async Task Info(string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            await Log(new []
            {
                message,
                memberName,
                sourceLineNumber.ToString()
            }, ILogger.LogLevel.INFO);
        }
        
        public async Task Warning(string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            await Log(new []
            {
                message,
                memberName,
                sourceLineNumber.ToString()
            }, ILogger.LogLevel.WARNING);
        }
        
        public async Task Error(string message, 
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            await Log(new []
            {
                message,
                memberName,
                sourceLineNumber.ToString()
            }, ILogger.LogLevel.ERROR);
        }
        
        public async Task Critical(string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            await Log(new []
            {
                message,
                memberName,
                sourceLineNumber.ToString()
            }, ILogger.LogLevel.CRITICAL);
        }

        private async Task Log(string[] message, ILogger.LogLevel logLevel)
        {
            if (Senders == null) return;
            foreach (ISender sender in Senders)
            {
                await sender.SendLog(message, logLevel);
            }
        }
        
        public void AddSender(Dictionary<string, string> config)
        {
            string senderName = config["logsender"];
            if (!senderName.Contains("Sender"))
            {
                senderName += "Sender";
            }

            ISender sender = null;
            Type senderType = Type.GetType("InfoLog.Senders." + senderName, false, false);
            if (senderType != null)
            {
                sender = senderType.GetConstructor(Array.Empty<Type>())?
                    .Invoke(Array.Empty<object>()) as ISender;
            }

            if (sender == null)
            {
                var types = Assembly.GetEntryAssembly()?.GetTypes();
                foreach (var type in types)
                {
                    if (!type.FullName.Contains(senderName)) continue;
                    sender = type.GetConstructor(Array.Empty<Type>())?
                        .Invoke(Array.Empty<object>()) as ISender;
                    break;
                }
            }

            if (sender == null) return;
            sender.Config = config;
            Senders.Add(sender);
        }
        
        public void AddSender(ISender sender)
        {
            Senders.Add(sender);
        }
        
        public void RemoveSender(ISender sender)
        {
            Senders.Remove(sender);
        }
        
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
}