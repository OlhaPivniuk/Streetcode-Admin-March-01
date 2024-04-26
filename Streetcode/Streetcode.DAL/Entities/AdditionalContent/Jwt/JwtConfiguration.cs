namespace Streetcode.DAL.Entities.AdditionalContent.Jwt;

public class JwtConfiguration
{
    public int ExpirationMinutes { get; set; }
    public string? Key { get; set; } = string.Empty;
}
