using System.Collections.Generic;
using InfoLog.Config;

namespace InfoLog;

public class LoggerFactory<T> where T : ILogger, new()
{
    private ILogger Logger { get; set; }
    private List<Dictionary<string, string>> Configs { get; }

    /// <summary>
    /// The constructor which sets the configuration (from xml).
    /// The generic parameter is the type that implements the interface ILogger.
    /// </summary>
    /// <param name="xmlPath">Absolute or relative path to .xml file</param>
    public LoggerFactory(string xmlPath)
    {
        Configs = new Configuration(xmlPath).Configs;
    }

    /// <summary>
    /// The constructor which sets the configuration (from xml).
    /// The generic parameter is the type that implements the interface ILogger.
    /// </summary>
    /// <param name="configuration">The configuration is set when the factory is created and cannot be changed</param>
    public LoggerFactory(Configuration configuration)
    {
        Configs = configuration.Configs;
    }

    /// <summary>
    /// The constructor which sets the configuration (from code).
    /// The generic parameter is the type that implements the interface ILogger.
    /// </summary>
    /// <param name="configuration">The configuration is set when the factory is created and cannot be changed</param>
    public LoggerFactory(List<Dictionary<string, string>> configuration)
    {
        Configs = configuration;
    }

    /// <summary>
    /// Retrieves an ILogger object that is modified-restricted.
    /// </summary>
    /// <returns></returns>
    public ILogger CreateLogger()
    {
        Logger = new T();
        if (Configs == null)
        {
            return new T();
        }
        foreach (var config in Configs)
        {
            Logger.AddSender(config);
        }
        return Logger;
    }
}