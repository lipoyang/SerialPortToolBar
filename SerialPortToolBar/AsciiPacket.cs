using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerialPortToolBar
{
    /// <summary>
    /// アスキー形式のパケット
    /// </summary>
    public class AsciiPacket : BasePacket
    {
        #region 公開メソッド

        /// <summary>
        /// コンストラクタ(受信時用)
        /// </summary>
        /// <param name="data">パケットのバイト配列データ(開始コード/終了コード等も含む)</param>
        public AsciiPacket(byte[] data) : base(data) { }

        /// <summary>
        /// コンストラクタ(送信時用)
        /// </summary>
        /// <param name="size">パケットの全バイト数(開始コード/終了コードを含む)</param>
        /// <param name="startCode">開始コード</param>
        /// <param name="endCode">終了コード</param>
        public AsciiPacket(int size, byte startCode = AsciiCode.STX, byte endCode = AsciiCode.ETX)
        {
            Data = new byte[size];
            Data[0]        = startCode;
            Data[size - 1] = endCode;
        }

        /// <summary>
        /// コンストラクタ(送信時用)
        /// </summary>
        /// <param name="size">パケットの全バイト数(開始コード/終了コードを含む)</param>
        /// <param name="startChar">開始コード(アスキー文字)</param>
        /// <param name="endChar">終了コード(アスキー文字)</param>
        public AsciiPacket(int size, char startChar, char endChar)
        {
            Data = new byte[size];
            Data[0]        = (byte)startChar;
            Data[size - 1] = (byte)endChar;
        }

        /// <summary>
        /// 数値を16進文字列に変換して格納する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="width">桁数</param>
        /// <param name="value">数値</param>
        public void SetHex(int offset, int width, int value)
        {
            for(int i= width - 1; i >= 0; i--)
            {
                Data[offset + i] = HEXCHAR[value & 0x0000000F];
                value >>= 4;
            } 
        }

        /// <summary>
        /// 数値を10進文字列に変換して格納する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="width">桁数</param>
        /// <param name="value">数値</param>
        public void SetDec(int offset, int width, int value)
        {
            for (int i = width - 1; i >= 0; i--)
            {
                Data[offset + i] = HEXCHAR[value % 10];
                value /= 10;
            }
        }

        /// <summary>
        /// 16進文字列を数値に変換して取得する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="width">文字数</param>
        /// <param name="value">数値を返す</param>
        /// <returns>成否</returns>
        public bool GetHex(int offset, int width, out int value)
        {
            value = 0;
            for (int i = 0; i < width; i++)
            {
                value <<= 4;

                if (Hex2Int(Data[offset + i], out int digitVal))
                {
                    value += digitVal;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 10進文字列を数値に変換して取得する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="width">文字数</param>
        /// <param name="value">数値を返す</param>
        /// <returns>成否</returns>
        public bool GetDec(int offset, int width, out int value)
        {
            value = 0;
            for (int i = 0; i < width; i++)
            {
                value *= 10;

                if (Dec2Int(Data[offset + i], out int digitVal))
                {
                    value += digitVal;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 内部処理

        // 数値を表現するキャラクタのテーブル (0123456789ABCDEF)
        readonly byte[] HEXCHAR = {
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
            0x38, 0x39, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46
        };

        // 0～9, A～F, a～f のキャラクタを数値に変換
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
            else if ((0x61 <= c) && (c <= 0x66))
            {
                val = 10 + (c - 0x61);
                return true;
            }
            else
            {
                val = 0;
                return false;
            }
        }

        // 0～9のキャラクタを数値に変換
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
