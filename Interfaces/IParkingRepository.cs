using ParkingSystem.Models;

namespace ParkingSystem.Interfaces;

public interface IParkingRepository
{
    IReadOnlyList<ParkingSpot> GetAllSpots();
    ParkingSpot? GetSpotByKey(string key);
    ParkingSpot? GetSpotById(int id);
    List<ParkingSpot> GetFreeSpots();
    void AddReservation(Reservation reservation);
    Reservation? GetReservation(int id);
    IReadOnlyList<Reservation> GetAllReservations();
    int NextReservationId { get; }
}
