namespace ParkingSystem.Models;

public class Reservation
{
    public int Id { get; set; }
    public string SpotKey { get; }
    public string UserName { get; }
    public int DurationMinutes { get; }
    public bool IsActive { get; set; } = true;

    public Reservation(string spotKey, string userName, int durationMinutes)
    {
        SpotKey = spotKey;
        UserName = userName;
        DurationMinutes = durationMinutes;
    }

    public override string ToString()
    {
        return $"Res #{Id}: Spot {SpotKey} for {UserName} ({DurationMinutes}min)";
    }
}
