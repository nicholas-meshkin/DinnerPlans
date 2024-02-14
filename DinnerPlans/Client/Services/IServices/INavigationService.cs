namespace DinnerPlans.Client.Services.IServices
{
    public interface INavigationService
    {
        /// <summary>
        /// set list to draw from
        /// </summary>
        /// <param name="newList"></param>
        void SetList(List<int> newList);
        /// <summary>
        /// set search string to retain on return to page
        /// </summary>
        /// <param name="search"></param>
        void SetSearch(string search);
        /// <summary>
        /// get search string (if any) on page load
        /// </summary>
        /// <returns></returns>
        string GetSearch();
        /// <summary>
        /// get id after current, 0 if at end
        /// </summary>
        /// <param name="curr"></param>
        /// <returns></returns>
        int Next(int curr);
        /// <summary>
        /// get id befor current, 0 if at beginning
        /// </summary>
        /// <param name="curr"></param>
        /// <returns></returns>
        int Previous(int curr);
        /// <summary>
        /// clear list
        /// </summary>
        void Clear();
    }
}
