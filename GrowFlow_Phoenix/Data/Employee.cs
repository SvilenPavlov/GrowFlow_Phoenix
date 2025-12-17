namespace GrowFlow_Phoenix.Data
{
    public class Employee
    {
        public Guid Id { get; set; }

        // Leviathan mapping
        public string? LeviathanId { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Telephone { get; set; } = null!;
        public string? Email { get; set; }
        public string? Role { get; set; }

        public bool IsSynced { get; set; }
        public DateTime? LastSyncedAt { get; set; }
    }
}
