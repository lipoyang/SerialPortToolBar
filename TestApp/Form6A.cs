using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports; // SerialPort
using System.Globalization; // NumberStyles
using SerialPortToolBar;

namespace TestApp
{
    // テスト6
    public partial class Form6A : Form
    {
        public Form6A()
        {
            InitializeComponent();
        }

        // シリアルポート
        SerialPort serialPort;
        // シリアルパケット受信器
        SerialPacketReceiver receiver;
        // パケットヘッダ
        readonly byte[] header = new byte[] { 0xA5, 0x5A };
        
        // 送信パケット数
        int sendPackNum = 0;
        // 正常応答の数
        int recvAckNum = 0;
        // 異常応答の数
        int recvNakNum = 0;
        // 無応答の数
        int recvNoneNum = 0;

        // 対話通信スレッド
        Thread threadInterComm;
        bool threadInterCommQuit;
        // 送信データのキュー
        readonly Queue<int> sendQueue = new Queue<int>();

        // 開始処理
        private void Form_Load(object sender, EventArgs e)
        {
            // フォームのLoadイベントで開始処理を呼ぶ
            serialPortToolStrip.Begin(@"SETTING.INI", this.Text);

            // シリアルポート
            serialPort = serialPortToolStrip.Port;
            // シリアルパケット受信器の設定
            receiver = new SerialPacketReceiver(serialPort)
            {
                PacketMode = PacketMode.Binary, // バイナリーモード
                Header = header,  // パケットヘッダ
                LengthOffset = 2, // パケット長指定子の開始位置
                LengthWidth  = 2, // パケット長指定子のバイト幅
                LengthExtra  = 4, // パケット長指定に加算する値 (全バイト数算出のため)
                LengthEndian = Endian.BigEndian, // パケット長指定子のエンディアン
                TimeOut = 500 // タイムアウト時間[ミリ秒]
            };

            // パケット数カウンタ表示
            updateCounter();

            // 対話通信スレッド開始
            threadInterCommQuit = false;
            threadInterComm = new Thread(new ThreadStart(() => {
                threadInterCommFunc();
            }));
            threadInterComm.Start();
        }

        // 終了処理
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 対話通信スレッド開始
            threadInterCommQuit = true;
            threadInterComm.Join();

            // フォームのFormClosingイベントで終了処理を呼ぶ
            serialPortToolStrip.End();
        }

        // 対話通信スレッドの関数
        private void threadInterCommFunc()
        {
            while (!threadInterCommQuit)
            {
                if(serialPort.IsOpen)
                {
                    if(sendQueue.Count > 0)
                    {
                        // トラックバーを速く動かすと送信データのキューが詰まるので
                        // 最後の値だけ取ってキューをクリア
                        int val = sendQueue.Last();
                        sendQueue.Clear();

                        // パケット送信/応答待ち
                        sendPacketWaitResponse(val);
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        // シリアルポートが開いたとき
        private void serialPortToolStrip_Opened(object sender, EventArgs e)
        {
            sendPackNum = 0;
            recvAckNum = 0;
            recvNakNum = 0;
            recvNoneNum = 0;
            updateCounter();

            // 送信データのキューに入れる
            sendQueue.Enqueue(trackBar.Value);
        }

        // トラックバーの値が変化したとき
        private void trackBar_Scroll(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                // 送信データのキューに入れる
                sendQueue.Enqueue(trackBar.Value);
            }
        }

        // パケット送信/応答待ち
        private void sendPacketWaitResponse(int val)
        {
            // パケット作成
            byte[] packet = new byte[6];
            Array.Copy(header, packet, 2);
            packet[2] = 0x00; // Length 上位バイト
            packet[3] = 0x02; // Length 下位バイト
            packet[4] = (byte)val;    // データ
            packet[5] = (byte)(~val); // データ反転

            sendPackNum++;
            this.BeginInvoke((Action)(() => {
                textBox1.Text = sendPackNum.ToString();
            }));

            // パケット送信
            serialPort.WriteBytes(packet);

            // パケット受信
            byte[] resPacket = receiver.WaitPacket(1000); // TODO
            // 応答はあったか？
            if (resPacket != null) {
                // ACK応答か？
                byte res = resPacket[4];
                byte ires = resPacket[5];
                if(((byte)~res == ires) && (res == AsciiCode.ACK)) {
                    recvAckNum++;
                } else {
                    recvNakNum++;
                }
            } else {
                recvNoneNum++;
            }
            // 表示更新
            this.BeginInvoke((Action)(() => {
                updateCounter();
            }));
        }

        // パケット数表示更新
        private void updateCounter()
        {
            textBox1.Text = sendPackNum.ToString();
            textBox2.Text = recvAckNum.ToString();
            textBox3.Text = recvNakNum.ToString();
            textBox4.Text = recvNoneNum.ToString();
        }
    }
}
