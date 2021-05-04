using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading; // CountdownEvent 
using System.IO.Ports; // SerialPort

namespace SerialPortToolBar
{
    /// <summary>
    /// シリアル通信のパケット受信器
    /// </summary>
    public class SerialPacketReceiver
    {
        #region イベント

        /// <summary>
        /// パケット受信時のイベント。 
        /// イベントドリブンで受信する場合に設定する。WaitPacket()を用いる場合は設定しない。
        /// </summary>
        public event EventHandler PacketReceived = null;

        #endregion

        #region 公開フィールド (通信開始後は変更しないこと)

        /// <summary>
        /// 受信ポーリング周期[ミリ秒]
        /// </summary>
        public int PollingInterval = 20;

        /// <summary>
        /// パケットモード。バイナリー形式かアスキー形式か。
        /// </summary>
        public PacketMode PacketMode = PacketMode.Ascii;

        /// <summary>
        /// パケットタイムアウト時間[ミリ秒]。
        /// パケット先頭バイト受信からこの時間を経過すると受信中のデータは破棄されます。
        /// </summary>
        public int TimeOut = 500;

        /// <summary>
        /// パケット開始コード (アスキーモード用)
        /// </summary>
        public byte StartCode = AsciiCode.STX;
        /// <summary>
        /// パケット終了コード (アスキーモード用)
        /// </summary>
        public byte EndCode = AsciiCode.ETX;

        /// <summary>
        /// パケット開始ヘッダ (バイナリーモード用)
        /// </summary>
        public byte[] Header = null;

        /// <summary> 
        /// バイナリーモードのパケット長指定子(Length)の先頭位置 (パケット先頭を0とする)
        /// </summary>
        public int LengthOffset = 1;
        /// <summary>
        /// バイナリーモードのパケット長指定子(Length)のバイト幅 (1または2)
        /// </summary>
        public int LengthWidth = 1;
        /// <summary>
        /// バイナリーモードのパケット長指定子(Length)のエンディアン(バイト幅が2のとき)
        /// </summary>
        public Endian LengthEndian = Endian.BigEndian;
        /// <summary>
        /// バイナリーモードのパケット長指定子(Length)の追加値。
        /// (パケットの全バイト数 - Length)の値を指定する。
        /// </summary>
        public int LengthExtra = 1;

        #endregion

        #region 公開メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="port">シリアルポート</param>
        /// <param name="maxSize">パケットの最大バイト数</param>
        public SerialPacketReceiver(SerialPort port, int maxSize = 256)
        {
            // パケットの最大バイト数と受信バッファ
            this.packetMaxSize = maxSize;
            this.rxBuff = new byte[maxSize];
            // シリアルポートの受信設定
            this.serialPort = port;
            // SerialPortのeventは使わない
            //this.serialPort.DataReceived += serialPort_DataReceived;
        }

        /// <summary>
        /// パケット受信を待って取得する (ブロッキング)。タイムアウトした場合はnullを返す。
        /// </summary>
        /// <remarks>
        /// 同期処理で使用する。受信スレッド(Start()/Stop()/GetPacket())の利用とは排他。
        /// </remarks>
        /// <param name="timeout">タイムアウト時間[ミリ秒]</param>
        /// <returns>
        /// 受信したパケットのバイト配列 
        /// (開始コード/終了コード、ヘッダなども含む全データ)
        /// </returns>
        public byte[] WaitPacket(int timeout)
        {
            DateTime startTime = DateTime.Now;
            rxState = RxState.Ready;
            
            // 受信パケットのキューをいったん破棄
            rxPackets.Clear(); 
            
            // パケット受信かタイムアウトまで
            while (true)
            {
                // パケット受信処理
                receivePacket();
                // 受信パケットがあれば返す
                if(rxPackets.Count > 0)
                {
                    byte[] packet = rxPackets.Dequeue();
                    return packet;
                }

                // タイムアウト判定
                DateTime endTime = DateTime.Now;
                TimeSpan ts = endTime - startTime;
                int elasped = ts.Milliseconds;
                if (elasped >= timeout) break;

                Thread.Sleep(PollingInterval);
            }
            return null;
        }

