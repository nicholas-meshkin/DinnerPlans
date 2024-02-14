
namespace DinnerPlans.Shared.DTOs
{
    public class MeasureDropdownDto
    {
        public int Id { get; set; }
        public string Unit { get; set; } = "";
        public int AmountTypeId { get; set; }
        public bool IsMetric { get; set; }

        public override bool Equals(object o)
        {
            var other = o as MeasureDropdownDto;
            return (bool)other?.Unit.Equals(Unit);
        }

        public override int GetHashCode() => Unit.GetHashCode();

        public override string ToString()
        {
            return Unit;
        }

      
    }
}
