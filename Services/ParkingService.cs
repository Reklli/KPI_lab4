using System;
using System.Collections.Generic;
using System.Text;
using ParkingSystem.Models;

namespace ParkingSystem.Services
{
    public class ParkingService
    {
        public ParkingSpot? FindSuitableSpot(
            Driver driver,
            List<ParkingSpot> spots)
        {
            return spots
                .Where(x => x.Status == SpotStatus.FREE)
                .Where(x => x.Length >= driver.CarLength)
                .OrderBy(x => x.DistanceKm)
                .FirstOrDefault();
        }

        public void Reserve(ParkingSpot spot)
        {
            spot.Status = SpotStatus.RESERVED;
        }

        public void Occupy(ParkingSpot spot)
        {
            spot.Status = SpotStatus.OCCUPIED;
        }

        public void Release(ParkingSpot spot)
        {
            spot.Status = SpotStatus.FREE;
        }
    }
}
