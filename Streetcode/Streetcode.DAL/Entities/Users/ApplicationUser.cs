using Microsoft.AspNetCore.Identity;

namespace Streetcode.DAL.Entities.Users;

public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? RefreshToken { get; set; } = string.Empty;
    public DateTime? RefreshTokenExpirationDateTime { get; set; }
}
