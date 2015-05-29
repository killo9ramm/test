using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProcessServiceWorker;
using CustomLogger;
using System.IO;

namespace ProcessKiller
{
    class Program
    {
        public static string successPath="";
        static void Main(string[] args)
        {
            
                Logger logger = new Logger();
                logger.LoggerName = "ProcessKiller";
                try
                {
                logger.Log("Вошли в киллер");
                ProcessWorker.LogEvent = logger.Log;

                logger.Log("Будем убивать " + args[0]);
                ProcessWorker.StopProcess(args[0]);

                logger.Log("Вышли");
            }catch(Exception ex)
            {
                logger.Log(ex.Message);
            }
        }
    }
}
