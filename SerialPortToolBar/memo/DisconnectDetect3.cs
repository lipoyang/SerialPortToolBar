// PnPデバイス切断イベント監視でデバイス切断を検出する方法

using System.Management; // ManagementEventWatcher
using System.Text.RegularExpressions; // Regex.Match

...

        // デバイス切断検出器
        ManagementEventWatcher disconnectWatcher;

        // デバイス切断監視を開始
        public void startDisconnectWatch()
        {
            // 監視のクエリ (1秒ごとにPnPデバイスのインスタンス破棄を監視)
            var query = new WqlEventQuery(
                "SELECT * FROM __InstanceDeletionEvent " +
                "WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity' "
                );
            // 監視する名前空間
            var scope = new ManagementScope("root\\CIMV2") { Options = { EnablePrivileges = true } };

            // 監視を開始
            disconnectWatcher = new ManagementEventWatcher(scope, query);
            disconnectWatcher.EventArrived += deviceDisconnected;
            disconnectWatcher.Start();
        }

        // デバイス切断監視を終了
        public void stopDisconnectWatch()
        {
            disconnectWatcher.Stop();
            disconnectWatcher.Dispose();
        }

        // なんらかのPnPデバイスが切断されたとき
        private void deviceDisconnected(object sender, EventArrivedEventArgs e)
        {
            // デバイス名を取得
            var targetInstanceData = e.NewEvent.Properties["TargetInstance"];
            var targetInstanceObject = (ManagementBaseObject)targetInstanceData.Value;
            if (targetInstanceObject == null) return;
            var deviceName = targetInstanceObject.Properties["Name"].Value.ToString();

            // デバイス名に COM[数字] が含まれていたら抽出
            var match = Regex.Match(deviceName, "COM[0-9]{1,}"); // 正規表現
            string comPort = match.Value;
            if (comPort == "") return;

            Console.WriteLine("PORT[" + comPort + "] REMOVED!");
        }
