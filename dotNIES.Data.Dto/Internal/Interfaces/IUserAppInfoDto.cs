
namespace dotNIES.Data.Dto.Internal;

public interface IUserAppInfoDto
{
    Guid AppSessionId { get; set; }
    bool IsDevelopment { get; set; }
    bool LogEntireRecord { get; set; }
    bool LogSqlStatements { get; set; }
    Guid UserLoggerInfoId { get; set; }
    string? UserName { get; set; }
    string? WindowsUserName { get; set; }
}