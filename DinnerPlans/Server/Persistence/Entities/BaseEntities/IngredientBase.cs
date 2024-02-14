namespace DinnerPlans.Server.Persistence.Entities.BaseEntities
{
    public class IngredientBase : BaseUpdateEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDryMeasure { get; set; } 
    }
}
