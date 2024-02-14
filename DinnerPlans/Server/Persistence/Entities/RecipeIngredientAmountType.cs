using DinnerPlans.Server.Persistence.Entities.BaseEntities;

namespace DinnerPlans.Server.Persistence.Entities
{
    public class RecipeIngredientAmountType : RecipeIngredientAmountTypeBase
    {
        public Recipe Recipe { get; set; }
        public Ingredient Ingredient { get; set; }
        public AmountType AmountType { get; set; }
    }
}
