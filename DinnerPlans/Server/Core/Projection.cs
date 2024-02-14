using DinnerPlans.Server.Persistence.Entities;
using DinnerPlans.Shared.DTOs;
using System.Linq.Expressions;

namespace DinnerPlans.Server.Core
{
    public static class Projection
    {
        public static Expression<Func<Measure,MeasureDropdownDto>> MeasureDropdownDtoProjection
        {
            get
            {
                return e => new MeasureDropdownDto
                {
                    Id= e.Id,
                    Unit = e.Measure,
                    AmountTypeId= e.AmountTypeId,
                    IsMetric= e.IsMetric
                };
            }
        }
        public static Expression<Func<UserRecipeComment, RecipeCommentDto>> RecipeCommentDtoProjection
        {
            get
            {
                return e => new RecipeCommentDto 
                {
                    UserName = e.User.Name,
                    Updated = e.UpdatedDate,
                    Comment = e.Comment,
                    CommentId = e.Id,
                    RecipeId = e.RecipeId,
                    UserId = e.UserId
                };
            }
        }
        public static Expression<Func<Recipe, RecipeEditDto>> RecipeEditDtoProjection
        {
            get
            {
                return e => new RecipeEditDto
                {
                    Id= e.Id,
                    Name= e.Name
                };
            }
        }
        public static Expression<Func<RecipeIngredientAmountType, RecipeIngredientEditDto>> RecipeIngredientEditDtoProjection
        {
            get
            {
                return e => new RecipeIngredientEditDto
                {
                    Id= e.Id,
                    IngredientId= e.IngredientId,
                    Ingredient = e.Ingredient.Name,
                    RecipeId = e.RecipeId,
                    Measure = new MeasureDropdownDto()
                    {
                        AmountTypeId = e.AmountTypeId,
                        Unit = e.AmountType.Type
                    },
                   // AmountTypeId = e.AmountTypeId,
                    //Unit = e.AmountType.Type,
                    Amount= e.Amount,
                    IsActive= e.IsActive

                };
            }
        }
        public static Expression<Func<Instruction, RecipeInstructionEditDto>> RecipeInstructionEditDtoProjection
        {
            get
            {
                return e => new RecipeInstructionEditDto
                {
                    Id = e.Id,
                    Instruction = e.Instruction,
                    RecipeId = e.RecipeId,
                    Order = e.Order,
                    IsActive = e.IsActive

                };
            }
        }
        public static Expression<Func<Recipe, RecipeListItemDto>> RecipeListItemDtoProjection
        {
            get
            {
                return e => new RecipeListItemDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    NeedsEdit = e.NeedsEdit,
                    DefaultServings= e.DefaultServings
                };
            }
        }
        public static Expression<Func<Recipe, RecipeViewDto>> RecipeViewDtoProjection
        {
            get
            {
                return e => new RecipeViewDto
                {
                    Name = e.Name,
                    UpdatedDate = e.UpdatedDate,
                    NeedsEdit = e.NeedsEdit,
                    CreatedById= e.CreatedById
                };
            }
        }
        public static Expression<Func<RecipeIngredientAmountType, RecipeViewIngredientListItemDto>> RecipeViewIngredientListItemDtoProjection
        {
            get
            {
                return e => new RecipeViewIngredientListItemDto
                {
                    Amount = e.Amount,
                    Unit = e.AmountType.Type,
                    Item = e.Ingredient.Name,
                    IngredientId= e.IngredientId,
                    IsDryMeasure = e.Ingredient.IsDryMeasure
                };
            }
        }

        public static Expression<Func<Ingredient, RecipeViewIngredientListItemDto>> RecipeViewIngredientListItemDtoProjection2
        {
            get
            {
                return e => new RecipeViewIngredientListItemDto
                {
                    Amount = 0,
                    Unit="",
                    Item = e.Name,
                    IngredientId = e.Id,
                    IsDryMeasure = e.IsDryMeasure
                };
            }
        }

        public static Expression<Func<Instruction, RecipeViewInstructionListItemDto>> RecipeViewInstructionListItemDtoProjection
        {
            get
            {
                return e => new RecipeViewInstructionListItemDto
                {
                    Instruction = e.Instruction,
                    Order = e.Order
                };
            }
        }
        public static Expression<Func<User,UserDto>> UserDtoProjection
        {
            get
            {
                return e => new UserDto
                {
                    AoId = e.AoId,
                    Name = e.Name,
                    Id = e.Id,
                    PreferredStore = e.PreferredStore,
                    MetricPreferred = e.MetricPreferred
                };
            }
        }
        public static Expression<Func<UserIngredientProductFavorite, UserIngredientProductFavoriteDto>> UIPFProjection
        {
            get
            {
                return e => new UserIngredientProductFavoriteDto
                {
                    UserId = e.UserId,
                    ProductId = e.ProductId,
                    IngredientId = e.IngredientId
                };
            }
        }
    }
}
