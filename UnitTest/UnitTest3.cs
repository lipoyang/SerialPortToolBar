using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using SerialPortToolBar;

namespace UnitTest
{
    /// <summary>
    /// チェックサム計算のテスト
    /// </summary>
    [TestClass]
    public class UnitTest3
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Sum
            byte[] data = new byte[100];
            for(int i = 1; i <= 10; i++)
            {
                data[19 + i] = (byte)i;
            }
            var packet = new BinaryPacket(data);
            byte sum = packet.Sum(20, 10);
            Assert.AreEqual(55, sum);

            // SetSum
            packet.SetSum(99, 20, 10);
            Assert.AreEqual(55, packet.Data[99]);

            // CheckSum
            bool check = packet.CheckSum(99, 20, 10);
            Assert.AreEqual(true, check);
            packet.Data[22] = 100;
            check = packet.CheckSum(99, 20, 10);
            Assert.AreEqual(false, check);
        }

        [TestMethod]
        public void TestMethod2()
        {
            // Xor
            byte[] data = new byte[100];
            for (int i = 1; i <= 10; i++)
            {
                data[19 + i] = (byte)i;
            }
            var packet = new BinaryPacket(data);
            byte xor = packet.Xor(20, 10);
            Assert.AreEqual(0x0B, xor);

            // SetXor
            packet.SetXor(99, 20, 10);
            Assert.AreEqual(0x0B, packet.Data[99]);

            // CheckXor
            bool check = packet.CheckXor(99, 20, 10);
            Assert.AreEqual(true, check);
            packet.Data[22] = 100;
            check = packet.CheckXor(99, 20, 10);
            Assert.AreEqual(false, check);
        }

        [TestMethod]
        public void TestMethod3()
        {
            byte[] data = Encoding.ASCII.GetBytes("123456789");
            CRC16 crc16;
            int val;

            int[] init = { 0x0000, 0x0000, 0x0000, 0x0000, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF };
            bool[] rshift = { false, false, true, true, false, false, true, true };
            int[] xorout = { 0x0000, 0xFFFF, 0x0000, 0xFFFF, 0x0000, 0xFFFF, 0x0000, 0xFFFF };

            // IBM (x16 + x15 + x2 + 1) : 0x8005 / 0xA001
            for (int i = 0; i < 8; i++)
            {
                int[] expected = { 0xFEE8, 0x0117, 0xBB3D, 0x44C2, 0xAEE7, 0x5118, 0x4B37, 0xB4C8 };

                int poly = rshift[i] ? 0xA001 : 0x8005;
                ShiftDir shift = rshift[i] ? ShiftDir.Right : ShiftDir.Left;

                crc16 = new CRC16(poly, init[i], shift, xorout[i]);
                val = crc16.Get(data, 0, 9);

                Assert.AreEqual(expected[i], val);
            }

            // CCITT (x16 + x12 + x5 + 1) : 0x1021 / 0x8408
            for (int i = 0; i < 8; i++)
            {
                int[] expected = { 0x31C3, 0xCE3C, 0x2189, 0xDE76, 0x29B1, 0xD64E, 0x6F91, 0x906E };

                int poly = rshift[i] ? 0x8408 : 0x1021;
                ShiftDir shift = rshift[i] ? ShiftDir.Right : ShiftDir.Left;

                crc16 = new CRC16(poly, init[i], shift, xorout[i]);
                val = crc16.Get(data, 0, 9);

                Assert.AreEqual(expected[i], val);
            }

        }
    }
}
