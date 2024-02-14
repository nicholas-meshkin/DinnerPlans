using DinnerPlans.Server.Persistence.Entities.BaseEntities;

namespace DinnerPlans.Server.Persistence.Entities
{
    public class AmountType : AmountTypeBase
    {
        public ICollection<Measure> Measures { get; set; }
    }
}
