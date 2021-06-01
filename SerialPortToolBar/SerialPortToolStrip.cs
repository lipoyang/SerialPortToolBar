using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO.Ports; // SerialPort
using System.Drawing; // Size
using IniFileSharp;

namespace SerialPortToolBar
{
    /// <summary>
    /// シリアルポート通信ツールバーのToolStrip
    /// </summary>
    [DefaultEvent("DataReceived")]
    public class SerialPortToolStrip : ToolStrip
    {
        #region イベント

        /// <summary>
        /// シリアルポートを開いた時のイベント
        /// </summary>
        [Browsable(true)]
        [Category("拡張機能")]
        [Description("シリアルポートを開いたときに発生します。")]
        public event EventHandler Opened = null;

        /// <summary>
        /// シリアルポートを閉じた時のイベント
        /// </summary>
        [Browsable(true)]
        [Category("拡張機能")]
        [Description("シリアルポートを閉じたときに発生します。")]
        public event EventHandler Closed = null;

        /// <summary>
        /// デバイスが切断された時のイベント
        /// </summary>
        [Browsable(true)]
        [Category("拡張機能")]
        [Description("デバイスが切断されたときに発生します。")]
        public event EventHandler Disconnected = null;

        /// <summary>
        /// シリアルポートのデータ受信時のイベント
        /// </summary>
        [Browsable(true)]
        [Category("拡張機能")]
        [Description("シリアルポートがデータを受信したときに発生します。")]
        public event SerialDataReceivedEventHandler DataReceived
        {
            add => serialPort.DataReceived += value;
            remove => serialPort.DataReceived -= value;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// シリアルポート
        /// </summary>
        public SerialPort Port { get => serialPort; }

        /// <summary>
        /// 半二重通信か？(RS-485用)
        /// </summary>
        //public bool HalfDuplex = false;

        #endregion

        #region 公開メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SerialPortToolStrip()
        {
            // コンポーネントの初期化
            InitializeComponent();
            
            // シリアルポートのデフォルト設定
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.None;
            //serialPort.Encoding = Encoding.UTF8;
            //serialPort.NewLine = "\r";

            // 表示状態の初期化
            listBaudRate.Enabled = true;
            listComPort.Enabled = true;
            buttonConnect.Enabled = true;
            buttonDisconnect.Enabled = false;
        }

        /// <summary>
        /// 初期化処理。フォームの開始時(Loadイベント)に呼んでください。
        /// </summary>
        /// <param name="iniFileName">設定INIファイルのパス</param>
        /// <param name="section">設定INIファイルのセクション名</param>
        public void Begin(string iniFileName = @".\SETTING.INI", string section = "COM_PORT")
        {
            // COMポートとボーレートの既定値を設定ファイルから読み出し
            iniFile = new IniFile(iniFileName);
            iniSection = section;
            defaultPortName = iniFile.ReadString(iniSection, "PORT_NAME", "COM3");
            defaultBaudRate = iniFile.ReadInteger(iniSection, "BAUD_RATE", 9600);

            // COMポートリストの更新
            updateComPortList();
            // ボーレートリストの初期化
            initBaudRateList();

            // 周期的にCOMポートリストを更新
            timerListUpdate.Interval = 2000;
            timerListUpdate.Tick += (sender, e) => {
                // COMポートリストの更新
                updateComPortList();
                // デバイス切断をチェック
                checkDisconnect();
            };
            timerListUpdate.Start();
        }

        /// <summary>
        /// 終了処理。フォームの終了時(FormClosingイベント)に呼んでください。
        /// </summary>
        public void End()
        {
            // シリアルポートが開いていたら閉じる
            try{
                if (serialPort.IsOpen)
                {
                    // if (HalfDuplex) serialPort.ClearHalfDuplex();
                    serialPort.Close();
                }
            } catch {
                ;
            }

            // COMポートリスト更新用タイマを停止
            timerListUpdate.Stop();

            // COMポートとボーレートの既定値を設定ファイルに保存
            iniFile.WriteString(iniSection, "PORT_NAME", defaultPortName);
            iniFile.WriteInteger(iniSection, "BAUD_RATE", defaultBaudRate);
        }

        /// <summary>
        /// シリアルポートを開く
        /// </summary>
        /// <returns>成否</returns>
        public bool Open()
        {
            this.Invoke((Action)(()=> {
                buttonConnect.PerformClick();
            }));
            return Port.IsOpen;
        }

        /// <summary>
        /// シリアルポートを閉じる
        /// </summary>
        public void Close()
        {
            this.Invoke((Action)(() => {
                buttonDisconnect.PerformClick();
            }));
        }

        #endregion

        #region 内部処理

        // 設定ファイル
        IniFile iniFile;
        // 設定ファイルのセクション名
        string iniSection;

        // デフォルトのCOMポート名とボーレート
        string defaultPortName = "COM3";
        int defaultBaudRate = 9600;

        // COMポートが無い場合の表示
        const string NO_COM_PORT = "ありません";

        // COMポートを開いたか
        bool isOpen = false;

        // COMポートリストのドロップダウン時の処理
        private void listComPort_DropDown(object sender, EventArgs e)
        {
            // COMポートリストの更新
            updateComPortList();
        }

        // 接続ボタンクリック時の処理
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            // 念のため、いったんCOMポートを閉じる
            if (serialPort.IsOpen) serialPort.Close();

            // COMポート名のチェック
            string portName = listComPort.Text;
            if (portName == "")
            {
                showErrorMessage("COMポートを選択してください");
                return;
            }
            else if (portName == NO_COM_PORT)
            {
                showErrorMessage("COMポートがありません");
                return;
            }
            // ボーレートのチェック
            if(!int.TryParse(listBaudRate.Text, out int baudRate))
            {
                showErrorMessage("ボーレートが不正です");
                return;
            }
            // COMポートを開く
            if (!openComPort(portName, baudRate))
            {
                showErrorMessage("COMポートが開けません");
                return;
            }
            // COMポートとボーレートの既定値を更新 (ここで毎回ファイル保存はしない)
            defaultPortName = portName;
            defaultBaudRate = baudRate;

            listBaudRate.Enabled = false;
            listComPort.Enabled = false;
            buttonConnect.Enabled = false;
            buttonDisconnect.Enabled = true;

            this.Update(); // 受信が始まるので念のために強制的に表示更新
        }

