namespace ParkingSystem.Models;

public class ParkingSpot
{
    public int Id { get; }
    public string Zone { get; }
    public string Key => $"{Zone}{Id:D2}";
    public SpotStatus Status { get; set; } = SpotStatus.FREE;
    public string? ReservedBy { get; set; }
    public string? OccupiedBy { get; set; }

    public ParkingSpot(int id, string zone)
    {
        Id = id;
        Zone = zone;
    }

    public override string ToString()
    {
        return $"[{Key}] {Status,-10}";
    }
}
