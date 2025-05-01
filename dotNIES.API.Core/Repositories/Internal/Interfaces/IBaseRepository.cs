
namespace dotNIES.API.Core.Repositories.Internal;

public interface IBaseRepository
{
    /// <summary>
    /// Deletes a record from the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<bool> DeleteRecordAsync<T>(T model) where T : class;

    /// <summary>
    /// Gets all records from the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableName">the table which records you want</param>
    /// <returns></returns>
    Task<IEnumerable<T>> GetAllAsync<T>(string tableName);


    /// <summary>
    /// Gets all records from the database from a specific schema.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableName">the table which records you want</param>
    /// <param name="schemaName">the schemaname</param>
    /// <returns></returns>
    Task<IEnumerable<T>> GetAllAsync<T>(string tableName, string schemaName);

    /// <summary>
    /// Gets all records from the database with custom sql statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sqlStatement"></param>
    /// <returns></returns>
    Task<IEnumerable<T>> GetDataAsync<T>(string sqlStatement);

    /// <summary>
    /// Get a single record from the database with custom sql statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sqlStatement"></param>
    /// <returns>The record if found, otherwise null</returns>
    Task<T?> GetRecordAsync<T>(string sqlStatement);

    /// <summary>
    /// Inserts a record into the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<int> InsertAsync<T>(T model) where T : class;

    /// <summary>
    /// Inserts a record into the database with a Guid as Id.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <param name="userGuidAsId"></param>
    /// <returns></returns>
    Task<Guid> InsertAsync<T>(T model, bool userGuidAsId) where T : class;

    /// <summary>
    /// Marks a record as deleted in the database .
    /// </summary>
    /// <remarks>flag IsDeleted needs to be present in the table</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<bool> SoftDeleteRecordAsync<T>(T model) where T : class;

    /// <summary>
    /// Updates a record in the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<bool> UpdateRecordAsync<T>(T model) where T : class;
}