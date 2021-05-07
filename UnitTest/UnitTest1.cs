using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using SerialPortToolBar;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // SetHex / GetHex
            var packet = new AsciiPacket(6, '#', '$');
            packet.SetHex(1, 4, 0xA50F);
            string str = packet.ToString();
            Assert.AreEqual("#A50F$", str);
            
            int val = 0;
            bool ret = packet.GetHex(1, 4, ref val);
            Assert.AreEqual(true, ret);
            Assert.AreEqual(0xA50F, val);

            packet.Data[3] = (byte)'Z';
            ret = packet.GetHex(1, 4, ref val);
            Assert.AreEqual(false, ret);

            packet.SetHex(1, 4, -1);
            str = packet.ToString();
            Assert.AreEqual("#FFFF$", str);
        }

        [TestMethod]
        public void TestMethod2()
        {
            // SetDec / GetDec
            var packet = new AsciiPacket(6, '#', '$');
            packet.SetDec(1, 4, 1234);
            string str = packet.ToString();
            Assert.AreEqual("#1234$", str);

            int val = 0;
            bool ret = packet.GetDec(1, 4, ref val);
            Assert.AreEqual(true, ret);
            Assert.AreEqual(1234, val);

            packet.Data[3] = (byte)'A';
            ret = packet.GetDec(1, 4, ref val);
            Assert.AreEqual(false, ret);
        }

        [TestMethod]
        public void TestMethod3()
        {
            // SetChar / GetChar
            byte[] data = Encoding.ASCII.GetBytes("#ABCDEFG$");
            var packet = new AsciiPacket(data);
            packet.SetChar(2, 'b');
            string str = packet.ToString();
            Assert.AreEqual("#AbCDEFG$", str);

            char c = packet.GetChar(3);
            Assert.AreEqual('C', c);
        }

        [TestMethod]
        public void TestMethod4()
        {
            // SetString / GetString
            byte[] data = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMN");
            var packet = new AsciiPacket(data);
            packet.SetString(4, "efghij");
            string str = packet.ToString();
            Assert.AreEqual("ABCDefghijKLMN", str);

            str = packet.GetString(2, 4);
            Assert.AreEqual("CDef", str);
        }

    }
}
