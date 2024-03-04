using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using DinnerPlans.Server.Core.IServices;
using DinnerPlans.Server.Core.ErrorHandling;
using DinnerPlans.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using DinnerPlans.Server.Core.Util;

namespace DinnerPlans.Server.Controllers
{
    [Route("api/[controller]")]
   
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IAppService _appService;
        private readonly ILogger<MainController> _logger;

        public MainController(IAppService appService, ILogger<MainController> logger)
        {
            _appService = appService;
            _logger = logger;
        }

        [HttpGet("home")]
        [Authorize]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Home()
        {
            try
            {
                return Ok(await _appService.Home());
            }
            catch (Exception ex) 
            { 
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpGet("user/{aoId}")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUser(string aoId)
        {
            try
            {
                return Ok(await _appService.GetUser(aoId));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpPost("user/create")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateUser(UserDto dto)
        {
            try
            {
                return Ok(await _appService.CreateUser(dto.AoId, dto.Name));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpPost("user/updatePreferences")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateUser(UserDto dto)
        {
            try
            {
                return Ok(await _appService.UpdateUserPreferences(dto));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpGet("recipes/suggested/{userId}")]
        [ProducesResponseType(typeof(IList<RecipeListItemDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBaseRecipeList(int userId)
        {
            try
            {
                return Ok(await _appService.GetBaseRecipeList(userId));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        //TODO do this better once logins are set up
        [HttpGet("recipes/{userId}")]
        [ProducesResponseType(typeof(IList<RecipeListItemDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRecipesForUser(int userId)
        {
            try
            {
                return Ok(await _appService.GetRecipesForUser(userId));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpGet("recipes/copy/{userId}/{recipeId}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CopyRecipeForUser(int userId, int recipeId)
        {
            try
            {
                if(await _appService.CanUserCopyRecipe(userId, recipeId))
                    return Ok(await _appService.CopyRecipeForUser(userId, recipeId));
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        //TODO do this better once logins are set up
        [HttpGet("recipes/{id}/{servings}/{metricPreffered}/{userId}")]
        [ProducesResponseType(typeof(RecipeViewDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRecipeById(int id, int servings, bool metricPreffered, int userId)
        {
            try
            {
                //TODO I think the FE makes this unecessary, remove later
                //int serv;
                //if (servings == null || servings == 0) serv = 2;
                //else serv = (int)servings;
                //TODO move metric preference to a user-based thing maybe
                return Ok(await _appService.GetRecipeById(id, servings, metricPreffered, userId));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpGet("recipes/defServ/{id}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDefaultServingsForRecipeById(int id)
        {
            try
            {
                return Ok(await _appService.GetDefaultServingsForRecipeById(id));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpGet("recipes/rating/{id}/{rating}/{userId}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateUserRecipeRating(int id, int rating, int userId)
        {
            try
            {
                return Ok(await _appService.UpdateUserRating(id, rating, userId));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpPost("recipes/comment/update")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateRecipeComment(RecipeCommentDto dto)
        {
            try
            {
                return Ok(await _appService.UpdateRecipeComment(dto));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpGet("recipes/flag/{id}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ChangeRecipeEditFlag(int id)
        {
            try
            {
                return Ok(await _appService.ChangeRecipeEditFlag(id));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        //TODO do this better once logins are set up
        [HttpGet("recipes/editview/{id}/{servings}/{metricPreffered}")]
        [ProducesResponseType(typeof(RecipeEditDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRecipeByIdForEdit(int id, int? servings, bool metricPreffered)
        {
            try
            {
                int serv;
                if (servings == null || servings == 0) serv = 2;
                else serv = (int)servings;
                return Ok(await _appService.GetRecipeByIdForEdit(id, serv, metricPreffered));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpGet("recipes/new/{metricPreffered}")]
        [ProducesResponseType(typeof(RecipeEditDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRecipeDtoForAdd(bool metricPreffered)
        {
            try
            {
                return Ok(await _appService.GetRecipeDtoForAdd(metricPreffered));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpPost("recipes/shoppingList/view")]
        [ProducesResponseType(typeof(List<RecipeViewIngredientListItemDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetShoppingList(ShoppingListDto dto)
        {
            try
            {
                //TODO fix this
                return Ok(await _appService.GetShoppingList(dto, false));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpPost("recipes/shoppingList/view/favorite/update")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetProductFavorite(UserIngredientProductFavoriteDto dto)
        {
            try
            {
                return Ok(await _appService.UpdateUserProductFavorite(dto));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpGet("recipes/shoppingList/ingredients")]
        [ProducesResponseType(typeof(List<RecipeViewIngredientListItemDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetIngredientsForShoppingList()
        {
            try
            {
                return Ok(await _appService.GetIngredientsForShoppingList());
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpPost("recipes/shoppingList/view/favorite/delete")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> RemoveProductFavorite(UserIngredientProductFavoriteDto dto)
        {
            try
            {
                return Ok(await _appService.DeleteUserProductFavorite(dto));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpPost("recipes/edit/{userId}/image")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> StoreRecipeImage(int userId/*, IBrowserFile imageFile*/)
        {
            try
            {
                //method only stores image and returns location, assn with recipe is in later step
                var imageFile = Request.Form.Files[0];
                if (imageFile.Length > AppConstants.MAX_FILE_SIZE) return new JsonResult(ErrorMessages.FileTooBig);
                if(imageFile.Length == 0) return new JsonResult(ErrorMessages.FileSizeZero);

                return Ok(await _appService.StoreRecipeImage( userId, imageFile));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }
        [HttpPost("recipes/edit/{userId}/processIngredientFile")]
        [ProducesResponseType(typeof(IList<RecipeIngredientDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ProcessIngredientFile(int userId)
        {
            try
            {
                //method only stores image and returns location, assn with recipe is in later step
                var imageFile = Request.Form.Files[0];
                if (imageFile.Length > AppConstants.MAX_FILE_SIZE) return new JsonResult(ErrorMessages.FileTooBig);
                if (imageFile.Length == 0) return new JsonResult(ErrorMessages.FileSizeZero);

                return Ok(await _appService.GetIngredientsFromUpload(userId, imageFile));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpPost("recipes/edit/{userId}/processInstructionFile")]
        [ProducesResponseType(typeof(IList<InstructionDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ProcessInstructionFile(int userId)
        {
            try
            {
                //method only stores image and returns location, assn with recipe is in later step
                var imageFile = Request.Form.Files[0];
                if (imageFile.Length > AppConstants.MAX_FILE_SIZE) return new JsonResult(ErrorMessages.FileTooBig);
                if (imageFile.Length == 0) return new JsonResult(ErrorMessages.FileSizeZero);

                return Ok(await _appService.GetInstructionsFromUpload(userId, imageFile));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        [HttpPost("recipes/edit/{userId}/{isMetric}/{servings}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> EditRecipe(RecipeEditDto dto, int userId, bool isMetric, int servings)
        {
            try
            {
                //if new, check if name is valid
                if(dto.Id== 0)
                {
                    var isNameValid = await _appService.IsRecipeNameValid(dto.Name, userId);
                    if(!isNameValid) return new JsonResult(ErrorMessages.ErrorInvalidName); 
                }

                return Ok(await _appService.EditRecipe(dto, userId, isMetric, servings));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.Error500, ex.Message);
                return new JsonResult(ErrorMessages.Error500);
            }
        }

        #region OcrTestRegion
        //not used any more, keeping in case I want to test out OCR again

        //[HttpGet("OcrTest")]
        //[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        //public async Task<IActionResult> OcrTest()
        //{
        //    try
        //    {
        //        var success = await _appService.OcrTest();
        //        return Ok(success);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical(ErrorMessages.Error500, ex.Message);
        //        return new JsonResult(ErrorMessages.Error500);
        //    }
        //}


        //[HttpGet("GetIngredientList")]
        //[ProducesResponseType(typeof(OcrTestRecipeAddDto), (int)HttpStatusCode.OK)]
        //public async Task<IActionResult> GetIngredientList(string filePath)
        //{
        //    try
        //    {
        //        var dto = await _appService.ProcessIngredientListOcr(filePath);
        //        return Ok(dto);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical(ErrorMessages.Error500, ex.Message);
        //        return new JsonResult(ErrorMessages.Error500);
        //    }
        //}

        //[HttpGet("testMethod")]
        //[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        //public async Task<IActionResult> Test()
        //{
        //    try
        //    {
        //        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("ntm-recipe-test-app-a80d772306573ea64c0acb57f2c9dc6f872075977812836639:DaIcTncyvZrZ3DmYmMMlyOuGcL6CHLuXgOEpGenb");
        //        return Ok(System.Convert.ToBase64String(plainTextBytes));

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical(ErrorMessages.Error500, ex.Message);
        //        return new JsonResult(ErrorMessages.Error500);
        //    }
        //}

        //[HttpPost("recipes/save")]
        //[ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        //public async Task<IActionResult> SaveNewRecipe(RecipeAddDto dto)
        //{
        //    try
        //    {
        //        return Ok(await _appService.SaveNewRecipe(dto));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical(ErrorMessages.Error500, ex.Message);
        //        return new JsonResult(ErrorMessages.Error500);
        //    }
        //}

        #endregion OcrTestRegion

    }
}
