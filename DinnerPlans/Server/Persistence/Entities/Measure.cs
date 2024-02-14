using DinnerPlans.Server.Persistence.Entities.BaseEntities;

namespace DinnerPlans.Server.Persistence.Entities
{
    public class Measure : MeasureBase
    {
        public AmountType AmountType { get; set; }
    }
}
