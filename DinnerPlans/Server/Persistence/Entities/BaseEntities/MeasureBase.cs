namespace DinnerPlans.Server.Persistence.Entities.BaseEntities
{
    public class MeasureBase : BaseEntity
    {
        public string Measure { get; set; }
        public int AmountTypeId { get; set; }
        public double ConversionFrom { get; set; }
        public bool IsMetric { get; set; }
    }
}
