using DinnerPlans.Server.Persistence.Entities.BaseEntities;

namespace DinnerPlans.Server.Persistence.Entities
{
    public class Instruction : InstructionBase
    {
        public Recipe Recipe { get; set; }
    }
}
