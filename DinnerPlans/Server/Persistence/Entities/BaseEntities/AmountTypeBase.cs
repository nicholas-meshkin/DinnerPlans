namespace DinnerPlans.Server.Persistence.Entities.BaseEntities
{
    public class AmountTypeBase : BaseEntity
    {
        //think this will just be weight, volume, or unit maybe? probably grams, mls, and unit
        public string Type { get; set; }
    }
}
