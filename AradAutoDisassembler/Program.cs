using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsInput;

namespace AradAutoDisassembler
{
    class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr wnd, int x, int y, int width, int height, bool repaint);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr wnd, out RECT rect);

        const int ScreenWidth = 2560;
        const int ScreenHeight = 1440;

        // 1440x900
        static Point Base { get; } = new Point(850, 440);
        static Point Offset { get; } = new Point(45, 40);

        static double GetAbsoluteX(int x) => x * 65535 / ScreenWidth;
        static double GetAbsoluteY(int y) => y * 65535 / ScreenHeight;

        static Process GetAradProcess()
        {
            var process = Process.GetProcessesByName("ARAD");

            if (!process.Any())
            {
                throw new Exception("Process not found");
            }

            return process.First();
        }

        static void Main()
        {
            var simulator = new InputSimulator();
            var process = GetAradProcess();
            var hWnd = process.MainWindowHandle;
            GetClientRect(hWnd, out var rect);
            MoveWindow(hWnd, 0, 0, rect.Width, rect.Height, true);
            Microsoft.VisualBasic.Interaction.AppActivate(process.Id);
            Thread.Sleep(500);

            for (var x = 0; x < 6; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    simulator.Mouse.MoveMouseTo(GetAbsoluteX(Base.X + (x * Offset.X)), GetAbsoluteY(Base.Y + (y * Offset.Y)));
                    simulator.Mouse.LeftButtonDown();
                    Thread.Sleep(30);
                    simulator.Mouse.LeftButtonUp();
                    Thread.Sleep(30);
                }
            }
        }
    }
}