
namespace DinnerPlans.Shared.DTOs
{
    public class RecipeIngredientEditDto 
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public string Ingredient { get; set; } = "";
        public MeasureDropdownDto Measure { get; set; } = new MeasureDropdownDto();
        //public int AmountTypeId { get; set; }
        //public string Unit { get; set; }
        public double Amount { get; set; }
        public bool IsActive { get; set; }

       
    }
}
