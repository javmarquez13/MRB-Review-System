using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MRB_Review_System
{
    class ConfigFiles
    {
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string seccion, string llave, string def, StringBuilder StrBuilder, int size, string filepath);

        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string seccion, string llave, string valor, string filepath);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);

        public class configFiles
        {
               
        }

        public static string reader(string _seccion, string _llave, string _FilePath)
        {
            string _result;
            StringBuilder _builder = new StringBuilder(500);
            GetPrivateProfileString(_seccion, _llave, "", _builder, _builder.Capacity, _FilePath);
            _result = _builder.ToString();
            return _result;
        }

        public static void write(string _seccion, string _llave, string _valor, string _filePath)
        {
            WritePrivateProfileString(_seccion, _llave, _valor, _filePath);
        }


        public static List<string> GetKeys(string category)
        {
            byte[] buffer = new byte[2048];
            GetPrivateProfileSection(category, buffer, 2048, Globals.CONFIG_FILE);
            String[] tmp = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');

            List<string> result = new List<string>();

            foreach (String entry in tmp)
            {
                result.Add(entry.Substring(0, entry.IndexOf("=")));
            }

            return result;
        }
    }
}

