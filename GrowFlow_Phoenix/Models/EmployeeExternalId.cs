namespace GrowFlow_Phoenix.Models
{
    public class EmployeeExternalId
    {
        public int Id { get; set; }
        public Guid EmployeeId { get; set; } 
        public string Provider { get; set; } = null!;
        public string ExternalId { get; set; } = null!;

        public Employee Employee { get; set; } = null!;
    }
}
