using System.Linq.Expressions;

namespace DinnerPlans.Shared.DTOs
{
    public class RecipeInstructionEditDto 
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string Instruction { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }

        
    }
}
