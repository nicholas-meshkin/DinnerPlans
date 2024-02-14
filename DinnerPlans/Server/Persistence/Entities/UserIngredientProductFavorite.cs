namespace DinnerPlans.Server.Persistence.Entities
{
    public class UserIngredientProductFavorite
    {
        public int UserId { get; set; }
        public int IngredientId { get; set; }
        public string ProductId { get; set; }

        public User User { get; set; }
        public Ingredient Ingredient { get; set; }
    }
}
