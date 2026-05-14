using ParkingSystem.Interfaces;
using ParkingSystem.Models;

namespace ParkingSystem.Services;

public class ParkingRepository : IParkingRepository
{
    private readonly List<ParkingSpot> _spots;
    private readonly Dictionary<int, Reservation> _reservations = new();
    private int _nextReservationId = 1;

    public ParkingRepository(int rows, int spotsPerRow)
    {
        _spots = new List<ParkingSpot>();
        string[] zones = { "A", "B", "C" };
        int sid = 1;

        for (int row = 0; row < rows; row++)
        {
            string zone = row < zones.Length ? zones[row] : row.ToString();
            for (int col = 0; col < spotsPerRow; col++)
            {
                _spots.Add(new ParkingSpot(sid, zone));
                sid++;
            }
        }
    }

    public IReadOnlyList<ParkingSpot> GetAllSpots() => _spots.AsReadOnly();

    public ParkingSpot? GetSpotByKey(string key)
        => _spots.FirstOrDefault(s => s.Key == key);

    public ParkingSpot? GetSpotById(int id)
        => _spots.FirstOrDefault(s => s.Id == id);

    public List<ParkingSpot> GetFreeSpots()
        => _spots.Where(s => s.Status == SpotStatus.FREE).ToList();

    public void AddReservation(Reservation reservation)
    {
        reservation.Id = _nextReservationId++;
        _reservations[reservation.Id] = reservation;
    }

    public Reservation? GetReservation(int id)
        => _reservations.GetValueOrDefault(id);

    public IReadOnlyList<Reservation> GetAllReservations()
        => _reservations.Values.ToList().AsReadOnly();

    public int NextReservationId => _nextReservationId;
}
