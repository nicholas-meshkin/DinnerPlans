namespace DinnerPlans.Shared.DTOs
{
    public class RecipeViewIngredientListItemDto
    {

        //TODO think about rounding
        public double Amount { get; set; }
        public string Unit { get; set; }
        public string Item { get; set; }
        public string DisplayAmount { get; set; }
        public int IngredientId { get; set; }
        public bool IsDryMeasure { get; set; }

        // stuff for product search
        public bool ShowDetails { get; set; } = false;
        public string AltQuery { get; set; } = "";
        public bool UseAltQuery { get; set; } = false;
        public string PreviousAltQuery { get; set; } = "";
        public string FavoriteProductId { get; set; } = "";

    }
}
