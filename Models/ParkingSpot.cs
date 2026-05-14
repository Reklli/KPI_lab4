using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSystem.Models
{
    public class ParkingSpot
    {
        public int Id { get; }
        public string Zone { get; }
        public string Key => $"{Zone}{Id:D2}";

        public SpotStatus Status { get; set; } = SpotStatus.FREE;
        public string? ReservedBy { get; set; }
        public string? OccupiedBy { get; set; }

        public double Length { get; }
        public double DistanceKm { get; }

        public int CurrentDuration { get; set; } = 0;

        public ParkingSpot(int id, string zone, double length, double distanceKm)
        {
            Id = id;
            Zone = zone;
            Length = length;
            DistanceKm = distanceKm;
        }

        public override string ToString()
        {
            return $"[{Key}] {Length:F1}m, {DistanceKm:F1}km {Status,-10}";
        }
    }
}