        /// <summary>
        /// 受信したパケットを取得する (ノンブロッキング)。未受信の場合はnullを返す。
        /// </summary>
        /// <remarks>
        /// Start()で受信スレッドを開始しておき、PacketReceivedイベントでこのメソッドを呼ぶ。
        /// </remarks>
        /// <returns>
        /// 受信したパケットのバイト配列 
        /// (開始コード/終了コード、ヘッダなども含む全データ)
        /// </returns>
        public byte[] GetPacket()
        {
            if(rxPackets.Count > 0)
            {
                byte[] packet = rxPackets.Dequeue();
                return packet;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// パケット受信スレッドを開始する。
        /// パケットを受信したらPacketReceivedイベントを発行する
        /// </summary>
        public void Start()
        {
            // 受信スレッドを開始
            rxState = RxState.Ready;
            rxPackets.Clear();
            threadRxQuit = false;
            threadRx = new Thread(new ThreadStart(threadRxFunc));
            threadRx.Start();
        }

        /// <summary>
        /// パケット受信スレッドを停止する。
        /// フォーム/アプリ終了時までに停止しないとスレッドがゾンビ化する。
        /// </summary>
        public void Stop()
        {
            // スレッドの終了待ち合わせ
            threadRxQuit = true;
            threadRx.Join();
        }

        #endregion

        #region 内部処理

        // シリアルポート
        private readonly SerialPort serialPort;
        // 受信パケットの最大バイト数
        readonly int packetMaxSize;
        // 受信バッファ
        readonly byte[] rxBuff;
        // 受信インデックス
        int rxIndex;
        // 受信パケット長 (バイナリーモード)
        int rxLength;
        // 受信状態
        enum RxState
        {
            Ready,      // 受信待ち
            Receiving,  // 受信中 (アスキーモード)
            WaitHeader, // ヘッダ受信待ち (バイナリーモード)
            WaitLength, // Length受信待ち (バイナリーモード)
            WaitData    // 残りのデータ受信待ち (バイナリーモード)
        }
        RxState rxState = RxState.Ready;
        // 受信スレッド
        Thread threadRx;
        // 受信スレッド終了フラグ
        bool threadRxQuit = false;
        // 受信パケットのキュー (受信スレッド/GetPacket()用)
        readonly Queue<byte[]> rxPackets = new Queue<byte[]>();
        // パケット受信開始時刻
        DateTime rxStartTime;

        // 受信スレッド関数
        private void threadRxFunc()
        {
            while (!threadRxQuit)
            {
                if (serialPort.IsOpen)
                {
                    // パケット受信処理
                    receivePacket();
                    // パケット受信があればイベント発行
                    if (rxPackets.Count > 0)
                    {
                        if(PacketReceived != null) {
                            PacketReceived.Invoke(this, EventArgs.Empty);
                        } else {
                            rxPackets.Clear(); // イベントハンドラが無いならキューを破棄
                        }
                    }
                }
                Thread.Sleep(PollingInterval);
            }
        }

        // シリアルパケット受信処理
        private void receivePacket()
        {
            // タイムアウト判定
            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime - rxStartTime;
            int elasped = ts.Milliseconds;
            if (elasped >= this.TimeOut) {
                rxState = RxState.Ready;
            }
            // 受信データの取得
            int size = serialPort.BytesToRead;
            byte[] data = new byte[size];
            int readSize = serialPort.Read(data, 0, size);
            if (readSize == 0) return;

            // アスキーモードの場合
            if(PacketMode == PacketMode.Ascii)
            {
                receivePacketAscii(data);
            }
            // バイナリーモードの場合
            else
            {
                receivePacketBinary(data);
            }
        }

        // シリアルパケット受信処理(アスキーモード)
        // data:受信したデータ
        private void receivePacketAscii(byte[] data)
        {
            foreach (byte c in data)
            {
                switch (rxState)
                {
                    // パケット待ち状態
                    case RxState.Ready:
                        // 開始コードが来たら受信中状態へ
                        if (c == StartCode)
                        {
                            rxStartTime = DateTime.Now;
                            rxState = RxState.Receiving;
                            rxBuff[0] = c;
                            rxIndex = 1;
                        }
                        break;
                    // パケット受信中状態
                    case RxState.Receiving:
                        // もしも開始コードが来たら受信中のデータを破棄してやり直し
                        if (c == StartCode)
                        {
                            rxStartTime = DateTime.Now;
                            rxIndex = 1;
                        }
                        // 終了コードが来たら、受信したパケットを返す
                        else if (c == EndCode)
                        {
                            rxBuff[rxIndex] = c;
                            rxIndex++;
                            rxState = RxState.Ready;

                            // 受信パケットをキューに入れる
                            byte[] packet = new byte[rxIndex];
                            Array.Copy(rxBuff, packet, rxIndex);
                            rxPackets.Enqueue(packet);
                        }
                        // 1文字格納
                        else
                        {
                            rxBuff[rxIndex] = c;
                            rxIndex++;
                            if (rxIndex >= packetMaxSize)
                            {
                                rxState = RxState.Ready;
                            }
                        }
                        break;
                    default:
                        rxState = RxState.Ready;
                        break;
                }
            }
        }

        // シリアルパケット受信処理(バイナリーモード)
        // data:受信したデータ
        private void receivePacketBinary(byte[] data)
        {
            foreach (byte d in data)
            {
                switch (rxState)
                {
                    // パケット待ち状態
                    case RxState.Ready:
                        // ヘッダの先頭バイトか？
                        if (d == Header[0])
                        {
                            rxStartTime = DateTime.Now;
                            rxBuff[0] = d;
                            rxIndex = 1;

                            if (Header.Length == 1) {
                                // 1バイトのヘッダならLength待ち状態へ
                                rxState = RxState.WaitLength;
                            } else {
                                // 多バイトのヘッダならヘッダ待ち状態へ
                                rxState = RxState.WaitHeader;
                            }
                        }
                        break;
                    // ヘッダ待ち状態
                    case RxState.WaitHeader:
                        // ヘッダと一致するか？
                        if (d == Header[rxIndex])
                        {
                            rxBuff[rxIndex] = d;
                            rxIndex++;
                            if (rxIndex == Header.Length)
                            {
                                rxState = RxState.WaitLength;
                            }
                        }
                        else
                        {
                            rxState = RxState.Ready;
                        }
                        break;
                    // Lengh待ち状態
                    case RxState.WaitLength:
                        // 受信データを格納
                        rxBuff[rxIndex] = d;
                        rxIndex++;
                        // Lengthデータが来たか？
                        if(rxIndex == LengthOffset + LengthWidth)
                        {
                            // パケット長を計算
                            rxLength = rxLengthCalc();
                            if(rxLength <= packetMaxSize) {
                                rxState = RxState.WaitData;
                            } else {
                                rxState = RxState.Ready;
                            }
                        }
                        break;
                    // 残りのデータ受信待ち (バイナリーモード)
                    case RxState.WaitData:
                        // 受信データを格納
                        rxBuff[rxIndex] = d;
                        rxIndex++;

                        // 全パケットの受信が完了たら、受信したパケットを返す
                        if (rxIndex == rxLength)
                        {
                            rxState = RxState.Ready;

                            // 受信パケットをキューに入れる
                            byte[] packet = new byte[rxLength];
                            Array.Copy(rxBuff, packet, rxLength);
                            rxPackets.Enqueue(packet);
                        }
                        else if (rxIndex >= packetMaxSize)
                        {
                            rxState = RxState.Ready;
                        }
                        break;
                    default:
                        rxState = RxState.Ready;
                        break;
                }
            }
        }

        // パケット長計算
        private int rxLengthCalc()
        {
            int len;

            // パケット長指定子(Length)が1バイト幅の場合
            if(LengthWidth == 1)
            {
                len = rxBuff[LengthOffset] + LengthExtra;
            }
            // パケット長指定子(Length)が2バイト幅の場合
            else
            {
                if(LengthEndian == Endian.BigEndian)
                {
                    len = ((int)rxBuff[LengthOffset] << 8) |
                          ((int)rxBuff[LengthOffset + 1]);
                    len += LengthExtra;
                }
                else
                {
                    len = ((int)rxBuff[LengthOffset + 1] << 8) |
                          ((int)rxBuff[LengthOffset]);
                    len += LengthExtra;
                }
            }
            return len;
        }

        #endregion
    }

    #region 定数定義

    /// <summary>
    /// シリアルパケットのモード
    /// </summary>
    public enum PacketMode
    {
        /// <summary>
        /// アスキーモード。
        /// キャラクタデータ形式のパケットを受信します。
        /// </summary>
        Ascii,
        /// <summary>
        /// バイナリーモード。
        /// バイナリーデータ形式のパケットを受信します。
        /// </summary>
        Binary
    }

    /// <summary>
    /// エンディアン
    /// </summary>
    public enum Endian
    {
        LittleEndian,
        BigEndian
    }

    /// <summary>
    /// アスキー制御キャラクタコード
    /// </summary>
    public class AsciiCode
    {
        public const byte NULL = 0x00;
        public const byte SOH = 0x01;
        public const byte STX = 0x02;
        public const byte ETX = 0x03;
        public const byte EOT = 0x04;
        public const byte ENG = 0x05;
        public const byte ACK = 0x06;
        public const byte BEL = 0x07;
        public const byte BS = 0x08;
        public const byte HT = 0x09;
        public const byte LF = 0x0A;
        public const byte VT = 0x0B;
        public const byte FF = 0x0C;
        public const byte CR = 0x0D;
        public const byte SO = 0x0E;
        public const byte SI = 0x0F;
        public const byte DLE = 0x10;
        public const byte DC1 = 0x11;
        public const byte DC2 = 0x12;
        public const byte DC3 = 0x13;
        public const byte DC4 = 0x14;
        public const byte NAK = 0x15;
        public const byte SYN = 0x16;
        public const byte ETB = 0x17;
        public const byte CAN = 0x18;
        public const byte EM = 0x19;
        public const byte SUB = 0x1A;
        public const byte ESC = 0x1B;
        public const byte FS = 0x1C;
        public const byte GS = 0x1D;
        public const byte RS = 0x1E;
        public const byte US = 0x1F;
        public const byte DEL = 0x7F;
    }
    #endregion
}
