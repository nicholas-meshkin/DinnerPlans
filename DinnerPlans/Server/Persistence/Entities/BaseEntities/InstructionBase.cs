namespace DinnerPlans.Server.Persistence.Entities.BaseEntities
{
    public class InstructionBase : BaseUpdateEntity
    {
        public string Instruction { get; set; }
        public int RecipeId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}
