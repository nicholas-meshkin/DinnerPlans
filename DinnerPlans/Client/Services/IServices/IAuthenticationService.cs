using DinnerPlans.Shared.DTOs;

namespace DinnerPlans.Client.Services.IServices
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// set user on login
        /// </summary>
        /// <param name="dto"></param>
        void SetUser(UserDto dto);
        /// <summary>
        /// get user
        /// </summary>
        /// <returns></returns>
        UserDto? GetUser();
        /// <summary> 
        /// get id of current user
        /// </summary>
        /// <returns></returns>
        int? GetUserId();
        /// <summary>
        /// get user's preferred store, if any
        /// </summary>
        /// <returns></returns>
        string? GetUserStore();
        /// <summary>
        /// get user's metric preference, defaults to false
        /// </summary>
        /// <returns></returns>
        bool GetUserMetricPref();
        /// <summary>
        /// clear when logout
        /// </summary>
        void Clear();
    }
}
