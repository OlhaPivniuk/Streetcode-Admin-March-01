﻿using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Entities.Users;
using Microsoft.IdentityModel.Tokens;
using Streetcode.DAL.Entities.AdditionalContent.Jwt;
using Streetcode.BLL.DTO.Account;

namespace Streetcode.BLL.Services.Users;

public class TokenService : ITokenService
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly RefreshTokenConfiguration _refreshTokenConfiguration;
    public TokenService(JwtConfiguration jwtConfiguration, RefreshTokenConfiguration refreshTokenConfiguration)
    {
        _jwtConfiguration = jwtConfiguration;
        _refreshTokenConfiguration = refreshTokenConfiguration;
    }

    public AuthenticationResponseDto GenerateJWTToken(ApplicationUser user)
    {
        DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtConfiguration.ExpirationMinutes));

        Claim[] claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject (user id)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT unique ID
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), // Date and time of token generation
            new Claim(ClaimTypes.NameIdentifier, user.Email.ToString()), // Email
            new Claim(ClaimTypes.Name, user.UserName.ToString()), // Name of the user
            new Claim(ClaimTypes.Email, user.Email), // Name of the user
        };

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key!));

        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken tokenGenerator = new JwtSecurityToken(
            claims: claims,
            expires: expiration,
            signingCredentials: signingCredentials);

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        var token = jwtSecurityTokenHandler.WriteToken(tokenGenerator);

        AuthenticationResponseDto response = new AuthenticationResponseDto()
        {
            UserName = user.UserName,
            Email = user.Email,
            Token = token,
            Expiration = expiration,
            RefreshToken = GenerateRefreshToken(),
            RefreshTokenExpirationDateTime = DateTime.UtcNow.AddDays(_refreshTokenConfiguration.ExpirationDays)
        };
        return response;
    }

    public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key!)),
            ValidateLifetime = false
        };

        JwtSecurityTokenHandler tokenValidationHandler = new JwtSecurityTokenHandler();

        ClaimsPrincipal claims = tokenValidationHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid Token");
        }

        return claims;
    }

    private static string GenerateRefreshToken()
    {
        byte[] bytes = new byte[64];

        var randomNumberGenerator = RandomNumberGenerator.Create();

        randomNumberGenerator.GetBytes(bytes);

        return Convert.ToBase64String(bytes);
    }
}
