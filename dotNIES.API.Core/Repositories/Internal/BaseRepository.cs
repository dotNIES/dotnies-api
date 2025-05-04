using Dapper;
using Dapper.Database.Extensions;
using dotNIES.API.Core.Helpers;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Models;
using dotNIES.Data.Logging.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace dotNIES.API.Core.Repositories.Internal;
public class BaseRepository : IBaseRepository
{
    private readonly IAppInfoDto _appInfoDto;
    private readonly IUserAppInfoDto _userAppInfoDto;
    private readonly ILoggerService _loggerService;

    private string _connectionString = string.Empty;
    private IDbConnection _connection;

    public BaseRepository(IAppInfoDto appInfoDto, IUserAppInfoDto userAppInfoDto, ILoggerService loggerService)
    {
        _appInfoDto = appInfoDto;
        _userAppInfoDto = userAppInfoDto;
        _loggerService = loggerService;

        CheckConnectionString();
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(string tableName)
    {
        LogMessageModel logMessage;

        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException("The tablename cannot be null or empty.", nameof(tableName));
        }

        // Log the SQL statement if logging is enabled
        if (_userAppInfoDto.LogSqlStatements)
        {
            _loggerService.SendInformation($"Executing base SQL Statement GetAll for table: {tableName}");
        }

        using IDbConnection connection = new SqlConnection(_appInfoDto.ConnectionString);
        var result = await connection.QueryAsync<T>($"SELECT * FROM {tableName}");

        // Log the outcome if logging is enabled with debug or trace level
        _loggerService.SendDebugInfo($"Executing base SQL Statement GetAll for table: {tableName} returned {result.Count()} records");

        return result;
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(string tableName, string schemaName)
    {
        LogMessageModel logMessage;

        if (string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(schemaName))
        {
            throw new ArgumentException("The tablename and schema name cannot be null or empty.", nameof(tableName));
        }

        // Log the SQL statement if logging is enabled
        if (_userAppInfoDto.LogSqlStatements)
        {
            _loggerService.SendInformation($"Executing base SQL Statement GetAll for table: {tableName} and schema: {schemaName}");
        }

        using IDbConnection connection = new SqlConnection(_appInfoDto.ConnectionString);
        var result = await connection.QueryAsync<T>($"SELECT * FROM {schemaName}.{tableName}");

        // Log the outcome if logging is enabled with debug or trace level
        _loggerService.SendDebugInfo($"Executing base SQL Statement GetAll for table: {tableName} and schema: {schemaName} returned {result.Count()} records");

        return result;
    }

    public async Task<IEnumerable<T>> GetDataAsync<T>(string sqlStatement)
    {
        LogMessageModel logMessage;

        if (string.IsNullOrWhiteSpace(sqlStatement))
            return [];

        // Log the SQL statement if logging is enabled
        if (_userAppInfoDto.LogSqlStatements)
        {
            _loggerService.SendInformation($"Executing SQL Statement: {SqlFormatter.SimplifySpaces(sqlStatement)}");
        }

        // ACTUAL DATABASE CALL

        using IDbConnection connection = new SqlConnection(_connectionString);
        var result = await connection.QueryAsync<T>(sqlStatement);

        // Log the outcome if logging is enabled with debug or trace level
        _loggerService.SendDebugInfo($"Executing base SQL Statement GetAll with sql statement {sqlStatement} returned {result.Count()} records");

        return result;
    }

    public async Task<T?> GetRecordAsync<T>(string sqlStatement)
    {
        LogMessageModel logMessage;

        // Log the SQL statement if logging is enabled
        if (_userAppInfoDto.LogSqlStatements)
        {
            _loggerService.SendInformation($"Executing SQL Statement: {SqlFormatter.SimplifySpaces(sqlStatement)}");
        }

        // ACTUAL DATABASE CALL

        using IDbConnection connection = new SqlConnection(_connectionString);
        var result = await connection.QuerySingleOrDefaultAsync<T>(sqlStatement);

        // Log the outcome if logging is enabled with debug or trace level
        _loggerService.SendDebugInfo($"Executing base SQL Statement GetAll with sql statement {sqlStatement} returned the record {result is not null}");

        return result;
    }

    public async Task<int> InsertAsync<T>(T model) where T : class
    {
        LogMessageModel logMessage;

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "The model cannot be null.");
        }

        var guidProperty = typeof(T).GetProperty("Id");

