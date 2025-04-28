using CommunityToolkit.Mvvm.Messaging;
using Dapper;
using Dapper.Database;
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

namespace dotNIES.API.Core.Services;
public class BaseDataService
{
    // NOG DE 2 objecten opnieuw zoeken in devops
    private readonly IAppInfoDto _appInfoDto;
    private readonly IUserAppInfoDto _userAppInfoDto;
    private readonly ILoggerService _loggerService;

    private string _connectionString = string.Empty;

    public BaseDataService(IAppInfoDto appInfoDto, IUserAppInfoDto userAppInfoDto, ILoggerService loggerService)
    {
        _appInfoDto = appInfoDto;
        _userAppInfoDto = userAppInfoDto;
        _loggerService = loggerService;

        CheckConnectionString();
    }

    public async Task<IEnumerable<T>> GetAll<T>(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException("The tablename cannot be null or empty.", nameof(tableName));
        }

        using IDbConnection connection = new SqlConnection(_appInfoDto.ConnectionString);
        var result = await connection.QueryAsync<T>($"SELECT * FROM {tableName}");

        return result;
    }

    /// <summary>
    /// Returns the output of a SQL statement as a list of objects of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sqlStatement"></param>
    /// <returns></returns>
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

        // Log the result count
        logMessage = new LogMessageModel
        {
            Message = $"Query returned: {result.Count()} records",
            LogLevel = LogLevel.Information,
        };
        WeakReferenceMessenger.Default.Send(logMessage);
        
        return result;
    }


    /// <summary>
    /// Returns the output of a SQL statement as a single object of type T.
    /// </summary>
    /// <remarks>
    /// If multiple rows are returned, only the first row will be returned.
    /// </remarks>
    /// <param name="sqlStatement"></param>
    /// <returns></returns>
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

        return result;
    }

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
                    LogLevel = LogLevel.Information,
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
                    LogLevel = LogLevel.Information,
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
                        LogLevel = LogLevel.Information,
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
            throw new InvalidOperationException("The model doen't have a INT primary key, the method expects a primary key with name Id and type integer");
        }
    }

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
                    LogLevel = LogLevel.Information,
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
                    LogLevel = LogLevel.Information,
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
                        LogLevel = LogLevel.Information,
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
