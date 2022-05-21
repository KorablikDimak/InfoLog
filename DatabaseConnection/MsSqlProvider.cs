using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InfoLog.DatabaseConnection;

public class MsSqlProvider : IDatabaseProvider
{
    private Dictionary<string, string> Config { get; }

    public MsSqlProvider(Dictionary<string, string> config)
    {
        Config = config;
    }

    public async Task<bool> IsTableCreated()
    {
        if (!Config.ContainsKey("tablename")) return false;
        
        await using var connection = new SqlConnection(Config["connectionstring"]);
        try
        {
            await connection.OpenAsync();
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
                    if (reader.GetValue(i).ToString() != $"{Config["tablename"]}") continue;
                    await reader.CloseAsync();
                    await connection.CloseAsync();
                    return false;
                }
            }
        
            await reader.CloseAsync();
            await connection.CloseAsync();
            return true;
        }
        catch (Exception e)
        {
            await connection.CloseAsync();
            return false;
        }
    }

    public async Task<bool> CreateTable()
    {
        await using var connection = new SqlConnection(Config["connectionstring"]);

        try
        {
            await connection.OpenAsync();
            string commandText = TableStructParse();
            var command = new SqlCommand(commandText, connection);
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
            return true;
        }
        catch (Exception e)
        {
            await connection.CloseAsync();
            return false;
        }
    }

    private string TableStructParse()
    {
        string sqlCommand = $"CREATE TABLE {Config["tablename"]} ( Id int IDENTITY PRIMARY KEY,\n";
                
        var layoutParts = Config["layout"].Split("|", StringSplitOptions.RemoveEmptyEntries); 
        foreach (var layoutPart in layoutParts)
        {
            string sourceString = layoutPart.Substring(
                layoutPart.IndexOf("{") + 1, 
                layoutPart.IndexOf("}") - layoutPart.IndexOf("{") - 1);
            if (sourceString == "")
            {
                sourceString = layoutPart;
            }

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
    
    public async Task<bool> InsertIntoDatabase(string message)
    {
        string commandText = $"INSERT INTO {Config["tablename"]} VALUES (";

        commandText = message
            .Split("|")
            .Aggregate(commandText, (current, part) => current + $"'{part}'" + ",\n");
        commandText = commandText[..^2] + ")";
            
        await using var connection = new SqlConnection(Config["connectionstring"]);

        try
        {
            await connection.OpenAsync();
            var command = new SqlCommand(commandText, connection);
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
            return true;
        }
        catch (Exception e)
        {
            await connection.CloseAsync();
            return false;
        }
    }
}