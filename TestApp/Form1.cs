using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports; // SerialPort
using SerialPortToolBar;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 開始処理
        private void Form1_Load(object sender, EventArgs e)
        {
            // フォームのLoadイベントで開始処理を呼ぶ
            serialPortToolStrip.Begin(@"SETTING.INI", this.Text);
        }

        // 終了処理
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // フォームのFormClosingイベントで終了処理を呼ぶ
            serialPortToolStrip.End();
        }

        // シリアル通信データ受信イベント
        private void serialPortToolStrip_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // 受信したデータを取得
            //var serialPort = (SerialPort)sender; // こう書いても同じ
            var serialPort = serialPortToolStrip.Port;
            int size = serialPort.BytesToRead;
            byte[] data = new byte[size];
            serialPort.Read(data, 0, size);
            string str = Encoding.ASCII.GetString(data);

            // テキストボックスに表示
            this.BeginInvoke((Action)(()=> {
                textBox2.Text += str;
            }));
        }

        // テキストボックスにキー入力されたとき
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            var serialPort = serialPortToolStrip.Port;
            if (serialPort.IsOpen)
            {
                char[] data = new char[] { e.KeyChar };
                serialPort.Write(data, 0, 1);
            }
        }

        // クリアボタン
        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }
    }
}
