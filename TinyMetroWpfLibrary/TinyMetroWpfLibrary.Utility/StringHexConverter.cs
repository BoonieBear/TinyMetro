using System;
using System.Text;

namespace TinyMetroWpfLibrary.Utility
{
    public class StringHexConverter
    {
        /// <summary>
        /// 返回数据的固定长度字符串，如2.34，固定长度为5，小数点位置在3，则返回00234，小数点位置在2，则返回02340
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dotindex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetFormedString(string src, int dotindex, int length)
        {
            int SrcDotIndex = src.IndexOf('.');
            src = src.Replace(".", "");
            if (SrcDotIndex > dotindex)
            {
                throw (new Exception("GetFormedString输入格式错误！"));
            }
            int move = dotindex - SrcDotIndex;
            src = src.PadLeft(src.Length + move, '0');
            src = src.PadRight(length, '0');
            return src;

        }
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
