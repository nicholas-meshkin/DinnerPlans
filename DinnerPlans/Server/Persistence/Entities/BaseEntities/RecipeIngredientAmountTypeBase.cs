namespace DinnerPlans.Server.Persistence.Entities.BaseEntities
{
    public class RecipeIngredientAmountTypeBase : BaseEntity
    {
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public int AmountTypeId { get; set; }
        public double Amount { get; set; }
        public bool IsActive { get; set; }

    }
}
