using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RBClient
{
    class MDIParentMain
    {
        public static void Log(string message)
        {
            //Program.log.Log(message);
        }

        public static void Log(Exception ex, string message)
        {
            //Program.log.Log("Error " + ex.Message + " Message:" + message);
        }
    }
}
