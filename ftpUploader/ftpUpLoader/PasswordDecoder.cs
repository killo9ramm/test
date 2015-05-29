using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient.Classes.CustomClasses
{
    class PasswordDecoder
    {
        private const int NUM = 12462;
        private const int CAP = 4195;
        private const int LET = 47513;
        private static string a = "\"";
        private string symbXAML = a;
        private string symbCS = @"[^\w0-9]";


        public static char decode_char(char ch)
        {
            int code = (int)ch;
            char chr = ' ';

            if (code >= NUM && code < LET) { code -= NUM; return (char)code; }
            if (code >= CAP && code < NUM) { code -= CAP; return (char)code; }
            if (code >= LET) { code -= LET; return (char)code; }

            return (char)code;
        }

        public static char encode_char(char ch)
        {
            int code = (int)ch;
            char chr = ' ';
            if (code >= 65 && code <= 90) code += CAP;
            if (code >= 48 && code <= 57) code += NUM;
            if (code >= 87 && code <= 122) code += LET;

            return (char)code;
        }

        public static string encode_string(string str)
        {
            string st = "";
            for (int i = 0; i < str.Length; i++)
            {
                st += encode_char(str[i]);
            }
            return st;
        }

        public static string decode_string(string str)
        {
            string st = "";
            for (int i = 0; i < str.Length; i++)
            {
                st += decode_char(str[i]);
            }
            return st;
        }
    }
}
