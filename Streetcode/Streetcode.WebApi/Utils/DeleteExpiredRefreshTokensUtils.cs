using Microsoft.AspNetCore.Identity;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.WebApi.Utils;

public class DeleteExpiredRefreshTokensUtils
{
    private readonly UserManager<ApplicationUser> _userManager;
    public DeleteExpiredRefreshTokensUtils(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task DeleteExpiredRefreshTokens()
    {
        var users = _userManager.Users.Where(x => x.RefreshTokenExpirationDateTime <= DateTime.UtcNow).ToList();

        if (users.Count > 0)
        {
            foreach (var user in users)
            {
                user.RefreshToken = null;

                user.RefreshTokenExpirationDateTime = null;

                await _userManager.UpdateAsync(user);
            }
        }
    }
}
