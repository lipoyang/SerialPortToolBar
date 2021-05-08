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
            var packet = new BinaryPacket(6, header, Endian.Big);
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
            var packet = new BinaryPacket(data, Endian.Big);

            // GetIntU
            uint uValue = packet.GetIntU(2, 2);
            Assert.AreEqual(65535u, uValue);

            // GetIntS
            int sValue = packet.GetIntS(2, 2);
            Assert.AreEqual(-1, sValue);
        }

        [TestMethod]
        public void TestMethod3()
        {
            // SetFloat / GetFloat

            byte[] header = new byte[] { 0xA5, 0x5A };
            var packet = new BinaryPacket(6, header, Endian.Big);

            // SetFloat
            packet.SetFloat(2, 12.59375f);
            Assert.AreEqual(0x41, packet.Data[2]);
            Assert.AreEqual(0x49, packet.Data[3]);
            Assert.AreEqual(0x80, packet.Data[4]);
            Assert.AreEqual(0x00, packet.Data[5]);

            // GetFloat
            float value = packet.GetFloat(2);
            Assert.AreEqual(12.59375f, value);

            packet = new BinaryPacket(6, header, Endian.Little);

            // SetFloat
            packet.SetFloat(2, 12.59375f);
            Assert.AreEqual(0x00, packet.Data[2]);
            Assert.AreEqual(0x80, packet.Data[3]);
            Assert.AreEqual(0x49, packet.Data[4]);
            Assert.AreEqual(0x41, packet.Data[5]);

            // GetFloat
            value = packet.GetFloat(2);
            Assert.AreEqual(12.59375f, value);
        }
    }
}
