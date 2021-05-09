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

            // CRC16-IBM
            CRC16 crc16IBM = new CRC16(0x0000, CRC16Poly.IBM, 0x0000);
            int val = crc16IBM.Get(data, 0, 9);
            //Assert.AreEqual(0xbb3d, val);
            Assert.AreEqual(0xFEE8, val);

            // CRC16-CCITT
            CRC16 crc16CCITT = new CRC16(0xFFFF, CRC16Poly.CCITT, 0x0000);
            val = crc16CCITT.Get(data, 0, 9);
            //Assert.AreEqual(0xbb3d, val);
            Assert.AreEqual(0x29b1, val);
        }
    }
}
