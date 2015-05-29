using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CustomLogger
{
    class Logger
    {
        public static string homedir = AppDomain.CurrentDomain.BaseDirectory;
        public string LoggerName = Path.Combine(homedir,"Trace");
        public string LogFileName = Path.Combine(homedir,"Logger.txt");
        public string LogFileNameError = Path.Combine(homedir, "Error.txt");
            
        int counter = 0;
        FileInfo LogFile = null;

       
        public Logger()
        {

        }

        public Logger(string filename)
        {
            LogFileName = Path.Combine(homedir, filename);
        }

        public void Log(Exception ex, string message)
        {
                if (ex != null)
                {
                    Log("error: " + ex.Message + " message: " + message);
                }
        }
        public void Log(string message)
        {
            lock (loko)
            {
                //CheckLogSize();
                System.IO.File.AppendAllText(LogFileName, LoggerName + "----" + DateTime.Now.ToString() + "-----" + message + "\r\n");
            }
        }

        private object loko = new object();
        public int MaxLogFileNameSize = 20000000;
        //public int MaxLogFileNameSize = 3000;
        public int MaxLogFileCount = 5;
        public void CheckLogSize()
        {
                try
                {
                    if (System.IO.File.Exists(LogFileName) && System.IO.File.ReadAllBytes(LogFileName).Length > MaxLogFileNameSize)
                    {
                        File.Copy(LogFileName, LogFileName + "_" + DateTime.Now.ToOADate().ToString());
                        File.Delete(LogFileName);
                        CheckLogFilesCount();
                    }
                }
                catch (Exception ex)
                {

                    System.IO.File.AppendAllText(LogFileNameError, "Error ----" + DateTime.Now.ToString() + "-----" + ex.Message + "\r\n");
                }
            
        }

        private void CheckLogFilesCount()
        {
           
                try
                {
                    var dir = new DirectoryInfo(homedir);
                    var list = dir.GetFiles("Logger*").ToList();
                    list.Sort((a, b) => b.CreationTime.CompareTo(a.CreationTime));

                    var del_list = list.Where((a, i) => i > MaxLogFileCount).ToList();
                    del_list.ForEach(a => a.Delete());
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(LogFileNameError, "Error ----" + DateTime.Now.ToString() + "-----" + ex.Message + "\r\n");
                }
            
        }

        
    }
}
