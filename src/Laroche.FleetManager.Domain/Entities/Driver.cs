using Laroche.FleetManager.Domain.Common;
using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Domain.Entities;

/// <summary>
/// Driver domain entity
/// </summary>
public class Driver : BaseEntity
{
    /// <summary>
    /// Driver first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Driver last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Driver email address
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
    /// Driver license type
    /// </summary>
    public LicenseTypeEnums LicenseType { get; set; }

    /// <summary>
    /// License expiry date
    /// </summary>
    public DateTime LicenseExpiryDate { get; set; }

    /// <summary>
    /// Driver hire date
    /// </summary>
    public DateTime HireDate { get; set; }

    /// <summary>
    /// Driver date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Driver address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Emergency contact name
    /// </summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// Emergency contact phone
    /// </summary>
    public string? EmergencyContactPhone { get; set; }

    /// <summary>
    /// Driver status (active/inactive)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Driver's vehicle assignments
    /// </summary>
    public virtual ICollection<VehicleAssignment> VehicleAssignments { get; set; } = [];

    /// <summary>
    /// Incidents involving this driver
    /// </summary>
    public virtual ICollection<Incident> Incidents { get; set; } = [];

    /// <summary>
    /// Driver's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
}
