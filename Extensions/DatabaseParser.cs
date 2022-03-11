using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace InfoLog.Extensions
{
    public static class DatabaseParser
    {
        public static async Task CreateTable(Dictionary<string, string> config)
        {
            await using var connection = new SqlConnection(config["connectionstring"]);
            await connection.OpenAsync();
            
            if (!config.ContainsKey("tablename")) return;
            string commandText = "SELECT * FROM INFORMATION_SCHEMA.TABLES";
                
            var command = new SqlCommand(commandText, connection);
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (!reader.HasRows)
                {
                    break;
                }
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var f = reader.GetValue(i);
                    if (reader.GetValue(i).ToString() != $"{config["tablename"]}") continue;
                    await reader.CloseAsync();
                    await connection.CloseAsync();
                    return;
                }
            }

            await reader.CloseAsync();
            if (!config.ContainsKey("layout")) return;

            commandText = TableStructParse(config);
            command = new SqlCommand(commandText, connection);
            await command.ExecuteNonQueryAsync();
            
            await connection.CloseAsync();
        }

        private static string TableStructParse(Dictionary<string, string> config)
        {
            string sqlCommand = $"CREATE TABLE {config["tablename"]} ( Id int IDENTITY PRIMARY KEY,\n";
                
            var layoutParts = config["layout"].Split("|", StringSplitOptions.RemoveEmptyEntries);
            foreach (var layoutPart in layoutParts)
            {
                string sourceString = layoutPart.Substring(
                    layoutPart.IndexOf("{") + 1, 
                    layoutPart.IndexOf("}") - layoutPart.IndexOf("{") - 1);

                sqlCommand = sourceString switch
                {
                    "message" => sqlCommand + " " + sourceString + " " + "NVARCHAR(1000) NOT NULL" + ",\n",
                    "class" => sqlCommand + " " + sourceString + " " + "NVARCHAR(100) NOT NULL" + ",\n",
                    _ => sqlCommand + " " + sourceString + " " + "NVARCHAR(50) NOT NULL" + ",\n"
                };
            }

            sqlCommand += ")";
            return sqlCommand;
        }
    }
}