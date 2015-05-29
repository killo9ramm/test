using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace ProcessServiceWorker
{
    public class ProcessWorker
    {
        public static Action<string> LogEvent;
        public static List<string> stopped_processes = new List<string>();

        private static void Log(string message)
        {
            if (null != LogEvent)
            {
                LogEvent(message);
            }
        }

        public static void StopProcess(string process)
        {
            Process[] prc = null;

            prc = System.Diagnostics.Process.GetProcesses();
            prc = System.Diagnostics.Process.GetProcessesByName(process);
            if (prc!=null){
                prc.ToList<Process>().ForEach(a =>
                {
                    try
                    {
                        a.Kill();
                        Log("Process Stopped: " + a.ProcessName);
                        stopped_processes.Add(a.MainModule.FileName);
                    }
                    catch (Exception ex)
                    {
                        Log("Process Stopped Error: " + a.ProcessName + "Error: " + ex.Message);
                        throw new Exception(ex.Message);
                    }
                });
            }
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
                process.StartInfo.WorkingDirectory = new FileInfo(process_name).Directory.FullName;
                
                process.Start();
                Log("Process Started: " + process_name);
            }
            catch (Exception ex)
            {
                Log("Process Started Error: " + process_name + " Error: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public static void StartProcess(List<string> process)
        {
            process.ForEach(a => StartProcess(a));
        }
    }
}
