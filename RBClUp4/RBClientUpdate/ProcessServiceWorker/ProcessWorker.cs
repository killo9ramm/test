using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Debug_classes;
using RBServer.Debug_classes;

namespace ProcessServiceWorker
{
    public class ProcessWorker
    {
        public static event Debug_classes.FileSystemHelper.MessageEventHandler LogEvent;
        public static List<string> stopped_processes = new List<string>();

        private static void Log(string message)
        {
            if (null != LogEvent)
            {
                MessageEventArgs mess = new MessageEventArgs(message);
                LogEvent(null, mess);
            }
        }

        public static void StopProcess(string process)
        {
            Process[] prc = null;
            string prc_str = "";
            prc = System.Diagnostics.Process.GetProcesses();
            prc = System.Diagnostics.Process.GetProcessesByName(process);
            if (prc!=null){
                prc.ToList<Process>().ForEach(a =>
                {
                    try
                    {
                      //  prc_str = a.MainModule.FileName;
                    //    stopped_processes.Add(prc_str);
                        a.Kill();
                        Log("Process Stopped: " + a.ProcessName);
                    }
                    catch (Exception ex)
                    {
                    //    stopped_processes.Remove(prc_str);
                        Log("Process Stopped Error: " + a.ProcessName + "Error: " + ex.Message);
                        throw new CException(ex.Message);
                    }
                });
            }
        }

        public static IEnumerable<Process> GetProcessByName(string process)
        {
            Process[] prc = null;
            prc = System.Diagnostics.Process.GetProcessesByName(process);

            return prc;
        }

        public static void StopProcess(List<string> process)
        {
            process.ForEach(a => StopProcess(a));
        }

        public static void StartProcess(string process_name)
        {

            Process process = null;
            try
            {
                process = new Process();
                process.StartInfo.FileName = process_name;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                Log("Process Started: " + process_name);
            }
            catch (Exception ex)
            {
                Log("Process Started Error: " + process_name + " Error: " + ex.Message);
                throw new CException(ex.Message);
            }
        }

        public static void StartProcess(List<string> process)
        {
            process.ForEach(a => StartProcess(a));
        }
    }
}
