
namespace dotNIES.API.Core.Services;

public interface IBaseDataService
{
    /// <summary>
    /// Fysically deletes a record from the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> DeleteRecordAsync<T>(T model) where T : class;

    /// <summary>
    /// Return all records in a table.
    /// </summary>
    /// <remarks>
    /// be careful with large tables.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    Task<IEnumerable<T>> GetAllAsync<T>(string tableName);

    /// <summary>
    /// Returns the output of a SQL statement as a list of objects of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sqlStatement"></param>
    /// <returns></returns>
    Task<IEnumerable<T>> GetDataAsync<T>(string sqlStatement);

    /// <summary>
    /// Returns the output of a SQL statement as a single object of type T.
    /// </summary>
    /// <remarks>
    /// If multiple rows are returned, only the first row will be returned.
    /// </remarks>
    /// <param name="sqlStatement"></param>
    /// <returns></returns>
    Task<T?> GetRecordAsync<T>(string sqlStatement);

    /// <summary>
    /// Inserts a new record into the database.
    /// </summary>
    /// <remarks>The Id must be of type INT</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<int> InsertAsync<T>(T model) where T : class;

    /// <summary>
    /// Inserts a new record into the database.
    /// </summary>
    /// <remarks>The Id must be of type GUID</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    Task<Guid> InsertAsync<T>(T model, bool userGuidAsId) where T : class;

    /// <summary>
    /// Updates the record setting the 'IsActive' and/or 'IsDeleted' flag. the record is not deleted from the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> SoftDeleteRecordAsync<T>(T model) where T : class;

    /// <summary>
    /// Updates an existing record in the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> UpdateRecordAsync<T>(T model) where T : class;
}