        // 切断ボタンクリック時の処理
        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            // COMポートを閉じる
            closeComPort();
            
            listBaudRate.Enabled = true;
            listComPort.Enabled = true;
            buttonConnect.Enabled = true;
            buttonDisconnect.Enabled = false;
        }

        // エラーメッセージの表示
        private void showErrorMessage(string text)
        {
            MessageBox.Show(
                text,
                "エラー", 
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
                );
        }

        // COMポートリストの更新
        private void updateComPortList()
        {
            // COMポートを列挙
            string[] ports = SerialPort.GetPortNames();
            if(ports.Length == 0)
            {
                ports = new string[]{ NO_COM_PORT };
            }
            // 変化があったか？
            bool changed = false;
            if(ports.Length != listComPort.Items.Count)
            {
                changed = true;
            }
            else
            {
                for(int i = 0; i < ports.Length; i++)
                {
                    if(ports[i] != (string)listComPort.Items[i])
                    {
                        changed = true;
                    }
                }
            }
            // 初回か変化があったとき
            if (changed)
            {
                // COMポートリストの更新
                listComPort.Items.Clear();
                listComPort.Items.AddRange(ports);

                // 既定値を選択
                listComPort.SelectedItem = defaultPortName;
                // 無ければ0番目の項目を選択
                if ((string)listComPort.SelectedItem != defaultPortName)
                {
                    listComPort.SelectedIndex = 0;
                }
            }
        }

        // ボーレートリストの初期化
        private void initBaudRateList()
        {
            // ボーレートのリストを作成
            int[] baudRates = { 9600, 19200, 38400, 57600, 115200 };
            listBaudRate.Items.Clear();
            //listBaudRate.Items.AddRange(baudRates); // int[] は object[] ではないので不可
            foreach (int val in baudRates) listBaudRate.Items.Add(val);

            // 既定値を選択 (無ければ追加して選択)
            listBaudRate.SelectedItem = defaultBaudRate;
            if((int)listBaudRate.SelectedItem != defaultBaudRate)
            {
                listBaudRate.Items.Add(defaultBaudRate);
                listBaudRate.SelectedItem = defaultBaudRate;
            }
        }

