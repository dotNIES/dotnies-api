using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNIES.Data.Dto.Internal;

public class UserAppInfoDto : IUserAppInfoDto
{
    /// <summary>
    /// Unique session ID for the user.
    /// </summary>
    public Guid UserLoggerInfoId { get; set; }

    /// <summary>
    /// The name of the user who is logged in to the application.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The name of the user who is logged in to the application with windows authentication.
    /// </summary>
    public string? WindowsUserName { get; set; }

    /// <summary>
    /// If true, the application is running in development mode.
    /// </summary>
    public bool IsDevelopment { get; set; } = true;

    /// <summary>
    /// If true, the SQL statements will be logged.
    /// </summary>
    public bool LogSqlStatements { get; set; } = false;

    /// <summary>
    /// Unique session ID for the application.
    /// </summary>
    public Guid AppSessionId { get; set; }

    /// <summary>
    /// If true, the entire record will be logged as a JSON object.
    /// </summary>
    public bool LogEntireRecord { get; set; } = false;

    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Debug;
}
