using Chinook.Domain.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        public UserService(AuthenticationStateProvider authenticationStateProvider)
        {
            this.authenticationStateProvider = authenticationStateProvider;
        }
        public async Task<string> GetAuthenticatedUserIdAsync()
        {
            var user = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var userIdClaim = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier));

            if (userIdClaim != null)
            {
                return userIdClaim.Value;
            }
            else
            {
                return null;
            };
        }
    }
}
