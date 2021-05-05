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
    // テスト5
    public partial class Form5A : Form
    {
        public Form5A()
        {
            InitializeComponent();
        }

        // シリアルポート
        SerialPort serialPort;
        // シリアルパケット受信器
        SerialPacketReceiver receiver;

        // 送信パケット数
        int sendPackNum = 0;
        // 正常応答の数
        int recvAckNum = 0;
        // 異常応答の数
        int recvNakNum = 0;
        // 無応答の数
        int recvNoneNum = 0;

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

            // パケット数カウンタ表示
            updateCounter();
        }

        // 終了処理
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // フォームのFormClosingイベントで終了処理を呼ぶ
            serialPortToolStrip.End();
        }

        // シリアルポートが開いたとき
        private void serialPortToolStrip_Opened(object sender, EventArgs e)
        {
            sendPackNum = 0;
            recvAckNum  = 0;
            recvNakNum  = 0;
            recvNoneNum = 0;
            updateCounter();

            // トラックバーの値をパケットで送信
            sendPacket(trackBar.Value);
        }

        // トラックバーの値が変化したとき
        private void trackBar1_Scroll(object sender, EventArgs e)
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
            string hex = val.ToString("X2");
            byte[] hexbyte = Encoding.ASCII.GetBytes(hex);
            byte[] packet = new byte[4];
            packet[0] = AsciiCode.STX;
            packet[1] = hexbyte[0];
            packet[2] = hexbyte[1];
            packet[3] = AsciiCode.ETX;

            sendPackNum++;
            textBox1.Text = sendPackNum.ToString();

            //serialPort.WriteBytes(packet);

            // パケット送信・応答確認のタスク
            Task task = Task.Run(() =>
            {
                // パケット送信
                serialPort.WriteBytes(packet);

                // パケット受信
                byte[] resPacket = receiver.WaitPacket(1000); // TODO
                // 応答はあったか？
                if (resPacket != null) {
                    // ACK応答か？
                    if (resPacket[1] == AsciiCode.ACK) {
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
            });

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
