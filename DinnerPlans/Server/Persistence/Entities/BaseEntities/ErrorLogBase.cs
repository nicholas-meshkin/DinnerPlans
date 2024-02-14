namespace DinnerPlans.Server.Persistence.Entities.BaseEntities
{
    public class ErrorLogBase : BaseEntity
    {
        public string Method { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        public string Identifier { get; set; }
        public int Count { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
