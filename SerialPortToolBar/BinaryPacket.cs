using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerialPortToolBar
{
    /// <summary>
    /// アスキー形式のパケット
    /// </summary>
    public class BinaryPacket : BasePacket
    {
        #region 公開メソッド

        /// <summary>
        /// コンストラクタ(受信時用)
        /// </summary>
        /// <param name="data">パケットのバイト配列データ(ヘッダ等を含む)</param>
        /// <param name="endian">エンディアン</param>
        public BinaryPacket(byte[] data, Endian endian = Endian.Big)
        {
            this.Data = data;
            this.Endian = endian;
        }

        /// <summary>
        /// コンストラクタ(送信時用)
        /// </summary>
        /// <param name="size">パケットの全バイト数(ヘッダ等を含む)</param>
        /// <param name="header">ヘッダ</param>
        /// <param name="endian">エンディアン</param>
        public BinaryPacket(int size, byte[] header, Endian endian = Endian.Big)
        {
            this.Data = new byte[size];
            this.Endian = endian;
            Array.Copy(header, this.Data, header.Length);
        }

        /// <summary>
        /// 整数値を格納する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="width">バイト数</param>
        /// <param name="value">数値</param>
        public void SetInt(int offset, int width, int value)
        {
            // ビッグエンディアンの場合
            if(Endian == Endian.Big)
            {
                for (int i = width - 1; i >= 0; i--)
                {
                    Data[offset + i] = (byte)(value & 0xFF);
                    value >>= 8;
                }
            }
            // リトルエンディアンの場合
            else
            {
                for(int i = 0; i < width; i++)
                {
                    Data[offset + i] = (byte)(value & 0xFF);
                    value >>= 8;
                }
            }
        }

        /// <summary>
        /// float型実数値を格納する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="value">数値</param>
        public void SetFloat(int offset, float value)
        {
            byte[] bData = BitConverter.GetBytes(value);

            if (Endian == Endian.Big)
            {
                // ビッグエンディアンの場合はバイト順を反転
                Array.Reverse(bData);
            }
            Array.Copy(bData, 0, Data, offset, 4);
        }


        /// <summary>
        /// 非負整数値を取得する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="width">バイト数</param>
        /// <returns>数値を返す</returns>
        public int GetInt(int offset, int width)
        {
            int value = 0;

            // ビッグエンディアンの場合
            if (Endian == Endian.Big)
            {
                for (int i = 0; i < width; i++)
                {
                    value <<= 8;
                    value |= Data[offset + i];
                }
            }
            // リトルエンディアンの場合
            else
            {
                for (int i = width - 1; i >= 0; i--)
                {
                    value <<= 8;
                    value |= Data[offset + i];
                }
            }
            return value;
        }

        /// <summary>
        /// 符号なし整数値を取得する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="width">バイト数</param>
        /// <returns>数値を返す</returns>
        public uint GetIntU(int offset, int width)
        {
            uint value = 0;

            // ビッグエンディアンの場合
            if (Endian == Endian.Big)
            {
                for (int i = 0; i < width; i++)
                {
                    value <<= 8;
                    value |= Data[offset + i];
                }
            }
            // リトルエンディアンの場合
            else
            {
                for (int i = width - 1; i >= 0; i--)
                {
                    value <<= 8;
                    value |= Data[offset + i];
                }
            }
            return value;
        }

        /// <summary>
        /// 符号なし整数値を取得する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="width">バイト数</param>
        /// <returns>数値を返す</returns>
        public int GetIntS(int offset, int width)
        {
            int value;
            uint uValue = GetIntU(offset, width);

            if ((uValue & SIGN[width - 1]) == 0) {
                value = (int)uValue;
            } else {
                if (width == 4) {
                    value = (int)uValue;
                } else {
                    value = (int)uValue - COMP[width - 1];
                }
            }
            return value;
        }

        /// <summary>
        /// float型実数値を取得する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <returns>数値を返す</returns>
        public float GetFloat(int offset)
        {
            byte[] bData = new byte[4];
            Array.Copy(this.Data, offset, bData, 0, 4);

            if (Endian == Endian.Big)
            {
                // ビッグエンディアンの場合はバイト順を反転
                Array.Reverse(bData);
            }
            float value = BitConverter.ToSingle(bData, 0);

            return value;
        }
        #endregion

        #region 内部処理

        // エンディアン
        Endian Endian;

        // 符号ビットのテーブル
        readonly uint[] SIGN = {
            0x80,
            0x8000,
            0x800000,
            0x80000000
        };

        // 補数換算用のテーブル
        readonly int[] COMP = {
            0x100,
            0x10000,
            0x1000000,
        };

        #endregion
    }
}
