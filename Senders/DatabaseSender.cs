using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using InfoLog.Extensions;

namespace InfoLog.Senders
{
    public class DatabaseSender : ISender
    {
        public Dictionary<string, string> Config { get; set; }

        public async Task SendLog(string[] message, ILogger.LogLevel logLevel)
        {
            if (!this.ValidateLogLevel(logLevel)) return;
            if (!Config.ContainsKey("connectionstring")) return;
            string logMessage = LogParser.CreateLogMessage(message, Config["layout"], logLevel);
            
            await DatabaseParser.CreateTable(Config);
            await SendIntoDatabase(logMessage);
        }
        
        private async Task SendIntoDatabase(string message)
        {
            string commandText = $"INSERT INTO {Config["tablename"]} VALUES (";

            commandText = message
                .Split("|")
                .Aggregate(commandText, (current, part) => current + $"'{part}'" + ",\n");
            commandText = commandText[..^2] + ")";
            
            await using var connection = new SqlConnection(Config["connectionstring"]);
            await connection.OpenAsync();
            var command = new SqlCommand(commandText, connection);
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
    }
}