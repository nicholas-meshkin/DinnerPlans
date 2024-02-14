using System.ComponentModel.DataAnnotations.Schema;

namespace DinnerPlans.Server.Persistence.Entities.BaseEntities
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
      
    }
}
