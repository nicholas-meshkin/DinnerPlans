namespace DinnerPlans.Server.Persistence.Entities.BaseEntities
{
    public class RecipeBase : BaseUpdateEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool NeedsEdit { get; set; }
        public int DefaultServings { get; set; }
        public bool IsIngredient { get; set; }
        public string ImageFilePath { get; set; }
    }
}
