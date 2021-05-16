using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports; // SerialPort
using System.Runtime.InteropServices; // DllImport
using Microsoft.Win32.SafeHandles; // SafeFileHandle
using System.ComponentModel; // Win32Exception
using System.Reflection; // BindingFlags

namespace SerialPortToolBar
{
    /// <summary>
    /// SerialPortクラスの拡張メソッド定義クラス
    /// </summary>
    public static class SerialPortExtensions
    {
        #region 送信系メソッド

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

        /// <summary>
        /// パケットを送信します。
        /// </summary>
        /// <param name="packet">パケット</param>
        public static void Send(this SerialPort serialPort, BasePacket packet)
        {
            serialPort.Write(packet.Data, 0, packet.Data.Length);
        }

        #endregion

        #region RTS機能設定用メソッド

        /// <summary>
        /// DCB構造体のfRtsControlの値を設定する(必ずポートをOpenしてから設定すること)
        /// </summary>
        /// <param name="vallue">fRtsControlの値</param>
        public static void SetRtsControl(this SerialPort serialPort, int vallue)
        {
            if (serialPort.BaseStream != null)
            {
                SetDcbFlag(serialPort, FRTSCONTROL, vallue);
            }
        }

        // Win32APIの宣言
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetCommState(SafeFileHandle hFile, IntPtr lpDCB);

        // C#は構造体のビットフィールドアクセスをサポートしていないが
        // Win32API の DCB構造体はビットフィールドで定義されている。
        // SerialStreamクラスのSetDcbFlag() / GetDcbFlag() メソッドでは
        // DCB構造体のビット位置を引数として指定する。
        // https://referencesource.microsoft.com/#system/compmod/microsoft/win32/NativeMethods.cs

        internal const int FBINARY = 0;
        internal const int FPARITY = 1;
        internal const int FOUTXCTSFLOW = 2;
        internal const int FOUTXDSRFLOW = 3;
        internal const int FDTRCONTROL = 4;
        internal const int FDSRSENSITIVITY = 6;
        internal const int FTXCONTINUEONXOFF = 7;
        internal const int FOUTX = 8;
        internal const int FINX = 9;
        internal const int FERRORCHAR = 10;
        internal const int FNULL = 11;
        internal const int FRTSCONTROL = 12;
        internal const int FABORTONOERROR = 14;
        internal const int FDUMMY2 = 15;

        // DCB構造体のビットフィールドの値を設定する
        // serialPort: シリアルポート
        // whichFlag: どのビットフィールドか
        // value: ビットフィールドに設定する値
        private static void SetDcbFlag(SerialPort serialPort, int whichFlag, int value)
        {
            // リフレクションでprivateなインスタンスメソッド/フィールドにアクセスするためのフラグ
            BindingFlags bflag = BindingFlags.NonPublic | BindingFlags.Instance;

            // SerialPortオブジェクトが持つストリーム(SerialStreamオブジェクト)を取得
            object stream = serialPort.BaseStream;
            Type baseStreamType = stream.GetType();

            // ビットフィールドの元の値を取得　(stream.GetDcbFlag(whichFlag) を実行)
            object oldValue = baseStreamType.GetMethod("GetDcbFlag", bflag)
              .Invoke(stream, new object[] { whichFlag });

            // ビットフィールドに値を設定　(stream.SetDcbFlag(whichFlag, value) を実行)
            baseStreamType.GetMethod("SetDcbFlag", bflag)
              .Invoke(stream, new object[] { whichFlag, value });

            // DCB構造体をSetCommStateでシリアルポートデバイスに設定
            try
            {
                // シリアルポートデバイスのハンドルを渡す　(stream._handle を取得)
                SafeFileHandle _handle = 
                    (SafeFileHandle)baseStreamType.GetField("_handle", bflag)
                        .GetValue(stream);

                // DCB構造体を取得
                object dcb = baseStreamType.GetField("dcb", bflag).GetValue(stream);

                // DCB構造体サイズぶんのメモリをアンマネージドメモリから確保
                IntPtr newDCB = Marshal.AllocHGlobal(Marshal.SizeOf(dcb));
                try
                {
                    // DCB構造体を確保したメモリにコピー
                    Marshal.StructureToPtr(dcb, newDCB, false);

                    // SetCommStateでDCB構造体をシリアルポートデバイスに設定
                    if (!SetCommState(_handle, newDCB))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                finally
                {
                    // 確保したアンマネージドメモリを解放
                    Marshal.FreeHGlobal(newDCB);
                }
            }
            catch
            {
                // 失敗したらビットフィールドの値を元に戻す
                baseStreamType.GetMethod("SetDcbFlag", bflag)
                  .Invoke(stream, new object[] { whichFlag, oldValue });
                throw;
            }
        }
        #endregion
    }

    /// <summary>
    /// シリアルポートのDCB構造体のfRtsControlの値
    /// </summary>
    public static class RtsControl
    {
        public const int Disable   = 0x00; // RTS_CONTROL_DISABLE
        public const int Enable    = 0x01; // RTS_CONTROL_ENABLE
        public const int Handshake = 0x02; // RTS_CONTROL_HANDSHAKE
        public const int Toggle    = 0x03; // RTS_CONTROL_TOGGLE
    }
}
