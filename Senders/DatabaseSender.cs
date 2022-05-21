using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfoLog.DatabaseConnection;
using InfoLog.Extensions;

namespace InfoLog.Senders;

public class DatabaseSender : ISender
{
    public Dictionary<string, string> Config { get; set; }

    public async Task SendLog(string[] message, ILogger.LogLevel logLevel)
    {
        if (!this.ValidateLogLevel(logLevel)) return;
        if (!Config.ContainsKey("connectionstring")) 
            throw new Exception("Config file does not have attribute 'connectionstring'");
        string logMessage = LogParser.CreateLogMessage(message, Config["layout"], logLevel);

        IDatabaseProvider databaseProvider = GetDatabaseProvider();
        if (!await databaseProvider.IsTableCreated())
        {
            if (!await databaseProvider.CreateTable()) throw new Exception("Table can not be created");
        }
        if (!await databaseProvider.InsertIntoDatabase(logMessage))
            throw new Exception("Can not insert message in table");
    }

    private IDatabaseProvider GetDatabaseProvider()
    {
        if (!Config.ContainsKey("provider")) throw new Exception("Database target does not have attribute 'provider'");
        return Config["provider"].ToLower() switch
        {
            "mssql" => new MsSqlProvider(Config),
            "postgresql" => new PostgreSqlProvider(Config),
            _ => throw new Exception("Incorrect database provider name")
        };
    }
}