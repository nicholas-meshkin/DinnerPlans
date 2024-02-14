using DinnerPlans.Server.Persistence.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace DinnerPlans.Server.Persistence.Entities
{
    public class Ingredient : IngredientBase
    {
        [ForeignKey("CreatedById")]
        public User CreatedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public User? UpdatedBy { get; set; }
    }
}
