using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace InfoLog.DatabaseProviders;

/// <summary>
/// 
/// </summary>
public class PostgreSqlProvider : IDatabaseProvider
{
    private Dictionary<string, string> Config { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    public PostgreSqlProvider(Dictionary<string, string> config)
    {
        Config = config;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> IsTableCreated()
    {
        if (!Config.ContainsKey("tablename")) return false;
        
        await using var connection = new NpgsqlConnection(Config["connectionstring"]);
        try
        {
            await connection.OpenAsync();
            string commandText = 
                $"SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_schema IN('public', '{Config["tablename"]}');";
            var command = new NpgsqlCommand(commandText, connection);
            var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync()) 
            {
                if (!reader.HasRows)
                {
                    break;
                }
                await reader.CloseAsync();
                await connection.CloseAsync();
                return true;
            }
        
            await reader.CloseAsync();
            await connection.CloseAsync();
            return false;
        }
        catch (Exception e)
        {
            await connection.CloseAsync();
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> CreateTable()
    {
        await using var connection = new NpgsqlConnection(Config["connectionstring"]);

        try
        {
            await connection.OpenAsync();
            string commandText = TableStructParse();
            var command = new NpgsqlCommand(commandText, connection);
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
            return true;
        }
        catch (Exception e)
        {
            await connection.CloseAsync();
            throw new Exception(e.Message);
        }
    }
    
    private string TableStructParse()
    {
        string sqlCommand = $"CREATE TABLE {Config["tablename"]} ( Id SERIAL PRIMARY KEY ,\n";
                
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
                "message" => sqlCommand + " " + sourceString + " " + "character varying(1000) NOT NULL" + ",\n",
                "class" => sqlCommand + " " + sourceString + " " + "character varying(100) NOT NULL" + ",\n",
                _ => sqlCommand + " " + sourceString + " " + "character varying(50) NOT NULL" + ",\n"
            };
        }

        sqlCommand = sqlCommand[..^2] + ")";
        return sqlCommand;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InsertIntoDatabase(string message)
    {
        string commandText = $"INSERT INTO {Config["tablename"]} VALUES ( default,";

        commandText = message
            .Split("|")
            .Aggregate(commandText, (current, part) => current + $"'{part}'" + ",\n");
        commandText = commandText[..^2] + ")";
            
        await using var connection = new NpgsqlConnection(Config["connectionstring"]);

        try
        {
            await connection.OpenAsync();
            var command = new NpgsqlCommand(commandText, connection);
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
            return true;
        }
        catch (Exception e)
        {
            await connection.CloseAsync();
            throw new Exception(e.Message);
        }
    }
}