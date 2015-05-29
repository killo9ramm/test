using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CustomLogger
{
    class Logger
    {
        public string LoggerName = "Trace";
        public string LogFileName = "Logger.txt";
        int counter = 0;
        FileInfo LogFile = null;

       
        public Logger()
        {

        }

        public void Log(string message)
        {
            CheckLogSize();
            System.IO.File.AppendAllText(LogFileName, LoggerName + "----" + DateTime.Now.ToString() + "-----" + message + "\r\n");
        }
        public int MaxLogFileNameSize = 20000000;
        public void CheckLogSize()
        {
            if (System.IO.File.Exists(LogFileName) && System.IO.File.ReadAllBytes(LogFileName).Length > MaxLogFileNameSize)
            {
                System.IO.File.Delete(LogFileName);
            }

        }

        public void Log(Exception ex, string message)
        {
            if (ex != null)
            {
                Log("error: " + ex.Message + " message: " + message);
            }
        }
    }
}
