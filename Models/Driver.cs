using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSystem.Models
{
    public class Driver
    {
        public string Name { get; set; }
        public double CarLength { get; set; }

        public Driver(string name, double carLength)
        {
            Name = name;
            CarLength = carLength;
        }
    }
}