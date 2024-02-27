using DinnerPlans.Server.Persistence.Entities;
using DinnerPlans.Shared.DTOs;
using Microsoft.AspNetCore.Components.Forms;

namespace DinnerPlans.Server.Core.IServices
{
    public interface IAppService
    {
        /// <summary>
        /// test first method
        /// </summary>
        /// <returns></returns>
        Task<string> Home();
        
        /// <summary>
        /// get list of recipes for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IList<RecipeListItemDto>> GetRecipesForUser(int userId);
        /// <summary>
        /// get list of default recipes
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IList<RecipeListItemDto>> GetBaseRecipeList(int userId);

        /// <summary>
        /// get recipe by id, multiply amounts by servings and convert from metric if necessary, default servings is 2
        /// </summary>
        /// <param name="recipeId"></param>
        /// <param name="servings"></param>
        /// <param name="metricPreffered"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<RecipeViewDto> GetRecipeById(int recipeId, int servings, bool metricPreffered, int userId);
        /// <summary>
        /// check if it is ok for user to make copy of recipe
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        Task<bool> CanUserCopyRecipe(int userId, int recipeId);
        /// <summary>
        /// create user-specific copy of recipe
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        Task<bool> CopyRecipeForUser(int userId, int recipeId);
        /// <summary>
        /// add new ingredients to table
        /// </summary>
        /// <param name="items"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> AddNewIngredients(List<string> items, int userId);
        /// <summary>
        /// check if recipe name is in use for user
        /// </summary>
        /// <param name="recipeName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> IsRecipeNameValid(string recipeName, int userId);
        /// <summary>
        /// save image file and return path
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        Task<string> StoreRecipeImage(int userId, IFormFile imageFile);
        /// <summary>
        /// create recipe entity
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recipeName"></param>
        /// <param name="servings"></param>
        /// <returns></returns>
        Task<Recipe> CreateRecipeEntity(int userId, string recipeName, int servings, string path);
        /// <summary>
        /// get default servings for recipe on initial page load
        /// </summary>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        Task<int> GetDefaultServingsForRecipeById(int recipeId);
        /// <summary>
        /// flag or unflag recipe for later edit
        /// </summary>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        Task<bool> ChangeRecipeEditFlag(int recipeId);
        /// <summary>
        /// update user's rating of recipe
        /// </summary>
        /// <param name="recipeId"></param>
        /// <param name="userId"></param>
        /// <param name="rating"></param>
        /// <returns></returns>
        Task<bool> UpdateUserRating(int recipeId, int rating, int userId);
        /// <summary>
        /// add or update comment on recipe
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> UpdateRecipeComment(RecipeCommentDto dto);
        /// <summary>
        /// get dto for editing recipe
        /// </summary>
        /// <param name="recipeId"></param>
        /// <param name="servings"></param>
        /// <param name="metricPreffered"></param>
        /// <returns></returns>
        Task<RecipeEditDto> GetRecipeByIdForEdit(int recipeId, int servings, bool metricPreffered);
        /// <summary>
        /// edit recipe
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userId"></param>
        /// <param name="isMetric"></param>
        /// <param name="servings"></param>
        /// <returns></returns>
        Task<bool> EditRecipe(RecipeEditDto dto, int userId, bool isMetric, int servings);
        /// <summary>
        /// get empty dto for adding new recipe
        /// </summary>
        /// <param name="metricPreferred"></param>
        /// <returns></returns>
        Task<RecipeEditDto> GetRecipeDtoForAdd(bool metricPreferred);
        /// <summary>
        /// get list of items needed for selected recipes
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="metricPreferred"></param>
        /// <returns></returns>
        Task<List<RecipeViewIngredientListItemDto>> GetShoppingList(ShoppingListDto dto, bool metricPreferred);
        /// <summary>
        /// get list of ingredients to manually add to shopping list
        /// </summary>
        /// <returns></returns>
        Task<IList<RecipeViewIngredientListItemDto>?> GetIngredientsForShoppingList();

        /// <summary>
        /// log error
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="e"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        Task CreateErrorLog(string identifier, Exception? e, string method);

        /// <summary>
        /// get user
        /// </summary>
        /// <param name="aoId"></param>
        /// <returns></returns>
        Task<UserDto?> GetUser(string aoId);
        /// <summary>
        /// create user if doesn't exist
        /// </summary>
        /// <param name="aoId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<UserDto?> CreateUser(string aoId, string name);
        /// <summary>
        /// create or update assn between user, ingredient, and productId
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> UpdateUserProductFavorite(UserIngredientProductFavoriteDto dto);
        /// <summary>
        /// delete association between product and ingredient for user
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> DeleteUserProductFavorite(UserIngredientProductFavoriteDto dto);
        /// <summary>
        /// update user preferences
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<UserDto?> UpdateUserPreferences(UserDto dto);


        ///// <summary>
        ///// get recipe dto from image file with list of items and amounts, return to FE for corrections
        ///// </summary>
        ///// <param name="filepath"></param>
        ///// <returns></returns>
        //Task<OcrTestRecipeAddDto> ProcessIngredientListOcr(string filepath);
        ///// <summary>
        ///// test of ocr
        ///// </summary>
        ///// <returns></returns>
        //Task<string> OcrTest();

    }
}
