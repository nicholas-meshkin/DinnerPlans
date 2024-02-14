namespace DinnerPlans.Server.Persistence.Entities
{
    public class UserRecipeRating
    {
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public int Rating { get; set; }

        public User User { get; set; }
        public Recipe Recipe { get; set; } 
    }
}
