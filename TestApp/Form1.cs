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
    // テスト1
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 開始処理
        private void Form_Load(object sender, EventArgs e)
        {
            // フォームのLoadイベントで開始処理を呼ぶ
            serialPortToolStrip.Begin(@"SETTING.INI", this.Text);
        }

        // 終了処理
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // フォームのFormClosingイベントで終了処理を呼ぶ
            serialPortToolStrip.End();
        }

        // シリアル通信データ受信イベント
        private void serialPortToolStrip_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // シリアルポート
            var serialPort = serialPortToolStrip.Port;
            //var serialPort = (SerialPort)sender; // こう書いても良い

            // 受信したデータを文字列として取得
            string str = serialPort.ReadExisting();

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
                // 入力された文字を送信
                serialPort.WriteChar(e.KeyChar); // 拡張メソッド
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
