using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var formA = new Form1() { Text = "テスト1 ポートA側" };
            formA.Show();
            var formB = new Form1() { Text = "テスト1 ポートB側" };
            formB.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var formA = new Form2() { Text = "テスト2 ポートA側" };
            formA.Show();
            var formB = new Form2() { Text = "テスト2 ポートB側" };
            formB.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var formA = new Form3() { Text = "テスト3 ポートA側" };
            formA.Show();
            var formB = new Form3() { Text = "テスト3 ポートB側" };
            formB.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var formA = new Form4() { Text = "テスト4 ポートA側" };
            formA.Show();
            var formB = new Form4() { Text = "テスト4 ポートB側" };
            formB.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var formA = new Form5A() { Text = "テスト5 ポートA側" };
            formA.Show();
            var formB = new Form5B() { Text = "テスト5 ポートB側" };
            formB.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }
    }
}
