
namespace TestApp
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.serialPortToolStrip = new SerialPortToolBar.SerialPortToolStrip();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "トラックバーの値を送信します";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(220, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "ここに受信した値をプログレスバーで表示します";
            // 
            // trackBar
            // 
            this.trackBar.Location = new System.Drawing.Point(14, 79);
            this.trackBar.Maximum = 100;
            this.trackBar.Name = "trackBar";
            this.trackBar.Size = new System.Drawing.Size(400, 45);
            this.trackBar.TabIndex = 5;
            this.trackBar.TickFrequency = 10;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(23, 186);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(400, 23);
            this.progressBar.TabIndex = 6;
            // 
            // serialPortToolStrip
            // 
            this.serialPortToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.serialPortToolStrip.Location = new System.Drawing.Point(0, 0);
            this.serialPortToolStrip.Name = "serialPortToolStrip";
            this.serialPortToolStrip.Size = new System.Drawing.Size(452, 25);
            this.serialPortToolStrip.TabIndex = 0;
            this.serialPortToolStrip.Text = "serialPortToolStrip1";
            this.serialPortToolStrip.Opened += new System.EventHandler(this.serialPortToolStrip_Opened);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 248);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.trackBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serialPortToolStrip);
            this.Name = "Form3";
            this.Text = "テスト2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SerialPortToolBar.SerialPortToolStrip serialPortToolStrip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}