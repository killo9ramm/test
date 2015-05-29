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
        static string GetFQDNName()
        {
            try
            {
                WmiMethods wmi = new WmiMethods(); wmi.LogEvent = Log;
                ManagementScope scope = wmi.CreateScope();
                if (wmi.CheckConnection(scope) != null)
                {
                    string ls1 = wmi.GetMachineName(scope);
                    Log("dns nsme is " + Serializer.JsonSerialize(ls1));
                    return ls1;
                }
                return "";
            }
            catch (Exception ex)
            {
                Log("error " + ex.Message);
                return "";
            }
        }

        static void Main_get_second_rest()
        {
            string step = "";
            try
            {

                getMName();

                CBData cd = new CBData();
                var a = cd.CountCurrentTeremok();
                string message = terem_name + ":\r\n";
                if (a.Rows.Count > 1)
                {
                    foreach (DataRow b in a.Rows)
                    {
                        message += String.Format(" id={0} 1c={1} name={2} \r\n",
                            CellHelper.FindCell(b, "teremok_id").ToString(),
                            CellHelper.FindCell(b, "teremok_name").ToString(),
                            CellHelper.FindCell(b, "teremok_1C").ToString()
                        );

                    }
                    WebService.SendNotification(terem_name + " slavery", message, 0);
                }
            }
            catch (Exception ex)
            {
                string message = "error step=" + step + " error:" + ex.Message;
                Log(message);
                WebService.SendNotification(terem_name, message, 99);
            }
        }

        static void Main_install_puppet()
        {
            string step = "";
            try
            {

                getMName();

                string fqdnName = GetFQDNName();

                string r = CNFG.GetProperty<string>("pc_name_regex", @"\D+?-pos-\d+?.msk.teremok.biz");
                Regex reg = new Regex(r);

                step = "Check_pc_name";  //проверить имя - поставить
                if (!reg.IsMatch(fqdnName))
                {
                    Main_kkm_rename1();
                }

                //проверить паппет - поставить
                string pr_name = CNFG.GetProperty<string>("check_program_is_installed", "Puppet");
                step = "Check " + pr_name + " is installed?";
                if (!check_program_is_installed(pr_name))
                {
                    execute_file(CNFG.GetProperty<string>("start_process_name", "puppet-2.7.23.msi"));
                }

                //проверить - отправить сообщение
                step = "Check Main_dns_puppet_checker1";
                Main_dns_puppet_checker1();
            }
            catch (Exception ex)
            {
                string message = "error step=" + step + " error:" + ex.Message;
                Log(message);
                WebService.SendNotification(terem_name, message, 99);
            }
        }

        static void Main_dns_puppet_checker()
        {
            try
            {
                GetTeremokName();

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

        static void Main_copy__and_launch_prKiller()
        {
            try
            {
                //get kkms
                List<string> kkmsRoot = getKkmsRoot();
                Log("kkms " + Serializer.JsonSerialize(kkmsRoot));
                GetTeremokName();

                foreach (var a in kkmsRoot)
                {
                    try
                    {
                        //foreach kkm
                        Log("start for kkm " + a);

                        WmiMethods wmi = new WmiMethods(); wmi.LogEvent = Log;
                        ManagementScope scope = wmi.CreateScope(a);
                        List<WmiMethods.Credentials> clist = SetCredentials();

                        if (wmi.CheckConnection(scope, clist) != null)
                        {
                            Log("connected to kkm " + a);
                            //copy app
                            DirectoryInfo new_dir = new DirectoryInfo(Path.Combine(a, "temp"));
                            new_dir.CreateOrReturn();
                            DirectoryInfo old_dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FindDirectory("prkiller", true);

                            Log(String.Format("copy dir {0} in {1}", old_dir.FullName, new_dir.FullName));
                            old_dir.CopyDirectory(new_dir.FullName);

                            //execute app 
                            string f = new_dir.FindFile("AccessControlApp.exe", true).FullName;
                            Log("run process" + f);
                            wmi.RunProcess(scope, f);

                            //start pr c:\prkiller\ProcessKillerLauncher.exe
                            Log(@"c:\prkiller\ProcessKillerLauncher.exe" + f);
                            wmi.RunProcess(scope, @"c:\prkiller\ProcessKillerLauncher.exe");

                            new DirectoryInfo(a).Delete(true);

                            send_notify(terem_name + " wmi", "success on " + a, 0);
                        }
                        else
                        {
                            Log("error connecting to kkm=" + a);
                            send_notify(terem_name + " wmi", "error on " + a, 99);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("error starting prk on kkm=" + a + " " + ex.Message);
                    }
                }

                //ConnectionOptions connectoptions = new ConnectionOptions();
                ////  connectoptions.Username = string.Format(@"{0}", @"msk\krigel.s");
                //connectoptions.Username = "Администратор";//string.Format(@"{0}", @"Admin");
                //connectoptions.Password = "1 ";

                ////?connectoptions.Impersonation = ImpersonationLevel.Impersonate;
                //connectoptions.EnablePrivileges = true;

                //ManagementScope scope = new ManagementScope(@"\\" + "10.15.127.3" + @"\root\cimv2");
                ////ManagementScope scope = new ManagementScope(@"\\" + "10.11.0.177" + @"\root\cimv2");
                //scope.Options = connectoptions;
                //scope.Options.EnablePrivileges = true;
                //scope.Options.Impersonation = System.Management.ImpersonationLevel.Impersonate;

                //scope.Connect();

                //SelectQuery query = new SelectQuery("select * from Win32_Process WHERE Name = 'ProcessKillerLauncher.exe'");

                //ManagementObjectSearcher theSearcher = new ManagementObjectSearcher(scope, query);

                //ManagementObjectCollection theCollection = theSearcher.Get();

                //foreach (ManagementObject theCurObject in theCollection)
                //{

                //    theCurObject.InvokeMethod("Terminate", null);

                //}

            }
            catch (Exception ex)
            {
                Log("main exception" + ex.Message);
            }
        }

        static void Main_set_gateway()
        {
            GetTeremokName();
            //check trigger
            if (CNFG.GetProperty<string>("MPC_Loaded", "0") == "0")
            {
                RunOnAdmPc();
            }
            else
            {
                RunOnKKmPc();
            }


        }

        static void MainDotNet()
        {
            try
            {
                GetTeremokName();
                string dotnetVers = GetVersionFromRegistry();
                send_notify(terem_name + " dotnet", dotnetVers, 0);

            }
            catch (Exception ex)
            {
                send_notify(terem_name + " dotnet", "error " + ex.Message, 99);
            }
        }

        static void Main_kkm_rename()
        {

            try
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
            catch (Exception ex)
            {
                Log("error " + ex.Message);
                WebService.SendNotification(terem_name, "error " + ex.Message, 99);
            }
        }
    }
}
