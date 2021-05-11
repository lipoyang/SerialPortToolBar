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

            int[]  init   = { 0x0000, 0x0000, 0x0000, 0x0000, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF };
            bool[] rshift = { false,  false,  true,   true,   false,  false,  true,   true   };
            int[]  xorout = { 0x0000, 0xFFFF, 0x0000, 0xFFFF, 0x0000, 0xFFFF, 0x0000, 0xFFFF };

            // IBM (x16 + x15 + x2 + 1) : 0x8005 / 0xA001
            for (int i = 0; i < 8; i++)
            {
                int[] expected = { 0xFEE8, 0x0117, 0xBB3D, 0x44C2, 0xAEE7, 0x5118, 0x4B37, 0xB4C8 };
                
                int poly       = rshift[i] ? 0xA001 : 0x8005;
                ShiftDir shift = rshift[i] ? ShiftDir.Right : ShiftDir.Left;

                crc16 = new CRC16(poly, init[i], shift, xorout[i]);
                val = crc16.Get(data, 0, 9);
                Console.Write("poly=" + poly.ToString("X04") + " ");
                Console.Write("init=" + init[i].ToString("X04") + " ");
                Console.Write("shift=" + (rshift[i] ? "Left  " : "Right "));
                Console.Write("xorout=" + xorout[i].ToString("X04") + " ");
                Console.Write("check=" + val.ToString("X04") + " ");
                Console.WriteLine((val == expected[i]) ? "OK" : "NG");
            }

            // CCITT (x16 + x15 + x2 + 1) : 0x1021 / 0x8408
            for (int i = 0; i < 8; i++)
            {
                int[] expected = { 0x31C3, 0xCE3C, 0x2189, 0xDE76, 0x29B1, 0xD64E, 0x6F91, 0x906E };

                int poly       = rshift[i] ? 0x8408 : 0x1021;
                ShiftDir shift = rshift[i] ? ShiftDir.Right : ShiftDir.Left;

                crc16 = new CRC16(poly, init[i], shift, xorout[i]);
                val = crc16.Get(data, 0, 9);
                Console.Write("poly=" + poly.ToString("X04") + " ");
                Console.Write("init=" + init[i].ToString("X04") + " ");
                Console.Write("shift=" + (rshift[i] ? "Left  " : "Right "));
                Console.Write("xorout=" + xorout[i].ToString("X04") + " ");
                Console.Write("check=" + val.ToString("X04") + " ");
                Console.WriteLine((val == expected[i]) ? "OK" : "NG");
            }
        }
    }
}
