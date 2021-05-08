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
using SerialPortToolBar;

namespace TestApp
{
    // テスト4
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        // シリアルポート
        SerialPort serialPort;
        // シリアルパケット受信器
        SerialPacketReceiver receiver;
        // パケットヘッダ
        readonly byte[] header = new byte[] { 0xA5, 0x5A };

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
            // トラックバーの値をパケットで送信
            sendPacket(trackBar.Value);
        }

        // トラックバーの値が変化したとき
        private void trackBar_Scroll(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                // トラックバーの値をパケットで送信
                sendPacket(trackBar.Value);
            }
        }

        // パケット送信
        private void sendPacket(int val)
        {
            // パケット作成
            var packet = new BinaryPacket(6, header);
            packet.SetInt(2, 2, 2);   // Length
            packet.SetInt(4, 2, val); // データ
            // パケット送信
            serialPort.WriteBytes(packet.Data);
        }

        // パケットを受信したとき
        private void Receiver_PacketReceived(object sender, EventArgs e)
        {
            while (true)
            {
                // パケットを取得
                byte[] data = receiver.GetPacket();
                if (data == null) break;

                // パケットを解釈
                var packet = new BinaryPacket(data);
                int val = packet.GetInt(4, 2);

                // プログレスバーに表示
                this.BeginInvoke((Action)(() => {
                    progressBar.SetValue(val);
                }));
            }
        }
    }
}
