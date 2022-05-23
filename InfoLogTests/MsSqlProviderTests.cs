using System.Data.SqlClient;
using InfoLog.Config;
using InfoLog.DatabaseProviders;
using Xunit;

namespace InfoLogTests;

public class MsSqlProviderTests
{
    [Fact]
    public async void IsTableCreatedWhenCreated()
    {
        // Arrange
        var configuration =
            new Configuration("C:\\Users\\korab\\RiderProjects\\InfoLog\\InfoLogTests\\XmlConfigs\\IsTableCreatedMssql.xml");
        Dictionary<string, string> config = configuration.Configs.First();
        IDatabaseProvider databaseProvider = new MsSqlProvider(config);

        const string commandCreateTable = "CREATE TABLE TableForInfoLogTest (Id int IDENTITY PRIMARY KEY)";
        const string commandDeleteTable = "DROP TABLE TableForInfoLogTest";
        
        // Act
        await using var connection = new SqlConnection(config["connectionstring"]);
        await connection.OpenAsync();
        var command = new SqlCommand(commandCreateTable, connection);
        await command.ExecuteNonQueryAsync();
        bool result = await databaseProvider.IsTableCreated();

        // Assert
        command = new SqlCommand(commandDeleteTable, connection);
        await command.ExecuteNonQueryAsync();
        await connection.CloseAsync();
        Assert.True(result);
    }
    
    [Fact]
    public async void IsTableCreatedWhenNotCreated()
    {
        // Arrange
        var configuration =
            new Configuration("C:\\Users\\korab\\RiderProjects\\InfoLog\\InfoLogTests\\XmlConfigs\\IsTableCreatedMssql.xml");
        Dictionary<string, string> config = configuration.Configs.First();
        IDatabaseProvider databaseProvider = new MsSqlProvider(config);
        
        // Act
        bool result = await databaseProvider.IsTableCreated();

        // Assert
        Assert.True(!result);
    }
}