using Laroche.FleetManager.Domain.Common;

namespace Laroche.FleetManager.Domain.Entities;

/// <summary>
/// GPS tracking record entity
/// </summary>
public class GpsTrackingRecord : BaseEntity
{
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }

    /// <summary>
    /// GPS coordinates (latitude)
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// GPS coordinates (longitude)
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Vehicle speed in km/h
    /// </summary>
    public double Speed { get; set; }

    /// <summary>
    /// Heading/direction
    /// </summary>
    public double? Heading { get; set; }

    /// <summary>
    /// Altitude in meters
    /// </summary>
    public double? Altitude { get; set; }

    /// <summary>
    /// GPS timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Vehicle odometer reading
    /// </summary>
    public int? OdometerReading { get; set; }

    /// <summary>
    /// Engine status (on/off)
    /// </summary>
    public bool EngineOn { get; set; }

    /// <summary>
    /// Additional sensor data (JSON)
    /// </summary>
    public string? SensorData { get; set; }

    /// <summary>
    /// Navigation property to Vehicle
    /// </summary>
    public virtual Vehicle Vehicle { get; set; } = null!;
}
