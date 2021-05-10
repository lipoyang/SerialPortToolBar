using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortToolBar
{
    /// <summary>
    /// CRC16計算機
    /// </summary>
    public class CRC16
    {
        // 初期値
        private readonly int Init;
        // 多項式値
        private readonly int Poly;
        // 出力XOR値
        private readonly int Xorout;
        // シフト方向
        private readonly ShiftDir Shift;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="init">初期値</param>
        /// <param name="poly">生成多項式値</param>
        /// <param name="shift">シフト方向</param>
        /// <param name="xorout">出力XOR値</param>
        public CRC16(int init, int poly, ShiftDir shift = ShiftDir.Left, int xorout = 0x0000)
        {
            this.Init = init;
            this.Poly = poly;
            this.Shift = shift;
            this.Xorout = xorout;
        }

        /// <summary>
        /// CRC16を計算する
        /// </summary>
        /// <param name="data">バイトデータ列</param>
        /// <param name="offset">開始位置</param>
        /// <param name="length">長さ(バイト数)</param>
        /// <returns>CRC16の値</returns>
        public int Get(byte[] data, int offset, int length)
        {
            int crc16;
            int i, j;

            crc16 = Init; // 初期値

            // 左シフト
            if(Shift == ShiftDir.Left)
            {
                for (i = 0; i < length; i++)
                {
                    crc16 ^= data[offset + i] << 8;
                    for (j = 0; j < 8; j++)
                    {
                        if ((crc16 & 0x8000) != 0)
                        {
                            crc16 = (crc16 << 1) ^ Poly; // 生成多項式
                        }
                        else
                        {
                            crc16 <<= 1;
                        }
                    }
                }
            }
            // 右シフト
            else
            {
                for (i = 0; i < length; i++)
                {
                    crc16 ^= data[offset + i];

                    for (j = 0; j < 8; j++)
                    {
                        if ((crc16 & 0x0001) != 0)
                        {
                            crc16 = (crc16 >> 1) ^ Poly; // 生成多項式
                        }
                        else
                        {
                            crc16 >>= 1;
                        }
                    }
                }
            }
            crc16 ^= Xorout; // 出力XOR
            crc16 &= 0xFFFF;
            return (int)crc16;
        }
    }

    /// <summary>
    /// CRC16の生成多項式を表す値
    /// </summary>
    public static class CRC16Poly
    {
        public const int CCITT = 0x1021;
        public const int IBM   = 0x8005;
    }

    /// <summary>
    /// CRC16のシフト演算の方向
    /// </summary>
    public enum ShiftDir
    {
        Left,
        Right
    }
}
