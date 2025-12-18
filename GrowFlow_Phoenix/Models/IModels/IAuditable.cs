namespace GrowFlow_Phoenix.Models.IModels
{
    public interface IAuditable <T>
    {
        public T AuditRecord { get; set; }
    }
}
