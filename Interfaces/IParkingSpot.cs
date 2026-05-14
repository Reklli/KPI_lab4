using ParkingSystem.Models;

namespace ParkingSystem.Interfaces;

public interface IParkingSpot
{
    int Id { get; }
    string Zone { get; }
    string Key { get; }
    SpotStatus Status { get; set; }
    string? ReservedBy { get; set; }
    string? OccupiedBy { get; set; }
}
