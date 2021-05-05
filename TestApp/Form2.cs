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
    // テスト2
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        // 受信スレッド
        Thread threadRx;
        bool threadRxQuit;

        // 開始処理
        private void Form_Load(object sender, EventArgs e)
        {
            // フォームのLoadイベントで開始処理を呼ぶ
            serialPortToolStrip.Begin(@"SETTING.INI", this.Text);

            // シリアルポート
            var serialPort = serialPortToolStrip.Port;
            serialPort.NewLine = "\r";    // 改行コードはCRとする
            serialPort.ReadTimeout = 500; // 受信タイムアウト時間[ミリ秒]

            // 受信スレッド開始
            threadRxQuit = false;
            threadRx = new Thread(new ThreadStart(threadRxFunc));
            threadRx.Start();
        }

        // 終了処理
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 受信スレッド終了
            threadRxQuit = true;
            threadRx.Join();

            // フォームのFormClosingイベントで終了処理を呼ぶ
            serialPortToolStrip.End();
        }

        // 受信スレッド関数
        private void threadRxFunc()
        {
            // シリアルポート
            var serialPort = serialPortToolStrip.Port;

            while (!threadRxQuit)
            {
                if (serialPort.IsOpen)
                {
                    try {
                        // コマンドラインを受信
                        string command = serialPort.ReadLine();
                        // テキストボックスに表示
                        this.BeginInvoke((Action)(() => {
                            textBox2.Text += command + "\r\n"; // 取得した文字列に改行コードは含まれないことに注意
                        }));
                    } catch {
                        ; // 受信タイムアウト
                    }
                }
            }
        }

        // 送信ボタン
        private void buttonSend_Click(object sender, EventArgs e)
        {
            // シリアルポート
            var serialPort = serialPortToolStrip.Port;

            // コマンドラインを送信
            if(textBox1.Text.Length > 0)
            {
                string command = textBox1.Text + "\r"; // 改行コードはCRとする
                serialPort.Write(command);
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
