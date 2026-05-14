using System;
using System.Linq;
using ParkingSystem.Interfaces;
using ParkingSystem.Models;

namespace ParkingSystem.Controllers
{
    public class ParkingController : IParkingController
    {
        private readonly IParkingRepository _repository;
        private readonly IPaymentService _paymentService;
        private readonly IDisplayService _display;

        public ParkingController(
            IParkingRepository repository,
            IPaymentService paymentService,
            IDisplayService display)
        {
            _repository = repository;
            _paymentService = paymentService;
            _display = display;
        }

        public void Run()
        {
            _display.Initialize();
            _display.ShowWelcome();
            Console.ReadLine(); // Чекаємо натискання Enter

            _display.AddMessage("Систему успішно ініціалізовано.");
            MainLoop();
        }

        private void MainLoop()
        {
            while (true)
            {
                // Оновлюємо екран перед кожною дією
                _display.Refresh(_repository.GetAllSpots());
                _display.ShowMenu();

                string? choice = _display.ReadChoice();

                if (choice == "0")
                {
                    _display.AddMessage("Вихід з програми. До побачення!");
                    break;
                }

                switch (choice)
                {
                    case "1": SearchOnRoute(); break;      // Реалізація Варіанту 10
                    case "2": ReserveSpot(); break;        // Ручне бронювання
                    case "3": CancelReservation(); break;
                    case "4": OccupySpot(); break;
                    case "5": ReleaseSpot(); break;
                    default:
                        _display.AddMessage("Помилка: невірний вибір. Спробуйте ще раз.");
                        break;
                }
            }
        }

        // ЛОГІКА ВАРІАНТУ 10 (Пошук по маршруту)
        private void SearchOnRoute()
        {
            string? lenStr = _display.ReadLine("Довжина вашого авто (м)");
            if (!double.TryParse(lenStr, out double carLen) || carLen <= 0) return;

            string? distStr = _display.ReadLine("Радіус пошуку по курсу (км)");
            if (!double.TryParse(distStr, out double maxDist) || maxDist <= 0) return;

            _display.AddMessage($"Моніторинг: шукаю місця (Довжина >= {carLen}m, Відстань <= {maxDist}km)...");

            var suitableSpot = _repository.GetFreeSpots()
                .Where(s => s.Length >= carLen && s.DistanceKm <= maxDist)
                .OrderBy(s => s.DistanceKm)
                .FirstOrDefault();

            if (suitableSpot == null)
            {
                _display.AddMessage($"[УВАГА] Підходящих вільних місць не знайдено.");
                return;
            }
            _display.AddMessage($"[ЗНАЙДЕНО] {suitableSpot.Key} | Довж: {suitableSpot.Length}m | Відст: {suitableSpot.DistanceKm}km");

            string? confirm = _display.ReadLine($"Бажаєте зарезервувати місце {suitableSpot.Key}? (Y/N)");
            if (confirm?.ToUpper() == "Y")
            {
                string name = _display.ReadLine("Ваше ім'я") ?? "Водій";
                string? durStr = _display.ReadLine("Тривалість (хв)");
                int duration = int.TryParse(durStr, out int d) ? d : 60;

                suitableSpot.Status = SpotStatus.RESERVED;
                suitableSpot.ReservedBy = name;
                suitableSpot.CurrentDuration = duration;

                var res = new Reservation(suitableSpot.Key, name, duration);
                _repository.AddReservation(res);

                decimal cost = _paymentService.CalculateCost(duration);

                _display.AddMessage($"Успіх! Місце {suitableSpot.Key} зарезервовано для {name}.");
                _display.AddMessage($"Тривалість: {duration} хв. Вартість: ${cost:F2}. Номер броні: {res.Id:D3}");
            }
        }

