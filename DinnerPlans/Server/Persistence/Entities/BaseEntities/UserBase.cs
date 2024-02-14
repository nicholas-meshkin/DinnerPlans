namespace DinnerPlans.Server.Persistence.Entities.BaseEntities
{
    public class UserBase : BaseEntity
    {
        public string Name { get; set; }
        public string AoId { get; set; }
        public string? PreferredStore { get; set; }
        public bool MetricPreferred { get; set; } = false;
    }
}
