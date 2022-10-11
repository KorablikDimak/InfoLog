[![](https://img.shields.io/badge/System.Data.SqlClient-4.8.3-informational)](https://www.nuget.org/packages/System.Data.SqlClient)
[![](https://img.shields.io/badge/Npgsql-6.0.4-informational)](https://www.npgsql.org/)
[![](https://img.shields.io/badge/xunit-2.4.2-black)](https://xunit.net/)
# InfoLog
### Description
A library for custom creation of logs that supports many built-in tools for obtaining detailed information in the log log and their subsequent output to the console/file/database. Supports creating your own logging methods by implementing interfaces `ILogger` and `ISender`.
### Logger configuration
[Example and xml documentation](https://github.com/KorablikDimak/InfoLog/blob/master/LogConfig.xml) 
The **.xml** file is a convenient way to set the type of output message and its parameters. Builtins can output the method that raised the message, the class of that method, line numbers, exact timestamps, and logging levels. You can set an unlimited number of ways to write log messages in addition to the three built-in initially.
### Async
Logger supports asynchrony. all methods of sending messages are asynchronous. Asynchrony can be used as you wish, or you can leave the synchronous call to the logger.
### Creating a logger
Basic way to get logger with [example xml](https://github.com/KorablikDimak/InfoLog/blob/master/LogConfig.xml):
```C#
Configuration configuration = new Configuration("LogConfig.xml");
LoggerFactory<Logger> loggerFactory = new LoggerFactory<Logger>(configuration);
ILogger logger = loggerFactory.CreateLogger();

logger.Info("hello I am logger!");
```
Will give the following output to the console:

![screenConsoleLog](https://github.com/KorablikDimak/InfoLog/blob/master/console%20output.png)
### Creating your own loggers
ISender is the main interface for message processing:
```C#
public interface ISender
{
    Dictionary<string, string> Config { get; set; }
    Task SendLog(string[] message, LogLevel logLevel);
}
```
`SendLog` the main method for sending messages supporting asynchrony. It is the implementation of this method that will allow you to create your own ways of logging logs.
A more detailed description of the methods can be found in the **documentation**.

### Database providers

When logging to the database, you must specify in the config file which DBMS will be used.
Currently supported:
- MsSql
- PostgreSql

Support for these subds will be added soon:
- MySql
- SqLite
- Oracle

Using the interface:
```C#
public interface IDatabaseProvider
{
    public Task<bool> IsTableCreated();
    public Task<bool> CreateTable();
    public Task<bool> InsertIntoDatabase(string message);
}
``` 
you can independently implement the method of connecting and sending messages to the database.

### Unit tests

All unit tests are in the directory [InfoLogTests](https://github.com/KorablikDimak/InfoLog/tree/master/InfoLogTests).
Gradually, they will be supplemented until the library is fully covered with tests.

### `InfoLog` can be downloaded by [NuGet](https://www.nuget.org/packages/InfoLog).