        // СТАНДАРТНІ ОПЕРАЦІЇ 
        private void ReserveSpot()
        {
            var free = _repository.GetFreeSpots();
            if (!free.Any())
            {
                _display.AddMessage("Помилка: немає вільних місць.");
                return;
            }

            string? key = _display.ReadLine("Введіть ключ місця (напр. A01)");
            var spot = _repository.GetSpotByKey(key?.ToUpper() ?? "");

            if (spot == null || spot.Status != SpotStatus.FREE)
            {
                _display.AddMessage("Помилка: місце не знайдено або вже зайняте.");
                return;
            }

            string? name = _display.ReadLine("Ваше ім'я");
            string? durStr = _display.ReadLine("Тривалість (хв)");
            int duration = int.TryParse(durStr, out int d) ? d : 60; // 60 хв за замовчуванням
            spot.CurrentDuration = duration;
            spot.Status = SpotStatus.RESERVED;
            spot.ReservedBy = name;
            _repository.AddReservation(new Reservation(spot.Key, name ?? "Unknown", 60));
            _display.AddMessage($"Місце {spot.Key} успішно зарезервовано!");
        }

        private void CancelReservation()
        {
            string? key = _display.ReadLine("Введіть ключ місця (напр. C15)");
            var spot = _repository.GetSpotByKey(key?.ToUpper() ?? "");

            if (spot == null)
            {
                _display.AddMessage("Помилка: місце не знайдено.");
                return;
            }

            if (spot.Status != SpotStatus.RESERVED)
            {
                _display.AddMessage($"Помилка: місце {spot.Key} зараз не зарезервоване.");
                return;
            }

            spot.Status = SpotStatus.FREE;
            spot.ReservedBy = null;
            _display.AddMessage($"Бронювання місця {spot.Key} успішно скасовано!");
        }

        private void OccupySpot()
        {
            string key = _display.ReadLine("Ключ місця для фізичного заїзду") ?? "";
            var spot = _repository.GetSpotByKey(key.ToUpper());

            if (spot == null)
            {
                _display.AddMessage("Помилка: місце не знайдено.");
                return;
            }

            if (spot.Status == SpotStatus.OCCUPIED)
            {
                _display.AddMessage($"Помилка: місце {spot.Key} вже зайняте іншим авто.");
                return;
            }

            string name = _display.ReadLine("Введіть ваше ім'я") ?? "Гість";

            if (spot.Status == SpotStatus.RESERVED)
            {
                if (!string.Equals(spot.ReservedBy, name, StringComparison.OrdinalIgnoreCase))
                {
                    _display.AddMessage($"Відмова: місце {spot.Key} зарезервовано для {spot.ReservedBy}.");
                    return;
                }
                _display.AddMessage($"Бронювання підтверджено. Ласкаво просимо, {name}!");
            }
            else
            {
                // Якщо заїхав без броні - запитуємо час і одразу показуємо тариф
                string durStr = _display.ReadLine("Планова тривалість (хв)") ?? "";
                spot.CurrentDuration = int.TryParse(durStr, out int d) ? d : 60;

                decimal expectedCost = _paymentService.CalculateCost(spot.CurrentDuration);
                _display.AddMessage($"Тариф: {spot.CurrentDuration} хв = ${expectedCost:F2}.");
            }

            // Оновлюємо стан
            spot.Status = SpotStatus.OCCUPIED;
            spot.OccupiedBy = name;
            spot.ReservedBy = null;

            _display.AddMessage($"Датчик: Автомобіль ({spot.OccupiedBy}) зайняв місце {spot.Key}");
            _display.AddMessage("Оплата буде автоматично знята під час виїзду.");
        }

        private void ReleaseSpot()
        {
            string? key = _display.ReadLine("Ключ місця для виїзду");
            var spot = _repository.GetSpotByKey(key?.ToUpper() ?? "");

            if (spot != null && spot.Status == SpotStatus.OCCUPIED)
            {
                string userName = spot.OccupiedBy ?? "Гість";
                int duration = spot.CurrentDuration > 0 ? spot.CurrentDuration : 60;

                decimal cost = _paymentService.CalculateCost(duration);
                _paymentService.ProcessPayment(userName, cost);

                _display.AddMessage($"--- ЧЕК: {spot.Key} ---");
                _display.AddMessage($"Користувач: {userName}");
                _display.AddMessage($"Тривалість: {duration} хв.");
                _display.AddMessage($"До сплати: ${cost:F2}");
                _display.AddMessage("----------------------");

                spot.Status = SpotStatus.FREE;
                spot.OccupiedBy = null;
                spot.ReservedBy = null;
                spot.CurrentDuration = 0;
            }
            else
            {
                _display.AddMessage("Помилка: місце не зайняте.");
            }
        }
    }
}