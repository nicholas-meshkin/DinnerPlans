namespace DinnerPlans.Shared.DTOs
{
    public class ShoppingListDto
    {
        public int UserId { get; set; }
        public IList<ShoppingItemDto> Items { get; set; } = new List<ShoppingItemDto>();

        public class ShoppingItemDto
        {
            public int RecipeId { get; set; }
            public string RecipeName { get; set; }
            public int Servings { get; set;}

        }
    }
}
