using DinnerPlans.Server.Persistence.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace DinnerPlans.Server.Persistence.Entities
{
    public class Recipe : RecipeBase
    {
        public ICollection<RecipeIngredientAmountType> RecipeIngredients { get; set;}
        public ICollection<Instruction> Instructions { get; set;}
        public ICollection<UserRecipeComment> UserRecipeComments { get; set;}

        [ForeignKey ("CreatedById")]
        public User CreatedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public User? UpdatedBy { get; set; }
    }
}
