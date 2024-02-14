namespace DinnerPlans.Shared.DTOs
{
    public class KrogerProductSearchRequestDto
    {
        public string? Query { get; set; }
        public string? LocationId { get; set; }
        public string? ProductId { get; set; }
        public string? Brand { get; set; }
        public string? Fulfillment { get; set; }
    }
}
