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

        #endregion

    }
}
