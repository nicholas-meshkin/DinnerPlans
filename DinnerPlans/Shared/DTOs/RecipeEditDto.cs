
namespace DinnerPlans.Shared.DTOs
{
    public class RecipeEditDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public IList<RecipeIngredientEditDto> Ingredients { get; set; } = new List<RecipeIngredientEditDto>();
        public IList<RecipeInstructionEditDto> Instructions { get; set; } = new List<RecipeInstructionEditDto>();
        public IList<MeasureDropdownDto> Measures { get; set; } = new List<MeasureDropdownDto>();
        public IList<string> AvailableIngredients { get; set; } = new List<string>();
        public int NextId { get; set; }
        public int PreviousId { get; set; }
        public string ImageFilePath { get; set; }

        
    }
}
