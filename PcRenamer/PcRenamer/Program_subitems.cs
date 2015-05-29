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

namespace PcRenamer
{
    public static partial class Program
    {
        private static void RunOnAdmPc()
        {
            try
            {
                Log("RunOnAdmPc start");
                //get def gateway
                //get dns search order
                ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection objMOC = objMC.GetInstances();

                var gateways = WebConfigMethods.GetArrParamFromNetwAd(objMOC, "DefaultIPGateway");
                var dns_sorder = WebConfigMethods.GetArrParamFromNetwAd(objMOC, "DNSServerSearchOrder");

                if (!gateways.NotNullOrEmpty())
                {
                    Log("DefaultIPGateway is missing");
                    //throw new Exception("DefaultIPGateway is missing");
                }
                else
                {
                    //edit config
                    Log("DefaultIPGateway is " + Serializer.JsonSerialize(gateways));
                    for (int i = 0; i < gateways.Count; i++)
                    {
                        CNFG.SetCreateProperty("MPC_GateWay" + i, gateways[i]);
                    }
                }
                if (!dns_sorder.NotNullOrEmpty())
                {
                    Log("DNSServerSearchOrder is missing");
                    //throw new Exception("DNSServerSearchOrder is missing");
                }
                else
                {
                    Log("MPC_DNSOrder is " + Serializer.JsonSerialize(dns_sorder));
                    for (int i = 0; i < dns_sorder.Count; i++)
                    {
                        CNFG.SetCreateProperty("MPC_DNSOrder" + i, dns_sorder[i]);
                    }
                }

                //load trigger
                CNFG.SetCreateProperty("MPC_Loaded", "1");

                //make zip archive to inbox folder
                DirectoryInfo rbdir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FindParentDir("rbclient");

                Log("Make archive " + Path.Combine(rbdir.FullName, "inbox"));
                ArhiveManager.CreateArchiveFile("KKMPACKAGE_ADV_WEBCONFIG.zip", AppDomain.CurrentDomain.BaseDirectory,
                    Path.Combine(rbdir.FullName, "inbox"));

                send_notify_wc(terem_name + " WEBCONFIG", "get webparans and copy to inbox success", 0);

                Log("RunOnAdmPc end");

            }
            catch (Exception ex)
            {
                Log("RunOnAdmPc error " + ex.Message);
                send_notify(terem_name + " WEBCONFIG", "RunOnAdmPc error " + ex.Message, 99);
            }
        }

        private static void RunOnKKmPc()
        {
            try
            {
                Log("RunOnKKmPc start");
                string kkm = "";
                try
                {
                    kkm = GetkkmNumber();
                }
                catch (Exception ex)
                {
                    Log("GetkkmNumber error " + ex.Message);
                }
                //set web params

                var _1 = CNFG.GetAllProperties("MPC_GateWay").Values;
                if (_1.NotNullOrEmpty())
                {
                    var gateways = _1.OfType<string>().ToArray();
                    Log("gateways is " + Serializer.JsonSerialize(gateways));
                    Log("set gateway");
                    WebConfigMethods.setGateway(gateways);
                }

                var _2 = CNFG.GetAllProperties("MPC_DNSOrder").Values;
                if (_2.NotNullOrEmpty())
                {
                    var dnsorder = _2.OfType<string>().ToArray();
                    Log("dns order is " + Serializer.JsonSerialize(dnsorder));
                    Log("set dns");
                    WebConfigMethods.setDNS(dnsorder);
                }

                CustomAction ca = new CustomAction((o) =>
                {
                    send_notify_wc(kkm + " set web params on kkm", "success", 0);
                    Log("set web params on kkm " + kkm + " notify sended");
                }, null) { Timeout = 30000, MaxTries = 10 };
                ca.Start();

                Log("RunOnKKmPc end");

            }
            catch (Exception ex)
            {
                Log("RunOnKKmPc error" + ex.Message);
            }
        }

        static void send_notify(string header, string message, int code)
        {
            try
            {
                WebService.SendNotification(header, message, code);

            }
            catch (Exception ex)
            {
                Log("send_notify error " + ex.Message);
            }
        }

