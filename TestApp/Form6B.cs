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
    public partial class Form6B : Form
    {
        public Form6B()
        {
            InitializeComponent();
        }

        // シリアルポート
        SerialPort serialPort;
        // シリアルパケット受信器
        SerialPacketReceiver receiver;
        // パケットヘッダ
        readonly byte[] header = new byte[] { 0xA5, 0x5A };
        
        // 受信パケット数
        int recvPackNum = 0;
        // 正常応答の数
        int sendAckNum = 0;
        // 異常応答の数
        int sendNakNum = 0;

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
            receiver.PacketReceived += Receiver_PacketReceived; // パケット受信ハンドラ
            receiver.Start(); // パケット受信スレッド開始
            
            // パケット数カウンタ表示
            updateCounter();
        }

        // 終了処理
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            receiver.Stop(); // パケット受信スレッド終了

            // フォームのFormClosingイベントで終了処理を呼ぶ
            serialPortToolStrip.End();
        }

        // シリアルポートが開いたとき
        private void serialPortToolStrip_Opened(object sender, EventArgs e)
        {
            recvPackNum = 0;
            sendAckNum = 0;
            sendNakNum = 0;
            updateCounter();
        }

        // ACK送信
        private void sendAck()
        {
            // パケット作成
            byte[] packet = new byte[6];
            Array.Copy(header, packet, 2);
            packet[2] = 0x00; // Length 上位バイト
            packet[3] = 0x02; // Length 下位バイト
            packet[4] = AsciiCode.ACK;
            packet[5] = (byte)(~packet[4]);
            // パケット送信
            serialPort.WriteBytes(packet);

            sendAckNum++;
        }

        // NAK送信
        private void sendNak()
        {
            // パケット作成
            byte[] packet = new byte[6];
            Array.Copy(header, packet, 2);
            packet[2] = 0x00; // Length 上位バイト
            packet[3] = 0x02; // Length 下位バイト
            packet[4] = AsciiCode.NAK;
            packet[5] = (byte)(~packet[4]);
            // パケット送信
            serialPort.WriteBytes(packet);

            sendNakNum++;
        }

        // パケットを受信したとき
        private void Receiver_PacketReceived(object sender, EventArgs e)
        {
            while (true)
            {
                // パケットを取得
                byte[] packet = receiver.GetPacket();
                if (packet == null) break;
                recvPackNum++;

                // パケットを解釈
                bool ack = true;
                byte val = packet[4];
                byte ival = packet[5];
                if((byte)~val == ival){
                    if(val > 100){
                        ack = false;
                    }
                }else{
                    ack = false;
                }
                // ACK応答 or NAK応答
                if (ack) {
                    sendAck();
                } else {
                    sendNak();
                }
                // 表示更新
                this.BeginInvoke((Action)(() => {
                    if (ack)
                    {
                        progressBar.SetValue(val);
                    }
                    updateCounter();
                }));
            }
        }

        // パケット数表示更新
        private void updateCounter()
        {
            textBox1.Text = recvPackNum.ToString();
            textBox2.Text = sendAckNum.ToString();
            textBox3.Text = sendNakNum.ToString();
        }
    }
}
