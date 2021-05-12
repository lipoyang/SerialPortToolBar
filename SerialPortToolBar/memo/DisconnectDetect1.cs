// メインウィンドウのWndProcでデバイス切断を検出する方法

using System.Runtime.InteropServices; // StructLayout

...

        // Win32APIの宣言
        public const int WM_DEVICECHANGE = 0x0219;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVTYP_PORT = 0x0003;

        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_HDR
        {
            internal Int32 dbch_size;
            internal Int32 dbch_devicetype;
            internal Int32 dbch_reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class DEV_BROADCAST_PORT
        {
            public int dbcp_size;
            public int dbcp_devicetype;
            public int dbcp_reserved; // MSDN say "do not use"
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
            public byte[] dbcp_name;
        }
        
        // WndProcをオーバーライド
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            
            // デバイス変化イベントか？
            if(m.Msg == WM_DEVICECHANGE)
            {
                // デバイス切断完了イベントか？
                if((int)m.WParam == DBT_DEVICEREMOVECOMPLETE)
                {
                    // シリアルポートまたはパラレルポートか？
                    DEV_BROADCAST_HDR devBroadcastHeader = new DEV_BROADCAST_HDR();
                    Marshal.PtrToStructure(m.LParam, devBroadcastHeader);
                    if(devBroadcastHeader.dbch_devicetype == DBT_DEVTYP_PORT)
                    {
                        // ポート名を取得
                        DEV_BROADCAST_PORT port = new DEV_BROADCAST_PORT();
                        Marshal.PtrToStructure(m.LParam, port);
                        string portName = ASCIIEncoding.Unicode.GetString(port.dbcp_name, 0, port.dbcp_size - (4 * 3));
                        portName = portName.TrimEnd('\0');
                        
                        Console.WriteLine("PORT[" + portName + "] REMOVED!");
                    }
                }
            }
        }
