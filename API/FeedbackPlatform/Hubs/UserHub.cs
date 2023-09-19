using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.IService;
using Service;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Entities.Models;
using Nest;

namespace FeedbackPlatform.Hubs
{
    public class UserHub : Hub
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthenticationManager _authManager;
        private readonly IServiceManager _serviceManager;

        public UserHub(IAuthenticationManager authManager, IServiceManager serviceManager, UserManager<User> userManager)
        {
            _authManager = authManager;
            _serviceManager = serviceManager;
            _userManager = userManager;
        }

        public async Task BlockUser(IEnumerable<Guid> ids)
        {
            foreach(var id in ids)
            {
                await _authManager.BlockUser(id);
            }

            await _serviceManager.SaveAsync();

            await Clients.All.SendAsync("UserBlocked");
        }

        public async Task UnblockUser(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                await _authManager.UnblockUser(id);
            }

            await _serviceManager.SaveAsync();

            await Clients.All.SendAsync("UserUnblocked");
        }

        public async Task MakeUserAdmin([FromBody] IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id.ToString("D"));
                if (user == null)
                {
                    return;
                }

                var removeFromUserRoleResult = await _userManager.RemoveFromRoleAsync(user, "User");
                if (!removeFromUserRoleResult.Succeeded)
                {
                    return;
                }

                var addToAdminRoleResult = await _userManager.AddToRoleAsync(user, "Administrator");
                if (!addToAdminRoleResult.Succeeded)
                {
                    return;
                }
            }

            await Clients.All.SendAsync("UserRoleChange");
        }
    }
}
