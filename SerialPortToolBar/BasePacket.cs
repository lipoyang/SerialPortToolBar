using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortToolBar
{
    /// <summary>
    /// アスキー形式/バイナリー形式のパケットの基底クラス
    /// </summary>
    public class BasePacket
    {
        #region 公開フィールド

        /// <summary>
        /// パケットのバイト配列データ(開始コード/終了コードを含む)
        /// </summary>
        public byte[] Data;

        #endregion

        #region 公開メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BasePacket() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data">パケットのバイト配列データ</param>
        public BasePacket(byte[] data)
        {
            Data = data;
        }

        /// <summary>
        /// パケットデータを文字列に変換する
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return Encoding.ASCII.GetString(Data);
        }

        /// <summary>
        /// 1バイトのデータ(制御コードなど)を格納する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="value">1バイトのデータ</param>
        public void SetByte(int offset, byte value)
        {
            Data[offset] = value;
        }

        /// <summary>
        /// 1文字のアスキー文字を格納する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="value">1文字のデータ</param>
        public void SetChar(int offset, char value)
        {
            Data[offset] = (byte)value;
        }

        /// <summary>
        /// 文字列を格納する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="stringData">文字列データ</param>
        public void SetString(int offset, string stringData)
        {
            byte[] bData = Encoding.ASCII.GetBytes(stringData);
            Array.Copy(bData, 0, this.Data, offset, bData.Length);
        }

        /// <summary>
        /// 1バイトのデータ(制御コードなど)を取得する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <returns>1バイトのデータ</returns>
        public byte GetByte(int offset)
        {
            return Data[offset];
        }

        /// <summary>
        /// 1文字のアスキー文字を取得する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <returns>1文字のデータ</returns>
        public char GetChar(int offset)
        {
            return (char)Data[offset];
        }

        /// <summary>
        /// 文字列を取得する
        /// </summary>
        /// <param name="offset">位置</param>
        /// <param name="value">文字列データ</param>
        public string GetString(int offset, int length)
        {
            byte[] bData = new byte[length];
            Array.Copy(this.Data, offset, bData, 0, length);
            string strData = Encoding.ASCII.GetString(bData);
            return strData;
        }

        /// <summary>
        /// 算術加算によるチェックサム値を計算する
        /// </summary>
        /// <param name="start">開始位置</param>
        /// <param name="length">バイト数</param>
        /// <returns>チェックサム値</returns>
        public byte Sum(int start, int length)
        {
            byte sum = 0;

            for(int i = 0; i < length; i++)
            {
                sum += this.Data[start + i];
            }
            return sum;
        }

        /// <summary>
        /// 算術加算によるチェックサム値を計算して指定位置に格納する
        /// </summary>
        /// <param name="offset">格納位置</param>
        /// <param name="start">開始位置</param>
        /// <param name="length">バイト数</param>
        public void SetSum(int offset, int start, int length)
        {
            byte sum = this.Sum(start, length);
            this.SetByte(offset, sum);
        }

        /// <summary>
        /// 算術加算によるチェックサム値を計算して指定位置の値と比較する
        /// </summary>
        /// <param name="offset">格納位置</param>
        /// <param name="start">開始位置</param>
        /// <param name="length">バイト数</param>
        /// <returns>一致すればtrue</returns>
        public bool CheckSum(int offset, int start, int length)
        {
            byte sum = this.Sum(start, length);
            byte val = this.GetByte(offset);
            return (sum == val);
        }

        /// <summary>
        /// 排他的論理和によるチェックサム値を計算する
        /// </summary>
        /// <param name="start">開始位置</param>
        /// <param name="length">バイト数</param>
        /// <returns>チェックサム値</returns>
        public byte Xor(int start, int length)
        {
            byte xor = 0;

            for (int i = 0; i < length; i++)
            {
                xor ^= this.Data[start + i];
            }
            return xor;
        }

        /// <summary>
        /// 排他的論理和によるチェックサム値を計算して指定位置に格納する
        /// </summary>
        /// <param name="offset">格納位置</param>
        /// <param name="start">開始位置</param>
        /// <param name="length">バイト数</param>
        public void SetXor(int offset, int start, int length)
        {
            byte xor = this.Xor(start, length);
            this.SetByte(offset, xor);
        }

        /// <summary>
        /// 排他的論理和によるチェックサム値を計算して指定位置の値と比較する
        /// </summary>
        /// <param name="offset">格納位置</param>
        /// <param name="start">開始位置</param>
        /// <param name="length">バイト数</param>
        /// <returns>一致すればtrue</returns>
        public bool CheckXor(int offset, int start, int length)
        {
            byte xor = this.Xor(start, length);
            byte val = this.GetByte(offset);
            return (xor == val);
        }

        #endregion

    }
}