        if (guidProperty != null && guidProperty.PropertyType == typeof(int))
        {
            // Log the SQL statement if logging is enabled
            if (_userAppInfoDto.LogSqlStatements)
            {
                _loggerService.SendInformation($"Executing Insert statement for {model?.GetType()}");
            }

            // ACTUAL DATABASE CALL

            using IDbConnection connection = new SqlConnection(_connectionString);
            var result = await connection.InsertAsync<T>(model);

            if (_userAppInfoDto.LogEntireRecord)
            {
                // Log the entire record as JSON String
                var recordAsJson = System.Text.Json.JsonSerializer.Serialize(model);
                _loggerService.SendInformation($"Record: {recordAsJson}");
            }

            if (!result)
            {
                // Log the insert as NOT successful (if exception occurs then it will be caught in the catch block in the caller)
                _loggerService.SendError("Insert statement not successfully executed, record not inserted.");

                return 0;
            }
            else
            {
                if (guidProperty.GetValue(model) is int id)
                {
                    // Log the insert as successful (if error then it will be caught in the catch block in the caller)
                    _loggerService.SendInformation($"Insert statement executed successfully, record inserted with id {id}.");

                    return id;
                }
                else
                {
                    // highly unlikely, but just in case
                    throw new ArgumentException("The insert statement returned an unknown format, please check the data.");
                }
            }
        }
        else
        {
            throw new InvalidOperationException("The model doesn't have a INT primary key, the method expects a primary key with name Id and type integer");
        }
    }

    public async Task<Guid> InsertAsync<T>(T model, bool userGuidAsId) where T : class
    {
        LogMessageModel logMessage;

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "The model cannot be null.");
        }

        var guidProperty = typeof(T).GetProperty("Id");

        if (guidProperty != null && guidProperty.PropertyType == typeof(Guid))
        {
            // Log the SQL statement if logging is enabled
            _loggerService.SendInformation($"Executing Insert statement for {model?.GetType()}");

            // prepare new guid
            var newGuid = new Guid();

            if (guidProperty.GetValue(model, null) == null)
            {
                newGuid = Guid.NewGuid();
                guidProperty.SetValue(model, newGuid);
            }
            else
            {
                newGuid = (Guid)guidProperty.GetValue(model);
            }

            // ACTUAL DATABASE CALL

            using IDbConnection connection = new SqlConnection(_connectionString);
            var result = await connection.InsertAsync<T>(model);

            if (_userAppInfoDto.LogEntireRecord)
            {
                // Log the entire record as JSON String
                var recordAsJson = System.Text.Json.JsonSerializer.Serialize(model);
                _loggerService.SendInformation($"Record: {recordAsJson}");
            }

            if (!result)
            {
                // Log the insert as NOT successful (if exception occurs then it will be caught in the catch block in the caller)
                _loggerService.SendError("Insert statement not successfully executed, record not inserted.");

                return Guid.Empty;
            }
            else
            {
                if (guidProperty.GetValue(model) is Guid id)
                {
                    // Log the insert as successful (if error then it will be caught in the catch block in the caller)
                    _loggerService.SendInformation($"Insert statement executed successfully, record inserted with id {id}.");

                    return id;
                }
                else
                {
                    // highly unlikely, but just in case
                    throw new ArgumentException("The insert statement returned an unknown format, please check the data.");
                }
            }
        }
        else
        {
            throw new InvalidOperationException("The model doen't have a GUID primary key, the method expects a primary key with name Id and type Guid");
        }
    }

    public async Task<bool> UpdateRecordAsync<T>(T model) where T : class
    {
        LogMessageModel logMessage;

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "The model cannot be null.");
        }

        // Log the SQL statement if logging is enabled
        if (_userAppInfoDto.LogSqlStatements)
        {
            _loggerService.SendInformation($"Executing update statement for {model?.GetType()}");
        }


        // ACTUAL DATABASE CALL
        using IDbConnection connection = new SqlConnection(_connectionString);
        var result = await connection.UpdateAsync<T>(model);

        if (_userAppInfoDto.LogEntireRecord)
        {
            // Log the entire record as JSON String
            var recordAsJson = System.Text.Json.JsonSerializer.Serialize(model);
            _loggerService.SendInformation($"Record: {recordAsJson}");
        }

        return result;
    }

    public async Task<bool> DeleteRecordAsync<T>(T model) where T : class
    {
        LogMessageModel logMessage;

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "The model cannot be null.");
        }

        // Log the SQL statement if logging is enabled
        if (_userAppInfoDto.LogSqlStatements)
        {
            _loggerService.SendInformation($"Executing delete statement for {model?.GetType()}");
        }

        if (_userAppInfoDto.LogEntireRecord)
        {
            // Log the entire record as JSON String
            var recordAsJson = System.Text.Json.JsonSerializer.Serialize(model);
            _loggerService.SendInformation($"Record: {recordAsJson}");
        }


        // ACTUAL DATABASE CALL
        using IDbConnection connection = new SqlConnection(_connectionString);

        try
        {
            var result = await connection.DeleteAsync(model);

            if (!result)
            {
                _loggerService.SendError("Delete statement not successfully executed, record not deleted.");
            }
            else
            {
                _loggerService.SendInformation($"Delete statement executed successfully, record deleted.");
            }

            return result;
        }
        catch (SqlException sqlE) when (sqlE.Message.Contains("REFERENCE constraint"))
        {
            _loggerService.SendError("Delete statement not successfully executed, record has references to other records and cannot be deleted. A softdelete was issued instead.", sqlE);

            var result = await SoftDeleteRecordAsync(model);
            return result;
        }
    }

    public async Task<bool> SoftDeleteRecordAsync<T>(T model) where T : class
    {
        // TODO: this uses reflection for checking delete / isactive property
        //       this is not the best way to do this, but it works for now.

        LogMessageModel logMessage;
        bool propertyIsActiveChanged = false;
        bool propertyIsDeletedChanged = false;

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "The model cannot be null.");
        }

        // Log the SQL statement if logging is enabled
        if (_userAppInfoDto.LogSqlStatements)
        {
            _loggerService.SendInformation($"Executing delete statement for {model?.GetType()}");
        }

        if (_userAppInfoDto.LogEntireRecord)
        {
            // Log the entire record as JSON String
            var recordAsJson = System.Text.Json.JsonSerializer.Serialize(model);
            _loggerService.SendInformation($"Record: {recordAsJson}");
        }


        // check if there is a field IsActive and if so, change it to false
        var isActiveProperty = typeof(T).GetProperty("IsActive");
        var isDeleteProperty = typeof(T).GetProperty("IsDeleted");

        if (isActiveProperty != null && isActiveProperty.PropertyType == typeof(bool) && isActiveProperty.CanWrite)
        {
            isActiveProperty.SetValue(model, false);
            propertyIsActiveChanged = true;
        }
        else
        {
            propertyIsActiveChanged = false;
        }

        isDeleteProperty = typeof(T).GetProperty("IsDeleted");

        if (isDeleteProperty != null && isDeleteProperty.PropertyType == typeof(bool) && isDeleteProperty.CanWrite)
        {
            isDeleteProperty.SetValue(model, true);
            propertyIsDeletedChanged = true;
        }
        else
        {
            propertyIsDeletedChanged = false;
        }

        if (!propertyIsActiveChanged || !propertyIsDeletedChanged)
        {
            _loggerService.SendError($"The record {model?.GetType()} cannot be soft-deleted because neither IsActive nor IsDeleted exists in the table");

            return false;
        }


        // ACTUAL DATABASE CALL
        using IDbConnection connection = new SqlConnection(_connectionString);
        var result = await connection.UpdateAsync(model);

        if (!result)
        {
            _loggerService.SendError($"The record {model?.GetType()} is NOT updated for softdelete!");
        }
        else
        {
            _loggerService.SendInformation($"The record {model?.GetType()} was successfully soft deleted");
        }

        return result;
    }

    /* VERSIONS WITH TRANSACTIONS */
    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null)
    {
        var fallBackConnection = await GetConnectionAsync();
        var connection = transaction?.Connection ?? fallBackConnection;
        return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters, transaction);
    }

    public async Task<IEnumerable<T>?> QueryAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null)
    {
        var fallBackConnection = await GetConnectionAsync();
        var connection = transaction?.Connection ?? fallBackConnection;
        return await connection.QueryAsync<T>(sql, parameters, transaction);
    }

    public async Task<bool> DeleteRecordAsync<T>(T model, IDbTransaction? transaction = null) where T : class
    {
        LogMessageModel logMessage;

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "The model cannot be null.");
        }

        // Log the SQL statement if logging is enabled
        if (_userAppInfoDto.LogSqlStatements)
        {
            _loggerService.SendDebugInfo($"Executing delete statement for {model?.GetType()}");
        }

        if (_userAppInfoDto.LogEntireRecord)
        {
            // Log the entire record as JSON String
            var recordAsJson = System.Text.Json.JsonSerializer.Serialize(model);
            _loggerService.SendDebugInfo($"Record: {recordAsJson}");
        }


        // ACTUAL DATABASE CALL       
        try
        {
            var connection = transaction?.Connection ?? await GetConnectionAsync();
            var result = await connection.DeleteAsync(model, transaction: transaction);

            if (!result)
            {
                _loggerService.SendError($"The record {model?.GetType()} is NOT deleted!");
            }
            else
            {
                _loggerService.SendInformation($"The record {model?.GetType()} was successfully deleted");
            }

            return result;
        }
        catch (SqlException sqlE) when (sqlE.Message.Contains("REFERENCE constraint"))
        {
            _loggerService.SendError($"The record {model?.GetType()} is NOT deleted because it has references to other records!", sqlE);
            return false;
        }
    }

    public async Task<bool> SoftDeleteRecordAsync<T>(T model, IDbTransaction? transaction = null) where T : class
    {
        // TODO: this uses reflection for checking delete / isactive property
        //       this is not the best way to do this, but it works for now.

        LogMessageModel logMessage;
        bool propertyIsActiveChanged = false;
        bool propertyIsDeletedChanged = false;

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "The model cannot be null.");
        }

        // Log the SQL statement if logging is enabled
        if (_userAppInfoDto.LogSqlStatements)
        {
            _loggerService.SendDebugInfo($"Executing delete statement for {model?.GetType()}");
        }

        if (_userAppInfoDto.LogEntireRecord)
        {
            // Log the entire record as JSON String
            var recordAsJson = System.Text.Json.JsonSerializer.Serialize(model);
            _loggerService.SendDebugInfo($"Record: {recordAsJson}");
        }


        // check if there is a field IsActive and if so, change it to false
        var isActiveProperty = typeof(T).GetProperty("IsActive");
        var isDeleteProperty = typeof(T).GetProperty("IsDeleted");

        if (isActiveProperty != null && isActiveProperty.PropertyType == typeof(bool) && isActiveProperty.CanWrite)
        {
            isActiveProperty.SetValue(model, false);
            propertyIsActiveChanged = true;
        }
        else
        {
            propertyIsActiveChanged = false;
        }

        isDeleteProperty = typeof(T).GetProperty("IsDeleted");

        if (isDeleteProperty != null && isDeleteProperty.PropertyType == typeof(bool) && isDeleteProperty.CanWrite)
        {
            isDeleteProperty.SetValue(model, true);
            propertyIsDeletedChanged = true;
        }
        else
        {
            propertyIsDeletedChanged = false;
        }

        if (!propertyIsActiveChanged || !propertyIsDeletedChanged)
        {
            _loggerService.SendError($"The record {model?.GetType()} cannot be soft-deleted because neither IsActive nor IsDeleted exists in the table");

            return false;
        }


        // ACTUAL DATABASE CALL
        var fallBackConnection = await GetConnectionAsync();
        var connection = transaction?.Connection ?? fallBackConnection;
        var result = await connection.UpdateAsync(model, transaction: transaction);

        if (!result)
        {
            _loggerService.SendError($"The record {model?.GetType()} is NOT updated for softdelete!");
        }
        else
        {
            _loggerService.SendInformation($"The record {model?.GetType()} was successfully soft deleted");
        }

        return result;
    }

    #region Transactional methods
    /* TRANSACTIONAL METHODS */

    public async Task<IDbConnection> GetConnectionAsync()
    {
        if (_connection == null || _connection.State == ConnectionState.Closed)
        {
            _connection = new SqlConnection(_connectionString);
        }

        if (_connection.State != ConnectionState.Open)
        {
            await (_connection as SqlConnection).OpenAsync();
        }

        return _connection;
    }

    public async Task<IDbTransaction> StartTransactionAsync()
    {
        var connection = await GetConnectionAsync();
        return connection.BeginTransaction();
    }

    public Task CommitTransactionAsync(IDbTransaction transaction)
    {
        if (transaction != null)
        {
            transaction.Commit();
            transaction.Dispose();
        }

        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(IDbTransaction transaction)
    {
        if (transaction != null)
        {
            transaction.Rollback();
            transaction.Dispose();
        }

        return Task.CompletedTask;
    }

    #endregion

    /// <summary>
    /// Checks if the connection string is set in the app info DTO.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private void CheckConnectionString()
    {
        if (string.IsNullOrEmpty(_appInfoDto.ConnectionString))
        {
            throw new InvalidOperationException("Connection string is not set.");
        }
        else
        {
            _connectionString = _appInfoDto.ConnectionString;
        }
    }
}

