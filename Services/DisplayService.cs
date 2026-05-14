using System;
using System.Collections.Generic;
using ParkingSystem.Interfaces;
using ParkingSystem.Models;

namespace ParkingSystem.Services
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLen)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLen ? value : value[..(maxLen - 3)] + "...";
        }
    }

    public class DisplayService : IDisplayService
    {
        private int _rightWidth = 58;
        private int _rightLeft;
        private int _leftWidth;
        private int _topMargin = 1;
        private readonly List<string> _messages = new();
        private const int MaxMessages = 15;

        public void Initialize()
        {
            Console.CursorVisible = false;
            int w = Console.WindowWidth;
            _rightLeft = w - _rightWidth - 1;
            _leftWidth = _rightLeft - 2;
        }

        public void Refresh(IReadOnlyList<ParkingSpot> spots)
        {
            Console.Clear();
            DrawParkingLot(spots);
            DrawAllMessages();
        }

        public void ShowWelcome()
        {
            Console.Clear();
            int cw = 50;
            int left = Math.Max(0, (Console.WindowWidth - cw) / 2);
            DrawBox(left, _topMargin, cw, 10, " СИСТЕМА БРОНЮВАННЯ ПАРКОМІСЦЬ ");

            WriteAt(left + 2, _topMargin + 2, "Лабораторна робота 4: Конструювання");
            WriteAt(left + 2, _topMargin + 4, "Реалізація Варіанту 10:");
            WriteAt(left + 2, _topMargin + 8, "Натисніть [Enter] для запуску...");
        }

        private void DrawParkingLot(IReadOnlyList<ParkingSpot> spots)
        {
            int col = _rightLeft;
            int row = _topMargin;
            DrawBox(col, row, _rightWidth, spots.Count + 4, " ПАРКОМІСЦЯ ");

            WriteAt(col + 2, row + 1, " Ключ  Довж.   Відст.   Статус     Користувач");

            for (int i = 0; i < spots.Count; i++)
            {
                var s = spots[i];
                string lengthStr = $"{s.Length:F1}m";
                string distStr = $"{s.DistanceKm:F1}km";

                string user = s.ReservedBy ?? s.OccupiedBy ?? "-";
                if (user.Length > 10) user = user.Substring(0, 7) + "...";

                string line = $" {s.Key,-5} {lengthStr,-7} {distStr,-8} {s.Status,-10} {user}";
                WriteAt(col + 2, row + 2 + i, line);
            }
        }

        public void ShowMenu()
        {
            int row = _topMargin;
            DrawBox(0, row, _leftWidth, 10, " ГОЛОВНЕ МЕНЮ ");

            WriteAt(2, row + 2, "1. Знайти місце по маршруту");
            WriteAt(2, row + 3, "2. Зарезервувати місце вручну");
            WriteAt(2, row + 4, "3. Скасувати бронювання");
            WriteAt(2, row + 5, "4. Зайняти місце (Фізичний заїзд)");
            WriteAt(2, row + 6, "5. Звільнити місце (Виїзд)");
            WriteAt(2, row + 8, "0. Вихід з програми");
        }

        public void AddMessage(string msg)
        {
            _messages.Add(msg);
            if (_messages.Count > MaxMessages) _messages.RemoveAt(0);
        }

        private void DrawAllMessages()
        {
            int startRow = _topMargin + 12;
            WriteAt(2, startRow, "── ОПОВІЩЕННЯ ТА ЛОГИ ──");
            for (int i = 0; i < _messages.Count; i++)
            {
                WriteAt(2, startRow + 1 + i, _messages[i].Truncate(_leftWidth - 2));
            }
        }

        public string? ReadChoice() => ReadLine("Ваш вибір");

        public string? ReadLine(string prompt)
        {
            int row = _topMargin + 10;
            WriteAt(2, row, new string(' ', _leftWidth - 4)); // Очищення рядка
            Console.ForegroundColor = ConsoleColor.Cyan;
            WriteAt(2, row, $"{prompt}: ");
            Console.ResetColor();
            Console.CursorVisible = true;
            string? result = Console.ReadLine();
            Console.CursorVisible = false;
            return result;
        }

        private static void WriteAt(int left, int top, string text)
        {
            try { Console.SetCursorPosition(left, top); Console.Write(text); } catch { }
        }

        public static void DrawBox(int left, int top, int width, int height, string title)
        {
            WriteAt(left, top, "┌" + new string('─', width - 2) + "┐");
            WriteAt(left + (width - title.Length - 2) / 2, top, $" {title} ");
            for (int r = 1; r < height - 1; r++)
                WriteAt(left, top + r, "│" + new string(' ', width - 2) + "│");
            WriteAt(left, top + height - 1, "└" + new string('─', width - 2) + "┘");
        }
    }
}