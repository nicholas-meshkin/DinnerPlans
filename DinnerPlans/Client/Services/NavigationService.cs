using DinnerPlans.Client.Services.IServices;

namespace DinnerPlans.Client.Services
{
    public class NavigationService : INavigationService
    {
        public NavigationService() { }

        public List<int> CurrentRecipeIds { get; set; } = new List<int>();
        public string SearchString { get; set; }

        public void Clear()
        {
            CurrentRecipeIds.Clear();
        }

        public int Next(int curr)
        {
            var currInd = CurrentRecipeIds.IndexOf(curr);
            if (currInd + 1 == CurrentRecipeIds.Count) return 0;
            return CurrentRecipeIds[currInd + 1];
        }

        public int Previous(int curr)
        {
            var currInd = CurrentRecipeIds.IndexOf(curr);
            if (currInd -1 < 0 ) return 0;
            return CurrentRecipeIds[currInd - 1];
        }

        public void SetList(List<int> newList)
        {
            CurrentRecipeIds = newList;
        }

        public void SetSearch(string search)
        {
            SearchString = search;
        }
        public string GetSearch() 
        {
            return SearchString;
        }
    }
}
