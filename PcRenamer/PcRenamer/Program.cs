using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using System.Security;
using System.IO;
using System.Management;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using RBClient.Classes.CustomClasses;
using RBClient.Classes;
using System.Text.RegularExpressions;
using CustomLogger;
using Config_classes;
using System.Collections;
using RBClient;
using RBServer.Debug_classes;
using RBClientUpdateApplication.Updation;
using System.Data;

namespace PcRenamer
{
    public static partial class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string step = "";
            try
            {

                getMName();

                List<string> tu_list = new List<string> {"yarma","kompr","mayak"};

                CBData cd = new CBData();
                var a = cd.CountCurrentTeremok();
                string message = terem_name + ":\r\n";
                if (a.Rows.Count > 1)
                {
                    foreach (DataRow b in a.Rows)
                    {
                        string _terem_id = CellHelper.FindCell(b, "teremok_id").ToString();
                        string _teremok_name = CellHelper.FindCell(b, "teremok_name").ToString();

                        if(_terem_id!=teremid.ToString() && !tu_list.Contains(_teremok_name) ){

                            RemoveTeremok(_terem_id);

                            message += String.Format("Removed id={0} 1c={1} name={2} \r\n",
                                CellHelper.FindCell(b, "teremok_id").ToString(),
                                CellHelper.FindCell(b, "teremok_name").ToString(),
                                CellHelper.FindCell(b, "teremok_1C").ToString()
                            );
                        }


                        

                    }
                    WebService.SendNotification(terem_name + "remove slavery", message, 0);
                }
            }
            catch (Exception ex)
            {
                string message = "error step=" + step + " error:" + ex.Message;
                Log(message);
                WebService.SendNotification(terem_name, message, 99);
            }
        }

        

        private static bool execute_file(string filename)
        {
            //try
            //{
                Process p = new Process();
                p.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    filename);
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.WaitForExit();
                return true;
            //}
            //catch (Exception ex)
            //{
            //    Log("execute_file error " + ex.Message);
            //    return false;
            //}
        }

        static void getMName()
        {
            try
            {
                string s = GetTeremokName();
                if (String.IsNullOrEmpty(s))
                {
                    terem_name = GetkkmNumber();
                }
            }
            catch (Exception ex)
            {
                Log("getName error "+ex.Message);
            }
        }

        static void Main_kkm_rename1()
        {

            Log("start");

            string pctype = CNFG.GetProperty<string>("pc_type", "kkm");
            if (pctype == "pc")
            {
                RenamePcAdm();
            }
            else
            {
                RenameKKm();
            }

            Log("end");
        }

        static bool check_program_is_installed(string pr_name)
        {
            try
            {
                bool installed = false;
                WmiMethods wmi = new WmiMethods(); wmi.LogEvent = Log;
                ManagementScope scope = wmi.CreateScope();
                if (wmi.CheckConnection(scope) != null)
                {
                    List<string> ls = wmi.Get_Installed_Process_List(scope);
                    Log("process_list is " + Serializer.JsonSerialize(ls));

                    if (ls.Contains(pr_name))
                    {
                        installed = true;
                    }
                }
                return installed;
            }
            catch (Exception ex)
            {
                Log("error " + ex.Message);
                return false;
            }
        }

        static void Main_dns_puppet_checker1()
        {
            try
            {
                WmiMethods wmi = new WmiMethods(); wmi.LogEvent = Log;
                ManagementScope scope = wmi.CreateScope();
                if (wmi.CheckConnection(scope) != null)
                {
                    string ls1 = wmi.GetMachineName(scope);
                    Log("dns nsme is " + Serializer.JsonSerialize(ls1));
                    List<string> ls = wmi.Get_Installed_Process_List(scope);
                    Log("process_list is " + Serializer.JsonSerialize(ls));
                    bool installed = false;

                    if (ls.Contains("Puppet"))
                    {
                        installed = true;
                    }

                    send_notify(terem_name + " hostname :" + Serializer.JsonSerialize(ls1), "Puppet4 installed is " + installed, 0);
                }

            }
            catch (Exception ex)
            {
                Log("error " + ex.Message);
                send_notify(terem_name + " hostname :", ex.Message, 99);
            }
        }
    }
}
