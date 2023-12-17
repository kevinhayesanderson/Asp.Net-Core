using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.Contracts;
using Shared.DataTransferObjects;

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
        IMapper mapper,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration) : IAuthenticationService
    {
        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
        {
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
    }
}
