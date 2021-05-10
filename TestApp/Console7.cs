using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices; // DllImport
using SerialPortToolBar;

namespace TestApp
{
    // テスト7 (CRC-16)
    static class Console7
    {
        // Formアプリで一時的にコンソールを出すための呪文
        [DllImport("Kernel32.dll")]
        static extern bool AllocConsole();
        [DllImport("Kernel32.dll")]
        static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        static extern bool SetStdHandle(int nStdHandle, IntPtr handle);
        const int STD_INPUT_HANDLE = -10;
        const int STD_OUTPUT_HANDLE = -11;
        const int STD_ERROR_HANDLE = -12;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        static bool FirstTime = true;

        static public void Test()
        {
            if (FirstTime)
            {
                FirstTime = false;

                SetStdHandle(STD_OUTPUT_HANDLE, IntPtr.Zero);
                SetStdHandle(STD_INPUT_HANDLE, IntPtr.Zero);
                AllocConsole();
            }
            else
            {
                Console.Clear();
                IntPtr hwnd = GetConsoleWindow();
                ShowWindow(hwnd, SW_SHOW);
            }

            // テスト関数
            TestFunc();

            Console.WriteLine("続行するには何かキーを押してください");
            Console.ReadKey();

            {
                IntPtr hwnd = GetConsoleWindow();
                ShowWindow(hwnd, SW_HIDE);
            }
        }

        // テスト関数
        static private void TestFunc()
        {
            byte[] data = Encoding.ASCII.GetBytes("123456789");
            CRC16 crc16;
            int val;

            crc16 = new CRC16(0x0000, CRC16Poly.IBM, ShiftDir.Left, 0x0000);
            val = crc16.Get(data, 0, 9);
            Console.WriteLine(val.ToString("X04"));

            crc16 = new CRC16(0x0000, CRC16Poly.IBM, ShiftDir.Left, 0xFFFF);
            val = crc16.Get(data, 0, 9);
            Console.WriteLine(val.ToString("X04"));

            crc16 = new CRC16(0x0000, 0xA001, ShiftDir.Right, 0x0000);
            val = crc16.Get(data, 0, 9);
            Console.WriteLine(val.ToString("X04"));

            crc16 = new CRC16(0x0000, 0xA001, ShiftDir.Right, 0xFFFF);
            val = crc16.Get(data, 0, 9);
            Console.WriteLine(val.ToString("X04"));

            crc16 = new CRC16(0xFFFF, CRC16Poly.IBM, ShiftDir.Left, 0x0000);
            val = crc16.Get(data, 0, 9);
            Console.WriteLine(val.ToString("X04"));

            crc16 = new CRC16(0xFFFF, CRC16Poly.IBM, ShiftDir.Left, 0xFFFF);
            val = crc16.Get(data, 0, 9);
            Console.WriteLine(val.ToString("X04"));

            crc16 = new CRC16(0xFFFF, 0xA001, ShiftDir.Right, 0x0000);
            val = crc16.Get(data, 0, 9);
            Console.WriteLine(val.ToString("X04"));

            crc16 = new CRC16(0xFFFF, 0xA001, ShiftDir.Right, 0xFFFF);
            val = crc16.Get(data, 0, 9);
            Console.WriteLine(val.ToString("X04"));

        }
    }
}
