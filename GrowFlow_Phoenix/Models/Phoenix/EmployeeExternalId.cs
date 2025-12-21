using GrowFlow_Phoenix.Models.Utility.IUtility;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrowFlow_Phoenix.Models.Phoenix
{
    public class EmployeeExternalId :ISyncable
    {
        public int Id { get; set; }
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; } 
        public string Provider { get; set; } = null!;
        public string ExternalId { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public DateTime LastSyncedAt { get; set; } // not nullable as entries are only created when a sync event is performed
    }
}
