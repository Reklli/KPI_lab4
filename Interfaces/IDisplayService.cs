using ParkingSystem.Models;

namespace ParkingSystem.Interfaces;

public interface IDisplayService
{
    void Initialize();
    void Refresh(IReadOnlyList<ParkingSpot> spots);
    void ShowWelcome();
    void ShowTestScenarios();
    void DrawParkingLot(IReadOnlyList<ParkingSpot> spots);
    void ShowMenu();
    void AddMessage(string msg);
    string? ReadChoice();
    string? ReadLine(string prompt);
}
