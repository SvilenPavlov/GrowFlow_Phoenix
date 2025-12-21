using GrowFlow_Phoenix.Models.Utility;
using GrowFlow_Phoenix.Models.Utility.IModels;
using GrowFlow_Phoenix.Models.Utility.IUtility;

namespace GrowFlow_Phoenix.Models.Phoenix
{
    public class Employee : IAuditable<AuditRecord>, IEmployeeComparable
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Telephone { get; set; } = null!;
        public string? Email { get; set; }
        public string? Role { get; set; }
        public ICollection<EmployeeExternalId> ExternalIds { get; set; } = new List<EmployeeExternalId>();
        public AuditRecord AuditRecord { get; set; } = new AuditRecord();
    }
}
