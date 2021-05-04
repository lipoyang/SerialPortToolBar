using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports; // SerialPort

namespace SerialPortToolBar
{
    /// <summary>
    /// SerialPortクラスの拡張メソッド定義クラス
    /// </summary>
    public static class SerialPortExtensions
    {
        /// <summary>
        /// バイト配列(byte[])を送信します。
        /// </summary>
        /// <param name="data">送信するデータ</param>
        public static void WriteBytes(this SerialPort serialPort, byte[] data)
        {
            serialPort.Write(data, 0, data.Length);
        }

        /// <summary>
        /// キャラクタ配列(char[])を送信します。
        /// </summary>
        /// <param name="data">送信するデータ</param>
        public static void WriteChars(this SerialPort serialPort, char[] data)
        {
            serialPort.Write(data, 0, data.Length);
        }

        /// <summary>
        /// バイトデータ(byte)を送信します。
        /// </summary>
        /// <param name="data">送信するデータ</param>
        public static void WriteByte(this SerialPort serialPort, byte data)
        {
            serialPort.Write(new byte[] { data }, 0, 1);
        }

        /// <summary>
        /// キャラクタデータ(char)を送信します。
        /// </summary>
        /// <param name="data">送信するデータ</param>
        public static void WriteChar(this SerialPort serialPort, char data)
        {
            serialPort.Write(new char[] { data }, 0, 1);
        }

        /// <summary>
        /// 受信したデータをバイト配列(byte[])として取得します
        /// </summary>
        /// <returns>受信したデータ</returns>
        public static byte[] ReadBytes(this SerialPort serialPort)
        {
            int size = serialPort.BytesToRead;
            byte[] buff = new byte[size];
            size = serialPort.Read(buff, 0, size);
            if(size == buff.Length)
            {
                return buff;
            }
            else
            {
                byte[] buff2 = new byte[size];
                Array.Copy(buff, buff2, size);
                return buff2;
            }
        }

        /// <summary>
        /// 受信したデータをキャラクタ配列(char[])として取得します
        /// </summary>
        /// <returns>受信したデータ</returns>
        public static char[] ReadChars(this SerialPort serialPort)
        {
            int size = serialPort.BytesToRead;
            char[] buff = new char[size];
            size = serialPort.Read(buff, 0, size);
            if (size == buff.Length)
            {
                return buff;
            }
            else
            {
                char[] buff2 = new char[size];
                Array.Copy(buff, buff2, size);
                return buff2;
            }
        }
    }
}
