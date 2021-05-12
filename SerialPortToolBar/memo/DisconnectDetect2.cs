// PnPデバイスのリストをポーリングしてデバイス切断を検出する方法

using System.Management;

...

        // PnPデバイスから (COM番号) を含む名前のデバイスを検索する
        static ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                @"Select * from Win32_PNPEntity" +
                @" Where (Name like '%(COM%)')");

        // COMポートデバイス名のリストを取得
        static List<string> getComDevices()
        {
            List<string> devices = new List<string>();
            var collection = searcher.Get();
            foreach (var device in collection)
            {
                devices.Add((string)device.GetPropertyValue("Name"));
            }
            collection.Dispose();
            return devices;
        }

        // 前回のCOMポートデバイス名リスト
        List<string> usbDevicesBefore = new List<string>();

        // デバイス更新チェック
        private void CheckDevice()
        {
            // COMポートデバイス名のリストを取得
            var usbDevices = getComDevices();

            // 前回のチェック時より減ったか？
            if (usbDevices.Count < usbDevicesBefore.Count)
            {
                // 切断されたデバイス名を検索
                foreach (var usbDeviceBefore in usbDevicesBefore)
                {
                    bool bExistDevice = false;
                    foreach (var usbDevice in usbDevices)
                    {
                        if (usbDevice == usbDeviceBefore)
                        {
                            bExistDevice = true;
                            break;
                        }
                    }
                    if (!bExistDevice)
                    {
                        Console.WriteLine("PORT[" + usbDeviceBefore + "] REMOVED!");
                    }
                }
            }
            usbDevicesBefore = usbDevices;
        }
