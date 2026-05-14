#pragma warning disable CA1416

using ParkingSystem.Interfaces;
using ParkingSystem.Models;

namespace ParkingSystem.Services;

public class DisplayService : IDisplayService
{
    private int _rightWidth = 30;
    private int _rightLeft;
    private int _leftWidth;
    private int _topMargin = 1;
    private readonly List<string> _messages = new();
    private const int MaxMessages = 20;

    public void Initialize()
    {
        try
        {
            Console.CursorVisible = false;
            int w = Console.WindowWidth;
            _rightWidth = Math.Min(30, Math.Max(22, w / 3));
            _rightLeft = w - _rightWidth - 1;
            _leftWidth = _rightLeft - 2;
            if (_leftWidth < 20)
            {
                _rightWidth = 22;
                _rightLeft = w - _rightWidth - 1;
                _leftWidth = _rightLeft - 2;
            }
        }
        catch
        {
            _rightWidth = 58;
            _rightLeft = 50;
            _leftWidth = 45;
        }
    }

    public void Refresh(IReadOnlyList<ParkingSpot> spots)
    {
        try { Console.Clear(); } catch { }
        DrawParkingLot(spots);
        DrawAllMessages();
    }

    public void ShowWelcome()
    {
        try { Console.Clear(); } catch { }

        int cw = 46;
        int ch = 14;
        int left = Math.Max(0, (Console.WindowWidth - cw) / 2);
        int top = _topMargin;

        DrawBox(left, top, cw, ch, " PARKING RESERVATION SYSTEM v1.0 ");
        int r = top + 2;
        WriteAt(left + 2, r++, "Лабораторна робота 4: Конструювання,".PadRight(cw - 4));
        WriteAt(left + 2, r++, "кодування та тестування".PadRight(cw - 4));
        r++;
        WriteAt(left + 2, r++, "Система бронювання паркомісць".PadRight(cw - 4));
        WriteAt(left + 2, r++, "(на основі лабораторної роботи 2)".PadRight(cw - 4));
        r++;
        WriteAt(left + 2, r++, "  [Enter] - Запустити програму".PadRight(cw - 4));
        WriteAt(left + 2, r++, "  [T]    - Переглянути тест-сценарії".PadRight(cw - 4));
        WriteAt(left + 2, r++, "  [Esc]  - Вихід".PadRight(cw - 4));

        Console.CursorVisible = true;
        Console.SetCursorPosition(left + 2, top + ch + 1);
        Console.ResetColor();
    }

    public void ShowTestScenarios()
    {
        try { Console.Clear(); } catch { }

        string[] scenarios = new string[]
        {
            "ТЕСТ-СЦЕНАРІЇ ДЛЯ СИСТЕМИ БРОНЮВАННЯ ПАРКОМІСЦЬ",
            "═══════════════════════════════════════════════",
            "",
            "┌───────┬──────────────────────────────────────┐",
            "│ ID    │ TC-01                                │",
            "│ Вимога│ Зарезервувати вільне місце           │",
            "│ Умови │ Перед: система запущена, є вільні м. │",
            "│       │ Після: статус місця → RESERVED       │",
            "│ Вхід  │ Ім'я: 'Ivan', тривалість: 60 хв      │",
            "│ Кроки │ 1. Меню → 1 (Зарезервувати)          │",
            "│       │ 2. Ввести ім'я, натиснути Enter      │",
            "│       │ 3. Ввести тривалість, Enter          │",
            "│ Очік. │ Повідомлення про успішне бронювання  │",
            "│       │ Статус на панелі: R + RESERVED       │",
            "│ Реал. │ _____________________________________│",
            "├───────┼──────────────────────────────────────┤",
            "│ ID    │ TC-02                                │",
            "│ Вимога| Зайняти (оплатити) місце             │",
            "│ Умови │ Перед: місце в статусі FREE/RESERVED │",
            "│       │ Після: статус місця → OCCUPIED       │",
            "│ Вхід  │ Номер місця, ім'я користувача        │",
            "│ Кроки │ 1. Меню → 3 (Зайняти місце)          │",
            "│       │ 2. Ввести номер місця, Enter         │",
            "│       │ 3. Ввести ім'я, Enter                │",
            "│ Очік. │ Повідомлення про успішне зайняття    │",
            "│       │ Статус на панелі: O + OCCUPIED       │",
            "│ Реал. │ _____________________________________│",
            "├───────┼──────────────────────────────────────┤",
            "│ ID    │ TC-03                                │",
            "│ Вимога| Звільнити місце з оплатою            │",
            "│ Умови │ Перед: місце зайняте (OCCUPIED)      │",
            "│       │ Після: статус FREE, оплата проведена │",
            "│ Вхід  │ Номер зайнятого місця                │",
            "│ Кроки │ 1. Меню → 4 (Звільнити місце)        │",
            "│       │ 2. Ввести номер місця, Enter         │",
            "│ Очік. │ Повідомлення про оплату та звільнення│",
            "│       │ Статус на панелі: FREE               │",
            "│ Реал. │ _____________________________________│",
            "├───────┼──────────────────────────────────────┤",
            "│ ID    │ TC-04                                │",
            "│ Вимога| Скасувати бронювання                 │",
            "│ Умови │ Перед: є активне бронювання          │",
            "│       │ Після: бронювання неактивне, FREE    │",
            "│ Вхід  │ ID бронювання                        │",
            "│ Кроки │ 1. Меню → 2 (Скасувати бронювання)   │",
            "│       │ 2. Ввести ID бронювання, Enter       │",
            "│ Очік. │ Бронювання скасовано, місце вільне   │",
            "│ Реал. │ _____________________________________│",
            "└───────┴──────────────────────────────────────┘",
            "",
            "  Натисніть Enter щоб повернутись до меню..."
        };

        int startRow = 0;
        foreach (var line in scenarios)
        {
            WriteAt(0, startRow++, line);
        }

        Console.CursorVisible = true;
        Console.ReadLine();
    }

