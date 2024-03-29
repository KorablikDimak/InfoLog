using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InfoLog.DatabaseProviders;

/// <summary>
/// 
/// </summary>
public class MsSqlProvider : IDatabaseProvider
{
    private Dictionary<string, string> Config { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    public MsSqlProvider(Dictionary<string, string> config)
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
        
        await using var connection = new SqlConnection(Config["connectionstring"]);
        try
        {
            await connection.OpenAsync();
            var commandText = $"select table_name from INFORMATION_SCHEMA.TABLES where table_name='{Config["tablename"]}'";
            var command = new SqlCommand(commandText, connection);
            var reader = await command.ExecuteReaderAsync();
            
            if (reader.HasRows)
            {
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
            throw new Exception(e.Message);
        }
    }

    private string TableStructParse()
    {
        var sqlCommand = $"CREATE TABLE {Config["tablename"]} ( Id int IDENTITY PRIMARY KEY,\n";
                
        string[] layoutParts = Config["layout"].Split("|", StringSplitOptions.RemoveEmptyEntries); 
        foreach (string layoutPart in layoutParts)
        {
            string source = layoutPart.Substring(
                layoutPart.IndexOf("{", StringComparison.Ordinal) + 1, 
                layoutPart.IndexOf("}", StringComparison.Ordinal) - 
                layoutPart.IndexOf("{", StringComparison.Ordinal) - 1);
            
            if (source == "")
            {
                source = layoutPart;
            }

            sqlCommand = source switch
            {
                "message" => sqlCommand + " " + source + " " + "NVARCHAR(1000) NOT NULL" + ",\n",
                "class" => sqlCommand + " " + source + " " + "NVARCHAR(100) NOT NULL" + ",\n",
                _ => sqlCommand + " " + source + " " + "NVARCHAR(50) NOT NULL" + ",\n"
            };
        }

        sqlCommand += ")";
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
        var commandText = $"INSERT INTO {Config["tablename"]} VALUES (";

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
            throw new Exception(e.Message);
        }
    }
}