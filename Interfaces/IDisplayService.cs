using System.Collections.Generic;
using ParkingSystem.Models;

namespace ParkingSystem.Interfaces
{
    public interface IDisplayService
    {
        void Initialize();
        void Refresh(IReadOnlyList<ParkingSpot> spots);
        void ShowWelcome();
        void ShowMenu();

        void AddMessage(string msg);

        string? ReadChoice();
        string? ReadLine(string prompt);
    }
}