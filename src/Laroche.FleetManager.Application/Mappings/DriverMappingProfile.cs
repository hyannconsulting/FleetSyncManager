using AutoMapper;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Domain.Entities;

namespace Laroche.FleetManager.Application.Mappings;

/// <summary>
/// AutoMapper profile for Driver mappings
/// </summary>
public class DriverMappingProfile : Profile
{
    public DriverMappingProfile()
    {
        // Driver to DriverDto mapping
        CreateMap<Driver, DriverDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? "Active" : "Inactive"))
            .ForMember(dest => dest.AssignedVehicles, opt => opt.MapFrom(src => 
                src.VehicleAssignments
                    .Where(va => va.EndDate == null || va.EndDate > DateTime.UtcNow)
                    .Select(va => va.Vehicle != null ? va.Vehicle.LicensePlate : "N/A")
                    .ToList()));

        // DriverDto to Driver mapping (for updates)
        CreateMap<DriverDto, Driver>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status.ToLower() == "active"))
            .ForMember(dest => dest.VehicleAssignments, opt => opt.Ignore())
            .ForMember(dest => dest.Incidents, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