        static void send_notify_wc(string header, string message, int code)
        {
            WebService.SendNotification(header, message, code);
        }

        static string terem_name = "";
        static int teremid = 0;

        public static string GetTeremokName()
        {
            try
            {
                DirectoryInfo rbdir =
#if(DEBUG)
 new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);                    
#else
                    new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FindParentDir("rbclient");
#endif
                CBData cb = new CBData();
                cb.SetConnString(rbdir.FullName);
                teremid = cb.Get1();
                terem_name = cb.Get2(teremid);
                return terem_name;
            }
            catch (Exception ex)
            {
                Log("GetTeremokName error " + ex.Message);
                return "";
            }
        }

        private static void RemoveTeremok(string _terem_id)
        {
            try
            {
                DirectoryInfo rbdir =
#if(DEBUG)
                new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
#else
                    new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FindParentDir("rbclient");
#endif
                CBData cb = new CBData();
                cb.SetConnString(rbdir.FullName);
                cb.UnCurrentTeremok(_terem_id);
            }
            catch (Exception ex)
            {
                Log("UnCurrentTeremok error " + ex.Message);
            }
        }

        public static string GetTeremokCurrentCount()
        {
            try
            {
                DirectoryInfo rbdir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FindParentDir("rbclient");
                CBData cb = new CBData();
                cb.SetConnString(rbdir.FullName);
                int teremid = cb.Get1();
                terem_name = cb.Get2(teremid);
                return terem_name;
            }
            catch (Exception ex)
            {
                Log("GetTeremokName error " + ex.Message);
                return "";
            }
        }

        public static void RenamePcAdm()
        {
            terem_name = GetTeremokName();
            string suffix = CNFG.GetProperty<string>("suffix", "-PC-01");
            string dname = terem_name + suffix;
            string dns_suffix = CNFG.GetProperty<string>("dns_suffix", "msk.teremok.biz");

            Log("set '" + dname + "'");

            SetMachineNameWMI(dname);
            SetMachineName(dname);

            if (!EqualUserDomain(CNFG.GetProperty<string>("user_domain", "msk")))
            {
                Log("set dns suffix'" + dns_suffix + "'");
                SetDnsSuffix(dns_suffix);
            }

            string message = String.Format("send on webservice hostname={0} dns-suffix={1}", dname, dns_suffix);
            string header = terem_name;

            Log(message);
            try{
            WebService.SendNotification(header, message, 0);
            }catch(Exception ex)
                    {
                        Log("WebService.SendNotification error " + ex.Message);
                    }
            //Log("end");
            //Environment.Exit(1);
        }

        public static void RenameKKm()
        {
            string kkmnum = String.Format("{0:000}", GetkkmNumber());
            string suffix = CNFG.GetProperty<string>("suffix", "pos");
            string dname = "";
            string dns_suffix = "";

            fillrestolist();

            if (kkm_resto_list.Contains(kkmnum))
            {
                string resto = (string)kkm_resto_list[kkmnum];
                dname = resto + "-" + suffix + "-" + kkmnum;
                Log("set '" + dname + "'");

                SetMachineNameWMI(dname);
                SetMachineName(dname);

                if (!EqualUserDomain(CNFG.GetProperty<string>("user_domain", "msk")))
                {
                    dns_suffix = CNFG.GetProperty<string>("dns_suffix", "msk.teremok.biz");
                    Log("set dns suffix'" + dns_suffix + "'");
                    SetDnsSuffix(dns_suffix);

                    Log(String.Format("send on webservice kkm={0} hostname={1} dns-suffix={2}", kkmnum, dname, dns_suffix));
                    try
                    {
                        WebService.UpdateKKmHostName(int.Parse(kkmnum), dname, dns_suffix);
                    }catch(Exception ex)
                    {
                        Log("WebService.UpdateKKmHostName error " + ex.Message);
                    }
                }

            }
            else
            {
                ErrorHappend("No sucn key '" + kkmnum + "'");
            }
        }

