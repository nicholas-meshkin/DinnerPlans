using DinnerPlans.Server.Persistence.Entities.BaseEntities;

namespace DinnerPlans.Server.Persistence.Entities
{
    public class UserRecipeComment : UserRecipeCommentBase
    {
        public User User { get; set; }
        public Recipe Recipe { get; set; }
    }
}
