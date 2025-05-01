using CommunityToolkit.Mvvm.Messaging;
using Dapper;
using Dapper.Database.Extensions;
using dotNIES.API.Core.Helpers;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Models;
using dotNIES.Data.Logging.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNIES.API.Core.Repositories.Internal;
public class BaseRepository : IBaseRepository
{
    private readonly IAppInfoDto _appInfoDto;
    private readonly IUserAppInfoDto _userAppInfoDto;
    private readonly ILoggerService _loggerService;

    private string _connectionString = string.Empty;

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
            logMessage = new LogMessageModel
            {
                Message = $"Executing base SQL Statement GetAll for table: {tableName}",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }

        using IDbConnection connection = new SqlConnection(_appInfoDto.ConnectionString);
        var result = await connection.QueryAsync<T>($"SELECT * FROM {tableName}");

        // Log the outcome if logging is enabled with debug or trace level
        if (_userAppInfoDto.MinimumLogLevel == LogLevel.Debug || _userAppInfoDto.MinimumLogLevel == LogLevel.Trace)
        {
            logMessage = new LogMessageModel
            {
                Message = $"Executing base SQL Statement GetAll for table: {tableName} returned {result.Count()} records",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }

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
            logMessage = new LogMessageModel
            {
                Message = $"Executing base SQL Statement GetAll for table: {tableName}",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }

        using IDbConnection connection = new SqlConnection(_appInfoDto.ConnectionString);
        var result = await connection.QueryAsync<T>($"SELECT * FROM {schemaName}.{tableName}");

        // Log the outcome if logging is enabled with debug or trace level
        if (_userAppInfoDto.MinimumLogLevel == LogLevel.Debug || _userAppInfoDto.MinimumLogLevel == LogLevel.Trace)
        {
            logMessage = new LogMessageModel
            {
                Message = $"Executing base SQL Statement GetAll for table: {tableName} and schame: {schemaName} returned {result.Count()} records",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }

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
            logMessage = new LogMessageModel
            {
                Message = $"Executing SQL Statement: {SqlFormatter.SimplifySpaces(sqlStatement)}",
                LogLevel = LogLevel.Information,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }

        // ACTUAL DATABASE CALL

        using IDbConnection connection = new SqlConnection(_connectionString);
        var result = await connection.QueryAsync<T>(sqlStatement);

        // Log the outcome if logging is enabled with debug or trace level
        if (_userAppInfoDto.MinimumLogLevel == LogLevel.Debug || _userAppInfoDto.MinimumLogLevel == LogLevel.Trace)
        {
            logMessage = new LogMessageModel
            {
                Message = $"Executing base SQL Statement GetAll with sql statement {sqlStatement} returned {result.Count()} records",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }

        return result;
    }

    public async Task<T?> GetRecordAsync<T>(string sqlStatement)
    {
        LogMessageModel logMessage;

        // Log the SQL statement if logging is enabled
        if (_userAppInfoDto.LogSqlStatements)
        {
            logMessage = new LogMessageModel
            {
                Message = $"Executing SQL Statement: {SqlFormatter.SimplifySpaces(sqlStatement)}",
                LogLevel = LogLevel.Information,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }

        // ACTUAL DATABASE CALL

        using IDbConnection connection = new SqlConnection(_connectionString);
        var result = await connection.QuerySingleOrDefaultAsync<T>(sqlStatement);

        // Log the outcome if logging is enabled with debug or trace level
        if (_userAppInfoDto.MinimumLogLevel == LogLevel.Debug || _userAppInfoDto.MinimumLogLevel == LogLevel.Trace)
        {
            logMessage = new LogMessageModel
            {
                Message = $"Executing base SQL Statement {sqlStatement} returned the record {result is not null}",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }

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
                logMessage = new LogMessageModel
                {
                    Message = $"Executing Insert statement for {model?.GetType()}",
                    LogLevel = LogLevel.Debug,
                };
                WeakReferenceMessenger.Default.Send(logMessage);
            }

            // ACTUAL DATABASE CALL

            using IDbConnection connection = new SqlConnection(_connectionString);
            var result = await connection.InsertAsync<T>(model);

            if (_userAppInfoDto.LogEntireRecord)
            {
                // Log the entire record as JSON String
                var recordAsJson = System.Text.Json.JsonSerializer.Serialize(model);
                logMessage = new LogMessageModel
                {
                    Message = $"Record: {recordAsJson}",
                    LogLevel = LogLevel.Debug,
                };
                WeakReferenceMessenger.Default.Send(logMessage);
            }

            if (!result)
            {
                // Log the insert as NOT successful (if exception occurs then it will be caught in the catch block in the caller)
                logMessage = new LogMessageModel
                {
                    Message = "Insert statement not successfully executed, record not inserted.",
                    LogLevel = LogLevel.Error,
                };
                WeakReferenceMessenger.Default.Send(logMessage);

                return 0;
            }
            else
            {
                if (guidProperty.GetValue(model) is int id)
                {
                    // Log the insert as successful (if error then it will be caught in the catch block in the caller)
                    logMessage = new LogMessageModel
                    {
                        Message = $"Insert statement executed successfully, record inserted with id {id}.",
                        LogLevel = LogLevel.Debug,
                    };
                    WeakReferenceMessenger.Default.Send(logMessage);

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
            if (_userAppInfoDto.LogSqlStatements)
            {
                logMessage = new LogMessageModel
                {
                    Message = $"Executing Insert statement for {model?.GetType()}",
                    LogLevel = LogLevel.Debug,
                };
                WeakReferenceMessenger.Default.Send(logMessage);
            }

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
                logMessage = new LogMessageModel
                {
                    Message = $"Record: {recordAsJson}",
                    LogLevel = LogLevel.Debug,
                };
                WeakReferenceMessenger.Default.Send(logMessage);
            }

            if (!result)
            {
                // Log the insert as NOT successful (if exception occurs then it will be caught in the catch block in the caller)
                logMessage = new LogMessageModel
                {
                    Message = "Insert statement not successfully executed, record not inserted.",
                    LogLevel = LogLevel.Error,
                };
                WeakReferenceMessenger.Default.Send(logMessage);

                return Guid.Empty;
            }
            else
            {
                if (guidProperty.GetValue(model) is Guid id)
                {
                    // Log the insert as successful (if error then it will be caught in the catch block in the caller)
                    logMessage = new LogMessageModel
                    {
                        Message = $"Insert statement executed successfully, record inserted with id {id}.",
                        LogLevel = LogLevel.Debug,
                    };
                    WeakReferenceMessenger.Default.Send(logMessage);

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
            logMessage = new LogMessageModel
            {
                Message = $"Executing update statement for {model?.GetType()}",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }


        // ACTUAL DATABASE CALL
        using IDbConnection connection = new SqlConnection(_connectionString);
        var result = await connection.UpdateAsync<T>(model);

        if (_userAppInfoDto.LogEntireRecord)
        {
            // Log the entire record as JSON String
            var recordAsJson = System.Text.Json.JsonSerializer.Serialize(model);
            logMessage = new LogMessageModel
            {
                Message = $"Record: {recordAsJson}",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
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
            logMessage = new LogMessageModel
            {
                Message = $"Executing delete statement for {model?.GetType()}",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }

        if (_userAppInfoDto.LogEntireRecord)
        {
            // Log the entire record as JSON String
            var recordAsJson = System.Text.Json.JsonSerializer.Serialize(model);
            logMessage = new LogMessageModel
            {
                Message = $"Record: {recordAsJson}",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }


        // ACTUAL DATABASE CALL
        using IDbConnection connection = new SqlConnection(_connectionString);

        try
        {
            var result = await connection.DeleteAsync(model);

            if (!result)
            {
                logMessage = new LogMessageModel
                {
                    Message = $"The record {model?.GetType()} is NOT deleted!",
                    LogLevel = LogLevel.Error,
                };
                WeakReferenceMessenger.Default.Send(logMessage);
            }
            else
            {
                logMessage = new LogMessageModel
                {
                    Message = $"The record {model?.GetType()} was successfully deleted",
                    LogLevel = LogLevel.Information,
                };
                WeakReferenceMessenger.Default.Send(logMessage);
            }

            return result;
        }
        catch (SqlException sqlE) when (sqlE.Message.Contains("REFERENCE constraint"))
        {
            logMessage = new LogMessageModel
            {
                Message = $"The record {model?.GetType()} has references to other records and cannot be deleted. A softdelete was issued instead.",
                LogLevel = LogLevel.Error,
            };
            WeakReferenceMessenger.Default.Send(logMessage);

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
            logMessage = new LogMessageModel
            {
                Message = $"Executing delete statement for {model?.GetType()}",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }

        if (_userAppInfoDto.LogEntireRecord)
        {
            // Log the entire record as JSON String
            var recordAsJson = System.Text.Json.JsonSerializer.Serialize(model);
            logMessage = new LogMessageModel
            {
                Message = $"Record: {recordAsJson}",
                LogLevel = LogLevel.Debug,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
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
            logMessage = new LogMessageModel
            {
                Message = $"The record {model?.GetType()} cannot be soft-deleted because neither IsActive nor IsDeleted exists in the table",
                LogLevel = LogLevel.Error,
            };
            WeakReferenceMessenger.Default.Send(logMessage);

            return false;
        }


        // ACTUAL DATABASE CALL
        using IDbConnection connection = new SqlConnection(_connectionString);
        var result = await connection.UpdateAsync(model);

        if (!result)
        {
            logMessage = new LogMessageModel
            {
                Message = $"The record {model?.GetType()} is NOT updated for softdelete!",
                LogLevel = LogLevel.Error,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }
        else
        {
            logMessage = new LogMessageModel
            {
                Message = $"The record {model?.GetType()} was successfully soft deleted",
                LogLevel = LogLevel.Information,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
        }

        return result;
    }

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

