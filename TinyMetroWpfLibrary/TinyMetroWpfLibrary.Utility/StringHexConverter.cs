﻿using System;
using System.Text;

namespace TinyMetroWpfLibrary.Utility
{
    public class StringHexConverter
    {
        public static string ConvertStrToHex(string str)
        {
            string data = "";
            foreach (var b in str)
            {
                string s = Convert.ToString(b, 16);
                if (s.Length == 1)
                {
                    s = "0" + s;
                }
                data += s;
            }

            return data.ToUpper();

        }
        public static string ConvertCharToHex(byte[] str, int length)
        {
            var data = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                string s = Convert.ToString(str[i], 16);
                if (s.Length == 1)
                {
                    s = "0" + s;
                }
                data.Append(s);
            }

            return data.ToString().ToUpper();

        }
        public static byte[] ConvertHexToChar(string str)
        {
            var data = new byte[str.Length / 2];
            try
            {
                for (int i = 0; i < str.Length / 2; i++)
                {
                    string s = str.Substring(i * 2, 2);

                    int d = Convert.ToByte(s, 16);
                    data[i] = (byte)d;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return data;
        }
    }
}