    public void DrawParkingLot(IReadOnlyList<ParkingSpot> spots)
    {
        int col = _rightLeft;
        int row = _topMargin;
        int boxH = Math.Max(spots.Count + 3, 5);

        DrawBox(col, row, _rightWidth, boxH, " PARKING LOT ");

        string header = $" {"#",2}  {"Spot",-5} {"Status",-9}";
        WriteAt(col + 2, row + 1, header.Truncate(_rightWidth - 4));

        for (int i = 0; i < spots.Count; i++)
        {
            var s = spots[i];
            char sym = s.Status switch
            {
                SpotStatus.FREE => ' ',
                SpotStatus.RESERVED => 'R',
                SpotStatus.OCCUPIED => 'O',
                _ => '?'
            };
            string line = $" {sym} {s.Key,-5} {s.Status,-9}";
            if (line.Length > _rightWidth - 4)
                line = line[..(_rightWidth - 4)];
            WriteAt(col + 2, row + 2 + i, line);
        }
    }

    public void ShowMenu()
    {
        int row = _topMargin;

        int mw = Math.Min(40, _leftWidth);
        DrawBox(0, row, mw, 10, " MENU ");

        WriteAt(2, row + 2, "1. Зарезервувати місце".PadRight(mw - 4));
        WriteAt(2, row + 3, "2. Скасувати бронювання ".PadRight(mw - 4));
        WriteAt(2, row + 4, "3. Зайняти місце       ".PadRight(mw - 4));
        WriteAt(2, row + 5, "4. Звільнити місце     ".PadRight(mw - 4));
        WriteAt(2, row + 6, "".PadRight(mw - 4));
        WriteAt(2, row + 7, "0. Вихід               ".PadRight(mw - 4));
    }

    public void AddMessage(string msg)
    {
        _messages.Add(msg);
        if (_messages.Count > MaxMessages)
            _messages.RemoveAt(0);
    }

    private void DrawAllMessages()
    {
        int startRow = _topMargin + 12;
        int areaH = MaxMessages;

        for (int i = 0; i < areaH; i++)
        {
            WriteAt(0, startRow + i, new string(' ', _leftWidth));
        }

        int msgRow = startRow;
        WriteAt(2, msgRow++, "── ЛОГ ──".PadRight(_leftWidth - 2));
        foreach (var msg in _messages)
        {
            if (msgRow >= startRow + areaH - 1) break;
            string display = msg.Truncate(_leftWidth - 2);
            WriteAt(2, msgRow++, display);
        }
    }

    public string? ReadChoice()
    {
        int row = _topMargin + 11;
        WriteAt(0, row, new string(' ', _leftWidth));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.SetCursorPosition(2, row);
        Console.Write("  Ваш вибір: ");
        Console.ResetColor();
        Console.CursorVisible = true;
        string? result = Console.ReadLine();
        Console.CursorVisible = false;
        return result;
    }

    public string? ReadLine(string prompt)
    {
        int row = _topMargin + 11;
        WriteAt(0, row, new string(' ', _leftWidth));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.SetCursorPosition(2, row);
        Console.Write($"  {prompt}: ");
        Console.ResetColor();
        Console.CursorVisible = true;
        string? result = Console.ReadLine();
        Console.CursorVisible = false;
        return result;
    }

    private static void SetCursor(int left, int top)
    {
        try { Console.SetCursorPosition(left, top); } catch { }
    }

    private static void WriteAt(int left, int top, string text)
    {
        try
        {
            Console.SetCursorPosition(left, top);
            Console.Write(text);
        }
        catch { }
    }

    public static void DrawBox(int left, int top, int width, int height, string title)
    {
        SetCursor(left, top);
        Console.Write("┌");
        int pad = Math.Max(0, (width - title.Length - 2) / 2);
        Console.Write(new string('─', pad));
        Console.Write(title);
        Console.Write(new string('─', Math.Max(0, width - title.Length - 2 - pad)));
        Console.Write("┐");

        for (int r = 1; r < height - 1; r++)
        {
            SetCursor(left, top + r);
            Console.Write("│" + new string(' ', Math.Max(0, width - 2)) + "│");
        }

        SetCursor(left, top + height - 1);
        Console.Write("└" + new string('─', Math.Max(0, width - 2)) + "┘");
    }
}

public static class StringExtensions
{
    public static string Truncate(this string value, int maxLen)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLen ? value : value[..(maxLen - 3)] + "...";
    }
}
