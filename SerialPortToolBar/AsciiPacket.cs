using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortToolBar
{
    /// <summary>
    /// アスキー形式のパケット
    /// </summary>
    public class AsciiPacket
    {
        /// <summary>
        /// パケットのバイト配列データ(開始コード/終了コードを含む)
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// コンストラクタ(送信時用)
        /// </summary>
        /// <param name="size">パケットの全バイト数(開始コード/終了コードを含む)</param>
        /// <param name="startCode">開始コード</param>
        /// <param name="endCode">終了コード</param>
        public AsciiPacket(int size, byte startCode = AsciiCode.STX, byte endCode = AsciiCode.ETX)
        {
            Data = new byte[size];
            Data[0] = startCode;
            Data[1] = endCode;
        }

        /// <summary>
        /// コンストラクタ(受信時用)
        /// </summary>
        /// <param name="data">パケットのバイト配列データ(開始コード/終了コードを含む)</param>
        public AsciiPacket(byte[] data)
        {
            Data = data;
        }


        public void SetValue(int offset, byte value )
        {
            Data[offset] = value;
        }

        public void SetHex(int offset, int width, int value)
        {
            for(int i= width - 1; i >= 0; i--)
            {
                Data[offset + i] = HEXCHAR[value & 0x0000000F];
                value = value >> 4;
            } 
        }
        public void SetDec(int offset, int width, int value)
        {
            for (int i = width - 1; i >= 0; i--)
            {
                Data[offset + i] = HEXCHAR[value % 10];
                value = value / 10;
            }
        }
        public byte GetValue(int offset)
        {
            return Data[offset];
        }

        public bool GetHex(int offset, int width, ref int value)
        {
            int tempVal = 0;
            for (int i = 0; i < width; i++)
            {
                tempVal = tempVal << 4;

                if (Hex2Int(Data[offset + i], out int digitVal))
                {
                    tempVal += digitVal;
                }
                else
                {
                    return false;
                }
            }
            value = tempVal;
            return true;
        }

        public bool GetDec(int offset, int width, ref int value)
        {
            int tempVal = 0;
            for (int i = 0; i < width; i++)
            {
                tempVal = tempVal * 10;

                if (Dec2Int(Data[offset + i], out int digitVal))
                {
                    tempVal += digitVal;
                }
                else
                {
                    return false;
                }
            }
            value = tempVal;
            return true;
        }

        #region 内部処理

        // 数値を表現するキャラクタのテーブル (0123456789ABCDEF)
        readonly byte[] HEXCHAR = {
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
            0x38, 0x39, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46
        };

        static bool Hex2Int(byte c, out int val)
        {
            if((0x30 <= c) && (c <= 0x39))
            {
                val = c - 0x30;
                return true;
            }
            else if ((0x41 <= c) && (c <= 0x46))
            {
                val = 10 + (c - 0x41);
                return true;
            }
            else
            {
                val = 0;
                return false;
            }
        }
        static bool Dec2Int(byte c, out int val)
        {
            if ((0x30 <= c) && (c <= 0x39))
            {
                val = c - 0x30;
                return true;
            }
            else
            {
                val = 0;
                return false;
            }
        }

        #endregion

    }
}
