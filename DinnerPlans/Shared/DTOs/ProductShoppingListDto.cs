namespace DinnerPlans.Shared.DTOs
{
    public class ProductShoppingListDto
    {
        public string Ingredient { get; set; } = "";
        public int IngredientId { get; set; }
        public string ProductDescription { get; set; } = "";
        public string ProductId { get; set; } = "";
        
        public decimal? AmountRequired { get; set; }
        public string IngredientUnit { get; set; } = "";
        public string ProductUnit { get; set; } = "";
        public decimal? ProductAmount { get; set; }
        public decimal? AmountToPurchase { get; set; }
        public decimal? PricePerUnit { get; set; }
        public decimal? TotalPrice { get; set; }
        
    }
}
