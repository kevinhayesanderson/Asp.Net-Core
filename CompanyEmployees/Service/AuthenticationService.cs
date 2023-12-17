using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DataTransferObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="userManager">
    /// This class is used to provide the APIs for managing users in a persistence store</param>
    /// <param name="configuration"></param>
    internal class AuthenticationService(
        ILoggerManager logger,
        IMapper mapper,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration) : IAuthenticationService
    {
        private User? _user;

        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            var signingCredentials = GetSigningCredentials(); //returns our secret key as a byte array with the security algorithm.

            var claims = await GetClaims();//creates a list of claims with the user name inside and all the roles the user belongs to.

            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateRefreshToken();

            _user.RefreshToken = refreshToken;

            if (populateExp)
                _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await userManager.UpdateAsync(_user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new TokenDto(accessToken, refreshToken);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET1")!);

            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, _user.UserName)
            };

            var roles = await userManager.GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: claims, expires:
                DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: signingCredentials);
            return tokenOptions;
        }

        public async Task<IdentityResult?> RegisterUser(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == null)
            {
                logger.LogWarn($"{nameof(RegisterUser)}: User registration failed. Password is required.");
                return null;
            }

            var user = mapper.Map<User>(userForRegistration);

            var result = await userManager.CreateAsync(user, userForRegistration.Password);

            if (result.Succeeded && userForRegistration.Roles is ICollection<string> roles)
            {
                foreach (var role in roles)
                {
                    var exists = await roleManager.RoleExistsAsync(role);

                    if (exists)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// we fetch the user from the database and check whether they exist and if the password matches.
        /// The UserManager<TUser> class provides the FindByNameAsync method to find the user by user name and the CheckPasswordAsync
        ///     to verify the user’s password against the hashed password from the database.
        /// </summary>
        /// <param name="userForAuth"></param>
        /// <returns></returns>
        public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
        {
            if (userForAuth.UserName == null || userForAuth.Password == null)
            {
                logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. user name or password is required.");
                return false;
            }

            _user = await userManager.FindByNameAsync(userForAuth.UserName);

            var result = _user != null && await userManager.CheckPasswordAsync(_user, userForAuth.Password);

            if (!result)
                logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password.");

            return result;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// used to get the user principal from the expired access token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET1"))),

                ValidateLifetime = true,

                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

            var user = await userManager.FindByNameAsync(principal.Identity.Name);

            //// If the user doesn’t exist, or the refresh tokens are not equal, or the refresh token has expired,
            //// we stop the flow returning the BadRequest response to the user.

            if (user == null
                || user.RefreshToken != tokenDto.RefreshToken
                || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new RefreshTokenBadRequestException();

            _user = user;

            return await CreateToken(populateExp: false);
        }
    }
}