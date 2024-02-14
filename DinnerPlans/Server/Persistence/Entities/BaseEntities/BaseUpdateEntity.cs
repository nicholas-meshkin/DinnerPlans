namespace DinnerPlans.Server.Persistence.Entities.BaseEntities
{
    public class BaseUpdateEntity : BaseEntity
    {
        public int CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
