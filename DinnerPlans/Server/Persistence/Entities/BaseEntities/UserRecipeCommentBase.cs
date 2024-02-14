namespace DinnerPlans.Server.Persistence.Entities.BaseEntities
{
    public class UserRecipeCommentBase : BaseUpdateEntity
    {
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public string Comment { get; set; }
    }
}
