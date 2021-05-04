using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    /// <summary>
    /// ProgressBarの拡張メソッド定義クラス
    /// </summary>
    public static class ProgressBarExtensions
    {
        /// <summary>
        /// 値を設定します。(伸びるときにアニメーションになって反応が遅い問題の対策あり)
        /// </summary>
        /// <param name="vallue">設定する値</param>
        public static void SetValue(this ProgressBar progressBar, int vallue)
        {
            if (progressBar.Value < vallue)
            {
                if (vallue < progressBar.Maximum)
                {
                    progressBar.Value = vallue + 1;
                    progressBar.Value = vallue;
                }
                else
                {
                    progressBar.Maximum++;
                    progressBar.Value = vallue + 1;
                    progressBar.Value = vallue;
                    progressBar.Maximum--;
                }
            }
            else
            {
                progressBar.Value = vallue;
            }
        }
    }
}
