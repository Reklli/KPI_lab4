using ParkingSystem.Interfaces;
using ParkingSystem.Models;

namespace ParkingSystem.Controllers;

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

        while (true)
        {
            _display.ShowWelcome();
            Console.CursorVisible = true;
            var key = Console.ReadKey(true);
            Console.CursorVisible = false;

            if (key.Key == ConsoleKey.Escape)
                return;
            if (key.Key == ConsoleKey.T)
            {
                _display.ShowTestScenarios();
                continue;
            }
            if (key.Key == ConsoleKey.Enter)
                break;
        }

        MainLoop();
    }

    private void MainLoop()
    {
        _display.AddMessage("Систему запущено. Оберіть дію з меню.");

        while (true)
        {
            _display.Refresh(_repository.GetAllSpots());
            _display.ShowMenu();

            string? choice = _display.ReadChoice();

            if (choice == "0")
            {
                _display.AddMessage("Вихід. До побачення!");
                _display.Refresh(_repository.GetAllSpots());
                Console.CursorVisible = true;
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                return;
            }

            switch (choice)
            {
                case "1": ReserveSpot(); break;
                case "2": CancelReservation(); break;
                case "3": OccupySpot(); break;
                case "4": ReleaseSpot(); break;
                default:
                    _display.AddMessage("Помилка: невірний вибір. Спробуйте ще раз.");
                    break;
            }
        }
    }

    private void ReserveSpot()
    {
        var free = _repository.GetFreeSpots();
        if (free.Count == 0)
        {
            _display.AddMessage("Помилка: немає вільних місць для бронювання.");
            return;
        }

        string? name = _display.ReadLine("Ваше ім'я");
        if (string.IsNullOrWhiteSpace(name))
        {
            _display.AddMessage("Помилка: ім'я не може бути порожнім.");
            return;
        }

        string? durStr = _display.ReadLine("Тривалість (хв)");
        if (!int.TryParse(durStr, out int dur) || dur <= 0)
            dur = 60;

        var spot = free[0];
        spot.Status = SpotStatus.RESERVED;
        spot.ReservedBy = name;

        var res = new Reservation(spot.Key, name, dur);
        _repository.AddReservation(res);

        decimal cost = _paymentService.CalculateCost(dur);
        _display.AddMessage($"Місце #{spot.Id:D2} ({spot.Key}) зарезервовано для {name}.");
        _display.AddMessage($"Тривалість: {dur} хв. Вартість: ${cost:F2}. Номер броні: {res.Id}");
    }

    private void CancelReservation()
    {
        string? idStr = _display.ReadLine("ID бронювання");
        if (!int.TryParse(idStr, out int rid))
        {
            _display.AddMessage("Помилка: невірний ID.");
            return;
        }

        var res = _repository.GetReservation(rid);
        if (res == null)
        {
            _display.AddMessage($"Помилка: бронювання #{rid} не знайдено.");
            return;
        }
        if (!res.IsActive)
        {
            _display.AddMessage($"Помилка: бронювання #{rid} вже скасовано.");
            return;
        }

        var spot = _repository.GetSpotByKey(res.SpotKey);
        if (spot != null)
        {
            spot.Status = SpotStatus.FREE;
            spot.ReservedBy = null;
        }

        res.IsActive = false;
        _display.AddMessage($"Бронювання #{rid} скасовано. Місце знову вільне.");
    }

    private void OccupySpot()
    {
        string? idStr = _display.ReadLine("Номер місця");
        if (!int.TryParse(idStr, out int sid))
        {
            _display.AddMessage("Помилка: невірний номер.");
            return;
        }

        var spot = _repository.GetSpotById(sid);
        if (spot == null)
        {
            _display.AddMessage($"Помилка: місця #{sid} не існує.");
            return;
        }
        if (spot.Status == SpotStatus.OCCUPIED)
        {
            _display.AddMessage($"Помилка: місце #{sid} вже зайняте.");
            return;
        }

        string? name = _display.ReadLine("Ваше ім'я");
        if (string.IsNullOrWhiteSpace(name))
        {
            _display.AddMessage("Помилка: ім'я не може бути порожнім.");
            return;
        }

        if (spot.Status == SpotStatus.RESERVED && spot.ReservedBy != name)
        {
            _display.AddMessage($"Помилка: місце #{sid} зарезервовано для {spot.ReservedBy}.");
            return;
        }

        spot.Status = SpotStatus.OCCUPIED;
        spot.OccupiedBy = name;
        _display.AddMessage($"Місце #{sid} ({spot.Key}) зайнято користувачем {name}.");
    }

    private void ReleaseSpot()
    {
        string? idStr = _display.ReadLine("Номер місця");
        if (!int.TryParse(idStr, out int sid))
        {
            _display.AddMessage("Помилка: невірний номер.");
            return;
        }

        var spot = _repository.GetSpotById(sid);
        if (spot == null)
        {
            _display.AddMessage($"Помилка: місця #{sid} не існує.");
            return;
        }
        if (spot.Status != SpotStatus.OCCUPIED)
        {
            _display.AddMessage($"Помилка: місце #{sid} не зайняте.");
            return;
        }

        int duration = 60;
        decimal cost = _paymentService.CalculateCost(duration);

        _display.AddMessage($"Місце #{sid} ({spot.Key}) звільнено користувачем {spot.OccupiedBy}.");
        _display.AddMessage($"Тривалість: ~{duration} хв. Сума: ${cost:F2}");

        _paymentService.ProcessPayment(spot.OccupiedBy!, cost);
        _display.AddMessage($"Оплату прийнято. Дякуємо!");

        spot.Status = SpotStatus.FREE;
        spot.OccupiedBy = null;
    }
}
