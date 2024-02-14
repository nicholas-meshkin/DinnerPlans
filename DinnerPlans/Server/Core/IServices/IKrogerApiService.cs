using DinnerPlans.Shared.DTOs;

namespace DinnerPlans.Server.Core.IServices
{
    public interface IKrogerApiService
    {
        /// <summary>
        /// search products api with variety of parameters
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        Task<KrogerProductSearchResponseDto> ProductSearch(KrogerProductSearchRequestDto searchDto);
        /// <summary>
        /// search locations nearby with zip - radius is optional?
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        Task<LocationSearchResponseDto> LocationSearch(string zip, int radius);
    }
}
