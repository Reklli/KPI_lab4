using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSystem.Services
{
    public class NotificationService
    {
        public void Notify(string text)
        {
            Console.WriteLine();
            Console.WriteLine("====================");
            Console.WriteLine(text);
            Console.WriteLine("====================");
        }
    }
}
