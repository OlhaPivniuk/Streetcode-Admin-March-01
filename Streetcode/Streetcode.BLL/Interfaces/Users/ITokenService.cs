using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Streetcode.BLL.DTO.Account;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.Interfaces.Users
{
    public interface ITokenService
    {
        public AuthenticationResponseDto GenerateJWTToken(ApplicationUser user, List<Claim> claims);
        public Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user);
        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
    }
}
