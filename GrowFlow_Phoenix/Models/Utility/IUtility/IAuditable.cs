namespace GrowFlow_Phoenix.Models.Utility.IModels
{
    public interface IAuditable <T>
    {
        public T AuditRecord { get; set; }
    }
}
