namespace DinnerPlans.Server.Persistence.Entities
{
    public class IngredientRecipe
    {
        public int IngredientId { get; set; }
        public int RecipeId { get; set; }

        public Ingredient Ingredient { get; set; }
        public Recipe Recipe { get; set; }
    }
}