        public static bool EqualUserDomain(string udomain)
        {
            try
            {
                if (Environment.UserDomainName.ToLower() == udomain.ToLower())
                    return true;
                else return false;
            }
            catch (Exception ex)
            {
                Log("EqualUserDomain error " + ex.Message);
                return false;
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern bool SetComputerNameEx(COMPUTER_NAME_FORMAT NameType,
        string lpBuffer);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetComputerNameEx(COMPUTER_NAME_FORMAT NameType,
            StringBuilder lpBuffer, ref uint lpnSize);

        enum COMPUTER_NAME_FORMAT
        {
            ComputerNameNetBIOS,
            ComputerNameDnsHostname,
            ComputerNameDnsDomain,
            ComputerNameDnsFullyQualified,
            ComputerNamePhysicalNetBIOS,
            ComputerNamePhysicalDnsHostname,
            ComputerNamePhysicalDnsDomain,
            ComputerNamePhysicalDnsFullyQualified,
        }
        //ComputerNamePhysicalDnsHostname used to rename the computer name and netbios name before domain join
        public static bool SetDnsSuffix(string name)
        {
            try
            {
                return SetComputerNameEx(COMPUTER_NAME_FORMAT.ComputerNamePhysicalDnsDomain, name);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetDnsSuffix()
        {
            try
            {
                bool success;
                StringBuilder name = new StringBuilder(260);
                uint size = 260;
                success = GetComputerNameEx(COMPUTER_NAME_FORMAT.ComputerNameDnsDomain, name, ref size);
                return name.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static ConfigClass CNFG = new ConfigClass("Config.xml");

        private static string GetVersionFromRegistry()
        {
            string vers = "";
            using (RegistryKey ndpKe = Registry.LocalMachine)
            {

                RegistryKey ndpKey = ndpKe.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\");

                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {

                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        string name = (string)versionKey.GetValue("Version", "");
                        string sp = versionKey.GetValue("SP", "").ToString();
                        string install = versionKey.GetValue("Install", "").ToString();
                        if (install == "") //no install info, must be later.
                            vers += versionKeyName + "  " + name + "\r\n";
                        else
                        {
                            if (sp != "" && install == "1")
                            {
                                vers += versionKeyName + "  " + name + "  SP" + sp + "\r\n";
                            }

                        }
                        if (name != "")
                        {
                            continue;
                        }
                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                        {
                            RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string)subKey.GetValue("Version", "");
                            if (name != "")
                                sp = subKey.GetValue("SP", "").ToString();
                            install = subKey.GetValue("Install", "").ToString();
                            if (install == "") //no install info, must be later.
                                vers += versionKeyName + "  " + name + "\r\n";
                            else
                            {
                                if (sp != "" && install == "1")
                                {
                                    vers += "  " + subKeyName + "  " + name + "  SP" + sp + "\r\n";
                                }
                                else if (install == "1")
                                {
                                    vers += "  " + subKeyName + "  " + name + "\r\n";
                                }

                            }

                        }

                    }
                }
            }
            return vers;
        }

        static void Log(string message)
        {
            log.Log(message);
        }

        private static void fillrestolist()
        {
            List<string> lines = File.ReadAllLines(homeDir.FindFile("resto_kkm_sprav.txt", false).FullName).ToList();
            //    Regex replace = new Regex(@"\t|\n|\r");
            foreach (var a in lines)
            {
                try
                {
                    string replacement = Regex.Replace(a, @"\t|\n|\r", " ");
                    string[] str = replacement.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    kkm_resto_list.Add(str[1].Trim(), str[0].Trim());
                }
                catch
                {
                    continue;
                }
            }
        }

        public static Hashtable kkm_resto_list = new Hashtable();
        public static DirectoryInfo homeDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        internal static Logger log = new Logger();
        public static void SetMachineNameWMI(string newName)
        {
            ManagementScope scope = new ManagementScope(@"\\root\CIMV2");

            ObjectQuery query = new ObjectQuery(
                "SELECT * FROM Win32_ComputerSystem");


            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            ManagementObjectCollection theCollection = searcher.Get();

            foreach (ManagementObject queryObj in theCollection)
            {
                queryObj.InvokeMethod("Rename", new object[3] { newName, null, null });
                break;
            }
        }
        public static void SetMachineName(string newName)
        {
            RegistryKey key = Registry.LocalMachine;

            string activeComputerName = "SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ActiveComputerName";
            RegistryKey activeCmpName = key.CreateSubKey(activeComputerName);
            activeCmpName.SetValue("ComputerName", newName);
            activeCmpName.Close();
            string computerName = "SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName";
            RegistryKey cmpName = key.CreateSubKey(computerName);
            cmpName.SetValue("ComputerName", newName);
            cmpName.Close();
            string _hostName = "SYSTEM\\CurrentControlSet\\services\\Tcpip\\Parameters\\";
            RegistryKey hostName = key.CreateSubKey(_hostName);
            hostName.SetValue("Hostname", newName);
            hostName.SetValue("NV Hostname", newName);
            hostName.Close();
        }

        public static string GetkkmNumber()
        {
            List<string> kkmInList = new List<string>() { @"C:\In" };
            List<string> kkmOutList = new List<string>() { @"C:\Out\Otchet" };


            List<FileInfo> flist = GetFilesInDirByPatternAndSortBackwards(kkmOutList.First(), "X*");

            if (!flist.NotNullOrEmpty())
            {
                flist = GetFilesInDirByPatternAndSortBackwards(kkmOutList.First(), "T*");
                if (!flist.NotNullOrEmpty())
                {
                    flist = GetTfiles(kkmInList, kkmOutList);
                }
            }

            //пропарсить t-otchet и создать обеды
            if (flist.NotNullOrEmpty())
            {
                FileInfo trep_file = flist.First();
                Log(trep_file.Name);
                string kkm_name = returnKkmNameFromT(trep_file);
                if (kkm_name == "")
                {
                    ErrorHappend("Не удалось пропарсить т-отчет " + trep_file.Name);
                    throw new Exception("Не удалось пропарсить т-отчет " + trep_file.Name);
                }
                else
                {
                    return kkm_name;
                }
            }
            else
            {
                ErrorHappend("Нет т-отчета");
                throw new Exception("Нет т-отчета");
            }
        }

        private static List<FileInfo> GetFilesInDirByPatternAndSortBackwards(string kkmOutList, string pattern)
        {
            DirectoryInfo dir = new DirectoryInfo(kkmOutList);
            List<FileInfo> flist = null;
            flist = dir.GetFiles(pattern).ToList();
            if (flist.NotNullOrEmpty())
            {
                flist.Sort((a, b) => b.CreationTime.CompareTo(a.CreationTime));
                return flist;
            }
            return null;
        }

        private static string returnKkmNameFromT(FileInfo trep_file)
        {
            try
            {
                Regex reg = new Regex(@"Касса (\d+)");

                string text = System.IO.File.ReadAllText(trep_file.FullName, Encoding.GetEncoding(1251));

                if (reg.IsMatch(text))
                {
                    return reg.Match(text).Groups[1].Value.Trim();
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private static List<FileInfo> GetTfiles(List<string> kkmInList, List<string> kkmOutList)
        {
            try
            {
                kkmOutList.ForEach(a =>
                {
                    DirectoryInfo dir = new DirectoryInfo(a);
                    List<FileInfo> file_pathes = dir.GetFiles("T*", SearchOption.TopDirectoryOnly).ToList();
                    file_pathes.ForEach(b => new CustomAction((o) =>
                    {
                        b.Delete();
                    }
                    , null).Start());
                });

                kkmInList.ForEach(a => new CustomAction((o) =>
                { string filename = o.ToString(); File.Create(System.IO.Path.Combine(a, filename)); }, "0.dat").Start());

                DirectoryInfo s_treport_dir = null;
                s_treport_dir = homeDir.GetDecendantDirectory("temp");
                s_treport_dir.DeleteOldFilesInDir(0);

                kkmOutList.ForEach(a => new CustomAction((o) =>
                {
                    DirectoryInfo kkm_dir = new DirectoryInfo(a);
                    List<FileInfo> file_pathes = kkm_dir.GetFiles("T*", SearchOption.TopDirectoryOnly).ToList();
                    if (file_pathes == null || file_pathes.Count == 0) throw new Exception("Еще пока нет Трепорта");
                    file_pathes.Sort((c, b) => b.CreationTime.CompareTo(c.CreationTime));

                    new CustomAction((oo) =>
                    {
                        FileInfo o1 = oo as FileInfo; if (o1 == null) return;
#if(!DEB)
                        o1.MoveTo(System.IO.Path.Combine(s_treport_dir.FullName, o1.Name) + kkmOutList.IndexOf(a));
#endif
#if(DEB)
                    o1.CopyTo(System.IO.Path.Combine(s_treport_dir.FullName, o1.Name) + kkmOutList.IndexOf(a));
#endif
                    }, file_pathes[0]) { Timeout = 5000 }.Start();
                }, null) { Timeout = 10000 }.Start());
                return s_treport_dir.GetFiles("T*", SearchOption.TopDirectoryOnly).ToList();
            }
            catch (Exception ex)
            {
                ErrorHappend("GetTFiles error " + ex.Message);
                return null;
            }
        }

        private static void ErrorHappend(string p)
        {
            Log(p);
            throw new NotImplementedException();
        }


        private static List<WmiMethods.Credentials> SetCredentials()
        {
            List<string[]> ls = GetUserCreds();
            List<WmiMethods.Credentials> ls1 = new List<WmiMethods.Credentials>();
            ls.ForEach(a =>
            {
                WmiMethods.Credentials c = new WmiMethods.Credentials();
                c.UserName = a[0];
                c.Password = PasswordDecoder.decode_string(a[1]);
                ls1.Add(c);
            });
            return ls1;
        }

        private static List<string[]> GetUserCreds()
        {
            List<string[]> list = new List<string[]>();
            if (CNFG != null)
            {
                List<object> lo = CNFG.GetProperties("user_cred");
                foreach (object o in lo)
                {
                    list.Add(((string)o).Split(new string[] { "---" }, StringSplitOptions.None));
                }
            }
            return list;
        }

        private static List<string> getKkmsRoot()
        {
            try
            {
                DirectoryInfo rbdir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FindParentDir("rbclient");
                //  DirectoryInfo rbdir = new DirectoryInfo(@"G:\RRepo\myproject\RBClient");
                CBData cb = new CBData();
                cb.SetConnString(rbdir.FullName);

                List<string> lc = cb.getKkms();
                List<string> lc1 = new List<string>();

                if (lc.NotNullOrEmpty())
                {
                    //public static string GetGlobalRoot(this DirectoryInfo dir_path)
                    lc.ForEach(a =>
                    {
                        if (a.IndexOf("Out") != -1)
                        {
                            lc1.Add(new DirectoryInfo(a).Parent.FullName);
                        }
                    });
                }

                return lc1;
            }
            catch (Exception ex)
            {
                Log("GetTeremokName error " + ex.Message);
                return null;
            }
        }

        private static List<string> getKkmsIn()
        {
            try
            {
                DirectoryInfo rbdir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FindParentDir("rbclient");
                //  DirectoryInfo rbdir = new DirectoryInfo(@"G:\RRepo\myproject\RBClient");
                CBData cb = new CBData();
                cb.SetConnString(rbdir.FullName);

                List<string> lc = cb.getKkms();
                List<string> lc1 = new List<string>();

                if (lc.NotNullOrEmpty())
                {
                    //public static string GetGlobalRoot(this DirectoryInfo dir_path)
                    lc.ForEach(a =>
                    {
                        if (a.IndexOf("In") != -1)
                        {
                            lc1.Add(a);
                        }
                    });
                }

                return lc1;
            }
            catch (Exception ex)
            {
                Log("GetTeremokName error " + ex.Message);
                return null;
            }
        }

    
    }
}
