using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient.Classes
{
    public class LoggerBaseMdi
    {
        public static void Log(string message)
        {
            MDIParentMain.Log(message);
        }

        public static  void Log(Exception ex)
        {
            MDIParentMain.Log(ex.Message);
        }

        public static  void Log(Exception ex, string message)
        {
            MDIParentMain.Log(ex, message);
        }
    }
}
