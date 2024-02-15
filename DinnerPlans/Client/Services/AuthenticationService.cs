using DinnerPlans.Client.Services.IServices;
using DinnerPlans.Shared.DTOs;

namespace DinnerPlans.Client.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        //TODO this could def be used better, for now just trying to store logged in users app Id


        public AuthenticationService() { }

        private UserDto? User;

        public void SetUser(UserDto dto)
        {
            User = dto;
        }

        public UserDto? GetUser()
        {
            return User;
        }

        public int? GetUserId() { return User?.Id; }
        public string? GetUserStore() { return User?.PreferredStore; }
        public bool GetUserMetricPref() { return User.MetricPreferred; }

        public void Clear()
        {
            User = null;
        }

    }
}
