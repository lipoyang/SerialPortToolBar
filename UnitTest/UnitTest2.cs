using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using SerialPortToolBar;

namespace UnitTest
{
    /// <summary>
    /// BinaryPacketのテスト
    /// </summary>
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            // SetInt / GetInt

            // SetInt
            byte[] header = new byte[] { 0xA5, 0x5A };
            var packet = new BinaryPacket(6, header, Endian.BigEndian);
            packet.SetInt(2, 2, 0xABCD);
            Assert.AreEqual(0xA5, packet.Data[0]);
            Assert.AreEqual(0x5A, packet.Data[1]);
            Assert.AreEqual(0xAB, packet.Data[2]);
            Assert.AreEqual(0xCD, packet.Data[3]);

            // GetInt
            int value = packet.GetInt(2, 2);
            Assert.AreEqual(0xABCD, value);
        }

        [TestMethod]
        public void TestMethod2()
        {
            // GetIntU / GetIntS

            byte[] data = new byte[] { 0xA5, 0x5A, 0xFF, 0xFF };
            var packet = new BinaryPacket(data, Endian.BigEndian);

            // GetIntU
            uint uValue = packet.GetIntU(2, 2);
            Assert.AreEqual(65535u, uValue);

            // GetIntS
            int sValue = packet.GetIntS(2, 2);
            Assert.AreEqual(-1, sValue);
        }
    }
}
