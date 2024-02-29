namespace DinnerPlans.Shared.DTOs
{
    public class RecipeListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool NeedsEdit { get; set; }
        public int Rating { get; set; }
        public int DefaultServings { get; set; }
        public string ImageFilePath { get; set; }

        public List<string> Ingredients { get; set; } = new List<string>();

        
    }
}
