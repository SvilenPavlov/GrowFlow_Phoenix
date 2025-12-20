using System.ComponentModel.DataAnnotations.Schema;

namespace GrowFlow_Phoenix.Models.Phoenix
{
    public class EmployeeExternalId
    {
        public int Id { get; set; }
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; } 
        public string Provider { get; set; } = null!;
        public string ExternalId { get; set; } = null!;

        public Employee Employee { get; set; } = null!;
    }
}
