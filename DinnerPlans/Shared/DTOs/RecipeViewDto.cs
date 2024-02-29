namespace DinnerPlans.Shared.DTOs
{
    public class RecipeViewDto
    {
        public string Name { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool NeedsEdit { get; set; }
        public int? Rating { get; set; }
        public IList<RecipeViewIngredientListItemDto> Ingredients {get;set;} = new List<RecipeViewIngredientListItemDto>();
        public IList<RecipeViewInstructionListItemDto> Instructions {get;set;} = new List<RecipeViewInstructionListItemDto>();
        public IList<RecipeCommentDto> Comments {get;set;} = new List<RecipeCommentDto>();
        public string ImageFilePath { get; set; }
        public int CreatedById { get; set; }

        public int PreviousId { get; set; }
        public int NextId { get; set; }

        

    }
}
