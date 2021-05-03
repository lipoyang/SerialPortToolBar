using System;
using System.Text;
using System.IO; // File
using System.Runtime.InteropServices; // DllImport

namespace IniFileSharp
{
    public class IniFile
    {
        //********** WIN32API **********/

        // get string value
        [DllImport("KERNEL32.DLL")]
        private static extern uint GetPrivateProfileString(
            string lpAppName, // section name
            string lpKeyName, // key name
            string lpDefault, // default value
            StringBuilder lpReturnedString, // string buffer
            uint nSize,       // string max size
            string lpFileName // INI file name
            );

        // get integer value
        [DllImport("KERNEL32.DLL")]
        private static extern uint GetPrivateProfileInt(
            string lpAppName, // section name
            string lpKeyName, // key name
            int nDefault,     // default value
            string lpFileName // INI file name
            );

        // set string value
        [DllImport("KERNEL32.DLL")]
        private static extern int WritePrivateProfileString(
            string lpAppName, // section name
            string lpKeyName, // key name
            string lpstring,  // string value
            string lpFileName // INI file name
            );

        //********** private field **********/

        // INI file name
        private string FileName;

        //********** public API **********/

        // constructor
        public IniFile(string filename = @".\SETTING.INI")
        {
            // if no directory specified, 
            // the above WIN32APIs searches the Windows directory for the file,
            // NOT the current directory.
            string dir = Path.GetDirectoryName(filename);
            if (dir == "")
            {
                filename = @".\" + filename;
            }
            FileName = filename;
        }

        // INI file exists?
        public bool Exists()
        {
            return File.Exists(FileName);
        }

        // create INI file
        public void Create()
        {
            string dir = Path.GetDirectoryName(FileName);
            if((dir!= "") && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(FileName))
            {
                File.Create(FileName).Close();
            }
        }

        // get integer value
        public int ReadInteger(string section, string key, int defaultValue)
        {
            return (int)GetPrivateProfileInt(section, key, defaultValue, FileName);
        }

        // get string value
        public string ReadString(string section, string key, string defaultValue)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, defaultValue, sb, Convert.ToUInt32(sb.Capacity), FileName);
            return sb.ToString();
        }

        // get boolean value
        public bool ReadBoolean(string section, string key, bool defaultValue)
        {
            StringBuilder sb = new StringBuilder(16);
            GetPrivateProfileString(section, key, "", sb, Convert.ToUInt32(sb.Capacity), FileName);
            string s = sb.ToString();
            if (s.ToLower() == "true")
            {
                return true;
            }
            else if (s.ToLower() == "false")
            {
                return false;
            }
            else
            {
                return defaultValue;
            }
        }

        // get double data
        public double ReadDouble(string section, string key, double defaultValue)
        {
            StringBuilder sb = new StringBuilder(32);
            GetPrivateProfileString(section, key, "", sb, Convert.ToUInt32(sb.Capacity), FileName);
            try
            {
                return Convert.ToDouble(sb.ToString());
            }
            catch
            {
                return defaultValue;
            }
        }

        // get decimal data
        public decimal ReadDecimal(string section, string key, decimal defaultValue)
        {
            StringBuilder sb = new StringBuilder(32);
            GetPrivateProfileString(section, key, "", sb, Convert.ToUInt32(sb.Capacity), FileName);
            try
            {
                return Convert.ToDecimal(sb.ToString());
            }
            catch
            {
                return defaultValue;
            }
        }

        // set integer value
        public void WriteInteger(string section, string key, int value)
        {
            WritePrivateProfileString(section, key, value.ToString(), FileName);
        }

        // set string value
        public void WriteString(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, FileName);
        }

        // set boolean value
        public void WriteBoolean(string section, string key, bool value)
        {
            WritePrivateProfileString(section, key, value.ToString(), FileName);
        }

        // set double value
        public void WriteDouble(string section, string key, double value)
        {
            WritePrivateProfileString(section, key, value.ToString(), FileName);
        }

        // set decimal value
        public void WriteDecimal(string section, string key, decimal value)
        {
            WritePrivateProfileString(section, key, value.ToString(), FileName);
        }
    }
}
