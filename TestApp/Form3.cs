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
    // テスト3
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        // シリアルポート
        SerialPort serialPort;
        // シリアルパケット受信器
        SerialPacketReceiver receiver;

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
                PacketMode = PacketMode.Ascii, // アスキーモード
                StartCode = AsciiCode.STX, // 開始コード
                EndCode   = AsciiCode.ETX  // 終了コード
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
            var packet = new AsciiPacket(4);
            packet.SetHex(1, 2, val);

            // パケット送信
            serialPort.Send(packet);
        }

        // パケットを受信したとき
        private void Receiver_PacketReceived(object sender, EventArgs e)
        {
            while (true)
            {
                // パケットを取得
                var packet = receiver.GetAsciiPacket();
                if (packet == null) break;

                // パケットを解釈
                if (packet.GetHex(1, 2, out int val))
                {
                    // プログレスバーに表示
                    this.BeginInvoke((Action)(() => {
                        progressBar.SetValue(val);
                    }));
                }
            }
        }
    }
}
