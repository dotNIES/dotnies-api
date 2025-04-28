namespace dotNIES.Data.Dto.Internal;

public interface IAppInfoDto
{
    string? AppName { get; set; }
    string? AppVersion { get; set; }
    string ConnectionString { get; set; }
}