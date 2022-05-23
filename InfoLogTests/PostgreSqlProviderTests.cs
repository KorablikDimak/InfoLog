using InfoLog.Config;
using InfoLog.DatabaseProviders;
using Npgsql;
using Xunit;

namespace InfoLogTests;

public class PostgreSqlProviderTests
{
    [Fact]
    public async void IsTableCreatedWhenCreated()
    {
        // Arrange
        var configuration =
            new Configuration("C:\\Users\\korab\\RiderProjects\\InfoLog\\InfoLogTests\\XmlConfigs\\IsTableCreatedNpgsql.xml");
        Dictionary<string, string> config = configuration.Configs.First();
        IDatabaseProvider databaseProvider = new PostgreSqlProvider(config);

        const string commandCreateTable = "CREATE TABLE TableForInfoLogTest (Id SERIAL PRIMARY KEY)";
        const string commandDeleteTable = "DROP TABLE TableForInfoLogTest";
        
        // Act
        await using var connection = new NpgsqlConnection(config["connectionstring"]);
        await connection.OpenAsync();
        var command = new NpgsqlCommand(commandCreateTable, connection);
        await command.ExecuteNonQueryAsync();
        bool result = await databaseProvider.IsTableCreated();

        // Assert
        command = new NpgsqlCommand(commandDeleteTable, connection);
        await command.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        Assert.True(result);
    }
    
    [Fact]
    public async void IsTableCreatedWhenNotCreated()
    {
        // Arrange
        var configuration =
            new Configuration("C:\\Users\\korab\\RiderProjects\\InfoLog\\InfoLogTests\\XmlConfigs\\IsTableCreatedNpgsql.xml");
        Dictionary<string, string> config = configuration.Configs.First();
        IDatabaseProvider databaseProvider = new PostgreSqlProvider(config);
        
        // Act
        bool result = await databaseProvider.IsTableCreated();

        // Assert
        Assert.True(!result);
    }
}