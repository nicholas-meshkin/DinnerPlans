using Microsoft.VisualBasic;
using DinnerPlans.Server.Core.IServices;
using DinnerPlans.Shared.DTOs;
using DinnerPlans.Server.Persistence.Entities;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Forms;

namespace DinnerPlans.Server.Core.Services
{
    public class AppService : IAppService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public AppService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<string> Home()
        {
            try
            {
                return "succcess";
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "Home");
            }
            return "";
        }

        public async Task<IList<RecipeListItemDto>> GetBaseRecipeList(int userId)
        {
            try
            {
                //don't want to include copies
                var excludeList = new List<int>();

                var history = await _unitOfWork.ReadRepo.GetListWithPredicateAsNoTracking<RecipeCopyHistory>(a => a.UserId != 0);

                if (history.Any())
                {
                    excludeList = history.Select(b => b.CopyRecipeId).ToList();
                }

                var list = await _unitOfWork.ReadRepo.GetListWithPredicateAndProjection(a => a.IsActive && !excludeList.Contains(a.Id), Projection.RecipeListItemDtoProjection);

                var ratingList = await _unitOfWork.ReadRepo.GetListWithPredicate<UserRecipeRating>(a => a.UserId == userId);

                await GetSearchableIngredientListForRecipes(list);

                foreach (var item in list)
                {
                    if (!ratingList.Any() || !ratingList.Select(b => b.RecipeId).Contains(item.Id)) item.Rating = 0;
                    else item.Rating = ratingList.First(b => b.RecipeId == item.Id).Rating;
                }
                return list;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("userId: " + userId, ex, "GetBaseRecipeList");
            }
            return null;
        }

        public async Task<IList<RecipeListItemDto>> GetRecipesForUser(int userId)
        {
            try
            {
                var list = await _unitOfWork.ReadRepo.GetListWithPredicateAndProjection(a => a.IsActive && a.CreatedById == userId, Projection.RecipeListItemDtoProjection);

                var ratingList = await _unitOfWork.ReadRepo.GetListWithPredicate<UserRecipeRating>(a => a.UserId == userId);

                await GetSearchableIngredientListForRecipes(list);

                foreach (var item in list)
                {
                    if (!ratingList.Any() || !ratingList.Select(b => b.RecipeId).Contains(item.Id)) item.Rating = 0;
                    else item.Rating = ratingList.First(b => b.RecipeId == item.Id).Rating;
                }
                return list;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("userId: " + userId, ex, "GetRecipesForUser");
            }
            return null;
        }

        private async Task GetSearchableIngredientListForRecipes(IList<RecipeListItemDto> recipeDtos)
        {
            try
            {
                var recipeIds = recipeDtos.Select(d => d.Id).ToList();
                var list = await _unitOfWork.ReadRepo.GetListWithPredicateAndInclude<RecipeIngredientAmountType>(a => recipeIds.Contains(a.RecipeId), a => a.Ingredient);
                
                foreach(var item in recipeDtos)
                {
                    item.Ingredients = list.Where(b => b.RecipeId == item.Id).Select(c => c.Ingredient.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "GetSearchableIngredientListForRecipes");
            }
            return;
        }

        public async Task<RecipeViewDto> GetRecipeById(int recipeId, int servings, bool metricPreffered, int userId)
        {
            try
            {

                var dto = await _unitOfWork.ReadRepo.GetEntityWithPredicateAndProjection(a => a.Id == recipeId && a.IsActive, Projection.RecipeViewDtoProjection);
                var list = await _unitOfWork.ReadRepo.GetListWithPredicateAndProjection(
                    a => a.RecipeId == recipeId && a.IsActive, Projection.RecipeViewIngredientListItemDtoProjection);

                foreach (var item in list)
                {
                    await GetAmountAndConvertUnits(metricPreffered, servings, item);
                }
                dto.Ingredients = list;
                dto.Instructions = (await _unitOfWork.ReadRepo.GetListWithPredicateAndProjection(
                    a => a.RecipeId == recipeId && a.IsActive, Projection.RecipeViewInstructionListItemDtoProjection)).OrderBy(b => b.Order).ToList();

                //TODO figure out how to do comments - if they only show for user that made them or if they should be public
                dto.Comments = (await _unitOfWork.ReadRepo.GetListWithPredicateAndProjection(a => a.RecipeId == recipeId , Projection.RecipeCommentDtoProjection)).OrderByDescending(a => a.Updated).ToList();

                var userRating = await _unitOfWork.ReadRepo.GetEntityWithPredicateAsNoTracking<UserRecipeRating>(a => a.UserId == userId && a.RecipeId == recipeId);
                dto.Rating = userRating?.Rating;

                return dto;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("recipeId: " + recipeId, ex, "GetRecipe");
            }
            return null;
        }

        public async Task<int> GetDefaultServingsForRecipeById(int recipeId) 
        {
            try 
            {
                var defServ = (await _unitOfWork.ReadRepo.GetEntityWithPredicateAsNoTracking<Recipe>(a => a.IsActive && a.Id == recipeId)).DefaultServings;
                return defServ;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("recipeId: " + recipeId, ex, "GetDefaultServingsForRecipeById");
            }
            return 2;
        }

        private async Task<RecipeViewIngredientListItemDto> GetAmountAndConvertUnits(bool metricPreferred, int servings, RecipeViewIngredientListItemDto dto)
        {
            try
            {
                var amtBase = dto.Amount * servings;

                if (!metricPreferred)
                {
                    if (dto.Unit.Equals("G"))
                    {
                        //TODO test and apply elsewhere
                        if(amtBase / 28.3495 >= 16)
                        {
                            amtBase = amtBase / 453.592;
                            dto.Unit = "LB.";
                        }
                        else
                        {
                            amtBase = amtBase / 28.3495;
                            dto.Unit = "OZ.";
                        }
                    }
                    if (dto.Unit.Equals("ML"))
                    {
                        if (amtBase / 236.5882365 >= 1 || (dto.IsDryMeasure && amtBase / 236.5882365 >= .25))
                        {
                            amtBase = amtBase / 236.5882365;
                            dto.Unit = "CUP";
                        }
                        else if (!dto.IsDryMeasure && amtBase / 29.5735 >= 1)
                        {
                            amtBase = amtBase / 29.5735;
                            dto.Unit = "FL. OZ.";
                        }
                        else if (amtBase / 14.7868 >= 1)
                        {
                            amtBase = amtBase / 14.7868;
                            dto.Unit = "TBSP.";
                        }
                        else
                        {
                            amtBase = amtBase / 4.92892;
                            dto.Unit = "TSP.";
                        }
                    }
                }
                if (amtBase.ToString().Length > 5)
                {

                    var nearestInt = Math.Round(amtBase);
                    if (nearestInt - amtBase < 0.01 && nearestInt - amtBase > -0.01) amtBase = nearestInt;
                    else amtBase = Math.Round(amtBase, 2);
                }
                dto.Amount = amtBase;
                dto.DisplayAmount = dto.Unit.Equals("UNIT") ? dto.Amount.ToString() : dto.Amount + " " + dto.Unit;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("recipeId: ", ex, "GetAmountAndConvertUnits");
            }
            return dto;
        }

        private async Task GetAmountAndConvertUnitsForEdit(bool metricPreferred, int servings, RecipeIngredientEditDto dto)
        {
            try
            {
                var amtBase = dto.Amount * servings;

                if (!metricPreferred)
                {
                    if (dto.Measure.Unit.Equals("G"))
                    {
                        //TODO test and apply elsewhere
                        if (amtBase / 28.3495 >= 16)
                        {
                            amtBase = amtBase / 453.592;
                            dto.Measure.Unit = "LB.";
                        }
                        else
                        {
                            amtBase = amtBase / 28.3495;
                            dto.Measure.Unit = "OZ.";
                        }
                    }
                    if (dto.Measure.Unit.Equals("ML"))
                    {
                        if (amtBase / 236.5882365 >= 1)
                        {
                            amtBase = amtBase / 236.5882365;
                            dto.Measure.Unit = "CUP";
                        }
                        else if (amtBase / 29.5735 >= 1)
                        {
                            amtBase = amtBase / 29.5735;
                            dto.Measure.Unit = "FL. OZ.";
                        }
                        else if (amtBase / 14.7868 >= 1)
                        {
                            amtBase = amtBase / 14.7868;
                            dto.Measure.Unit = "TBSP.";
                        }
                        else
                        {
                            amtBase = amtBase / 4.92892;
                            dto.Measure.Unit = "TSP.";
                        }
                    }
                }

                if (amtBase.ToString().Length > 5)
                {

                    var nearestInt = Math.Round(amtBase);
                    if (nearestInt - amtBase < 0.01 && nearestInt - amtBase > -0.01) amtBase = nearestInt;
                    else amtBase = Math.Round(amtBase, 2);
                }
                dto.Amount = amtBase;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("recipeId: ", ex, "GetAmountAndConvertUnitsForEdit");
            }
            return;
        }

        private async Task ConvertUnitsFromImperial(int servings, RecipeIngredientEditDto dto)
        {
            try
            {
                var amtBase = dto.Amount / servings;

                if (dto.Measure.Unit.Equals("OZ."))
                {
                    amtBase = amtBase * 28.3495;
                    dto.Measure.Unit = "G";
                }

                if (dto.Measure.Unit.Equals("LB."))
                {
                    amtBase = amtBase * 453.592; 
                    dto.Measure.Unit = "G";
                }

                if (dto.Measure.Unit.Equals("CUP"))
                {
                    amtBase = amtBase * 236.5882365;
                    dto.Measure.Unit = "ML";
                }

                if (dto.Measure.Unit.Equals("FL. OZ."))
                {
                    amtBase = amtBase * 29.5735;
                    dto.Measure.Unit = "ML";
                }

                if (dto.Measure.Unit.Equals("TBSP."))
                {
                    amtBase = amtBase * 14.7868;
                    dto.Measure.Unit = "ML";
                }
                if (dto.Measure.Unit.Equals("TSP."))
                {
                    amtBase = amtBase * 4.92892;
                    dto.Measure.Unit = "ML";
                }

                if (amtBase.ToString().Length > 5)
                {
                    var nearestInt = Math.Round(amtBase);
                    if (nearestInt - amtBase < 0.01 && nearestInt - amtBase > -0.01) amtBase = nearestInt;
                    else amtBase = Math.Round(amtBase, 2);
                }
                dto.Amount = amtBase;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("recipeId: ", ex, "ConvertUnitsFromImperial");
            }
            return;
        }
        private async Task ConvertServingsForMetric(int servings, RecipeIngredientEditDto dto)
        {
            try
            {
                var amtBase = dto.Amount / servings;

                dto.Amount = amtBase;

            }
            catch (Exception ex)
            {
                await CreateErrorLog("recipeId: ", ex, "ConvertServingsForMetric");
            }
            return;
        }

        public async Task<bool> CanUserCopyRecipe(int userId, int recipeId)
        {
            try
            {
                var isOriginalCreator = (await _unitOfWork.ReadRepo.GetEntityWithPredicateAsNoTracking<Recipe>(a => a.Id == recipeId && a.CreatedById == userId)) != null;
                if (isOriginalCreator) return false;

                var extantCopy = await _unitOfWork.ReadRepo.GetEntityWithPredicateAsNoTracking<RecipeCopyHistory>(a => a.UserId == userId && a.OriginalRecipeId == recipeId);
                if (extantCopy == null) return true;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("userId: " + userId, ex, "CanUserCopyRecipe");
            }
            return false;
        }

        public async Task<bool> CopyRecipeForUser(int userId, int recipeId)
        {
            try
            {
                var origRec = await _unitOfWork.ReadRepo.GetEntityWithPredicateAsNoTracking<Recipe>(a => a.Id == recipeId);

                //create recipe entity
                var newRec = await CreateRecipeEntity(userId, origRec.Name, origRec.DefaultServings, origRec.ImageFilePath);
                if (newRec == null) return false;

                //create history
                var history = new RecipeCopyHistory { CopyRecipeId = newRec.Id, OriginalRecipeId = origRec.Id, UserId = userId };
                if(!await SaveRelationship(history)) return false;

                //create ingredient list
                if (!await CopyIngredientAssociations(recipeId, newRec.Id)) return false;

                //create instruction list
                if (!await CopyInstructions(recipeId, newRec.Id)) return false;

                return true;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("userId: " + userId, ex, "CopyRecipeForUser");
            }
            return false;
        }

        private async Task<bool> CopyIngredientAssociations(int origRecipeId, int newRecipeId)
        {
            try
            {
                var origAssns = await _unitOfWork.ReadRepo.GetListWithPredicateAsNoTracking<RecipeIngredientAmountType>(a => a.IsActive && a.RecipeId == origRecipeId);

                var newAssns = origAssns.Select(a => new RecipeIngredientAmountType { RecipeId = newRecipeId, IngredientId = a.IngredientId, AmountTypeId = a.AmountTypeId, Amount = a.Amount, IsActive = true }).ToList();

                return await SaveRange(newAssns);
            }
            catch (Exception ex)
            {
                await CreateErrorLog("origRecipeId: " + origRecipeId, ex, "CopyIngredientAssociations");
            }
            return false;
        }

        private async Task<bool> CopyInstructions(int origRecipeId, int newRecipeId)
        {
            try
            {
                var origAssns = await _unitOfWork.ReadRepo.GetListWithPredicateAsNoTracking<Instruction>(a => a.IsActive && a.RecipeId == origRecipeId);

                var newAssns = origAssns.Select(a => new Instruction { RecipeId = newRecipeId, Order = a.Order, Instruction = a.Instruction, IsActive = true }).ToList();

                return await SaveRange(newAssns);
            }
            catch (Exception ex)
            {
                await CreateErrorLog("origRecipeId: " + origRecipeId, ex, "CopyInstructions");
            }
            return false;
        }

        public async Task<string> StoreRecipeImage(int userId, IFormFile imageFile) 
        {
            try 
            {
                string trustedFileName = Guid.NewGuid().ToString();
                //TODO move this somewhere, also change on deploy
                var path = Path.Combine(@"C:\Users\nickt\source\repos\DinnerPlans\DinnerPlans\Client\wwwroot\Images", trustedFileName + ".jpg");

                await using var fs = new FileStream(path, FileMode.Create);
                await imageFile.OpenReadStream().CopyToAsync(fs);
                var bytes = new byte[imageFile.Length];
                fs.Position = 0;
                await fs.ReadAsync(bytes);
                fs.Flush();
                fs.Close();


                return Path.Combine("Images", trustedFileName + ".jpg");
            }
            catch (Exception ex)
            {
                await CreateErrorLog("userId: " + userId, ex, "StoreRecipeImage");
            }
            return null;
        }

        public async Task<Recipe> CreateRecipeEntity(int userId, string recipeName, int servings, string path)
        {
            try
            {
                var recipe = new Recipe
                {
                    CreatedById = userId,
                    UpdatedById = userId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsActive = true,
                    Name = recipeName.ToUpper(),
                    DefaultServings = servings,
                    ImageFilePath= path
            };
                if (await Save(recipe, 0)) return recipe;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("userId: " + userId, ex, "CreateRecipeEntity");
            }
            return null;
        }

        public async Task<bool> IsRecipeNameValid(string recipeName, int userId)
        {
            try
            {
                return (await _unitOfWork.ReadRepo.GetEntityWithPredicateAsNoTracking<Recipe>(
                    a => a.CreatedById == userId && a.Name.Equals(recipeName))) == null;

            }
            catch (Exception ex)
            {
                await CreateErrorLog("userId: " + userId, ex, "IsRecipeNameValid");
            }
            return false;
        }

        public async Task<bool> AddNewIngredients(List<string> items, int userId)
        {
            try
            {
                var ingredients = await _unitOfWork.ReadRepo.GetListWithPredicateAsNoTracking<Ingredient>(
               a => a.IsActive);
                var list = ingredients.Count == 0 ? new List<string>() : ingredients.Select(b => b.Name.ToUpper()).ToList();

                var newIngredients = items.Where(a => !list.Contains(a.ToUpper())).ToList().Select(b => new Ingredient
                {
                    Name = b.ToUpper(),
                    IsActive = true,
                    CreatedById = userId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedById = userId,
                    UpdatedDate = DateTime.UtcNow
                }).ToList();
                if(newIngredients.Any())
                    return await SaveRange(newIngredients);
                return true;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("items: " + items.Count, ex, "AddNewIngredients");
            }
            return false;
        }

        private async Task<bool> SaveIngredient(string item, int userId)
        {
            try
            {
                await Save(new Ingredient
                {
                    Name = item.ToUpper(),
                    IsActive = true,
                    CreatedById = userId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedById = userId,
                    UpdatedDate = DateTime.UtcNow

                }, 0);
            }
            catch (Exception ex)
            {
                await CreateErrorLog("item: " + item, ex, "SaveIngredient");
            }
            return false;
        }

        public async Task<bool> ChangeRecipeEditFlag(int recipeId)
        {
            try
            {
                var recipe = await _unitOfWork.ReadRepo.GetEntityById<Recipe>(recipeId);
                if (recipe == null) return false;
                recipe.NeedsEdit = !recipe.NeedsEdit;
                return await Save(recipe, recipe.Id);
            }
            catch (Exception ex)
            {
                await CreateErrorLog("recipe id: " + recipeId, ex, "ChangeRecipeEditFlag");
            }
            return false;
        }

        public async Task<bool> UpdateUserRating(int recipeId, int rating, int userId)
        {
            try
            {
                //TODO figure out update relationship

                UserRecipeRating? userRating;
                userRating = await _unitOfWork.ReadRepo.GetEntityWithPredicate<UserRecipeRating>(a =>
                a.UserId == userId && a.RecipeId == recipeId);
                if (userRating == null)
                {
                    userRating = new UserRecipeRating
                    {
                        UserId = userId,
                        RecipeId = recipeId,
                        Rating = rating
                    };
                    return await SaveRelationship(userRating);
                }
                userRating.Rating = rating;
                return await UpdateRelationship(userRating);
            }
            catch (Exception ex)
            {
                await CreateErrorLog("recipe id: " + recipeId, ex, "ChangeRecipeEditFlag");
            }
            return false;
        }

        public async Task<bool> UpdateRecipeComment(RecipeCommentDto dto)
        {
            try
            {
                //TODO if public need some kind of security so only user can edit own comment

                UserRecipeComment? comment;
                var now = DateTime.UtcNow;
                if (dto.CommentId == 0) 
                {
                    comment = new UserRecipeComment();
                    comment.CreatedDate = now;
                    comment.CreatedById = dto.UserId;
                    comment.UserId = dto.UserId;
                    comment.RecipeId = dto.RecipeId;
                } 
                else comment = await _unitOfWork.ReadRepo.GetEntityWithPredicateAsNoTracking<UserRecipeComment>(a => a.Id == dto.CommentId);

                comment.Comment = dto.Comment;
                comment.UpdatedDate = now;
                comment.UpdatedById = dto.UserId;

                return await Save(comment, comment.Id);

            }
            catch (Exception ex)
            {
                await CreateErrorLog("recipe id: " + dto.RecipeId, ex, "UpdateRecipeComment");
            }
            return false;
        }


        public async Task<RecipeEditDto> GetRecipeDtoForAdd(bool metricPreferred)
        {
            try
            {
                var dto = new RecipeEditDto();
                var ingList = await _unitOfWork.ReadRepo.GetListWithPredicateAsNoTracking<Ingredient>(
                    a => a.IsActive);

                dto.Measures = await GetMeasureDropdownForEdit(metricPreferred);
                
                dto.AvailableIngredients = ingList.Select(a => a.Name).OrderBy(b => b).ToList();

                return dto;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none ", ex, "GetRecipeDtoForAdd");
            }
            return null;
        }

        public async Task<RecipeEditDto> GetRecipeByIdForEdit(int recipeId, int servings, bool metricPreferred)
        {
            try
            {
                var dto = await _unitOfWork.ReadRepo.GetEntityWithPredicateAndProjection(a => a.Id == recipeId && a.IsActive, Projection.RecipeEditDtoProjection);
                var list = await _unitOfWork.ReadRepo.GetListWithPredicateAndProjection(
                    a => a.RecipeId == recipeId && a.IsActive, Projection.RecipeIngredientEditDtoProjection);

                dto.Measures = await GetMeasureDropdownForEdit(metricPreferred);
                foreach (var item in list)
                {
                    await GetAmountAndConvertUnitsForEdit(metricPreferred, servings, item);
                }
                dto.Ingredients = list;
                dto.Instructions = (await _unitOfWork.ReadRepo.GetListWithPredicateAndProjection(
                    a => a.RecipeId == recipeId && a.IsActive, Projection.RecipeInstructionEditDtoProjection)).OrderBy(b => b.Order).ToList();

                 var recList = await _unitOfWork.ReadRepo.GetListWithPredicateAsNoTracking<Recipe>(a => a.IsActive);

                var nextRec = recList.FirstOrDefault(a => a.Id > recipeId);
                dto.NextId = nextRec == null ? 0 : nextRec.Id;

                var prevRec = recList.OrderByDescending(b => b.Id).FirstOrDefault(a => a.Id < recipeId);
                dto.PreviousId = prevRec == null ? 0 : prevRec.Id;

                var ingList = await _unitOfWork.ReadRepo.GetListWithPredicateAsNoTracking<Ingredient>(
                    a => a.IsActive);
                dto.AvailableIngredients = ingList.Select(a => a.Name).OrderBy(b => b).ToList();

                return dto;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("recipeId: " + recipeId, ex, "GetRecipeByIdForEdit");
            }
            return null;
        }

        private async Task<List<MeasureDropdownDto>> GetMeasureDropdownForEdit(bool metricPreferred)
        {
            try
            {
                var list = await _unitOfWork.ReadRepo.GetListWithPredicateAndProjection(a => a.Id != 0, Projection.MeasureDropdownDtoProjection);
                if(metricPreferred) return list.Where(b => b.IsMetric).ToList();
                return list.Where(b => !b.IsMetric).ToList();
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "GetMeasureDropdownForEdit");
            }
            return null;
        }

        //TODO user stuff
        public async Task<bool> EditRecipe(RecipeEditDto dto, int userId, bool isMetric, int servings)
        {
            try
            {
                //TODO test if amount conversions need to be reversed at all
                var extantRecipe = await _unitOfWork.ReadRepo.GetEntityWithPredicateAsNoTracking<Recipe>(a => a.Id == dto.Id && a.IsActive);
                if (extantRecipe == null) 
                {
                    extantRecipe = await CreateRecipeEntity(userId, dto.Name, servings, dto.ImageFilePath);
                    dto.Id = extantRecipe.Id;
                }

                //edit ingredient associations
                if (!await UpdateRecipeIngredientAssociations(dto.Id, dto.Ingredients, userId, isMetric, servings)) return false;

                //edit instruction associations
                if (!await UpdateRecipeInstructionAssociations(dto.Id, dto.Instructions)) return false;

                //edit recipe entity
                return await UpdateRecipeEntity(dto, extantRecipe, userId);
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "EditRecipe");
            }
            return false;
        }

        private async Task<bool> UpdateRecipeIngredientAssociations(int recipeId, IList<RecipeIngredientEditDto> dtoIngredients, int userId, bool isMetric, int servings)
        {
            try
            {
                List<RecipeIngredientAmountType> ingredientsToEdit = new();

                var recipeIngredients = await _unitOfWork.ReadRepo.GetListWithPredicate<RecipeIngredientAmountType>(c => c.RecipeId == recipeId && c.IsActive);

                var ingredients = await _unitOfWork.ReadRepo.GetListWithPredicateAsNoTracking<Ingredient>(d => d.IsActive);

                foreach (var dtoIngredient in dtoIngredients)
                {
                    //TODO test
                    if(!isMetric) await ConvertUnitsFromImperial(servings, dtoIngredient);
                    else await ConvertServingsForMetric(servings, dtoIngredient);

                    var recipeIngredient = recipeIngredients.FirstOrDefault(a => a.IngredientId == dtoIngredient.IngredientId);
                    //TODO test, removing ID check and matching solely based on name
                    var ingredientEntity = ingredients.FirstOrDefault(f => f.Name.Equals(dtoIngredient.Ingredient.ToUpper()));
                    var roundedAmt = recipeIngredient == null ? 0 : recipeIngredient.Amount;

                    if (roundedAmt.ToString().Length > 5)
                    {
                        var nearestInt = Math.Round(roundedAmt);
                        if (nearestInt - roundedAmt < 0.01 && nearestInt - roundedAmt > -0.01) roundedAmt = nearestInt;
                        else roundedAmt = Math.Round(roundedAmt, 2);
                    }

                    //no changes needed
                    if (recipeIngredient != null 
                        && ingredientEntity != null
                        && dtoIngredient.IsActive 
                        && recipeIngredient.AmountTypeId == dtoIngredient.Measure.AmountTypeId
                        && roundedAmt == dtoIngredient.Amount ) continue;

                    //remove deactivated ingredients
                    if (!dtoIngredient.IsActive && recipeIngredient !=null)
                    {
                        recipeIngredient.IsActive = false;
                        ingredientsToEdit.Add(recipeIngredient);
                        continue;
                    }

                    if(ingredientEntity == null)
                    {
                        ingredientEntity = new Ingredient
                        {
                            Name = dtoIngredient.Ingredient.ToUpper(),
                            IsActive = true,
                            CreatedById = userId,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedById = userId,
                            UpdatedDate = DateTime.UtcNow
                        };
                        if (!await Save(ingredientEntity, 0)) return false;
                    }

                    //add new ingredients
                    if (recipeIngredient == null)
                    {

                        var newIngredient = new RecipeIngredientAmountType
                        {
                            Id = 0,
                            RecipeId = recipeId,
                            IngredientId = ingredientEntity.Id,
                            AmountTypeId = dtoIngredient.Measure.AmountTypeId,
                            Amount = dtoIngredient.Amount,
                            IsActive = true
                        };
                        ingredientsToEdit.Add(newIngredient);
                    }

                    //update extant ingredients
                    else
                    {
                        recipeIngredient.IngredientId = ingredientEntity.Id;
                        recipeIngredient.AmountTypeId = dtoIngredient.Measure.AmountTypeId;
                        recipeIngredient.Amount = dtoIngredient.Amount;
                        ingredientsToEdit.Add(recipeIngredient);
                    }
                   
                }

                foreach(var update in ingredientsToEdit)
                {
                    if (!await Save(update, update.Id)) return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("Recipe Id: "+ recipeId, ex, "UpdateRecipeIngredientAssociations");
            }
            return false;
        }

        private async Task<bool> UpdateRecipeInstructionAssociations(int recipeId, IList<RecipeInstructionEditDto> dtoInstructions)
        {
            try
            {
                List<Instruction> instructionsToEdit = new();

                var instructions = await _unitOfWork.ReadRepo.GetListWithPredicate<Instruction>(c => c.RecipeId == recipeId && c.IsActive);

                foreach (var dtoInstruction in dtoInstructions)
                {
                    var instruction = instructions.FirstOrDefault(a => a.Id == dtoInstruction.Id);

                    //no changes needed
                    if (instruction != null
                        && dtoInstruction.IsActive
                        && instruction.Instruction.Equals(dtoInstruction.Instruction)
                        && instruction.Order == dtoInstruction.Order) continue;

                    //add new ingredients
                    if (instruction == null)
                    {
                        var newInstruction = new Instruction
                        {
                            Id = 0,
                            RecipeId = recipeId,
                            Instruction = dtoInstruction.Instruction,
                            Order = dtoInstruction.Order,
                            IsActive = true
                        };
                        instructionsToEdit.Add(newInstruction);
                    }

                    //remove deactivated ingredients
                    else if (!dtoInstruction.IsActive)
                    {
                        instruction.IsActive = false;
                        instructionsToEdit.Add(instruction);
                    }

                    //update extant ingredients
                    else
                    {
                        instruction.Instruction = dtoInstruction.Instruction;
                        instruction.Order = dtoInstruction.Order;
                        instructionsToEdit.Add(instruction);
                    }
                }

                foreach (var update in instructionsToEdit)
                {
                    if (!await Save(update, update.Id)) return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("Recipe Id: " + recipeId, ex, "UpdateRecipeInstructionAssociations");
            }
            return false;
        }

        private async Task<bool> UpdateRecipeEntity(RecipeEditDto dto, Recipe recipe, int userId)
        {
            try
            {
                recipe.Name= dto.Name.ToUpper();
                recipe.UpdatedDate = DateTime.UtcNow;
                recipe.UpdatedById = userId;
                recipe.ImageFilePath= dto.ImageFilePath;

                return await Save(recipe, recipe.Id);
            }
            catch (Exception ex)
            {
                await CreateErrorLog("Recipe Id: " + recipe.Id, ex, "UpdateRecipeEntity");
            }
            return false;
        }


        public async Task<UserDto?> GetUser(string aoId)
        {
            try
            {
                var user = await _unitOfWork.ReadRepo.GetEntityWithPredicateAndProjection(a => a.AoId.Equals(aoId), Projection.UserDtoProjection);
                return user ?? null;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "GetUser");
            }
            return null;
        }

        public async Task<UserDto?> CreateUser(string aoId, string name)
        {
            try
            {
                var user = new User
                {
                    Name = name,
                    AoId = aoId
                };
                if (await Save(user, 0)) return await GetUser(aoId);
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "CreateUser");
            }
            return null;
        }

        #region ShoppingList

        public async Task<List<RecipeViewIngredientListItemDto>> GetShoppingList(ShoppingListDto dto, bool metricPreferred)
        {
            try
            {
                var list = new List<RecipeViewIngredientListItemDto>();
                foreach(var recipe in dto.Items)
                {
                    var subList = await _unitOfWork.ReadRepo.GetListWithPredicateAndProjection(
                        a => a.RecipeId == recipe.RecipeId && a.IsActive, Projection.RecipeViewIngredientListItemDtoProjection);

                    foreach (var item in subList)
                    {
                        await GetAmountAndConvertUnits(metricPreferred, recipe.Servings, item);
                        if (list.Any(b => b.Item.Equals(item.Item) && b.Unit.Equals(item.Unit)))
                            list.First(b => b.Item.Equals(item.Item) && b.Unit.Equals(item.Unit)).Amount += item.Amount;
                        else list.Add(item);
                    }

                }

                //get any favorite products for search
                var ingIds = list.Select(b => b.IngredientId).ToList();
                var favList = await _unitOfWork.ReadRepo.GetListWithPredicateAndProjection(a => a.UserId == dto.UserId && ingIds.Contains(a.IngredientId), Projection.UIPFProjection);
                foreach (var fav in favList)
                {
                    list.First(c => c.IngredientId == fav.IngredientId).FavoriteProductId = fav.ProductId;
                }

                // update display amounts
                foreach(var item in list)
                {
                    item.DisplayAmount = item.Unit.Equals("UNIT") ? item.Amount.ToString() : item.Amount + " " + item.Unit;
                }


                return list;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "GetShoppingList");
            }
            return null;
        }

        public async Task<bool> UpdateUserProductFavorite(UserIngredientProductFavoriteDto dto)
        {
            try
            {
                var extantFav = await _unitOfWork.ReadRepo.GetEntityWithPredicateAsNoTracking<UserIngredientProductFavorite>(a => a.UserId == dto.UserId && a.IngredientId == dto.IngredientId);

                if (extantFav != null)
                {
                    if (extantFav.ProductId.Equals(dto.ProductId)) return true;
                    else
                    {
                        extantFav.ProductId = dto.ProductId;
                        return await UpdateRelationship(extantFav);
                    }
                }
                var newFav = new UserIngredientProductFavorite { IngredientId = dto.IngredientId, ProductId = dto.ProductId, UserId = dto.UserId };
                return await SaveRelationship(newFav);

            }
            catch (Exception ex)
            {
                await CreateErrorLog("UserId" + dto.UserId, ex, "UpdateUserProductFavorite");
            }
            return false;
        }

        public async Task<bool> DeleteUserProductFavorite(UserIngredientProductFavoriteDto dto)
        {
            try
            {
                var extantFav = await _unitOfWork.ReadRepo.GetEntityWithPredicateAsNoTracking<UserIngredientProductFavorite>(a => a.UserId == dto.UserId && a.IngredientId == dto.IngredientId && a.ProductId.Equals(dto.ProductId));

                if (extantFav != null)
                {
                    return await Delete(extantFav);
                }
            }
            catch (Exception ex)
            {
                await CreateErrorLog("UserId" + dto.UserId, ex, "DeleteUserProductFavorite");
            }
            return false;
        }

        public async Task<UserDto?> UpdateUserPreferences(UserDto dto)
        {
            try
            {
                //TODO test
                var user = await _unitOfWork.ReadRepo.GetEntityWithPredicateAsNoTracking<User>(a => a.Id == dto.Id);
                if (user != null)
                {
                    user.PreferredStore = dto.PreferredStore;
                    user.MetricPreferred = dto.MetricPreferred;
                    user.Name= dto.Name;
                    await Save(user, user.Id);
                    return await GetUser(user.AoId);
                }
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "UpdateUserPreferences");
            }
            return null;
        }

        public async Task<IList<RecipeViewIngredientListItemDto>?> GetIngredientsForShoppingList()
        {
            try
            {
                var list = await _unitOfWork.ReadRepo.GetListWithPredicateAndProjection(a => a.IsActive, Projection.RecipeViewIngredientListItemDtoProjection2);
                return list;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "GetIngredientsForShoppingList");
            }
            return null;
        }


        #endregion ShoppingList


        private async Task<bool> Template()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                await CreateErrorLog("none", ex, "Template");
            }
            return false;
        }

        public async Task CreateErrorLog(string identifier, Exception? e, string method)
        {
                var existingError = await _unitOfWork.ReadRepo.GetEntityWithPredicate<ErrorLog>(
                    a => a.Identifier.Equals(identifier) && a.Method.Equals(method) 
                    && ((e == null && a.Message == null) || a.Message.Equals(e.Message))
                    && ((e == null && a.Stack == null) || a.Stack.Equals(e.StackTrace)));
                if(existingError != null)
                {
                    existingError.Count++;
                    existingError.UpdatedDate= DateTime.UtcNow;
                    await Save(existingError, existingError.Id);
                }
                else
                {
                    var error = new ErrorLog
                    {
                        CreatedDate = DateTime.UtcNow,
                        Identifier = identifier,
                        Message = e?.Message,
                        Method = method,
                        Stack = e?.StackTrace
                    };
                    await Save(error, 0);
                }
        }

        public async Task<T?> GetById<T>(int id) where T : class =>
            await _unitOfWork.ReadRepo.GetEntityById<T>(id);

        public async Task<IList<T>> GetList<T>() where T : class =>
            await _unitOfWork.ReadRepo.GetList<T>();

        public async Task<IList<T>> GetListWithPredicate<T>(Expression<Func<T,bool>> predicate) where T : class =>
            await _unitOfWork.ReadRepo.GetListWithPredicate(predicate);

        public async Task<bool> Save<T>(T entity, int id) where T : class
        {
            if (id == 0) await _unitOfWork.WriteRepo.AddEntityAsync(entity);
            else _unitOfWork.WriteRepo.UpdateEntity(entity);

            return await _unitOfWork.Commit();
        }

        public async Task<bool> SaveRelationship<T>(T entity) where T : class
        {
            await _unitOfWork.WriteRepo.AddEntityAsync(entity);
            return await _unitOfWork.Commit();
        }
        public async Task<bool> UpdateRelationship<T>(T entity) where T : class
        {
            _unitOfWork.WriteRepo.UpdateEntity(entity);
            return await _unitOfWork.Commit();
        }

        public async Task<bool> SaveRange<T>(List<T> entities) where T : class
        {
            await _unitOfWork.WriteRepo.AddRangeAsync(entities);
            return await _unitOfWork.Commit();
        }

        public async Task<bool> Delete<T>(T entity) where T : class
        {
            _unitOfWork.WriteRepo.DeleteEntity(entity);
            return await _unitOfWork.Commit();
        }

        public async Task<bool> DeleteRange<T>(List<T> entities) where T : class
        {
            _unitOfWork.WriteRepo.DeleteRange(entities);
            return await _unitOfWork.Commit();
        }
    }
}
