using System.ComponentModel.DataAnnotations.Schema;

namespace DinnerPlans.Server.Persistence.Entities
{
    public class RecipeCopyHistory
    {
        public int OriginalRecipeId { get; set; }
        public int CopyRecipeId { get; set; }
        public int UserId { get; set; }

        [ForeignKey("OriginalRecipeId")]
        public Recipe OriginalRecipe { get; set; }

        [ForeignKey("CopyRecipeId")]
        public Recipe CopyRecipe { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
