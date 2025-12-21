namespace GrowFlow_Phoenix.Models.Utility.IUtility
{
    public interface IEmployeeComparable
    {
        string FirstName { get; }
        string LastName { get; }
        string Telephone { get; }
        string? Email { get; }
        string? Role { get; }
        bool IsEquivalent(IEmployeeComparable other)
        {
            if (other == null) return false;
            return string.Equals(FirstName, other.FirstName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(LastName, other.LastName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Telephone, other.Telephone, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Email, other.Email, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Role, other.Role, StringComparison.OrdinalIgnoreCase);
        }
    }
}
