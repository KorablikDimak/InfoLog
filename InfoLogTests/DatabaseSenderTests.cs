using System.Data.SqlClient;
using InfoLog;
using InfoLog.Config;
using InfoLog.Senders;
using Npgsql;
using Xunit;

namespace InfoLogTests;

public class DatabaseSenderTests
{
    [Fact]
    public async void SendLogMessageMssqlInserted()
    {
        // Arrange
        ISender sender = new DatabaseSender();
        Configuration configuration =
            new Configuration("C:\\Users\\korab\\RiderProjects\\InfoLog\\InfoLogTests\\LogConfigMssql.xml");
        sender.Config = configuration.Configs.First();

        string message = "SendLogMessageMssqlInserted Ok";
        string commandTextSelect =
            $"SELECT TOP (1) message FROM {sender.Config["tablename"]} WHERE message = '{message}'";
        string commandTextDelete = $"DELETE FROM {sender.Config["tablename"]} WHERE message = '{message}'";

        // Act
        await sender.SendLog(new[] { $"{message}", "", "" }, ILogger.LogLevel.CRITICAL);
        await using var connection = new SqlConnection(sender.Config["connectionstring"]);
        await connection.OpenAsync();
        var command = new SqlCommand(commandTextSelect, connection);
        var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        
        // Assert
        Assert.True(reader.HasRows);

        await reader.CloseAsync();
        command = new SqlCommand(commandTextDelete, connection);
        await command.ExecuteNonQueryAsync();
        await connection.CloseAsync();
    }

    [Fact]
    public async void SendLogMessageNpgsqlInserted()
    {
        // Arrange
        ISender sender = new DatabaseSender();
        Configuration configuration =
            new Configuration("C:\\Users\\korab\\RiderProjects\\InfoLog\\InfoLogTests\\LogConfigNpgsql.xml");
        sender.Config = configuration.Configs.First();
        
        string message = "SendLogMessageNpgsqlInserted Ok";
        string commandTextSelect =
            $"SELECT message FROM {sender.Config["tablename"]} WHERE message = '{message}' limit 1";
        string commandTextDelete = $"DELETE FROM {sender.Config["tablename"]} WHERE message = '{message}'";

        // Act
        await sender.SendLog(new[] { $"{message}", "", "" }, ILogger.LogLevel.CRITICAL);
        await using var connection = new NpgsqlConnection(sender.Config["connectionstring"]);
        await connection.OpenAsync();
        var command = new NpgsqlCommand(commandTextSelect, connection);
        var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        
        // Assert
        Assert.True(reader.HasRows);

        await reader.CloseAsync();
        command = new NpgsqlCommand(commandTextDelete, connection);
        await command.ExecuteNonQueryAsync();
        await connection.CloseAsync();
    }
}