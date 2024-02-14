
namespace DinnerPlans.Shared.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AoId { get; set; }
        public string? PreferredStore { get; set; }
        public bool MetricPreferred { get; set; }

       
    }
}
