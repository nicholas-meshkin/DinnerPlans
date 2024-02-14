using DinnerPlans.Shared.DTOs;

namespace DinnerPlans.Client.Services.IServices
{
    public interface IShoppingListService
    {
        /// <summary>
        /// add recpie to list
        /// </summary>
        /// <param name="recipeId"></param>
        /// <param name="name"></param>
        /// <param name="servings"></param>
        void AddToList(int recipeId, string name, int servings);
        /// <summary>
        /// toggle whether user wants to see amounts in metric
        /// </summary>
        void ToggleMetric();
        /// <summary>
        /// remove item from shopping list
        /// </summary>
        /// <param name="index"></param>
        void RemoveRecipeFromList(int index);
        /// <summary>
        /// clear list
        /// </summary>
        void Clear();
        /// <summary>
        /// get shopping list
        /// </summary>
        /// <returns></returns>
        ShoppingListDto GetList();
        /// <summary>
        /// get metric preference
        /// </summary>
        /// <returns></returns>
        bool GetMetricPref();
        /// <summary>
        /// get number of items in shopping list
        /// </summary>
        /// <returns></returns>
        int GetCount();
        /// <summary>
        /// event raised when list has changed
        /// </summary>
        event EventHandler? ItemsChanged;
        /// <summary>
        /// use store Id for later product searches
        /// </summary>
        /// <param name="store"></param>
        void SelectStore(string store);
        /// <summary>
        /// get store Id
        /// </summary>
        /// <returns></returns>
        string GetStore();
        /// <summary>
        /// clear list of selected products
        /// </summary>
        void ClearSelectedProducts();
        /// <summary>
        /// add selected product and query to list
        /// </summary>
        /// <param name="query"></param>
        /// <param name="dto"></param>
        void AddToSelectedProducts(string query, KrogerProductSearchResponseDto.Datum dto);
        /// <summary>
        /// add lookup results to list
        /// </summary>
        /// <param name="query"></param>
        /// <param name="dto"></param>
        void AddToProductList(string query, KrogerProductSearchResponseDto dto);
        /// <summary>
        /// remove from product list
        /// </summary>
        /// <param name="query"></param>
        void RemoveFromProductList(string query);
        /// <summary>
        /// get results from list by query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        KrogerProductSearchResponseDto? GetFromProductList(string query);
        /// <summary>
        /// get count of results for show results button
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        int GetProductResultCount(string query);
        /// <summary>
        /// get full search response list
        /// </summary>
        /// <returns></returns>
        Dictionary<string, KrogerProductSearchResponseDto> GetProductList();
        /// <summary>
        /// set list of ingredients to search for
        /// </summary>
        /// <param name="list"></param>
        void SetIngredientList(List<RecipeViewIngredientListItemDto> list);
        /// <summary>
        /// get list of ingredients to search for
        /// </summary>
        /// <returns></returns>
        List<RecipeViewIngredientListItemDto> GetIngredientList();
        /// <summary>
        /// parse out amount from string
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        decimal? ParseAmount(string line);
        /// <summary>
        /// parse out unit from string
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        string ParseUnit(string line);
        /// <summary>
        /// get integer multiplier to compare amount product comes in to amount recipe requires
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        int? GetMultiplier(ProductShoppingListDto dto);
        /// <summary>
        /// get type of amount
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        string GetAmountType(string unit);
    }
}
