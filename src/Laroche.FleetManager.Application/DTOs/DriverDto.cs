using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Application.DTOs;

/// <summary>
/// Driver Data Transfer Object
/// </summary>
public class DriverDto
{
    /// <summary>
    /// Driver ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Driver first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Driver last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Driver email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Driver phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Driver license number
    /// </summary>
    public string LicenseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the operation.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    ///// <summary>
    ///// Gets or sets the type of license associated with the entity.
    ///// </summary>
    //public string? LicenseType { get; set; }

    /// <summary>
    /// Gets or sets the address associated with the entity.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Emergency contact name
    /// </summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the emergency contact.
    /// </summary>
    public string? EmergencyContactPhone { get; set; }

    /// <summary>
    /// Gets or sets the notes associated with the object.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// License type
    /// </summary>
    public LicenseTypeEnums LicenseType { get; set; }

    /// <summary>
    /// License expiry date
    /// </summary>
    public DateTime? LicenseExpiryDate { get; set; }

    /// <summary>
    /// Hire date
    /// </summary>
    public DateTime HireDate { get; set; }

    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Driver status
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Currently assigned vehicles
    /// </summary>
    public List<string> AssignedVehicles { get; set; } = [];

    /// <summary>
    /// Driver full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
}
