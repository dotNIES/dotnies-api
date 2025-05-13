using System.Text.Json.Serialization;

namespace dotNIES.Data.Dto.Internal;
public class LoginModel
{
    [JsonPropertyName("username")]  public string Username { get; set; }
    [JsonPropertyName("password")] public string Password { get; set; }
}
