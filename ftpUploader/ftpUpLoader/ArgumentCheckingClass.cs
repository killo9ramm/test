using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    class ArCh
    {
        public static void Check<T>(T arg,string name)
        {
            if (arg is string)
            {
                if (String.IsNullOrEmpty(arg as string))
                {
                    throw new ArgumentException("Parameter missing", name);
                }
            }
        }

        public static bool isn<T>(T arg)
        {
            if (arg is string)
            {
                if (String.IsNullOrEmpty(arg as string))
                {
                    return true;
                }
            }
            return false;
        }
        public static void isne<T>(T arg,string name)
        {
            if (arg is string)
            {
                if (String.IsNullOrEmpty(arg as string))
                {
                    throw new ArgumentException("Parameter missing", name);
                }
            }
        }
    }
}
