using ParkingSystem.Interfaces;

namespace ParkingSystem.Services;

public class PaymentService : IPaymentService
{
    private const decimal RatePerHour = 5.0m;

    public decimal CalculateCost(int durationMinutes)
    {
        return Math.Round(durationMinutes / 60.0m * RatePerHour, 2);
    }

    public bool ProcessPayment(string userName, decimal amount)
    {
        System.Threading.Thread.Sleep(300);
        return true;
    }
}
