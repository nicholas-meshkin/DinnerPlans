namespace DinnerPlans.Server.Persistence.Entities
{
    public class IngredientIngredientCategory
    {
        public Ingredient Ingredient { get; set; }
        public IngredientCategory IngredientCategory { get; set; }

        public User CreatedBy { get; set; }
        public User? UpdatedBy { get; set; }
        public int IngredientId { get; set; }
        public int IngredientCategoryId { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
       
        public DateTime? UpdatedDate { get; set; }
    }
}
