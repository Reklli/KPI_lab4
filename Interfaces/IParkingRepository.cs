using ParkingSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSystem.Interfaces
{
    public interface IParkingRepository
    {
        IReadOnlyList<ParkingSpot> GetAllSpots();
        ParkingSpot? GetSpotByKey(string key);
        ParkingSpot? GetSpotById(int id);

        List<ParkingSpot> GetFreeSpots();

        void AddReservation(Reservation reservation);
        Reservation? GetReservation(int id);
    }
}