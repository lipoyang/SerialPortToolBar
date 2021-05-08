using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using SerialPortToolBar;

namespace UnitTest
{
    /// <summary>
    /// AsciiPacketのテスト
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // SetHex / GetHex

            // SetHex
            var packet = new AsciiPacket(6, '#', '$');
            packet.SetHex(1, 4, 0xA50F);
            string str = packet.ToString();
            Assert.AreEqual("#A50F$", str);

            // GetHex 正常系
            bool ret = packet.GetHex(1, 4, out int val);
            Assert.AreEqual(true, ret);
            Assert.AreEqual(0xA50F, val);

            // GetHex 異常系
            packet.Data[3] = (byte)'Z';
            ret = packet.GetHex(1, 4, out _);
            Assert.AreEqual(false, ret);

            // SetHex 負数の場合
            packet.SetHex(1, 4, -1);
            str = packet.ToString();
            Assert.AreEqual("#FFFF$", str);
        }

        [TestMethod]
        public void TestMethod2()
        {
            // SetDec / GetDec

            // SetDec
            var packet = new AsciiPacket(6, '#', '$');
            packet.SetDec(1, 4, 1234);
            string str = packet.ToString();
            Assert.AreEqual("#1234$", str);

            // GetDec 正常系
            bool ret = packet.GetDec(1, 4, out int val);
            Assert.AreEqual(true, ret);
            Assert.AreEqual(1234, val);

            // GetDec 異常系
            packet.Data[3] = (byte)'A';
            ret = packet.GetDec(1, 4, out _);
            Assert.AreEqual(false, ret);
        }

        [TestMethod]
        public void TestMethod3()
        {
            // SetChar / GetChar

            // SetChar
            byte[] data = Encoding.ASCII.GetBytes("#ABCDEFG$");
            var packet = new AsciiPacket(data);
            packet.SetChar(2, 'b');
            string str = packet.ToString();
            Assert.AreEqual("#AbCDEFG$", str);

            // GetChar
            char c = packet.GetChar(3);
            Assert.AreEqual('C', c);
        }

        [TestMethod]
        public void TestMethod4()
        {
            // SetString / GetString

            // SetString
            byte[] data = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMN");
            var packet = new AsciiPacket(data);
            packet.SetString(4, "efghij");
            string str = packet.ToString();
            Assert.AreEqual("ABCDefghijKLMN", str);

            // GetString
            str = packet.GetString(2, 4);
            Assert.AreEqual("CDef", str);
        }

        [TestMethod]
        public void TestMethod5()
        {
            // GetHexU / GetHexS

            byte[] data;
            bool ret;
            AsciiPacket packet;

            data = Encoding.ASCII.GetBytes("####FFFF####");
            packet = new AsciiPacket(data);

            // GetHexU
            ret = packet.GetHexU(4, 4, out uint uval);
            Assert.AreEqual(true, ret);
            Assert.AreEqual(65535u, uval);

            // GetHexS
            ret = packet.GetHexS(4, 4, out int sval);
            Assert.AreEqual(true, ret);
            Assert.AreEqual(-1, sval);

            data = Encoding.ASCII.GetBytes("####7FFF####");
            packet = new AsciiPacket(data);

            // GetHexU
            ret = packet.GetHexU(4, 4, out uval);
            Assert.AreEqual(true, ret);
            Assert.AreEqual(32767u, uval);

            // GetHexS
            ret = packet.GetHexS(4, 4, out sval);
            Assert.AreEqual(true, ret);
            Assert.AreEqual(32767, sval);
        }
    }
}
