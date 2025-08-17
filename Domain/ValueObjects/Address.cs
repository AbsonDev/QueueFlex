using System.ComponentModel.DataAnnotations;

namespace QueueManagement.Domain.ValueObjects;

/// <summary>
/// Represents a physical address value object
/// </summary>
public class Address
{
    /// <summary>
    /// Street name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Street { get; private set; }

    /// <summary>
    /// Street number
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Number { get; private set; }

    /// <summary>
    /// Address complement (apartment, suite, etc.)
    /// </summary>
    [MaxLength(100)]
    public string? Complement { get; private set; }

    /// <summary>
    /// Neighborhood or district
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Neighborhood { get; private set; }

    /// <summary>
    /// City name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string City { get; private set; }

    /// <summary>
    /// State or province
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string State { get; private set; }

    /// <summary>
    /// ZIP or postal code
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string ZipCode { get; private set; }

    /// <summary>
    /// Country name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Country { get; private set; }

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Address() { }

    /// <summary>
    /// Creates a new address instance
    /// </summary>
    public Address(string street, string number, string neighborhood, string city, string state, string zipCode, string country, string? complement = null)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        Number = number ?? throw new ArgumentNullException(nameof(number));
        Neighborhood = neighborhood ?? throw new ArgumentNullException(nameof(neighborhood));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
        Country = country ?? throw new ArgumentNullException(nameof(country));
        Complement = complement;
    }

    /// <summary>
    /// Returns the full address as a formatted string
    /// </summary>
    public override string ToString()
    {
        var address = $"{Street}, {Number}";
        
        if (!string.IsNullOrEmpty(Complement))
            address += $" - {Complement}";
            
        address += $", {Neighborhood}, {City} - {State}, {ZipCode}, {Country}";
        
        return address;
    }

    /// <summary>
    /// Equality comparison
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Address other)
            return false;

        return Street == other.Street &&
               Number == other.Number &&
               Complement == other.Complement &&
               Neighborhood == other.Neighborhood &&
               City == other.City &&
               State == other.State &&
               ZipCode == other.ZipCode &&
               Country == other.Country;
    }

    /// <summary>
    /// Hash code generation
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Street, Number, Complement, Neighborhood, City, State, ZipCode, Country);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(Address? left, Address? right)
    {
        return EqualityComparer<Address>.Default.Equals(left, right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(Address? left, Address? right)
    {
        return !(left == right);
    }
}