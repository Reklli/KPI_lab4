namespace ParkingSystem.Interfaces;

public interface IPaymentService
{
    decimal CalculateCost(int durationMinutes);
    bool ProcessPayment(string userName, decimal amount);
}
