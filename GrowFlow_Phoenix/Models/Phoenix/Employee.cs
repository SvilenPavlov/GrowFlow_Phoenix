using GrowFlow_Phoenix.Models.Utility;
using GrowFlow_Phoenix.Models.Utility.IModels;

namespace GrowFlow_Phoenix.Models.Phoenix
{
    public class Employee : IAuditable<AuditRecord>
    {
        public Guid Id { get; set; }

        public Guid? LeviathanId { get; set; } //this might be a non-valid Guid as per Leviathan
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
