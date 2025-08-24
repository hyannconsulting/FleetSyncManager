using MediatR;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Application.Commands.Drivers;

/// <summary>
/// Command to create a new driver
/// </summary>
public class CreateDriverCommand : IRequest<Result<DriverDto>>
{
    /// <summary>
    /// Driver's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Driver's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Driver's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Driver's phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Driver's license number
    /// </summary>
    public string LicenseNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// License type (A, B, C, D, etc.)
    /// </summary>
    public string LicenseType { get; set; } = string.Empty;
    
    /// <summary>
    /// License expiration date
    /// </summary>
    public DateTime LicenseExpiryDate { get; set; }
    
    /// <summary>
    /// Driver's date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>
    /// Hire date
    /// </summary>
    public DateTime HireDate { get; set; }
    
    /// <summary>
    /// Driver's address
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
}

/// <summary>
/// Command to update an existing driver
/// </summary>
public class UpdateDriverCommand : IRequest<Result<DriverDto>>
{
    /// <summary>
    /// Driver ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Driver's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Driver's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Driver's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Driver's phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Driver's license number
    /// </summary>
    public string LicenseNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// License type (A, B, C, D, etc.)
    /// </summary>
    public string LicenseType { get; set; } = string.Empty;
    
    /// <summary>
    /// License expiration date
    /// </summary>
    public DateTime LicenseExpiryDate { get; set; }
    
    /// <summary>
    /// Driver's address
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
    /// Driver status
    /// </summary>
    public string Status { get; set; } = "Active";
}

/// <summary>
/// Command to delete a driver
/// </summary>
public class DeleteDriverCommand : IRequest<Result>
{
    /// <summary>
    /// Driver ID to delete
    /// </summary>
    public int Id { get; set; }
    
    public DeleteDriverCommand(int id)
    {
        Id = id;
    }
}
