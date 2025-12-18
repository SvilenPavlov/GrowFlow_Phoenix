using GrowFlow_Phoenix.Models.IModels;
using GrowFlow_Phoenix.Models.Utility;

namespace GrowFlow_Phoenix.Models
{
    public class Employee : IAuditable<AuditRecord>
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
        public ICollection<EmployeeExternalId> ExternalIds { get; set; } = new List<EmployeeExternalId>();
        public AuditRecord AuditRecord { get; set; } = new AuditRecord();
    }
}
