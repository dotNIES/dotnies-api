namespace dotNIES.Data.Dto.Internal;

public class AppInfoDto : IAppInfoDto
{
    public string ConnectionString { get; set; } = string.Empty;
    public string? AppVersion { get; set; }
    public string? AppName { get; set; }
}
