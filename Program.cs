using System;
using System.Text;
using ParkingSystem.Controllers;
using ParkingSystem.Interfaces;
using ParkingSystem.Services;

namespace ParkingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            IParkingRepository repository = new ParkingRepository(rows: 3, spotsPerRow: 5);
            IPaymentService paymentService = new PaymentService();
            IDisplayService display = new DisplayService();
            IParkingController controller = new ParkingController(repository, paymentService, display);

            controller.Run();
        }
    }
}