        // デバイス切断をチェック
        private void checkDisconnect()
        {
            if (isOpen)
            {
                if (!Port.IsOpen)
                {
                    isOpen = false;
                    Disconnected?.Invoke(this, EventArgs.Empty);
                    buttonDisconnect.PerformClick();
                }
            }
        }

        // COMポートを開く
        private bool openComPort(string portName, int baudRate)
        {
            try
            {
                // COMポートを開く
                serialPort.PortName = portName;
                serialPort.BaudRate = baudRate;
                serialPort.Open();
                serialPort.DiscardInBuffer(); // 受信バッファをフラッシュ
                // if (HalfDuplex) serialPort.SetHalfDuplex();
            }
            catch
            {
                // if (HalfDuplex && serialPort.IsOpen) serialPort.ClearHalfDuplex();
                serialPort.Close();
                return false;
            }
            isOpen = true;

            // イベント発行
            Opened?.Invoke(this, EventArgs.Empty);

            return true;
        }

        // COMポートを閉じる
        private void closeComPort()
        {
            try{
                // if (HalfDuplex) serialPort.ClearHalfDuplex();
                serialPort.Close();
            }catch{
                ;
            }
            isOpen = false;

            // イベント発行
            Closed?.Invoke(this, EventArgs.Empty);
        }

        // サイズ変更時の処理
        protected override void OnSizeChanged(EventArgs e)
        {
            int h = this.Font.Height;
            Size size = new Size(h * 4, h);
            listBaudRate.Size = size;
            listComPort.Size = size;

            base.OnSizeChanged(e);
        }
        // フォント変更時の処理
        protected override void OnFontChanged(EventArgs e)
        {
            listBaudRate.Font = this.Font;
            listComPort.Font = this.Font;

            base.OnFontChanged(e);
        }

        // マウスが入った時の処理
        // (フォーカスが無いときのボタンクリックが効かない問題の対策)
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.Focused)
            {
                this.Focus();
            }
        }
        #endregion

        #region 初期化処理(デザイナーの生成コードから流用)

        // COMポートリスト
        ToolStripComboBox listComPort;
        // ボーレートリスト
        ToolStripComboBox listBaudRate;
        // 接続ボタン
        ToolStripButton buttonConnect;
        // 切断ボタン
        ToolStripButton buttonDisconnect;
        // シリアルポート
        SerialPort serialPort;
        // COMポートリスト更新タイマ
        Timer timerListUpdate;

        // リソース管理用
        IContainer components = null;

        // 使用中のリソースをすべてクリーンアップします
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // コンポーネントの初期化
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listComPort = new ToolStripComboBox();
            this.listBaudRate = new ToolStripComboBox();
            this.buttonConnect = new ToolStripButton();
            this.buttonDisconnect = new ToolStripButton();
            this.serialPort = new SerialPort(this.components); // リソース管理のための引数
            this.timerListUpdate = new Timer(this.components); // リソース管理のための引数
            this.SuspendLayout();

            // ラベル
            var labelComPort = new ToolStripLabel("COMポート");
            var labelBaudRate = new ToolStripLabel("ボーレート");

            // listComPort
            this.listComPort.DropDownStyle = ComboBoxStyle.DropDownList; // 編集不可
            this.listComPort.DropDown += listComPort_DropDown;
            this.listComPort.ToolTipText = "COMポートの指定";
            this.listComPort.Items.Clear();

            // listBaudRate
            this.listBaudRate.DropDownStyle = ComboBoxStyle.DropDown; // 編集可
            this.listBaudRate.ToolTipText = "ボーレートの指定";
            this.listBaudRate.Items.Clear();

            // buttonConnect
            this.buttonConnect.Text = "接続";
            this.buttonConnect.ToolTipText = "接続";
            this.buttonConnect.Click += buttonConnect_Click;

            // buttonDisconnect
            this.buttonDisconnect.Text = "切断";
            this.buttonDisconnect.ToolTipText = "切断";
            this.buttonDisconnect.Click += buttonDisconnect_Click;

            // serialPort
            //this.serialPort.DataReceived += serialPort_DataReceived;

            // SerialPortToolStrip
            this.Items.Add(labelComPort);
            this.Items.Add(listComPort);
            this.Items.Add(labelBaudRate);
            this.Items.Add(listBaudRate);
            this.Items.Add(buttonConnect);
            this.Items.Add(buttonDisconnect);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
