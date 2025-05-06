using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace dotNIES.Data.Dto.Internal;
public class UserTokenDto
{
    //[Key]
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(1);
    public string User { get; set; } = Environment.UserName;
    public string? RefreshToken { get; set; }
    public bool IsRevoked { get; set; } = false;
}
