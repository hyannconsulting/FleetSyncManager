using AutoMapper;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Application.Mappings;

/// <summary>
/// Profil AutoMapper pour les véhicules
/// </summary>
public class VehicleMappingProfile : Profile
{
    public VehicleMappingProfile()
    {
        // Vehicle -> VehicleDto
        CreateMap<Vehicle, VehicleDto>()
            .ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.CurrentMileage))
            .ForMember(dest => dest.AssignedDriverName, opt => opt.MapFrom(src => 
                src.VehicleAssignments
                    .Where(va => va.EndDate == null || va.EndDate > DateTime.UtcNow)
                    .Select(va => va.Driver != null ? $"{va.Driver.FirstName} {va.Driver.LastName}".Trim() : null)
                    .FirstOrDefault()));

        // VehicleDto -> Vehicle (pour les updates)
        CreateMap<VehicleDto, Vehicle>()
            .ForMember(dest => dest.CurrentMileage, opt => opt.MapFrom(src => src.CurrentMileage))
            .ForMember(dest => dest.VehicleAssignments, opt => opt.Ignore());
    }
}
