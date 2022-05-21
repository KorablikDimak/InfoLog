using System.Threading.Tasks;

namespace InfoLog.DatabaseConnection;

public interface IDatabaseProvider
{
    /// <summary>
    /// Checks for the existence of a table with the given name
    /// </summary>
    /// <returns>true if exist, false if not</returns>
    public Task<bool> IsTableCreated();
    
    /// <summary>
    /// Creates a table based on the given structure
    /// </summary>
    /// <returns>true if table were created, fasle if not</returns>
    public Task<bool> CreateTable();
    
    /// <summary>
    /// Parses the message string and populates the string in the database with them based on the generated structure
    /// </summary>
    /// <param name="message">row info to parse</param>
    /// <returns>true if row were added, false if not</returns>
    public Task<bool> InsertIntoDatabase(string message);
}