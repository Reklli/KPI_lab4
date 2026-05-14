namespace ParkingSystem.Interfaces;

public interface IReservation
{
    int Id { get; }
    string SpotKey { get; }
    string UserName { get; }
    int DurationMinutes { get; }
    bool IsActive { get; set; }
}
