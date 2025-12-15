namespace GrowFlow_Phoenix.DTOs
{
    public class EmployeeResponseDto
    {
        public Guid Id { get; set; }
        public int? LeviathanId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool IsSynced { get; set; }
    }
}
